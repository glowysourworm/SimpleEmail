using SimpleWpf.IocFramework.Application;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.EventAggregation;
using SimpleWpf.IocFramework.RegionManagement.Interface;

namespace SimpleEmail.Ioc
{
    [IocExportDefault]
    public class MainModule : ModuleBase
    {
        [IocImportingConstructor]
        public MainModule(IIocRegionManager regionManager, IIocEventAggregator eventAggregator) : base(regionManager, eventAggregator)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            //_regionManager.LoadNamedInstance(MAIN_REGION, typeof(MainView));

            // ManagerView -> FileTreeView (needs loading)
            //var treeView = _regionManager.LoadNamedInstance<FileTreeView>("ManagerViewFileTreeViewRegion", true);

            // -> to set the search pattern
            //treeView.SearchPattern = "*.mp3";
        }

        public override void Run()
        {
            base.Run();

            //ApplicationHelpers.Log("Welcome to Audio Station!", LogMessageType.General, LogLevel.Information, null);
        }
    }
}
