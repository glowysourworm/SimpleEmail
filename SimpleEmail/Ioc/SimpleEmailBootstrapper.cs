using System.Windows;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Ioc;

using SimpleWpf.IocFramework.Application;

namespace SimpleEmail.Ioc
{
    /// <summary>
    /// IOC Bootstrapper:  Takes over primary control / startup of the application. The configuration is
    ///                    read here; and the components are initialized. Most / all major components will
    ///                    inherit from an interface; and have Initialize / Dispose methods. These are 
    ///                    handled during the UserPreModuleInitialize sequence - after the configuration is
    ///                    read. This configuration will also be injected into the primary view model. Changes
    ///                    to the primary view model / configuration may be handled there; and disposing of
    ///                    the main components will also be handled by our IDisposable pattern.
    /// </summary>
    public class SimpleEmailBootstrapper : IocWindowBootstrapper
    {
        public SimpleEmailBootstrapper()
        {

        }

        protected override async void UserPreModuleInitialize()
        {
            // This must happen first; and the dialog window must be called after initializing
            // the (base) "pre-module initialize" method because it tries to create the shell
            // window - which uses the configuration.
            //
            InitializeConfiguration();

            // Call base method (starts window)
            base.UserPreModuleInitialize();

            // Show Main Window
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Initialization of the configuration must occur before other components are initialized.
        /// </summary>
        private void InitializeConfiguration()
        {
            var configurationManager = IocContainer.Get<IConfigurationManager>();

            // Putting the config file in the binary folder (for now)
            var configurationFolder = Environment.CurrentDirectory;

            // Read / Create Configuration
            configurationManager.Initialize(configurationFolder);
        }

        public override IEnumerable<ModuleDefinition> DefineModules()
        {
            return new ModuleDefinition[]
            {
                new ModuleDefinition("MainModule", typeof(MainModule), true),
                new ModuleDefinition("CoreModule", typeof(CoreModule), false)
            };
        }

        public override Type DefineShell()
        {
            return typeof(MainWindow);
        }
    }
}
