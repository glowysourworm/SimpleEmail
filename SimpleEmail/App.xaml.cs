using System.Windows;

using SimpleEmail.Ioc;

namespace SimpleEmail
{
    public partial class App : Application
    {
        readonly SimpleEmailBootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new SimpleEmailBootstrapper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Next, initialize the bootstrapper
            _bootstrapper.Initialize();

            // Loads configuration prior to other injectors (MainViewModel needs PrimaryConfiguration)

            // Run() -> Window.Show()
            _bootstrapper.Run();
        }
    }

}
