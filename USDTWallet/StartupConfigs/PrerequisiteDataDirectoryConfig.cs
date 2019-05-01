using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common.Helpers;

namespace USDTWallet.StartupConfigs
{
    public static class PrerequisiteDataDirectoryConfig
    {
        public static void Config()
        {
            DataDirectoryHelper.InitializeDataDirectory();
            AppDomain.CurrentDomain.SetData("DataDirectory", DataDirectoryHelper.GetDataDirectoryPath());
        }
    }
}
