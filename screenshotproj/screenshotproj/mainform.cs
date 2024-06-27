using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using static FlaUI.Core.FrameworkAutomationElementBase;

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

        public ScreenCaptureForm()
        {
            this.Text = "ScreenMaster";

            Icon iconPath = screenshotproj.Properties.Resources.ICO;

            if (iconPath != null)
            {
                this.Icon = iconPath;
            }

            this.Size = new Size(300, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            // Кнопка для выбора exe файла
            System.Windows.Forms.Button selectExeButton = new System.Windows.Forms.Button();
            selectExeButton.Text = "Выбрать и запустить файл";
            selectExeButton.Size = new Size(265, 50);
            selectExeButton.Location = new Point(10, 10);
            selectExeButton.Click += SelectExeButton_Click;
            this.Controls.Add(selectExeButton);
   

            // Кнопка для создания скриншотов автоматически
            System.Windows.Forms.Button createAutomaticallyScreenshotsButton = new System.Windows.Forms.Button();
            createAutomaticallyScreenshotsButton.Text = "Создать скриншоты";
            createAutomaticallyScreenshotsButton.Size = new Size(205, 50);
            createAutomaticallyScreenshotsButton.Location = new Point(70, 70);
            createAutomaticallyScreenshotsButton.Click += CreateAutomaticallyScreenshotsButton_Click;
            this.Controls.Add(createAutomaticallyScreenshotsButton);

            // Кнопка для создания скриншотов самостоятельно
            System.Windows.Forms.Button createPersonallyScreenshotsButton = new System.Windows.Forms.Button();
            Image image = screenshotproj.Properties.Resources.personallyButton;
            createPersonallyScreenshotsButton.Image = new Bitmap(image, new Size(37, 30));
            createPersonallyScreenshotsButton.Size = new Size(50, 50);
            createPersonallyScreenshotsButton.Location = new Point(10, 70);
            createPersonallyScreenshotsButton.Click += CreatePersonallyScreenshotsButton_Click;
            this.Controls.Add(createPersonallyScreenshotsButton);

            // Кнопка для формирования руководства
            System.Windows.Forms.Button generateGuideButton = new System.Windows.Forms.Button();
            generateGuideButton.Text = "Формирование руководства";
            generateGuideButton.Size = new Size(265, 50);
            generateGuideButton.Location = new Point(10, 130);
            generateGuideButton.Click += GenerateGuideButton_Click;
            this.Controls.Add(generateGuideButton);

            // Кнопка для помощи
            System.Windows.Forms.Button helpButton = new System.Windows.Forms.Button();
            helpButton.Text = "Help";
            helpButton.Size = new Size(265, 50);
            helpButton.Location = new Point(10, 190);
            helpButton.Click += HelpButton_Click;
            this.Controls.Add(helpButton);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                byte[] helpFileBytes = screenshotproj.Properties.Resources.spravka;

                if (helpFileBytes != null)
                {
                    string tempFilePath = Path.Combine(Path.GetTempPath(), "spravka.chm");
                    File.WriteAllBytes(tempFilePath, helpFileBytes);
                    Process.Start("hh.exe", tempFilePath);
                    return true;
                }
                else
                {
                    MessageBox.Show("Инструкция не найдена");
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    if (keyData == Keys.F1)
        //    {
        //        string helpFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "spravka.chm");

        //        // Проверка, существует ли файл с инструкцией
        //        if (File.Exists(helpFilePath))
        //        {
        //            // Открытие файла справки с помощью Notepad
        //            Process.Start("hh.exe", helpFilePath);
        //            return true; // сообщение, что команда была обработана
        //        }
        //        else
        //        {
        //            MessageBox.Show("Инструкция не найдена");
        //            return true; // сообщение, что команда была обработана
        //        }
        //    }
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}

        private void SelectExeButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog exeFileDialog = new OpenFileDialog();
            exeFileDialog.Filter = "Executable Files|*.exe";
            exeFileDialog.Title = "Выберите исполняемый файл";

            if (exeFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedExePath = exeFileDialog.FileName;
                MessageBox.Show($"Выбран файл: {selectedExePath}");
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

            //Запуск выбранного exe файл
            Process startedProcess = Process.Start(new ProcessStartInfo(selectedExePath)
            {
                WindowStyle = ProcessWindowStyle.Normal
            });


            // Ожидание, чтобы приложение открылось, чтобы первые скриншоты не сделали снимки полупрозрачного окна
            Thread.Sleep(2000);

            // Получение главного окна приложения
            var app = FlaUI.Core.Application.Attach(startedProcess);
            var automation = new UIA3Automation();
            var mainWindow = app.GetMainWindow(automation);

            // Создание папки для скриншотов
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var screenshotsFolderPath = Path.Combine(desktopPath, "Screenshots");

            if (Directory.Exists(screenshotsFolderPath))
            {
                // Удаление всех файлов .png в папке
                Directory.GetFiles(screenshotsFolderPath, "*.png").ToList().ForEach(File.Delete);
            }
            else
            {
                Directory.CreateDirectory(screenshotsFolderPath);
            }

            // Создание скриншота всего окна сразу после открытия
            TakeMainWindowScreenshot(mainWindow, screenshotsFolderPath);

            // Создание скриншотов всех изначально видимых элементов
            TakeVisibleElementScreenshots(mainWindow, screenshotsFolderPath);

            // Создание скриншотов меню и элементов меню
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

            MessageBox.Show("Скриншоты успешно созданы и сохранены на рабочем столе в папке 'Screenshots'");
        }

        static void TakeMainWindowScreenshot(Window mainWindow, string screenshotsFolderPath)
        {
            var bitmap = mainWindow.Capture();
            var fileName = "0_Окно_всего-Приложения.png"; // Название файла, чтобы он был первым по алфавиту
            var filePath = Path.Combine(screenshotsFolderPath, fileName);
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
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
                    var fileName = $"скриншот_{el.Name}.png";
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
                var fileName = $"скриншот_{prefix}_{element.Name}.png";
                var filePath = Path.Combine(screenshotsFolderPath, fileName);

                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        static void TakeMenuScreenshots(AutomationElement menuWindow, string screenshotsFolderPath)
        {
            // Сначала делается скриншот всего меню без выделений
            TakeMenuScreenshot(menuWindow, screenshotsFolderPath, "Меню");

            // Затем делаются скриншоты с выделением по одному элементу
            var childElements = menuWindow.FindAllDescendants();

            foreach (var childElement in childElements)
            {
                if (childElement.ControlType == FlaUI.Core.Definitions.ControlType.MenuItem)
                {
                    // Создание нового скриншота меню
                    var bitmap = menuWindow.Capture();

                    // Вычисление относительных координат текущего элемента внутри меню
                    var relativeBoundingRect = childElement.BoundingRectangle;
                    relativeBoundingRect.Offset(-menuWindow.BoundingRectangle.Left, -menuWindow.BoundingRectangle.Top);

                    // Создание графического объект для рисования на скриншоте
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        // Выделение текущего элемента
                        var pen = new Pen(Color.Red, 3); // Рамка красная с шириной 3
                        graphics.DrawRectangle(pen, relativeBoundingRect);
                    }

                    // Сохранение скриншота с выделением текущего элемента
                    var fileName = $"скриншот_Меню_{menuWindow.Name}_{childElement.Name}.png";
                    var filePath = Path.Combine(screenshotsFolderPath, fileName);
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private void GenerateGuideButton_Click(object sender, EventArgs e)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var screenshotsFolderPath = Path.Combine(desktopPath, "Screenshots");
            var guideFilePath = Path.Combine(screenshotsFolderPath, "Руководство пользователя.md");

            if (!Directory.Exists(screenshotsFolderPath) || !Directory.EnumerateFiles(screenshotsFolderPath).Any())
            {
                MessageBox.Show("Скриншоты не найдены");
                return;
            }

            var currentScreenshotFiles = new HashSet<string>(Directory.EnumerateFiles(screenshotsFolderPath, "*.png").Select(Path.GetFileName));
            var existingMdLines = new List<string>();

            if (File.Exists(guideFilePath))
            {
                existingMdLines = File.ReadAllLines(guideFilePath).ToList();
            }

            // Удаление строк из руководства, которые ссылаются на удаленные скриншоты
            var updatedMdLines = existingMdLines.Where(line =>
                !line.StartsWith("![") || line.EndsWith(")") && currentScreenshotFiles.Contains(line.Substring(2, line.Length - 3).Split(']')[0])).ToList();

            // Создание новых или обновление существующего файла руководства
            using (StreamWriter writer = new StreamWriter(guideFilePath))
            {
                // Если файл пуст, добавление начальных строк
                if (!existingMdLines.Any())
                {
                    writer.WriteLine("# Руководство пользователя");
                    writer.WriteLine();
                    writer.WriteLine("## Описание");
                    writer.WriteLine("Это руководство пользователя для программы **ваш текст**");
                    writer.WriteLine();
                }
                else
                {
                    // Переписывание существующих строк, кроме тех, что ссылаются на несуществующие скриншоты
                    foreach (var line in updatedMdLines)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Добавление новых скриншотов, которых еще нет в документе
                foreach (var filePath in Directory.EnumerateFiles(screenshotsFolderPath, "*.png"))
                {
                    var fileName = Path.GetFileName(filePath);
                    var mdLine = $"![{fileName}]({fileName})";
                    if (!updatedMdLines.Contains(mdLine))
                    {
                        writer.WriteLine(mdLine);
                        writer.WriteLine();
                    }
                }
            }

            MessageBox.Show($"Руководство пользователя обновлено");
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            // Извлечение ресурса helpfile
            byte[] helpFileBytes = screenshotproj.Properties.Resources.helpfile;

            if (helpFileBytes != null)
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), "helpfile.chm");
                File.WriteAllBytes(tempFilePath, helpFileBytes);
                Process.Start("hh.exe", tempFilePath);
            }
            else
            {
                MessageBox.Show("Файл справки не найден");
            }
        }

        //private void HelpButton_Click(object sender, EventArgs e)
        //{
        //    string helpFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "helpfile.chm");

        //    if (File.Exists(helpFilePath))
        //    {
        //        Process.Start("hh.exe", helpFilePath);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Файл справки не найден");
        //    }
        //}

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
                    this.Opacity = 0;

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