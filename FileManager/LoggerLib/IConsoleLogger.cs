using System;

namespace GeekBrains.Learn.FileManager.LoggerLib
{
    /// <summary>
    /// Logger interface for Console
    /// </summary>
    public interface IConsoleLogger : ILogger
    {
        /// <summary>
        /// Buffer width
        /// </summary>
        int BufferWidth { get; set; }

        /// <summary>
        /// Draw text staying this line
        /// </summary>
        /// <param name="value">Text</param>
        void Send(string value);

        /// <summary>
        /// Draw text staying this line
        /// </summary>
        /// <param name="value">Text</param>
        void Send(char value);

        /// <summary>
        /// Draws line, using '-'
        /// </summary>
        /// <param name="lineLength">Simbol's count</param>
        void DrawLine(int lineLength = 20);

        /// <summary>
        /// Sets buffer size
        /// </summary>
        void SetBufferSize(int width, int height);

        /// <summary>
        /// Sets window size
        /// </summary>
        void SetWindowSize(int width, int height);

        /// <summary>
        /// Clear console
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets the position of the cursor
        /// </summary>
        void SetCursorPosition(int left, int top);

        /// <summary>
        /// Returns input key
        /// </summary>
        ConsoleKeyInfo ReadKey(bool intersept);

        /// <summary>
        /// Gets the position of the cursor
        /// </summary>
        (int Left, int Top) GetCursorPosition();
    }
}