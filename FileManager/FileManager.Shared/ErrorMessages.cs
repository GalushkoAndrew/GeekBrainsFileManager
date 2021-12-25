using System.Collections.Generic;

namespace GeekBrains.Learn.FileManager.Shared
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<string, string> _errorsRus = new()
        {
            { "Error", "Ошибка. " },
            { "In", " в " },
            { "Non-existent path", "Несуществующий путь" },
            { "Cannot copy a folder to a file", "Нельзя скопировать папку в файл или в несуществующую папку" },
            { "Incorrect number of command parameters", "Неверное количество параметров команды" },
            { "Failed to delete", "Не удалось удалить" },
            { "Failed to copy", "Не удалось скопировать" },
            { "The command has unclosed quotes", "В команде незакрытые кавычки" }
        };

        public static Dictionary<string, string> ErrorsRus() => _errorsRus;
    }
}
