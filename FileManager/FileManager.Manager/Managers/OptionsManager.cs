using System;
using System.Text.Json;
using GeekBrains.Learn.FileManager.Domain;
using GeekBrains.Learn.FileManager.Repository;
using GeekBrains.Learn.FileManager.Shared;

namespace GeekBrains.Learn.FileManager.Manager
{
    /// <summary>
    /// Options manager
    /// </summary>
    public class OptionsManager : IOptionsManager
    {
        private readonly IFileRepository _fileRepository;
        private readonly string _pathOptions;
        private readonly ILogManager _logManager;
        private readonly IDirectoryRepository _directoryRepository;

        /// <summary>
        /// ctor
        /// </summary>
        public OptionsManager()
        {
            _directoryRepository = new DirectoryRepository();
            _fileRepository = new FileRepository();
            _logManager = new LogManager();
            _pathOptions = Constants.OptionsFileName;
        }

        /// <inheritdoc/>
        public string GetWorkPath()
        {
            var path = GetOptions().Path;
            if (!_directoryRepository.Exists(path))
            {
                path = "";
            }

            return path;
        }

        /// <inheritdoc/>
        public bool SetWorkPath(string path)
        {
            var options = GetOptions();
            options.Path = path;
            return SetOptions(options);
        }

        /// <inheritdoc/>
        public bool SetOptions(IOptions options)
        {
            var json = SerializeOptions(options);
            if (json == null)
            {
                return false;
            }

            if (!_fileRepository.Update(_pathOptions, json, false))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public IOptions GetOptions()
        {
            var fileText = _fileRepository.Read(_pathOptions);
            if (fileText == null)
            {
                return Options.Empty;
            }

            return DeserializeOptions(fileText);
        }

        private string SerializeOptions(IOptions options)
        {
            var jsOptions = new JsonSerializerOptions { WriteIndented = true };
            try
            {
                var json = JsonSerializer.Serialize(options, jsOptions);
                return json;
            }
            catch (Exception ex)
            {
                _logManager.Log(ex.Message);
                return null;
            }
        }

        private IOptions DeserializeOptions(string value)
        {
            try
            {
                return JsonSerializer.Deserialize<Options>(value);
            }
            catch (Exception ex)
            {
                _logManager.Log(ex.Message);
                return null;
            }
        }
    }
}
