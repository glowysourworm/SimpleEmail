using SimpleEmail.Core.Model;

namespace SimpleEmail.Core.Component.Interface
{
    /// <summary>
    /// Component interface for the configuration management of the application. The implementation class
    /// will determine how the file is loaded, constructor patterns are typical.
    /// </summary>
    public interface IConfigurationManager
    {
        void Initialize(string folder);
        PrimaryConfiguration Get();
        void Save();
    }
}
