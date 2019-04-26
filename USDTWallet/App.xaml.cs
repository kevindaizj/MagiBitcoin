using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using USDTWallet.StartupConfigs;
using USDTWallet.Views;

namespace USDTWallet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ServiceRegisterConfig.RegisterService(containerRegistry);
            ViewRegionConfig.RegisterViewNavigation(containerRegistry);
        }

        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
            ViewControllerLocatorConfig.Config();
        }

        protected override void InitializeShell(Window shell)
        {
            var regionManager = Container.Resolve<IRegionManager>();
            ViewRegionConfig.InitViewWithRegion(regionManager);
            base.InitializeShell(shell);
        }
    }
}
