namespace Mod.Discord.Logging
{
    /// <summary>
    /// Logs the outputs to a file
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// The level of logging to apply to this logger.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Should the output be coloured?
        /// </summary>
        public string File { get; set; }

        private object filelock;

        /// <summary>
        /// Creates a new instance of the file logger
        /// </summary>
        /// <param name="path"></param>
        public FileLogger(string path)
        {
            File = path;
            filelock = new object();			
        }


        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="fullName"></param>
        /// <param name="args"></param>
        public void Info(object message)
        {
            if (Level != LogLevel.Info) return;
            lock(filelock)
                System.IO.File.AppendAllText(File, "\nINFO: " + message);
        }

        /// <summary>
        /// Warning log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Warning(object message)
        {
            if (Level != LogLevel.Info && Level != LogLevel.Warning) return;
            lock (filelock)
                System.IO.File.AppendAllText(File, "\nWARN: " + message);
        }

        /// <summary>
        /// Error log messsages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Error(object message)
        {
            if (Level != LogLevel.Info && Level != LogLevel.Warning && Level != LogLevel.Error) return;
            lock (filelock)
                System.IO.File.AppendAllText(File, "\nERR : " + message);
        }
        
        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="fullName"></param>
        /// <param name="args"></param>
        public void Info(object message, params object[] args)
        {
            if (Level != LogLevel.Info) return;
            lock(filelock)
                System.IO.File.AppendAllText(File, "\nINFO: " + string.Format(message.ToString(), args));
        }

        /// <summary>
        /// Warning log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Warning(object message, params object[] args)
        {
            if (Level != LogLevel.Info && Level != LogLevel.Warning) return;
            lock (filelock)
                System.IO.File.AppendAllText(File, "\nWARN: " + string.Format(message.ToString(), args));
        }

        /// <summary>
        /// Error log messsages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Error(object message, params object[] args)
        {
            if (Level != LogLevel.Info && Level != LogLevel.Warning && Level != LogLevel.Error) return;
            lock (filelock)
                System.IO.File.AppendAllText(File, "\nERR : " + string.Format(message.ToString(), args));
        }

    }
}
