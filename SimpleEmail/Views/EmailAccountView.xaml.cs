using System.Windows;
using System.Windows.Controls;

using SimpleEmail.ViewModel;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Views
{
    [IocExportDefault]
    public partial class EmailAccountView : UserControl
    {
        public EmailAccountViewModel ViewModel { get; private set; }

        public EmailAccountView()
        {
            this.ViewModel = new EmailAccountViewModel();

            InitializeComponent();

            this.DataContext = this.ViewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Password
            this.PasswordTB.Password = this.ViewModel.Password;
        }

        private void PasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Password = this.PasswordTB.Password;
        }
    }
}
