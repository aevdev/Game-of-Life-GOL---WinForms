using System.Runtime.CompilerServices;

namespace Game_of_Life_1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new Button();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new Point(10, rows * cellSize + 10);
            this.startButton.Size = new Size(100, 30);
            this.startButton.Text = "Start";
            this.startButton.Click += new EventHandler(this.StartButton_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new Size(cols * cellSize, rows * cellSize + 50);
            this.Controls.Add(this.startButton); // Добавляем кнопку на форму
            this.Name = "Form1";
            this.Text = "Game of Life with User Control";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
