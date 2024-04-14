using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    public partial class MainForm : Form
    {
        private Point _startPoint;
        private Rectangle _selectionRectangle;
        private bool _isSelecting;

        public MainForm()
        {
            InitializeComponent();
            InitializeScreenCapture();
        }

        private void InitializeScreenCapture()
        {
            this.MouseDown += MainForm_MouseDown;
            this.MouseMove += MainForm_MouseMove;
            this.MouseUp += MainForm_MouseUp;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isSelecting = true;
                _startPoint = e.Location;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting)
            {
                Point currentPoint = e.Location;

                int x = Math.Min(_startPoint.X, currentPoint.X);
                int y = Math.Min(_startPoint.Y, currentPoint.Y);
                int width = Math.Abs(_startPoint.X - currentPoint.X);
                int height = Math.Abs(_startPoint.Y - currentPoint.Y);

                _selectionRectangle = new Rectangle(x, y, width, height);

                this.Invalidate();
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isSelecting)
            {
                _isSelecting = false;

                // Create bitmap to store the screenshot
                Bitmap screenshot = new Bitmap(_selectionRectangle.Width, _selectionRectangle.Height);

                // Create Graphics object from the bitmap
                using (Graphics graphics = Graphics.FromImage(screenshot))
                {
                    // Copy the selected region of the screen to the bitmap
                    graphics.CopyFromScreen(_selectionRectangle.Location, Point.Empty, _selectionRectangle.Size);
                }

                // Save the screenshot to file or do something else with it
                screenshot.Save("screenshot.png");

                MessageBox.Show("Screenshot saved as screenshot.png");
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_isSelecting)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, _selectionRectangle);
                }
            }
        }
    }
}