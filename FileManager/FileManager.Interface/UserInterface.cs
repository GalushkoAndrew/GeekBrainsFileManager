using System;
using System.Collections.Generic;
using System.Text;
using GeekBrains.Learn.FileManager.Domain;
using GeekBrains.Learn.FileManager.LoggerLib;
using GeekBrains.Learn.FileManager.Manager;
using GeekBrains.Learn.FileManager.Shared;

namespace GeekBrains.Learn.FileManager.Interface
{
    /// <summary>
    /// Визуальная часть файлового менеджера
    /// </summary>
    public class UserInterface : IUserInterface
    {
        /// <summary>
        /// StringBuilder
        /// </summary>
        public StringBuilder _sb;

        private readonly IOptionsManager _optionsManager;
        private readonly Actions _actions;
        private readonly IConsoleLogger _console;

        private readonly IList<string> _listCommands;
        private readonly IList<string> _pagedList;
        private readonly ILogManager _logManager;

        private readonly int _linesInfo = Constants.InfoLinesCount;
        private readonly int _linesTree;
        private readonly IOptions _options;

        /// <summary>
        /// Индекс команды, отображаемой из истории команд
        /// </summary>
        private int _listCommandsIndex = -1;

        /// <summary>
        /// Если отображение в постраничном режиме, хранит номер отображаемой страницы,
        /// если не в постраничном режиме, хранит -1
        /// </summary>
        private int _page = -1;

        /// <summary>
        /// ctor
        /// </summary>
        public UserInterface()
        {
            _console = new ConsoleLogger(null);
            _optionsManager = new OptionsManager();
            _actions = new(_console);
            _listCommands = new List<string>();
            _pagedList = new List<string>();
            _sb = new();
            _logManager = new LogManager();

            _options = _optionsManager.GetOptions();
            _linesTree = _options.CountLines;
        }

        /// <inheritdoc/>
        public void Start()
        {
            Draw();
            _actions.ActionCd(new string[] { "cd" }, _options, DrawPath, _optionsManager, _pagedList, DrawTree);
            _page = -1;
            SetCursorToCommandLine();
            bool isEnd = false;
            while (!isEnd)
            {
                var key = _console.ReadKey(true);
                isEnd = KeyAction(key);
            }

            _console.SendLine();
            _console.SendLine(_sb.ToString());
        }

        /// <summary>
        /// рисует основной каркас интерфейса
        /// </summary>
        private void Draw()
        {
            int width = Constants.WindowWidthDefault;

            _console.Clear();

            // дерево
            _console.SetCursorPosition(0, _linesTree);
            _console.SendLine(new string('-', width));

            // инфо
            _console.SetCursorPosition(0, _linesTree + _linesInfo + 1);
            _console.SendLine(new string('-', width));

            DrawPath();
            SetCursorToCommandLine();
        }

        /// <summary>
        /// Рисует выбранную папку
        /// </summary>
        private void DrawPath()
        {
            string emptyString = new(' ', _console.BufferWidth);
            _console.SetCursorPosition(0, _linesTree + _linesInfo);
            _console.SendLine(emptyString);
            _console.SetCursorPosition(0, _linesTree + _linesInfo);
            _console.SendLine("Текущая папка: " + _optionsManager.GetWorkPath());
        }

        /// <summary>
        /// Устанавливает курсор на начало командной строки
        /// </summary>
        private void SetCursorToCommandLine()
        {
            _console.SetCursorPosition(0, _linesTree + _linesInfo + 2);
        }

        /// <summary>
        /// постранично рисует дерево папок
        /// </summary>
        /// <param name="next">Command to draw next page</param>
        private void DrawTree(bool next = false)
        {
            if (_page == -1)
            {
                _page = 1;
            }
            else
            {
                if (next && _linesTree * _page < _pagedList.Count)
                {
                    _page++;
                }
            }

            int start = (_page - 1) * _linesTree;
            int end = Math.Min(start + _linesTree - 1, _pagedList.Count - 1);
            ClearTree();
            SetCursorToTree();

            for (int i = start; i <= end; i++)
            {
                _console.SendLine(_pagedList[i]);
            }

            if (_linesTree * _page >= _pagedList.Count)
            {
                _page = -1;
                SetCursorToCommandLine();
                return;
            }
        }

        /// <summary>
        /// Очищает блок с деревом папок и файлов
        /// </summary>
        private void ClearTree()
        {
            string emptyString = new(' ', _console.BufferWidth);
            SetCursorToTree();
            for (int i = 0; i < _linesTree; i++)
            {
                _console.SendLine(emptyString);
            }
        }

        /// <summary>
        /// Устанавливает курсор на начало отображания структуры папок и файлов
        /// </summary>
        private void SetCursorToTree()
        {
            _console.SetCursorPosition(0, 0);
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
                        if (_page == -1)
                        {
                            _sb.Remove(_sb.Length - 1, 1);
                            _console.Send(key.KeyChar);
                            _console.Send(" \b");
                        }

                        break;
                    case ConsoleKey.Enter:
                        if (_page == -1)
                        {
                            _ = DoAction(_sb.ToString());
                            _listCommands.Add(_sb.ToString());
                            _listCommandsIndex = -1;
                            ClearCommand();
                        }
                        else
                        {
                            DrawTree(true);
                        }

                        break;
                    case ConsoleKey.Escape:
                        if (_page > -1)
                        {
                            _page = -1;
                        }

                        ClearCommand();
                        _listCommandsIndex = -1;
                        break;
                    case ConsoleKey.UpArrow:
                        if (_page == -1)
                        {
                            MoveByHistory(-1);
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (_page == -1)
                        {
                            MoveByHistory(1);
                        }

                        break;
                    case ConsoleKey.F10:
                        return true;
                    case ConsoleKey.F12:
                        _page = -1;
                        break;
                    default:
                        if (_page == -1)
                        {
                            _sb.Append(key.KeyChar);
                            _console.Send(key.KeyChar);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                _logManager.Log($"Ошибка. {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Очищает командную строку
        /// </summary>
        private void ClearCommand()
        {
            var textLength = _sb.Length;
            var lines = textLength / _console.BufferWidth;
            string emptyString = new(' ', _console.BufferWidth);
            var (left, top) = _console.GetCursorPosition();
            SetCursorToCommandLine();
            for (int i = 0; i < lines + 1; i++)
            {
                _console.SendLine(emptyString);
            }

            _sb.Clear();
            SetCursorToCommandLine();

            if (_page > -1)
            {
                _console.SetCursorPosition(left, top);
            }
        }

        /// <summary>
        /// Прокручивает введенные ранее в консоли значения
        /// </summary>
        /// <param name="direction">-1 - назад, 1 - вперед</param>
        private void MoveByHistory(int direction)
        {
            if (_listCommands.Count < 1)
            {
                _listCommandsIndex = -1;
                return;
            }

            if (_listCommandsIndex == -1)
            {
                if (direction == -1)
                {
                    _listCommandsIndex = _listCommands.Count - 1;
                    DrawCommand(_listCommands[_listCommandsIndex]);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (_listCommandsIndex > 0 && direction == -1)
            {
                _listCommandsIndex--;
                DrawCommand(_listCommands[_listCommandsIndex]);
                return;
            }

            if (_listCommandsIndex > -1 && direction == 1 && _listCommandsIndex < (_listCommands.Count - 1))
            {
                _listCommandsIndex++;
                DrawCommand(_listCommands[_listCommandsIndex]);
                return;
            }
        }

        /// <summary>
        /// Отрисовывает вводимые в командной строке символы
        /// </summary>
        /// <param name="value">Text</param>
        private void DrawCommand(string value)
        {
            ClearCommand();
            _sb.Append(value);
            SetCursorToCommandLine();
            _console.Send(value);
        }

        /// <summary>
        /// Определяет известные команды введенные в командной строке и выполняет их
        /// </summary>
        /// <param name="command">Command</param>
        private bool DoAction(string command)
        {
            var paramArr = Parser.Parse(command);
            if (paramArr.Length < 1)
            {
                return false;
            }

            return paramArr[0].ToLower() switch
            {
                "copy" => _actions.ActionCopy(paramArr),
                "delete" => _actions.ActionDelete(paramArr),
                "info" => _actions.ActionInfo(paramArr, _linesTree + 1, ClearInfo),
                "cd" => _actions.ActionCd(paramArr, _options, DrawPath, _optionsManager, _pagedList, DrawTree),
                "tree" => _actions.ActionTree(paramArr, _pagedList, DrawTree),
                _ => false,
            };
        }

        /// <summary>
        /// Очищает информационный блок
        /// </summary>
        private void ClearInfo()
        {
            string emptyString = new(' ', _console.BufferWidth);
            SetCursorToInfo();
            for (int i = 0; i < _linesInfo - 1; i++)
            {
                _console.SendLine(emptyString);
            }
        }

        /// <summary>
        /// Устанавливает курсор на начало информационной строки
        /// </summary>
        private void SetCursorToInfo()
        {
            _console.SetCursorPosition(0, _linesTree + 1);
        }
    }
}
