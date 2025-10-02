namespace ScreenSaver
{
    internal static class Program
    {
        /// <summary>
        /// Точка входа в программу
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}