using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common.Loggers;

namespace USDTWallet.Common.Helpers
{
    public static class DataDirectoryHelper
    {
        private static readonly string DataDirectoryName = @"USDT Wallet";

        private static readonly string KeyStoreDirectory = @"keystore";

        private static readonly string DbDirectory = @"Database";

        private static readonly string LogDirectory = @"Logs";

        private static readonly List<string> SubDirectories = new List<string> { KeyStoreDirectory, DbDirectory, LogDirectory };
        

        public static void InitializeDataDirectory()
        {
            var basePath = GetDataDirectoryPath();
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            foreach (var name in SubDirectories)
            {
                var path = Path.Combine(basePath, name);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            InitDatabase();
        }

        private static void InitDatabase()
        {
            string sourceDbFile = ConfigurationManager.AppSettings["SourceDBPath"];
            string sourceDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sourceDbFile);
            var db = new FileInfo(sourceDbPath);
            if (!db.Exists)
            {
                Logger.Instance.Error("Source DB does not exist.");
                return;
            }

            string destDbPath = Path.Combine(GetDatabaseDirectoryPath(), db.Name);

            if (!File.Exists(destDbPath))
                File.Copy(sourceDbPath, destDbPath);

        }


        public static string GetDataDirectoryPath()
        {
            string localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppDataDirectory, DataDirectoryName);
        }

        public static string GetKeyStoreDirectoryPath()
        {
            var basePath = GetDataDirectoryPath();
            return Path.Combine(basePath, KeyStoreDirectory);
        }

        public static string GetDatabaseDirectoryPath()
        {
            var basePath = GetDataDirectoryPath();
            return Path.Combine(basePath, DbDirectory);
        }

        public static string GetLogDirectoryPath()
        {
            var basePath = GetDataDirectoryPath();
            return Path.Combine(basePath, LogDirectory);
        }



    }
}
