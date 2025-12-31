using System.Windows.Controls;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Views
{
    [IocExportDefault]
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }
    }
}
