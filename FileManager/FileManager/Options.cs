namespace GeekBrains.Learn.FileManager
{
    /// <summary>
    /// Настройки программы
    /// </summary>
    public class Options
    {
        public string Path { get; set; }
        public int CountLines { get; set; }

        public Options()
        {
            Path = "";
            CountLines = 39;
        }
    }
}
