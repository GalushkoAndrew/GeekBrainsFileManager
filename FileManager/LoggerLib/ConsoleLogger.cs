using System;
using System.Text;

namespace GeekBrains.Learn.FileManager.LoggerLib
{
    /// <summary>
    /// Console logger
    /// </summary>
    public sealed class ConsoleLogger : Logger, IConsoleLogger
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ConsoleLogger()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public ConsoleLogger(ConsoleWindowPreset windowPreset)
        {
            ApplyWindowPreset(windowPreset);
        }

        /// <inheritdoc/>
        public int BufferWidth
        {
            get => Console.BufferWidth;
            set
            {
#pragma warning disable CA1416 // Проверка совместимости платформы
                Console.BufferWidth = value;
#pragma warning restore CA1416 // Проверка совместимости платформы
            }
        }

        /// <inheritdoc/>
        public override void SendLine(string value = null)
        {
            Console.WriteLine(value);
        }

        /// <inheritdoc/>
        public void Send(string value)
        {
            Console.Write(value);
        }

        /// <inheritdoc/>
        public void Send(char value)
        {
            Send(value.ToString());
        }

        /// <inheritdoc/>
        public void DrawLine(int lineLength = 20)
        {
            SendLine(new StringBuilder().Append('-', lineLength).ToString());
        }

        /// <inheritdoc/>
        public void SetBufferSize(int width, int height)
        {
            if (IsWindows(Environment.OSVersion.ToString()))
            {
#pragma warning disable CA1416 // Проверка совместимости платформы
                Console.SetBufferSize(width, height);
#pragma warning restore CA1416 // Проверка совместимости платформы
            }
        }

        /// <inheritdoc/>
        public void SetWindowSize(int width, int height)
        {
            if (IsWindows(Environment.OSVersion.ToString()))
            {
#pragma warning disable CA1416 // Проверка совместимости платформы
                Console.SetWindowSize(width, height);
#pragma warning restore CA1416 // Проверка совместимости платформы
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Console.Clear();
        }

        /// <inheritdoc/>
        public void SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(left, top);
        }

        /// <inheritdoc/>
        public ConsoleKeyInfo ReadKey(bool intersept)
        {
            return Console.ReadKey(intersept);
        }

        /// <inheritdoc/>
        public (int Left, int Top) GetCursorPosition()
        {
            return Console.GetCursorPosition();
        }

        /// <summary>
        /// Определяет, является ли операционная система Windows
        /// </summary>
        private static bool IsWindows(string osVersion)
        {
            if (osVersion.ToLower().Contains("windows"))
            {
                return true;
            }

            return false;
        }

        private void ApplyWindowPreset(ConsoleWindowPreset windowPreset)
        {
            windowPreset ??= ConsoleWindowPreset.FileManagerPreset();
            SetBufferSize(windowPreset.BufferWidth, windowPreset.BufferHeight);
            SetWindowSize(windowPreset.WindowWidth, windowPreset.WindowHeight);
        }
    }
}