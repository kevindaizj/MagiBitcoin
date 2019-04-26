using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.StartupConfigs
{
    public static class ViewControllerLocatorConfig
    {
        public static void Config()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}Controller, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
    }
}
