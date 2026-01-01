using System.Windows;
using System.Windows.Controls;

using SimpleEmail.ViewModel;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Views
{
    [IocExportDefault]
    public partial class EmailAccountSettingsView : UserControl
    {
        public EmailAccountSettingsView()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EmailAccountSettingsViewModel;

            // Default Password
            if (viewModel != null)
                this.PasswordTB.Password = viewModel.Password;
        }

        private void PasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EmailAccountSettingsViewModel;

            if (viewModel != null)
                viewModel.Password = this.PasswordTB.Password;
        }
    }
}
