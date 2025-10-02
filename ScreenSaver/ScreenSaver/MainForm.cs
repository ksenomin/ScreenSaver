using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ScreenSaver
{
    public partial class MainForm : Form
    {
        private List<Snowflake> snowflakes = new List<Snowflake>();
        private Random random = new Random();
        private System.Windows.Forms.Timer timer;
        private Image background;
        private Image snowflakeImage;

        private Bitmap bufferBitmap;
        private Graphics bufferGraphics;

        /// <summary>
        /// Главная форма 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
            LoadImages();
            InitializeBuffer();
            InitializeSnowflakes();
            StartSnowfall();
        }

        private void InitializeForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private void InitializeBuffer()
        {
            // для буферизации
            bufferBitmap = new Bitmap(Width, Height);
            bufferGraphics = Graphics.FromImage(bufferBitmap);
        }

        /// <summary>
        /// Загрузка изображений из ресурсов
        /// </summary>
        private void LoadImages()
        {
            try
            {
                // Преобразуем byte[] в Image
                if (Properties.Resources.CountryHouse != null)
                {
                    using (var ms = new MemoryStream(Properties.Resources.CountryHouse))
                    {
                        background = Image.FromStream(ms);
                    }
                }

                if (Properties.Resources.Snowflake != null)
                {
                    using (var ms = new MemoryStream(Properties.Resources.Snowflake))
                    {
                        snowflakeImage = Image.FromStream(ms);
                    }
                }

                if (background == null)
                    throw new Exception("Не удалось загрузить CountryHouse из ресурсов");
                if (snowflakeImage == null)
                    throw new Exception("Не удалось загрузить Snowflake из ресурсов");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений из ресурсов:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Инициализация всех снежинок
        /// </summary>
        private void InitializeSnowflakes()
        {
            for (int i = 0; i < 100; i++)
            {
                AddSnowflake(true);
            }
        }

        /// <summary>
        /// Создание снежинок
        /// </summary>
        private void AddSnowflake(bool initial = false)
        {
            var snowflake = new Snowflake();

            int sizeType = random.Next(3);
            if (sizeType == 0)
            {
                // маленькие
                snowflake.Size = random.Next(10, 20);
                snowflake.Speed = random.Next(2, 4);
            }
            else if (sizeType == 1)
            {
                // средние 
                snowflake.Size = random.Next(20, 30);
                snowflake.Speed = random.Next(3, 6);
            }
            else
            {
                // большие
                snowflake.Size = random.Next(30, 40);
                snowflake.Speed = random.Next(5, 8);
            }

            if (initial)
            {
                snowflake.X = random.Next(0, Width);
                snowflake.Y = random.Next(-500, -snowflake.Size);
            }
            else
            {
                snowflake.X = random.Next(0, Width);
                snowflake.Y = -snowflake.Size;
            }

            snowflakes.Add(snowflake);
        }

        /// <summary>
        /// Запуск снегопада
        /// </summary>
        private void StartSnowfall()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 30;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateAnimation();
            DrawToBuffer();
        }

        private void UpdateAnimation()
        {
            // обновление позиций снежинок
            for (int i = snowflakes.Count - 1; i >= 0; i--)
            {
                var snow = snowflakes[i];
                snow.Y += snow.Speed;

                // создание снежинки после выпадания
                if (snow.Y > Height)
                {
                    snowflakes.RemoveAt(i);
                    AddSnowflake();
                }
            }
        }

        private void DrawToBuffer()
        {
            bufferGraphics.Clear(Color.Black);

            // фон
            if (background != null)
            {
                bufferGraphics.DrawImage(background, 0, 0, Width, Height);
            }

            // снежинки рисуем
            if (snowflakeImage != null)
            {
                foreach (var snow in snowflakes)
                {
                    bufferGraphics.DrawImage(snowflakeImage, snow.X, snow.Y, snow.Size, snow.Size);
                }
            }

            // Принудительно вызываем перерисовку формы
            this.Invalidate();
        }

        /// <summary>
        /// Отрисовывает содержание буфера на форму
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (bufferBitmap != null)
            {
                e.Graphics.DrawImage(bufferBitmap, 0, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (bufferBitmap != null)
            {
                bufferBitmap.Dispose();
                bufferGraphics.Dispose();
            }
            InitializeBuffer();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Close();
        }

   
    }
}