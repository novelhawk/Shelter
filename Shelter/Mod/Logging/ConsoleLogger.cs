using System;
using System.Collections.Generic;

namespace Mod.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string[] _logs = new string[10];

        public void Log(string line, params object[] args) => Log(string.Format(line, args));
        public void Log(string line)
        {
            var split = line.Split('\n');
            if (split.Length > 1)
            {
                foreach (var realLine in split)
                    Log(realLine);
                return;
            }

            Array.Copy(_logs, 0, _logs, 1, 9);
            _logs[0] = line;
        }

        public IEnumerable<string> Logs => _logs;
    }
}