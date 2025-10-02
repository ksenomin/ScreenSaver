namespace ScreenSaver
{
    public partial class MainForm : Form
    {
        private List<Snowflake> snowflakes = new List<Snowflake>();
        private Random random = new Random();
        private System.Windows.Forms.Timer timer;
        private Image background;
        private Image snowflakeImage;
        private Bitmap sceneBuffer;

        /// <summary>
        /// Главная форма
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            LoadImages();
            InitializeSceneBuffer();
            InitializeSnowflakes();
            StartSnowfall();
        }

        private void LoadImages()
        {
            try
            {
                using (var ms = new MemoryStream(Properties.Resources.CountryHouse))
                    background = Image.FromStream(ms);

                using (var ms = new MemoryStream(Properties.Resources.Snowflake))
                    snowflakeImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private void InitializeSceneBuffer()
        {
            sceneBuffer = new Bitmap(Width, Height);
        }

        private void InitializeSnowflakes()
        {
            for (int i = 0; i < 100; i++)
            {
                AddSnowflake(true);
            }
        }

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
                var snow = snowflakes[i];
                snow.Y += snow.Speed;

                if (snow.Y > Height)
                {
                    snowflakes.RemoveAt(i);
                    AddSnowflake();
                }
            }
        }

        private void DrawToBuffer()
        {
            using (var sceneGraphics = Graphics.FromImage(sceneBuffer))
            {
                // фон
                sceneGraphics.DrawImage(background, 0, 0, Width, Height);

                // снежинки
                foreach (var snow in snowflakes)
                {
                    sceneGraphics.DrawImage(snowflakeImage, snow.X, snow.Y, snow.Size, snow.Size);
                }
            }

            using (var formGraphics = CreateGraphics())
            {
                formGraphics.DrawImage(sceneBuffer, 0, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (sceneBuffer != null)
            {
                sceneBuffer.Dispose();
            }
            InitializeSceneBuffer();

            // пересоздает снежинки при изменении размера
            snowflakes.Clear();
            InitializeSnowflakes();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
                Close();
        }
    }
}