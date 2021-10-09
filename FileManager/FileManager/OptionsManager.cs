using System.IO;
using System;
using System.Text.Json;

namespace GeekBrains.Learn.FileManager
{
    /// <summary>
    /// Хранит настройки в файле
    /// </summary>
    public class OptionsManager
    {
        private const string pathFile = "path.json";

        /// <summary>
        /// Получает настройки из файла
        /// </summary>
        public Options Get()
        {
            try
            {
                return JsonSerializer.Deserialize<Options>(File.ReadAllText(pathFile));
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                var path = new Options();
                Set(path);
                return path;
            }
        }

        /// <summary>
        /// Сохраняет настройки в файл
        /// </summary>
        public void Set(Options options)
        {
            try
            {
                var jsOptions = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(options, jsOptions);
                File.WriteAllText(pathFile, json);
            }
            catch (Exception ex)
            {
                {
                    Logger.Log(ex.Message);
                    return;
                }
            }
        }
    }
}
