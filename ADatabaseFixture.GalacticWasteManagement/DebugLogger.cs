using GalacticWasteManagement.Logging;
using GalacticWasteManagement.Output;
using GalacticWasteManagement.Utilities;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ADatabaseFixture.GalacticWasteManagement
{
    public class DebugLogger : ILogger, IOutput
    {
        public MiniProfiler? MiniProfiler { get; set; }

        public void Dump()
        {
            var text = MiniProfiler?.Root.Children
                ?.SelectMany(x =>
                 new List<string> { $"{Environment.NewLine}-- {x.Name}" }
                 .Concat(
                    x.CustomTimings.SelectMany(c =>
                       c.Value.Select(y => y.CommandString)
                       .Intersperse($"GO{Environment.NewLine}"))
                       .ToList()));

            if (text != null)
            {
                foreach (var line in text)
                {
                    Debug.WriteLine(line);
                }
            }
        }

        public void Log(string message, string type) => Debug.WriteLine($"{type}: {message}");
    }
}
