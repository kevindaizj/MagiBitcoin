using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.ControlService.MsgBox;

namespace USDTWallet.StartupConfigs
{
    public static class ServiceRegisterConfig
    {
        public static void RegisterService(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MessageBoxService>();
            Load(containerRegistry);
        }

        private static void Load(IContainerRegistry containerRegistry)
        {
            var assembly = Assembly.Load("USDTWallet.Biz");
            var types = assembly.GetExportedTypes()
                                .Where(t => t.Name.ToUpper().EndsWith("Manager".ToUpper()));
            foreach(var type in types)
            {
                containerRegistry.Register(type);
            }

            assembly = Assembly.Load("USDTWallet.Dao");
            types = assembly.GetExportedTypes()
                                .Where(t => t.Name.ToUpper().EndsWith("Dao".ToUpper()));
            foreach (var type in types)
            {
                containerRegistry.Register(type);
            }
        }
    }
}
