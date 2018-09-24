using epubed.Query;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace epubed.CommandLine
{
    public class CommandLineModel
    {
        public EpubQuery Query { get; } = new EpubQuery();

        public List<string> FileRequests { get; } = new List<string>();

        public string DefaultDirectory { get; set; }

        public IEnumerable<(string stem, string path)> GetFileNames()
        {
            var simples = new List<(string, string)>();
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            foreach (var request in FileRequests)
            {
                if (Path.IsPathRooted(request))
                {
                    simples.Add((request, request));
                    continue;
                }
                matcher.AddInclude(request);
            }

            var dirInfo = new DirectoryInfo(DefaultDirectory ?? Environment.CurrentDirectory);
            if (!dirInfo.Exists) dirInfo = new DirectoryInfo(Environment.CurrentDirectory);

            return matcher.Execute(new DirectoryInfoWrapper(dirInfo)).Files.Select(x => (x.Stem, x.Path)).Concat(simples);
        }
    }
}
