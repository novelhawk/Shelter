using JetBrains.Annotations;

namespace Mod.Logging
{
    public interface ILogger
    {
        [StringFormatMethod("line")]
        void Log(string line, params object[] args);
        void Log(string line);
    }
}