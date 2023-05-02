using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sequence_maker.Services
{
    public class LogManager : ILogManager
    {
        private string logFilePath = Path.Combine(Environment.CurrentDirectory, "Log\\${var:runtime}log.txt");
        private Logger _logManager;

        public Logger Logger { get { return _logManager; } }

        public LogManager()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var fileTarget = new FileTarget("target")
            {
                //FileName = "D:\\testlog\\Logs\\${var:runtime}log1.txt",
                FileName = logFilePath,
                Layout = "${longdate}|${level}| ${message}  ${exception} ${event-properties:myProperty}",
            };
            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, fileTarget);
            // Apply config
            NLog.LogManager.Configuration = config;
            _logManager = NLog.LogManager.GetCurrentClassLogger();
        }
    }
}
