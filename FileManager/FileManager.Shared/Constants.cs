using System;

namespace GeekBrains.Learn.FileManager.Shared
{
    public static class Constants
    {
        /// <summary>
        /// Клоичество строк в информационном блоке
        /// </summary>
        public static int InfoLinesCount => 5;

        /// <summary>
        /// Ширина консоли
        /// </summary>
        public static int WindowWidthDefault => 120;

        /// <summary>
        /// Количество строк в блоке дерева папок и файлов
        /// </summary>
        public static int DrawingLinesCountDefault => 39;

        /// <summary>
        /// Имя файла с настройками
        /// </summary>
        public static string OptionsFileName => "path.json";

        /// <summary>
        /// имя файла с логом ошибок
        /// </summary>
        public static string LogFileName => "errors/log_exception.txt";
    }
}
