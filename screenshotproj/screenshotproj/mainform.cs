using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            Bitmap screenshot = CaptureScreen();
            string fileName = $"screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            screenshot.Save(fileName, ImageFormat.Png);
            MessageBox.Show($"Screenshot saved as {fileName}");
        }

        private Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenshot;
        }
    }
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
