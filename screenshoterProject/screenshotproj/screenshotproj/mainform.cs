using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace ScreenCaptureApp
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new ScreenCaptureForm());
        }
    }

    public class ScreenCaptureForm : Form
    {
        private string selectedExePath = "";
        private Process startedProcess;

        public ScreenCaptureForm()
        {
            this.Text = "ScreenMaster";

            //this.Size = new Size(400, 350);

            string iconPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "ScreenMaster.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            this.Size = new Size(300, 230);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            // Кнопка для выбора exe файла
            System.Windows.Forms.Button selectExeButton = new System.Windows.Forms.Button();
            selectExeButton.Text = "Выбрать и запустить файл exe";

            //selectExeButton.Size = new Size(250, 50);
            //selectExeButton.Location = new Point(70, 30);

            selectExeButton.Size = new Size(265, 50);
            selectExeButton.Location = new Point(10, 10);

            selectExeButton.Click += SelectExeButton_Click;
            this.Controls.Add(selectExeButton);

            //// Кнопка для создания скриншотов
            //System.Windows.Forms.Button createScreenshotsButton = new System.Windows.Forms.Button();
            //createScreenshotsButton.Text = "Создать скриншоты";
            //createScreenshotsButton.Size = new Size(250, 50);
            //createScreenshotsButton.Location = new Point(70, 90);
            //createScreenshotsButton.Click += CreateScreenshotsButton_Click;
            //this.Controls.Add(createScreenshotsButton);

            // Кнопка для создания скриншотов автоматически
            System.Windows.Forms.Button createAutomaticallyScreenshotsButton = new System.Windows.Forms.Button();
            createAutomaticallyScreenshotsButton.Text = "Создать скриншоты";
            createAutomaticallyScreenshotsButton.Size = new Size(205, 50);
            createAutomaticallyScreenshotsButton.Location = new Point(70, 70);
            createAutomaticallyScreenshotsButton.Click += CreateAutomaticallyScreenshotsButton_Click;
            this.Controls.Add(createAutomaticallyScreenshotsButton);

            // Кнопка для создания скриншотов самостоятельно
            System.Windows.Forms.Button createPersonallyScreenshotsButton = new System.Windows.Forms.Button();
            string imagePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "personallyButton.png");
            if (File.Exists(imagePath))
            {
                Image image = Image.FromFile(imagePath);
                createPersonallyScreenshotsButton.Image = new Bitmap(image, new Size(37, 30));
            }
            createPersonallyScreenshotsButton.Size = new Size(50, 50);
            createPersonallyScreenshotsButton.Location = new Point(10, 70);
            createPersonallyScreenshotsButton.Click += CreatePersonallyScreenshotsButton_Click;
            this.Controls.Add(createPersonallyScreenshotsButton);

            // Кнопка для помощи
            System.Windows.Forms.Button helpButton = new System.Windows.Forms.Button();
            helpButton.Text = "Help";

            //helpButton.Size = new Size(250, 50);
            //helpButton.Location = new Point(70, 150);

            helpButton.Size = new Size(265, 50);
            helpButton.Location = new Point(10, 130);

            helpButton.Click += HelpButton_Click;
            this.Controls.Add(helpButton);

            //// Кнопка для выхода
            //System.Windows.Forms.Button exitButton = new System.Windows.Forms.Button();
            //exitButton.Text = "Выход";
            //exitButton.Size = new Size(250, 50);
            //exitButton.Location = new Point(70, 210);
            //exitButton.Click += ExitButton_Click;
            //this.Controls.Add(exitButton);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                string helpFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "spravka.chm");

                // Проверяем существует ли файл с инструкцией
                if (File.Exists(helpFilePath))
                {
                    // Открываем файл справки с помощью Notepad
                    Process.Start("hh.exe", helpFilePath);
                    return true; // сообщаем, что команда была обработана
                }
                else
                {
                    MessageBox.Show("Инструкция не найдена.");
                    return true; // сообщаем, что команда была обработана
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SelectExeButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog exeFileDialog = new OpenFileDialog();
            exeFileDialog.Filter = "Executable Files|*.exe";
            exeFileDialog.Title = "Выберите исполняемый файл";

            if (exeFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedExePath = exeFileDialog.FileName;
                MessageBox.Show($"Выбран файл: {selectedExePath}");

                // Запускаем выбранный exe файл
                startedProcess = Process.Start(new ProcessStartInfo(selectedExePath)
                {
                    WindowStyle = ProcessWindowStyle.Normal
                });

                // Ждем, чтобы приложение открылось
                Thread.Sleep(2000);
            }
        }

        private void CreatePersonallyScreenshotsButton_Click(object sender, EventArgs e)
        {
            ScreenMakerForm screenMakerForm = new ScreenMakerForm();
            screenMakerForm.ShowDialog();
        }

        private void CreateAutomaticallyScreenshotsButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedExePath))
            {
                MessageBox.Show("Сначала выберите и запустите файл exe");
                return;
            }

            // Получаем главное окно приложения
            var app = FlaUI.Core.Application.Attach(startedProcess);
            var automation = new UIA3Automation();
            var mainWindow = app.GetMainWindow(automation);

            // Делаем скриншоты видимых элементов
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var screenshotsFolderPath = Path.Combine(desktopPath, "Screenshots");
            Directory.CreateDirectory(screenshotsFolderPath);

            TakeVisibleElementScreenshots(mainWindow, screenshotsFolderPath);

            // Делаем скриншоты элементов меню
            var elements = mainWindow.FindAllDescendants();
            foreach (var element in elements)
            {
                if (element.ControlType == FlaUI.Core.Definitions.ControlType.MenuItem)
                {
                    element.Click();

                    Thread.Sleep(100);

                    var menuWindow = mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Menu));
                    if (menuWindow != null)
                    {
                        TakeMenuScreenshots(menuWindow, screenshotsFolderPath);
                    }

                    element.Click();

                    Thread.Sleep(10);
                }
            }

            MessageBox.Show("Скриншоты успешно созданы и сохранены на рабочем столе в папке 'Screenshots'.");
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string helpFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "helpfile.chm");

            if (File.Exists(helpFilePath))
            {
                Process.Start("hh.exe", helpFilePath);
            }
            else
            {
                MessageBox.Show("Файл справки не найден");
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        static void TakeVisibleElementScreenshots(AutomationElement element, string screenshotsFolderPath)
        {
            var elements = element.FindAllDescendants();
            foreach (var el in elements)
            {
                var boundingRect = el.BoundingRectangle;
                if (!boundingRect.IsEmpty)
                {
                    // Преобразуем координаты в целые числа
                    int x = (int)boundingRect.X;
                    int y = (int)boundingRect.Y;
                    int width = (int)boundingRect.Width;
                    int height = (int)boundingRect.Height;

                    // Увеличиваем область захвата
                    var enlargedRect = new Rectangle(
                        x - 10,
                        y - 10,
                        width + 20,
                        height + 20
                    );

                    // Создаем Bitmap для скриншота
                    var bitmap = new Bitmap(enlargedRect.Width, enlargedRect.Height);

                    // Копируем из экрана в Bitmap
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(
                            new Point(enlargedRect.X, enlargedRect.Y),
                            Point.Empty,
                            enlargedRect.Size
                        );
                    }

                    // Сохраняем скриншот на рабочем столе
                    var fileName = $"screenshot_{el.Name}_{DateTime.Now:yyyyMMddHHmmss}.png";
                    var filePath = Path.Combine(screenshotsFolderPath, fileName);
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        static void TakeMenuScreenshot(AutomationElement element, string screenshotsFolderPath, string prefix = "")
        {
            var window = element.AsWindow();
            if (window != null)
            {
                var bitmap = window.Capture();
                var fileName = $"{prefix}_screenshot.png";
                var filePath = Path.Combine(screenshotsFolderPath, fileName);

                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        static void TakeMenuScreenshots(AutomationElement menuWindow, string screenshotsFolderPath)
        {
            // Сначала делаем скриншот всего меню без выделений
            TakeMenuScreenshot(menuWindow, screenshotsFolderPath, "Menu");

            // Затем делаем скриншоты с выделением по одному элементу
            var childElements = menuWindow.FindAllDescendants();
            foreach (var childElement in childElements)
            {
                if (childElement.ControlType == FlaUI.Core.Definitions.ControlType.MenuItem)
                {
                    // Создаем новый скриншот меню
                    var bitmap = menuWindow.Capture();

                    // Вычисляем относительные координаты текущего элемента внутри меню
                    var relativeBoundingRect = childElement.BoundingRectangle;
                    relativeBoundingRect.Offset(-menuWindow.BoundingRectangle.Left, -menuWindow.BoundingRectangle.Top);

                    // Создаем графический объект для рисования на скриншоте
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        // Выделяем текущий элемент
                        var pen = new Pen(Color.Red, 3); // Выбираем красный цвет и толщину рамки
                        graphics.DrawRectangle(pen, relativeBoundingRect);
                    }

                    // Сохраняем скриншот с выделением текущего элемента
                    var fileName = $"screenshot_Menu_{childElement.Name}_{DateTime.Now:yyyyMMddHHmmss}.png";
                    var filePath = Path.Combine(screenshotsFolderPath, fileName);
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        public class ScreenMakerForm : Form
        {
            private Point startPoint;
            private Rectangle selectionRectangle;
            private bool isSelecting = false;

            public ScreenMakerForm()
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
}