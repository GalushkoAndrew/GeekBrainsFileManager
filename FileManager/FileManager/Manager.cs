using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBrains.Learn.FileManager
{
    /// <summary>
    /// Файловый менеджер
    /// </summary>
    public class Manager
    {
        public StringBuilder sb;
        public Options options;
        public OptionsManager optionsManager;
        public Actions actions;

        private readonly List<string> listCommands;
        private readonly List<string> pagedList;

        private int listCommandsIndex = -1;
        private int page = -1;

        private readonly int linesCommandMin = 3;
        private readonly int linesInfo = 5;
        private readonly int linesTree;
        private readonly int linesAll;

        public Manager()
        {
            optionsManager = new();
            actions = new();
            listCommands = new();
            pagedList = new();
            sb = new();

            options = optionsManager.Get();
            linesTree = options.CountLines;
            linesAll = linesTree + linesInfo + linesCommandMin + 3;
        }

        /// <summary>
        /// Рисует выбранную папку
        /// </summary>
        private void DrawPath()
        {
            string emptyString = new(' ', Console.BufferWidth);
            Console.SetCursorPosition(0, linesTree + linesInfo);
            Console.WriteLine(emptyString);
            Console.SetCursorPosition(0, linesTree + linesInfo);
            options.Path ??= "";
            Console.WriteLine("Текущая папка: " + (options.Path ?? ""));
        }

        /// <summary>
        /// рисует основной каркас интерфейса
        /// </summary>
        private void Draw()
        {

            int width = 120;

            if (IsWindows(Environment.OSVersion.ToString()))
            {
                Console.SetBufferSize(1000, 8000);
                Console.SetWindowSize(width, linesAll);
            }

            Console.Clear();

            // дерево
            Console.SetCursorPosition(0, linesTree);
            Console.WriteLine(new string('-', width));

            // инфо
            Console.SetCursorPosition(0, linesTree + linesInfo + 1);
            Console.WriteLine(new string('-', width));

            DrawPath();
            SetCursorToCommandLine();
        }

        /// <summary>
        /// постранично рисует дерево папок
        /// </summary>
        /// <param name="next"></param>
        private void DrawTree(bool next = false)
        {
            if (page == -1)
            {
                page = 1;
            }
            else
            {
                if (next && linesTree * page < pagedList.Count)
                {
                    page++;
                }
            }

            int start = (page - 1) * linesTree;
            int end = Math.Min(start + linesTree - 1, pagedList.Count - 1);
            ClearTree();
            SetCursorToTree();

            for (int i = start; i <= end; i++)
            {
                Console.WriteLine(pagedList[i]);
            }

            if (linesTree * page >= pagedList.Count)
            {
                page = -1;
                SetCursorToCommandLine();
                return;
            }
        }

        /// <summary>
        /// Определяет известные команды и выполняет их
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool DoAction(string command)
        {
            var paramArr = Parser.Parse(command);
            if (paramArr.Length < 1)
            {
                return false;
            }

            return paramArr[0].ToLower() switch
            {
                "copy" => actions.ActionCopy(paramArr),
                "delete" => actions.ActionDelete(paramArr),
                "info" => actions.ActionInfo(paramArr, linesTree + 1, ClearInfo),
                "cd" => actions.ActionCd(paramArr, options, DrawPath, optionsManager, pagedList, DrawTree),
                "tree" => actions.ActionTree(paramArr, pagedList, DrawTree),
                _ => false,
            };
        }

        /// <summary>
        /// Запуск работы файлового менеджера
        /// </summary>
        public void Start()
        {
            Draw();
            actions.ActionCd(new string[] { "cd" }, options, DrawPath, optionsManager, pagedList, DrawTree);
            page = -1;
            SetCursorToCommandLine();

            bool isEnd = false;
            while (!isEnd)
            {
                var key = Console.ReadKey(true);
                isEnd = KeyAction(key);
            }

            Console.WriteLine();
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Устанавливает курсор на начало командной строки
        /// </summary>
        private void SetCursorToCommandLine()
        {
            Console.SetCursorPosition(0, linesTree + linesInfo + 2);
        }

        /// <summary>
        /// Устанавливает курсор на начало информационной строки
        /// </summary>
        private void SetCursorToInfo()
        {
            Console.SetCursorPosition(0, linesTree + 1);
        }

        /// <summary>
        /// Устанавливает курсор на начало отображания структуры папок и файлов
        /// </summary>
        private void SetCursorToTree()
        {
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Очищает командную строку
        /// </summary>
        private void ClearCommand()
        {
            var textLength = sb.Length;
            var lines = textLength / Console.BufferWidth;
            string emptyString = new(' ', Console.BufferWidth);
            var lastCursorPosition = Console.GetCursorPosition();
            SetCursorToCommandLine();
            for (int i = 0; i < lines + 1; i++)
            {
                Console.WriteLine(emptyString);
            }
            sb.Clear();
            SetCursorToCommandLine();

            if (page > -1)
            {
                Console.SetCursorPosition(lastCursorPosition.Left, lastCursorPosition.Top);
            }
        }

        /// <summary>
        /// Прокручивает введенные ранее в консоли значения
        /// </summary>
        /// <param name="direction">-1 - назад, 1 - вперед</param>
        private void MoveByHistory(int direction)
        {
            if (listCommands.Count < 1)
            {
                listCommandsIndex = -1;
                return;
            }

            if (listCommandsIndex == -1)
            {
                if (direction == -1)
                {
                    listCommandsIndex = listCommands.Count - 1;
                    DrawCommand(listCommands[listCommandsIndex]);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (listCommandsIndex > 0 && direction == -1)
            {
                listCommandsIndex--;
                DrawCommand(listCommands[listCommandsIndex]);
                return;
            }

            if (listCommandsIndex > -1 && direction == 1 && listCommandsIndex < (listCommands.Count - 1))
            {
                listCommandsIndex++;
                DrawCommand(listCommands[listCommandsIndex]);
                return;
            }
        }

        /// <summary>
        /// Отрисовывает вводимые в командной строке символы
        /// </summary>
        /// <param name="value"></param>
        private void DrawCommand(string value)
        {
            ClearCommand();
            sb.Append(value);
            SetCursorToCommandLine();
            Console.Write(value);
        }

        /// <summary>
        /// Очищает информационный блок
        /// </summary>
        private void ClearInfo()
        {
            string emptyString = new(' ', Console.BufferWidth);
            SetCursorToInfo();
            for (int i = 0; i < linesInfo - 1; i++)
            {
                Console.WriteLine(emptyString);
            }
        }

        /// <summary>
        /// Очищает блок с деревом папок и файлов
        /// </summary>
        private void ClearTree()
        {
            string emptyString = new(' ', Console.BufferWidth);
            SetCursorToTree();
            for (int i = 0; i < linesTree; i++)
            {
                Console.WriteLine(emptyString);
            }
        }

        /// <summary>
        /// Реагирует на нажатие клавиш
        /// </summary>
        private bool KeyAction(ConsoleKeyInfo key)
        {
            try
            {
                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        if (page == -1)
                        {
                            sb.Remove(sb.Length - 1, 1);
                            Console.Write(key.KeyChar);
                            Console.Write(" \b");
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (page == -1)
                        {
                            _ = DoAction(sb.ToString());
                            listCommands.Add(sb.ToString());
                            listCommandsIndex = -1;
                            ClearCommand();
                        }
                        else
                        {
                            DrawTree(true);
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (page > -1)
                        {
                            page = -1;
                        }

                        ClearCommand();
                        listCommandsIndex = -1;
                        break;
                    case ConsoleKey.UpArrow:
                        if (page == -1)
                            MoveByHistory(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        if (page == -1)
                            MoveByHistory(1);
                        break;
                    case ConsoleKey.F10:
                        return true;
                    case ConsoleKey.F12:
                        page = -1;
                        break;
                    default:
                        if (page == -1)
                        {
                            sb.Append(key.KeyChar);
                            Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка. {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Определяет, является ли операционная система Windows
        /// </summary>
        private bool IsWindows(string osVersion)
        {
            if (osVersion.ToLower().Contains("windows"))
            {
                return true;
            }

            return false;
        }
    }
}
