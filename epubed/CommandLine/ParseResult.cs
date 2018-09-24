using epubed.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace epubed.CommandLine
{
    public class ParseResult
    {
        public CommandLineModel CommandLine { get; }

        public IEnumerable<string> Errors { get; }

        public IEnumerable<string> Warnings { get; }


        public ParseResult(CommandLineModel commandLine, IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            CommandLine = commandLine;
            Errors = errors;
            Warnings = warnings;
        }

        public static ParseResult Parse(string[] args)
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            var model = new CommandLineModel();

            (string path, string value) split(string value, string splitter = "=")
            {
                if (value == null || !value.Contains(splitter))
                {
                    errors.Add($"Switch value '{value}' must be formatted like PATH{splitter}VALUE.");
                    return (value, null);
                }

                var parts = value.Split(new string[] { splitter }, StringSplitOptions.None);
                return (parts[0].Trim(), string.Join("", parts.Skip(1)).Trim());
            }

            void processGetter(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    warnings.Add($"Cannot get an empty path. Ignoring this switch.");
                    return;
                }

                model.Query.Getters.Add(EpubPath.Parse(value));
            }

            void processFilter(string all)
            {
                var (path, value) = split(all);
                if (string.IsNullOrWhiteSpace(path))
                {
                    warnings.Add($"Cannot filter by an empty path. Ignoring this switch.");
                    return;
                }

                model.Query.Filters.Add((EpubPath.Parse(path), value));
            }

            void processSetter(string all)
            {
                var (path, value) = split(all);
                if (string.IsNullOrWhiteSpace(path))
                {
                    warnings.Add($"Cannot set an empty path. Ignoring this switch.");
                    return;
                }

                model.Query.Setters.Add((EpubPath.Parse(path), value));
            }

            void processVerb(string all)
            {
                var (path, verb) = split(all, "::");
                if (string.IsNullOrWhiteSpace(verb))
                {
                    warnings.Add($"Cannot run an empty verb {verb}. Ignoring this switch.");
                    return;
                }

                model.Query.Verbs.Add((EpubPath.Parse(path), verb));
            }

            void valueNotSpecified(string key)
            {
                errors.Add($"Value not specified for switch {key}");
            }

            IEnumerable<(string, string)> transformKeys()
            {
                string currentKey = null;
                foreach (var currentItem in args)
                {
                    if (currentItem.StartsWith("-"))
                    {
                        if (currentKey == null)
                        {
                            currentKey = currentItem;
                            continue;
                        }
                        else
                        {
                            valueNotSpecified(currentKey);
                            currentKey = currentItem;
                            continue;
                        }
                    }

                    yield return (currentKey, currentItem);
                    currentKey = null;
                }
                if (currentKey != null) valueNotSpecified(currentKey);
            }

            var pairs = transformKeys();

            foreach (var (key, value) in pairs)
            {
                switch (key?.ToLower())
                {
                    case null: // this is a globbing request
                        model.FileRequests.Add(value?.Replace("\\", "/"));
                        break;

                    case "-d":
                    case "-dir":
                    case "--directory":
                        if (model.DefaultDirectory != null)
                        {
                            warnings.Add($"Default directory specified multiple times: {model.DefaultDirectory}, {value}.");
                        }
                        model.DefaultDirectory = value;
                        break;

                    case "-g":
                    case "--get":
                        processGetter(value);
                        break;

                    case "-w":
                    case "--where":
                        processFilter(value);
                        break;

                    case "-s":
                    case "--set":
                        processSetter(value);
                        break;

                    case "-v":
                    case "--verb":
                        processVerb(value);
                        break;

                    default:
                        warnings.Add($"Unknown switch {key}.");
                        break;
                }
            }

            return new ParseResult(model, errors, warnings);
        }
    }
}
