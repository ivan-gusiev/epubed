using System;
using System.Collections.Generic;
using System.Linq;

namespace epubed.Query
{
    public class EpubPath
    {
        IEnumerable<string> Keys { get; }
        Lazy<string> fullPath;

        public EpubPath(IEnumerable<string> keys)
        {
            Keys = keys;
            fullPath = new Lazy<string>(ConstructFullPath);
        }

        public string FullPath => fullPath.Value;

        public string FirstKey => Keys.FirstOrDefault();

        public EpubPath Rest => new EpubPath(Keys.Skip(1));

        public bool ContainsKeys => Keys.Any();

        public bool IsEmpty => !ContainsKeys;

        public override string ToString()
        {
            return fullPath.Value;
        }

        private string ConstructFullPath()
        {
            return string.Join(".", Keys);
        }

        #region Parsing

        private static Dictionary<string, string> substitutions = new Dictionary<string, string>
        {
            { "lang", "Content.Language" },
            { ".", "" }
        };

        private static char[] pathSplitters = new[] { '.' };

        private static string Substitute(string input)
        {
            if (substitutions.TryGetValue(input, out var result))
            {
                return result;
            }

            return input;
        }

        public static EpubPath Parse(string pathString)
        {
            pathString = Substitute(pathString ?? throw new ArgumentNullException(nameof(pathString)));

            return new EpubPath(pathString.Split(pathSplitters, StringSplitOptions.RemoveEmptyEntries));
        }

        #endregion
    }
}
