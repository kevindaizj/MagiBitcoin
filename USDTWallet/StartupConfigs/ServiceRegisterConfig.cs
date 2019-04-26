using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }
    }
}
