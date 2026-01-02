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

            // NOTE*** THIS IS FIRE-AND-FORGET! The bootstrapper effectively needs a whole dialog / splash screen
            // feature for handling this. Context switching is causing major delays with other libraries. So, it
            // would be best to handle it without.. (?) .. perhaps synchronous waiter.

            // Run() -> Window.Show()
            _bootstrapper.Run();
        }
    }

}
