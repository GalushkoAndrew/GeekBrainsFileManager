using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBrains.Learn.FileManager
{
    public class Parser
    {
        /// <summary>
        /// Парсер консольной команды
        /// </summary>
        /// <param name="text">Текстовая команда</param>
        /// <returns>Массив строк-параметров команды. Если есть незакрытые кавычки, возвращает ошибку</returns>
        public static string[] Parse(string text)
        {
            StringBuilder sb = new();
            bool isStartParam = false; // обнаружены открывающие кавычки
            int paramsCount = 0; // количество значимых элементов команды. Включая команду и ее параметры
            List<string> list = new(); // список элементов

            foreach (var symbol in text)
            {
                if (!isStartParam && symbol == '"')
                {
                    AddParam(sb, list, true, ref paramsCount);
                    isStartParam = true;
                }
                else if (isStartParam)
                {
                    if (symbol == '"')
                    {
                        AddParam(sb, list, false, ref paramsCount);
                        isStartParam = false;
                    }
                    else
                    {
                        sb.Append(symbol);
                    }
                }
                else if (symbol == ' ' || symbol == '\t' || symbol == '\r' || symbol == '\n')
                {
                    AddParam(sb, list, true, ref paramsCount);
                }
                else
                {
                    sb.Append(symbol);
                }
            }

            if (isStartParam)
            {
                throw new Exception("Ошибка в команде. Незакрытые кавычки");
            }

            AddParam(sb, list, true, ref paramsCount);
            return list.ToArray();
        }

        /// <summary>
        /// Добавляет параметр в список
        /// </summary>
        private static void AddParam(StringBuilder sb, List<string> list, bool isCheckLength, ref int paramsCount)
        {
            if (isCheckLength && sb.Length < 1)
            {
                return;
            }

            list.Add(sb.ToString());
            paramsCount++;
            sb.Clear();
        }
    }
}
