using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Common.Loggers
{
    public class Logger : ILog
    {
        private Logger()
        {

        }

        private static Logger instance = new Logger();
        public static Logger Instance
        {
            get
            {
                return instance;
            }
        }

        public void Config()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Fatal(string msg)
        {
            var logger = LogManager.GetLogger("Fatal");
            logger.Fatal(msg);
        }

        public void Error(string msg)
        {
            var logger = LogManager.GetLogger("Error");
            logger.Error(msg);
        }

        public void Error(Exception ex)
        {
            var logger = LogManager.GetLogger("Error");
            logger.Error(ex.ToString());
        }

        public void Info(string msg)
        {
            var logger = LogManager.GetLogger("Info");
            logger.Info(msg);
        }

        public void Debug(string msg)
        {
            var logger = LogManager.GetLogger("Debug");
            logger.Debug(msg);
        }
    }
}
