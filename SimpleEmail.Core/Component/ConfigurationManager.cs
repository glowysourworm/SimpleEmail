using System.IO;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.RecursiveSerializer.Shared;
using SimpleWpf.Utilities;

namespace SimpleEmail.Core.Component
{
    [IocExport(typeof(IConfigurationManager))]
    public class ConfigurationManager : IConfigurationManager
    {
        const string FILE_NAME = "SimpleEmail.config";

        PrimaryConfiguration _configuration;
        string _file;

        public ConfigurationManager()
        {
            _file = string.Empty;
            _configuration = new PrimaryConfiguration();
        }

        public void Initialize(string folder)
        {
            _file = CreateFileName(folder);

            Load();
        }

        public PrimaryConfiguration Get()
        {
            return _configuration;
        }

        public void Set(PrimaryConfiguration configuration)
        {
            BasicHelpers.MapOnto(configuration, _configuration);
        }

        public void Save()
        {
            try
            {
                // Delete existing file
                if (File.Exists(_file))
                {
                    System.IO.File.Delete(_file);
                }

                var serializer = new RecursiveSerializer<PrimaryConfiguration>(new RecursiveSerializerConfiguration()
                {
                    IgnoreRemovedProperties = false,
                    PreviewRemovedProperties = false
                });

                using (var stream = System.IO.File.OpenWrite(_file))
                {
                    serializer.Serialize(stream, _configuration);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading configuration. Creating default");
                _configuration = new PrimaryConfiguration();
            }
        }

        private string CreateFileName(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return Path.Combine(Environment.CurrentDirectory, FILE_NAME);
            }
            else if (!Path.Exists(folder))
            {
                return Path.Combine(Environment.CurrentDirectory, FILE_NAME);
            }
            else
                return Path.Combine(folder, FILE_NAME);
        }

        /// <summary>
        /// Loads PrimaryConfiguration from the readonly file name
        /// </summary>
        private void Load()
        {
            try
            {
                if (!File.Exists(_file))
                {
                    _configuration = new PrimaryConfiguration();
                    return;
                }

                var serializer = new RecursiveSerializer<PrimaryConfiguration>(new RecursiveSerializerConfiguration()
                {
                    IgnoreRemovedProperties = false,
                    PreviewRemovedProperties = false
                });

                using (var stream = System.IO.File.OpenRead(_file))
                {
                    _configuration = serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading configuration. Creating default");
                _configuration = new PrimaryConfiguration();
            }
        }
    }
}
