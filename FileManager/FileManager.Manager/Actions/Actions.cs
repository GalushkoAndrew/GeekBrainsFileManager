using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GeekBrains.Learn.FileManager.Domain;
using GeekBrains.Learn.FileManager.LoggerLib;
using GeekBrains.Learn.FileManager.Manager;
using GeekBrains.Learn.FileManager.Repository;
using GeekBrains.Learn.FileManager.Shared;

namespace GeekBrains.Learn.FileManager
{
    /// <summary>
    /// Класс аккумулирует операции, доступные интерфейсу файлового менеджера
    /// </summary>
    public class Actions
    {
        private readonly ILogManager _logManager;
        private readonly IFileRepository _fileRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IConsoleLogger _console;

        /// <summary>
        /// ctor
        /// </summary>
        public Actions(IConsoleLogger console)
        {
            _logManager = new LogManager();
            _fileRepository = new FileRepository();
            _directoryRepository = new DirectoryRepository();
            _console = console;
        }

        /// <summary>
        /// Делегат для передачи отрисовывающего метода
        /// </summary>
        public delegate void DrawPathDelegate();

        /// <summary>
        /// Делегат для передачи отрисовывающего постранично метода
        /// </summary>
        public delegate void DrawTreeDelegate(bool next);

        /// <summary>
        /// Алгоритм команды Копировать
        /// </summary>
        public bool ActionCopy(string[] paramArray)
        {
            if (!ValidateParamsCount(paramArray, 3))
            {
                return false;
            }

            string source = paramArray[1];
            string dest = paramArray[2];

            ItemPhysics sourcePhysics = GetItemPhysics(source);
            ItemPhysics destPhysics = GetItemPhysics(dest);

            if (!ValidateSourceAndDestinationPaths(source, dest, sourcePhysics, destPhysics))
            {
                return false;
            }

            if (sourcePhysics == ItemPhysics.File && destPhysics == ItemPhysics.Folder)
            {
                dest = Path.Combine(dest, Path.GetFileName(source));
            }

            try
            {
                if (sourcePhysics == ItemPhysics.File)
                {
                    return _fileRepository.Copy(source, dest);
                }

                FolderCopy(source, dest);
                return true;
            }
            catch (Exception ex)
            {
                LogErrorActionCopy(paramArray, ex);
                return false;
            }
        }

        /// <summary>
        /// Алгоритм команды Удаление
        /// </summary>
        public bool ActionDelete(string[] paramArray)
        {
            if (!ValidateParamsCount(paramArray, 2))
            {
                return false;
            }

            string path = paramArray[1];
            try
            {
                if (_fileRepository.Exists(path))
                {
                    _fileRepository.Delete(path);
                    return true;
                }
                else if (_directoryRepository.Exists(path))
                {
                    _directoryRepository.Delete(path, recursive: true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogErrorActionDelete(path, ex);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Алгоритм команды Информация о файле/папке
        /// </summary>
        public bool ActionInfo(string[] paramArray, int lineInfo, DrawPathDelegate drawDelegate)
        {
            if (!ValidateParamsCount(paramArray, 2))
            {
                return false;
            }

            drawDelegate();
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            string value = paramArray[1];
            try
            {
                if (_fileRepository.Exists(value))
                {
                    var fileInfo = _fileRepository.GetFileInfo(value);
                    var fileAttibutes = fileInfo.Attributes;
                    _console.SetCursorPosition(0, lineInfo);
                    _console.SendLine($"Файл: {value}");
                    _console.SendLine($"Атрибуты: {fileAttibutes}");
                    _console.SendLine($"Размер, байт: {fileInfo.Length.ToString("#,0", nfi)}");
                    return true;
                }
                else if (_directoryRepository.Exists(value))
                {
                    var dirInfo = _directoryRepository.GetDirectoryInfo(value);
                    var folderAttibutes = dirInfo.Attributes;
                    long size = GetFolderSize(dirInfo);
                    _console.SetCursorPosition(0, lineInfo);
                    _console.SendLine($"Папка: {value}");
                    _console.SendLine($"Атрибуты: {folderAttibutes}");
                    _console.SendLine($"Размер: {size.ToString("#,0", nfi)}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Алгоритм команды Перейти в папку
        /// </summary>
        public bool ActionCd(string[] paramArray, IOptions options, DrawPathDelegate drawPath, IOptionsManager optionsManager, IList<string> pagedList, DrawTreeDelegate drawTree)
        {
            pagedList.Clear();
            int length = paramArray.Length;
            if (!ValidateParamsCount(paramArray, 1, 2))
            {
                return false;
            }

            try
            {
                if (length == 2)
                {
                    if (_directoryRepository.Exists(paramArray[1]))
                    {
                        var dirInfo = _directoryRepository.GetDirectoryInfo(paramArray[1]);
                        options.Path = dirInfo.FullName;
                        optionsManager.SetOptions(options);
                        drawPath();
                        FolderListRecursion(options.Path, pagedList, false);
                        drawTree(false);
                        return true;
                    }
                }

                if (length == 1)
                {
                    FolderListRecursion(options.Path, pagedList, false);
                    drawTree(false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Алгоритм команды Показать дерево папок
        /// </summary>
        public bool ActionTree(string[] paramArray, IList<string> pagedList, DrawTreeDelegate drawTree)
        {
            pagedList.Clear();
            if (!ValidateParamsCount(paramArray, 2))
            {
                return false;
            }

            try
            {
                if (_directoryRepository.Exists(paramArray[1]))
                {
                    FolderListRecursion(paramArray[1], pagedList, true);
                    drawTree(false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }

            return false;
        }

        ///// <summary>
        ///// Форматирует набор параметров в строку
        ///// </summary>
        //private static string ArraytoString(string[] paramArray, bool isAddSpaceDelimiter = false)
        //{
        //    StringBuilder sb = new();
        //    string delimiter = isAddSpaceDelimiter ? " " : "";

        //    foreach (var item in paramArray)
        //    {
        //        sb.Append(item ?? "");
        //        sb.Append(delimiter);
        //    }

        //    return sb.ToString();
        //}

        /// <summary>
        /// Строит рекурсией список папок и файлов в ней
        /// </summary>
        /// <param name="folderPath">путь к папке для построения дерева</param>
        private void FolderListRecursion(string folderPath, IList<string> pagedList, bool isRecursion)
        {
            string[] arrayPath;
            string[] arrayFiles;
            if (_directoryRepository.Exists(folderPath))
            {
                pagedList.Add(folderPath);
            }

            try
            {
                arrayPath = Directory.GetDirectories(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
                arrayPath = Array.Empty<string>();
            }

            try
            {
                arrayFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
                arrayFiles = Array.Empty<string>();
            }

            foreach (var dir in arrayPath)
            {
                if (isRecursion)
                {
                    FolderListRecursion(dir, pagedList, isRecursion);
                }
                else
                {
                    pagedList.Add(dir);
                }
            }

            foreach (var file in arrayFiles)
            {
                pagedList.Add(file);
            }
        }

        /// <summary>
        /// Копирует содержимое одной папки в другую
        /// </summary>
        private void FolderCopy(string source, string dest)
        {
            var dir = _directoryRepository.GetDirectoryInfo(source);
            Directory.CreateDirectory(dest);

            DirectoryInfo[] directories = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                _fileRepository.Copy(file.FullName, Path.Combine(dest, file.Name));
            }

            foreach (DirectoryInfo subdir in directories)
            {
                FolderCopy(subdir.FullName, Path.Combine(dest, subdir.Name));
            }
        }

        /// <summary>
        /// Возвращает размер папки
        /// </summary>
        private long GetFolderSize(DirectoryInfo dirInfo)
        {
            long size = dirInfo.EnumerateFiles().Sum(x => x.Length);
            var directories = dirInfo.EnumerateDirectories();
            foreach (var subdirInfo in directories)
            {
                size += GetFolderSize(subdirInfo);
            }

            return size;
        }

        /// <summary>
        /// Проверяет, существует ли путь и чем на диске он является
        /// </summary>
        /// <param name="path">Путь</param>
        private ItemPhysics GetItemPhysics(string path)
        {
            if (_fileRepository.Exists(path))
            {
                return ItemPhysics.File;
            }

            if (_directoryRepository.Exists(path))
            {
                return ItemPhysics.Folder;
            }

            return ItemPhysics.SomethingOther;
        }

        /// <summary>
        /// Логические проверки перед копированием
        /// </summary>
        /// <param name="source">путь-источник</param>
        /// <param name="dest">путь-назначение</param>
        /// <param name="sourcePhysics">чем является путь-источник на диске</param>
        /// <param name="destPhysics">чем является путь-назначение на диске</param>
        /// <returns>true если все корректно</returns>
        private bool ValidateSourceAndDestinationPaths(string source, string dest, ItemPhysics sourcePhysics, ItemPhysics destPhysics)
        {
            if (sourcePhysics == ItemPhysics.SomethingOther)
            {
                Log(new string[]
                {
                    ErrorMessages.ErrorsRus()["Error"],
                    ErrorMessages.ErrorsRus()["Non-existent path"],
                    " ",
                    source
                });

                return false;
            }

            if (sourcePhysics == ItemPhysics.Folder && destPhysics == ItemPhysics.File)
            {
                Log(new string[]
                {
                    ErrorMessages.ErrorsRus()["Error"],
                    ErrorMessages.ErrorsRus()["Cannot copy a folder to a file"],
                    " ",
                    source,
                    ErrorMessages.ErrorsRus()["In"],
                    dest
                });

                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка соответствия типа команды количеству параметров
        /// </summary>
        /// <param name="paramArray">массив параметров</param>
        /// <param name="expected">ожидаемое количество параметров</param>
        /// <param name="orExpected">вариация ожидаемого количества параметров</param>
        /// <returns>true if OK (Oll Korrect)</returns>
        private bool ValidateParamsCount(string[] paramArray, int expected, int orExpected = -1)
        {
            if (!(paramArray.Length == expected || paramArray.Length == orExpected))
            {
                Log(new string[]
                {
                    ErrorMessages.ErrorsRus()["Error"],
                    ErrorMessages.ErrorsRus()["Incorrect number of command parameters"],
                    " copy. ",
                    LogManager.ArraytoString(paramArray)
                });

                return false;
            }

            return true;
        }

        private void LogErrorActionCopy(string[] paramArray, Exception ex)
        {
            Log(new string[]
            {
                ErrorMessages.ErrorsRus()["Error"],
                ErrorMessages.ErrorsRus()["Failed to copy"],
                " ",
                paramArray[1] ?? "",
                ErrorMessages.ErrorsRus()["In"],
                paramArray[2] ?? "",
                ". ",
                ex.Message
            });
        }

        private void LogErrorActionDelete(string path, Exception ex)
        {
            Log(new string[]
            {
                ErrorMessages.ErrorsRus()["Error"],
                ErrorMessages.ErrorsRus()["Failed to delete"],
                " ",
                path ?? "",
                ". ",
                ex.Message
            });
        }

        private void LogError(Exception ex)
        {
            Log(new string[]
            {
                ErrorMessages.ErrorsRus()["Error"],
                ex.Message
            });
        }

        private void Log(string[] array) => _logManager.Log(LogManager.ArraytoString(array));
    }
}
