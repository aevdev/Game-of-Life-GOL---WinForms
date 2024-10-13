using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Game_of_Life_1
{
    public partial class Form1 : Form
    {
        private int[,] grid; // ������� ����� (���)
        private int rows = 50; // ���������� �����
        private int cols = 50; // ���������� ��������
        private int cellSize = 10; // ������ ������
        private System.Windows.Forms.Timer timer;

        //�����������
        private Bitmap buffer; // ����� ��� ���������
        private object gridLock = new object(); // ��� ������������� �������
        private Thread updateThread;

        //����������
        private Button startButton; // ������ ��� �������/��������� ����
        private bool running = false;

        public Form1()
        {
            InitializeComponent();

            // ������������� �����
            grid = new int[rows, cols];
            buffer = new Bitmap(cols * cellSize, rows * cellSize);
            this.DoubleBuffered = true; //����������� �� ��������

            // ��������� ����������
            //Random rand = new Random();
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        grid[i, j] = rand.Next(2);
            //    }
            //}

            // ����� ��� ���������� ������ ����
            updateThread = new Thread(UpdateLoop);
            updateThread.IsBackground = true;
            //updateThread.Start(); //������� � ������ �����

            // ������ ��� ���������� ����
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // �������� ���������� (� �������������)
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (gridLock)
            {
                DrawToBuffer(); // ������ � �����
            }
            Invalidate(); // �������������� ����� (����� Refresh �� ������ ������ � ���������)
        }

        private void UpdateLoop()
        {
            while (true)
            {
                if (running) // ��������� ������ ���� ���� ��������
                {
                    lock (gridLock)
                    {
                        UpdateGrid(); // ��������� ��������� ������
                    }
                    Thread.Sleep(100); // �������� ��� ��������������� �������� ����������
                }
            }
        }

        private void UpdateGrid()
        {
            int[,] newGrid = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int aliveNeighbors = CountAliveNeighbors(i, j);

                    if (grid[i, j] == 1) // ����� ������
                    {
                        if (aliveNeighbors < 2 || aliveNeighbors > 3)
                            newGrid[i, j] = 0; // �������
                        else
                            newGrid[i, j] = 1; // ����
                    }
                    else // ̸����� ������
                    {
                        if (aliveNeighbors == 3)
                            newGrid[i, j] = 1; // ������������
                        else
                            newGrid[i, j] = 0;
                    }
                }
            }

            grid = newGrid; // ��������� ����� �������� � �������� �����
        }

        private int CountAliveNeighbors(int x, int y)
        {
            int aliveCount = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int newX = x + i;
                    int newY = y + j;

                    if (newX >= 0 && newX < rows && newY >= 0 && newY < cols)
                    {
                        aliveCount += grid[newX, newY];
                    }
                }
            }

            return aliveCount;
        }

        // ����� ��������� ����� � �����
        private void DrawToBuffer()
        {
            /*
             ������� ���� Graphics �������� � �������������� ��������� (��������, ��������� GDI+),
            ������� �� ����������� ��������� ������ ��������. ������� �� ����� ���� �����������,
            ����� �������� ������ ������ � ������� � �������������������.(���� ������� ������ Dispose ��� ������� Graphics)
             */
            using (Graphics g = Graphics.FromImage(buffer))
            {
                g.Clear(Color.White);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Brush brush = grid[i, j] == 1 ? Brushes.Black : Brushes.White;
                        g.FillRectangle(brush, j * cellSize, i * cellSize, cellSize, cellSize);
                        g.DrawRectangle(Pens.Gray, j * cellSize, i * cellSize, cellSize, cellSize);
                    }
                }
            }
        }

        // ����� ��������� �����
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            //Graphics g = e.Graphics;

            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        Brush brush = grid[i, j] == 1 ? Brushes.Black : Brushes.White;
            //        g.FillRectangle(brush, j * cellSize, i * cellSize, cellSize, cellSize);
            //        g.DrawRectangle(Pens.Gray, j * cellSize, i * cellSize, cellSize, cellSize);
            //    }
            //}

            base.OnPaint(e);
            e.Graphics.DrawImage(buffer, 0, 0); // ������ ����� �� ������
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!running) // ������������ ����� �������� ����� ������ �� ������ ����
            {
                int col = e.X / cellSize;
                int row = e.Y / cellSize;

                if (col >= 0 && col < cols && row >= 0 && row < rows)
                {
                    grid[row, col] = grid[row, col] == 1 ? 0 : 1; // ����������� ��������� ������
                    Invalidate(); // �������������� ����� ��� ����������� ���������
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            running = !running; // ����������� ��������� ����
            startButton.Text = running ? "Stop" : "Start";

            if (running && !updateThread.IsAlive) // ���� ����� ��� �� �������, ��������� ���
            {
                updateThread.Start();
            }
        }

    }
}
