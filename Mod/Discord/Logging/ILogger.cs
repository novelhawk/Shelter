using System;

namespace Mod.Discord.Logging
{
    /// <summary>
    /// Logging interface to log the internal states of the pipe. Logs are sent in a NON thread safe way. They can come from multiple threads and it is upto the ILogger to account for it.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The level of logging to apply to this logger.
        /// </summary>
        LogLevel Level { get; set; }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Info(object message);

        /// <summary>
        /// Warning log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Warning(object message);

        /// <summary>
        /// Error log messsages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Error(object message);
        
        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Info(object message, params object[] args);

        /// <summary>
        /// Warning log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Warning(object message, params object[] args);

        /// <summary>
        /// Error log messsages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Error(object message, params object[] args);
    }
}
