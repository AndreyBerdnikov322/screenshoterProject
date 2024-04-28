using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScreenCaptureForm());
        }
    }

    public class ScreenCaptureForm : Form
    {
        private List<Point> startPoints = new List<Point>();
        private List<Point> endPoints = new List<Point>();
        private bool isSelecting = false;
        private float dpiX, dpiY;

        public ScreenCaptureForm()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.Opacity = 0.5;
            this.WindowState = FormWindowState.Maximized;
            this.Cursor = Cursors.Cross;
            this.MouseDown += ScreenCaptureForm_MouseDown;
            this.MouseMove += ScreenCaptureForm_MouseMove;
            this.MouseUp += ScreenCaptureForm_MouseUp;
            this.DoubleBuffered = true;
            this.Paint += ScreenCaptureForm_Paint;

            using (Graphics g = this.CreateGraphics())
            {
                dpiX = g.DpiX;
                dpiY = g.DpiY;
            }

            // Добавляем кнопку для выполнения скриншотов
            Button screenshotButton = new Button();
            screenshotButton.Text = "Сделать скриншоты";
            screenshotButton.Size = new Size(180, 50);
            screenshotButton.Location = new Point(50, 50);
            screenshotButton.BackColor = Color.White;
            screenshotButton.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(screenshotButton);
            screenshotButton.Click += ScreenshotButton_Click;

            // Добавляем кнопку для выхода из программы
            Button exitButton = new Button();
            exitButton.Text = "Выход";
            exitButton.Size = new Size(180, 50);
            exitButton.Location = new Point(50, 120);
            exitButton.BackColor = Color.White;
            exitButton.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(exitButton);
            exitButton.Click += ExitButton_Click;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ScreenshotButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < startPoints.Count; i++)
            {
                Point startPoint = startPoints[i];
                Point endPoint = endPoints[i];
                int x = (int)Math.Round(Math.Min(startPoint.X, endPoint.X) * (dpiX / 96f));
                int y = (int)Math.Round(Math.Min(startPoint.Y, endPoint.Y) * (dpiY / 96f));
                int width = (int)Math.Round(Math.Abs(startPoint.X - endPoint.X) * (dpiX / 96f));
                int height = (int)Math.Round(Math.Abs(startPoint.Y - endPoint.Y) * (dpiY / 96f));

                Rectangle selectionRectangle = new Rectangle(x, y, width, height);

                using (Bitmap bitmap = new Bitmap(selectionRectangle.Width, selectionRectangle.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(selectionRectangle.Location, Point.Empty, selectionRectangle.Size);
                    }

                    // Сохраняем скриншот в файл
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        bitmap.Save(saveDialog.FileName);
                    }
                }
            }
        }

        private void ScreenCaptureForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoints.Add(e.Location);
                endPoints.Add(e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
                isSelecting = false;
                startPoints.Clear();
                endPoints.Clear();
                this.Invalidate();
            }
        }

        private void ScreenCaptureForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                endPoints[endPoints.Count - 1] = e.Location;
                this.Invalidate();
            }
        }

        private void ScreenCaptureForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isSelecting)
            {
                isSelecting = false;
                this.Invalidate();
            }
        }

        private void ScreenCaptureForm_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                for (int i = 0; i < startPoints.Count; i++)
                {
                    Point startPoint = startPoints[i];
                    Point endPoint = endPoints[i];
                    int x = (int)Math.Round(Math.Min(startPoint.X, endPoint.X) * (dpiX / 96f));
                    int y = (int)Math.Round(Math.Min(startPoint.Y, endPoint.Y) * (dpiY / 96f));
                    int width = (int)Math.Round(Math.Abs(startPoint.X - endPoint.X) * (dpiX / 96f));
                    int height = (int)Math.Round(Math.Abs(startPoint.Y - endPoint.Y) * (dpiY / 96f));
                    Rectangle selectionRectangle = new Rectangle(x, y, width, height);
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }
    }
}