using System;
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
        private Point startPoint;
        private Rectangle selectionRectangle;
        private bool isSelecting = false;

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
        }

        private void ScreenCaptureForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                isSelecting = true;
            }
        }

        private void ScreenCaptureForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                int x = Math.Min(e.X, startPoint.X);
                int y = Math.Min(e.Y, startPoint.Y);
                int width = Math.Abs(e.X - startPoint.X);
                int height = Math.Abs(e.Y - startPoint.Y);

                selectionRectangle = new Rectangle(x, y, width, height);
                this.Invalidate();
            }
        }

        private void ScreenCaptureForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = false;
                if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
                {
                    using (Bitmap bitmap = new Bitmap(selectionRectangle.Width, selectionRectangle.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(selectionRectangle.Location, Point.Empty, selectionRectangle.Size);
                        }
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            bitmap.Save(saveDialog.FileName);
                        }
                    }
                }
                this.Close();
            }
        }

        private void ScreenCaptureForm_Paint(object sender, PaintEventArgs e)
        {
            if (isSelecting)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }
    }
}