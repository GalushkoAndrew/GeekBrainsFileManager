namespace GeekBrains.Learn.FileManager.LoggerLib
{
    /// <summary>
    /// Settings windows console
    /// </summary>
    public class ConsoleWindowPreset
    {
        /// <summary>
        /// Buffer width
        /// </summary>
        public int BufferWidth { get; set; }

        /// <summary>
        /// Buffer height
        /// </summary>
        public int BufferHeight { get; set; }

        /// <summary>
        /// Window width
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// Window height
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// Preset windows console for file manager
        /// </summary>
        public static ConsoleWindowPreset FileManagerPreset()
            => new()
            {
                BufferWidth = 1000,
                BufferHeight = 8000,
                WindowWidth = 120,
                WindowHeight = 50
            };
    }
}
