using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.Logging
{
    public enum TracingLevel
    {
        ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF
    }
    public class CustomLogging
    {
        private static ILog _log = null;
        private static string _logFile = null;

        public static void Initialize(string ApplicationPath)
        {
            _logFile = Path.Combine(ApplicationPath, "App_Data", "plants.log");
            GlobalContext.Properties["LogFileName"] = _logFile;

            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(ApplicationPath, "Log4Net.config")));

            _log = LogManager.GetLogger("plants");
        }

        public static string LogFile
        {
            get { return _logFile; }
        }

        public static void LogMessage(TracingLevel Level, Exception exception)
        {
            LogMessage(Level, exception.Message);
        }
        public static void LogMessage(TracingLevel Level, string Message)
        {
            switch (Level)
            {
                case TracingLevel.DEBUG:
                    _log.Debug(Message);
                    break;

                case TracingLevel.INFO:
                    _log.Info(Message);
                    break;

                case TracingLevel.WARN:
                    _log.Warn(Message);
                    break;

                case TracingLevel.ERROR:
                    _log.Error(Message);
                    break;

                case TracingLevel.FATAL:
                    _log.Fatal(Message);
                    break;
            }
        }
    }
}