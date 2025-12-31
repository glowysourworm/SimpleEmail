using SimpleWpf.IocFramework.Application;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.EventAggregation;
using SimpleWpf.IocFramework.RegionManagement.Interface;

namespace SimpleEmail.Core.Ioc
{
    [IocExportDefault]
    public class CoreModule : ModuleBase
    {
        [IocImportingConstructor]
        public CoreModule(IIocRegionManager regionManager, IIocEventAggregator eventAggregator) : base(regionManager, eventAggregator)
        {
        }
    }
}
