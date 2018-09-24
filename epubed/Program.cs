using epubed.CommandLine;
using epubed.Query;
using System;
using System.IO;
using System.Linq;

namespace epubed
{
    class Program
    {
        static void Main(string[] args)
        {
            var parse = ParseResult.Parse(args);
            var model = Analyze(parse);
            if (model != null)
            {
                Run(model);
            }
        }

        static void Run(CommandLineModel commandLine)
        {
            var defaultDirectory = commandLine.DefaultDirectory ?? Environment.CurrentDirectory;
            var fileNames = commandLine.GetFileNames();
            var files = fileNames.Select(pair => 
                new RootTraversable(pair.stem, new EpubRoot(Path.Combine(defaultDirectory, pair.path))));
            var executor = new SafeConsoleQueryExecutor(commandLine.Query, files);

            foreach (var result in executor.Execute())
            {
                PrintResult(result);
            }
        }
        
        static CommandLineModel Analyze(ParseResult result)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"ERROR: {error}");
            }
            foreach (var warning in result.Warnings)
            {
                Console.WriteLine($"WARNING: {warning}");
            }

            if (result.Errors.Any())
            {
                return null;
            }
            else
            {
                return result.CommandLine;
            }
        }
        
        static void PrintResult(EpubQuery.Result result)
        {
            Console.WriteLine();
            Console.WriteLine(result.Key);
            foreach (var (path, value) in result.GetResuts)
            {
                Console.Write('\t');
                Console.Write(path);
                Console.Write(":\t");
                Console.WriteLine(value);
            }
            foreach (var (path, oldValue, newValue) in result.SetResuts)
            {
                Console.Write('\t');
                Console.Write(path);
                Console.Write(":\t");
                Console.Write(oldValue);
                Console.Write(" -> ");
                Console.WriteLine(newValue);
            }
        }

    }
}
