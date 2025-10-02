namespace ScreenSaver
{
    public partial class MainForm : Form
    {
        private List<Snowflake> snowflakes = new List<Snowflake>();

        private Random random = new Random();
        private System.Windows.Forms.Timer timer;

        private Image backgroundImage;
        private Image snowflakeImage;
        private Bitmap imageBuffer;

        /// <summary>
        /// Главная форма
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            LoadImages();
            InitializeImageBuffer();
            InitializeSnowflakes();
            StartSnowfall();
        }

        private void LoadImages()
        {
            try
            {
                using (var ms = new MemoryStream(Properties.Resources.CountryHouse))
                    backgroundImage = Image.FromStream(ms);

                using (var ms = new MemoryStream(Properties.Resources.Snowflake))
                    snowflakeImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Инициализация буфера
        /// </summary>
        private void InitializeImageBuffer()
        {
            imageBuffer = new Bitmap(Width, Height);
        }

        /// <summary>
        /// Инициализация всех снежинок
        /// </summary>
        private void InitializeSnowflakes()
        {
            for (int i = 0; i < 130; i++)
            {
                AddSnowflake(true);
            }
        }

        /// <summary>
        /// Создание снежинок, задает размер и скорость
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
                snowflake.Size = random.Next(50, 70);
                snowflake.Speed = random.Next(12, 15);
            }

            if (initial)
            {
                snowflake.X = random.Next(0, Width);
                snowflake.Y = random.Next(-200, -snowflake.Size);
            }
            else
            {
                snowflake.X = random.Next(0, Width);
                snowflake.Y = -snowflake.Size;
            }

            snowflakes.Add(snowflake);
        }

        /// <summary>
        /// Начало движения
        /// </summary>
        private void StartSnowfall()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
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
            for (int i = snowflakes.Count - 1; i >= 0; i--)
            {
                var snowFall = snowflakes[i];
                snowFall.Y += snowFall.Speed;

                if (snowFall.Y > Height)
                {
                    snowflakes.RemoveAt(i);
                    AddSnowflake();
                }
            }
        }

        /// <summary>
        /// Загрузка картинки в буфер
        /// </summary>
        private void DrawToBuffer()
        {
            using (var sceneGraphics = Graphics.FromImage(imageBuffer))
            {
                // фон
                sceneGraphics.DrawImage(backgroundImage, 0, 0, Width, Height);

                // снежинки
                foreach (var snow in snowflakes)
                {
                    sceneGraphics.DrawImage(snowflakeImage, snow.X, snow.Y, snow.Size, snow.Size);
                }
            }

            // выгрузка
            using (var formGraphics = CreateGraphics())
            {
                formGraphics.DrawImage(imageBuffer, 0, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (imageBuffer != null)
            {
                imageBuffer.Dispose();
            }
            InitializeImageBuffer();

            // пересоздает снежинки при изменении размера
            snowflakes.Clear();
            InitializeSnowflakes();
        }

        /// <summary>
        /// Закрытите формы при нажатии на любую клавишу
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
                Close();
        }
    }
}