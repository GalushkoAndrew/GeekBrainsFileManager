namespace GeekBrains.Learn.FileManager.Domain
{
    /// <summary>
    /// FileManager options interface
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// Path to work directory
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Drawing lines count
        /// </summary>
        int CountLines { get; set; }
    }
}
