using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GeekBrains.Learn.FileManager
{
    public class Actions
    {
        public delegate void DrawPathDelegate();
        public delegate void DrawTreeDelegate(bool next);

        /// <summary>
        /// Форматирует набор параметров в строку
        /// </summary>
        private string ArraytoString(string[] paramArray)
        {
            StringBuilder sb = new();

            foreach (var item in paramArray)
            {
                sb.Append((item ?? "") + " ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Алгоритм команды Копировать
        /// </summary>
        public bool ActionCopy(string[] paramArray)
        {
            int length = paramArray.Length;
            if (length != 3)
            {
                Logger.Log($"Ошибка. Неверное количество параметров команды copy. {ArraytoString(paramArray)}");
                return false;
            }

            string source = paramArray[1];
            string dest = paramArray[2];

            bool sourceIsFile = false;
            bool destIsFile = false;

            if (File.Exists(source))
            {
                sourceIsFile = true;
            }
            else if (Directory.Exists(source))
            {
                sourceIsFile = false;
            }
            else
            {
                Logger.Log($"Ошибка. Несуществующий путь {source}");
                return false;
            }

            if (File.Exists(dest))
            {
                destIsFile = true;
            }
            else if (Directory.Exists(dest))
            {
                destIsFile = false;
            }
            else
            {
                Logger.Log($"Ошибка. Несуществующий путь {dest}");
                return false;
            }

            if (!sourceIsFile && destIsFile)
            {
                Logger.Log($"Ошибка. Нельзя скопировать папку в файл {source} в {dest}");
                return false;
            }

            try
            {
                if (sourceIsFile)
                {
                    File.Copy(source, dest);
                    return true;
                }

                if (!sourceIsFile)
                {
                    FolderCopy(source, dest);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка. Не удалось скопировать {paramArray[1]} в {paramArray[2]}. {ex.Message}");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Копирует содержимое одной папки в другую
        /// </summary>
        private void FolderCopy(string source, string dest)
        {
            DirectoryInfo dir = new(source);
            Directory.CreateDirectory(dest);

            DirectoryInfo[] directories = dir.GetDirectories();

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
                file.CopyTo(Path.Combine(dest, file.Name));

            foreach (DirectoryInfo subdir in directories)
                FolderCopy(subdir.FullName, Path.Combine(dest, subdir.Name));
        }

        /// <summary>
        /// Алгоритм команды Удаление
        /// </summary>
        public bool ActionDelete(string[] paramArray)
        {
            int length = paramArray.Length;
            if (length != 2)
            {
                Logger.Log($"Ошибка. Неверное количество параметров команды delete. {ArraytoString(paramArray)}");
                return false;
            }

            string s = paramArray[1];

            try
            {
                if (File.Exists(s))
                {
                    File.Delete(s);
                    return true;
                }
                else if (Directory.Exists(s))
                {
                    Directory.Delete(s, recursive: true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка. Не удалось удалить {s}. {ex.Message}");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Алгоритм команды Информация о файле/папке
        /// </summary>
        public bool ActionInfo(string[] paramArray, int lineInfo, DrawPathDelegate drawDelegate)
        {
            drawDelegate();
            int length = paramArray.Length;
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            if (length != 2)
            {
                Logger.Log($"Ошибка. Неверное количество параметров команды info. {ArraytoString(paramArray)}");
                return false;
            }

            string value = paramArray[1];
            try
            {
                if (File.Exists(value))
                {
                    var fileInfo = new FileInfo(value);
                    var fileAttibutes = fileInfo.Attributes;
                    Console.SetCursorPosition(0, lineInfo);
                    Console.WriteLine($"Файл: {value}");
                    Console.WriteLine($"Атрибуты: {fileAttibutes}");
                    Console.WriteLine($"Размер, байт: {fileInfo.Length.ToString("#,0", nfi)}");
                    return true;
                }
                else if (Directory.Exists(value))
                {
                    var dirInfo = new DirectoryInfo(value);
                    var folderAttibutes = dirInfo.Attributes;
                    long size = GetFolderSize(dirInfo);
                    Console.SetCursorPosition(0, lineInfo);
                    Console.WriteLine($"Папка: {value}");
                    Console.WriteLine($"Атрибуты: {folderAttibutes}");
                    Console.WriteLine($"Размер: {size.ToString("#,0", nfi)}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка. {ex.Message}");
                return false;
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
        /// Алгоритм команды Перейти в папку
        /// </summary>
        public bool ActionCd(string[] paramArray, Options options, DrawPathDelegate drawPath, OptionsManager optionsManager, List<string> pagedList, DrawTreeDelegate drawTree)
        {
            pagedList.Clear();
            int length = paramArray.Length;
            if (!(length == 2 || length == 1))
            {
                Logger.Log($"Ошибка. Неверное количество параметров команды cd. {ArraytoString(paramArray)}");
                return false;
            }

            try
            {
                if (length == 2)
                {
                    if (Directory.Exists(paramArray[1]))
                    {
                        var dirInfo = new DirectoryInfo(paramArray[1]);
                        options.Path = dirInfo.FullName;
                        optionsManager.Set(options);
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
                Logger.Log($"Ошибка. {ex.Message}");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Алгоритм команды Показать дерево папок
        /// </summary>
        public bool ActionTree(string[] paramArray, List<string> pagedList, DrawTreeDelegate drawTree)
        {
            pagedList.Clear();
            int length = paramArray.Length;
            if (length != 2)
            {
                Logger.Log($"Ошибка. Неверное количество параметров команды tree. {ArraytoString(paramArray)}");
                return false;
            }

            try
            {
                if (Directory.Exists(paramArray[1]))
                {
                    FolderListRecursion(paramArray[1], pagedList, true);
                    drawTree(false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Ошибка. {ex.Message}");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Строит рекурсией список папок и файлов в ней
        /// </summary>
        /// <param name="folderPath">путь к папке для построения дерева</param>
        private void FolderListRecursion(string folderPath, List<string> pagedList, bool isRecursion)
        {
            pagedList.Add(folderPath);
            var arrayPath = Directory.GetDirectories(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            var arrayFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var dir in arrayPath)
                if (isRecursion)
                    FolderListRecursion(dir, pagedList, isRecursion);
                else
                    pagedList.Add(dir);

            foreach (var file in arrayFiles)
                pagedList.Add(file);
        }

    }
}
