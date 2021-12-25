using GeekBrains.Learn.FileManager.Shared;

namespace GeekBrains.Learn.FileManager.Domain
{
    /// <summary>
    /// FileManager options
    /// </summary>
    public class Options : IOptions
    {
        private static readonly string PathEmpty = "";

        /// <summary>
        /// ctor
        /// </summary>
        public Options()
        {
            Path = PathEmpty;
            CountLines = Constants.DrawingLinesCountDefault;
        }

        /// <summary>
        /// Empty options
        /// </summary>
        public static Options Empty
            => new() { Path = PathEmpty, CountLines = Constants.DrawingLinesCountDefault };

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public int CountLines { get; set; }
    }
}
