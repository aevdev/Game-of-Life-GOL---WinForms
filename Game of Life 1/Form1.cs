using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Game_of_Life_1
{
    public partial class Form1 : Form
    {
        private int[,] grid; // Текущая сетка (мир)
        private int rows = 50; // Количество строк
        private int cols = 50; // Количество столбцов
        private int cellSize = 10; // Размер клетки
        private System.Windows.Forms.Timer timer;

        //Оптимизация
        private Bitmap buffer; // Буфер для отрисовки
        private object gridLock = new object(); // Для синхронизации потоков
        private Thread updateThread;

        //Управление
        private Button startButton; // Кнопка для запуска/остановки игры
        private bool running = false;

        public Form1()
        {
            InitializeComponent();

            // Инициализация сетки
            grid = new int[rows, cols];
            buffer = new Bitmap(cols * cellSize, rows * cellSize);
            this.DoubleBuffered = true; //Избавляемся от мерцаний

            // Случайное заполнение
            //Random rand = new Random();
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < cols; j++)
            //    {
            //        grid[i, j] = rand.Next(2);
            //    }
            //}

            // Поток для обновления логики игры
            updateThread = new Thread(UpdateLoop);
            updateThread.IsBackground = true;
            //updateThread.Start(); //Вынесли в другое место

            // Таймер для обновления игры
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Интервал обновления (в миллисекундах)
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (gridLock)
            {
                DrawToBuffer(); // Рисуем в буфер
            }
            Invalidate(); // Перерисовываем экран (метод Refresh не делает погоды с мерцанием)
        }

        private void UpdateLoop()
        {
            while (true)
            {
                if (running) // Обновляем только если игра запущена
                {
                    lock (gridLock)
                    {
                        UpdateGrid(); // Обновляем состояние клеток
                    }
                    Thread.Sleep(100); // Задержка для контролирования скорости обновления
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

                    if (grid[i, j] == 1) // Живая клетка
                    {
                        if (aliveNeighbors < 2 || aliveNeighbors > 3)
                            newGrid[i, j] = 0; // Умирает
                        else
                            newGrid[i, j] = 1; // Живёт
                    }
                    else // Мёртвая клетка
                    {
                        if (aliveNeighbors == 3)
                            newGrid[i, j] = 1; // Воскрешается
                        else
                            newGrid[i, j] = 0;
                    }
                }
            }

            grid = newGrid; // Переносим новые значения в основную сетку
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

        // Метод отрисовки сетки в буфер
        private void DrawToBuffer()
        {
            /*
             Объекты типа Graphics работают с неуправляемыми ресурсами (например, объектами GDI+),
            которые не управляются сборщиком мусора напрямую. Поэтому их нужно явно освобождать,
            чтобы избежать утечек памяти и проблем с производительностью.(либо вручную писать Dispose для объекта Graphics)
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

        // Метод отрисовки сетки
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
            e.Graphics.DrawImage(buffer, 0, 0); // Рисуем буфер на экране
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!running) // Пользователь может изменять сетку только до начала игры
            {
                int col = e.X / cellSize;
                int row = e.Y / cellSize;

                if (col >= 0 && col < cols && row >= 0 && row < rows)
                {
                    grid[row, col] = grid[row, col] == 1 ? 0 : 1; // Инвертируем состояние клетки
                    Invalidate(); // Перерисовываем экран для отображения изменений
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            running = !running; // Инвертируем состояние игры
            startButton.Text = running ? "Stop" : "Start";

            if (running && !updateThread.IsAlive) // Если поток еще не запущен, запускаем его
            {
                updateThread.Start();
            }
        }

    }
}
