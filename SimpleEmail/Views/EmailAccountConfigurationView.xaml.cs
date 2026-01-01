using System.Windows;
using System.Windows.Controls;

using SimpleEmail.ViewModel.Configuration;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Views
{
    [IocExportDefault]
    public partial class EmailAccountConfigurationView : UserControl
    {
        public EmailAccountConfigurationView()
        {
            InitializeComponent();

            this.DataContextChanged += EmailAccountSettingsView_DataContextChanged;
        }

        private void EmailAccountSettingsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = this.DataContext as EmailAccountConfigurationViewModel;

            // Default Password
            if (viewModel != null)
                this.PasswordTB.Password = viewModel.Password;
        }

        private void PasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EmailAccountConfigurationViewModel;

            if (viewModel != null)
                viewModel.Password = this.PasswordTB.Password;
        }
    }
}
