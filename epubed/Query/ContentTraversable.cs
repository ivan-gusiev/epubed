using System;
using System.Collections.Generic;

namespace epubed.Query
{
    public class ContentTraversable : ITraversable
    {
        static readonly Dictionary<string, string> substitutions = new Dictionary<string, string>
        {
            { "MetaCover", "//meta[@name='cover']" }
        };

        readonly ContentModel content;

        public ContentTraversable(ContentModel epubContent)
        {
            content = epubContent;
        }

        public object Value
        {
            get => "Content";
            set { /* this traversable is immutable */ }
        }

        public ITraversable Child(string key)
        {
            key = Substitute(key);
            return new LeafTraversable(content, key ?? throw new ArgumentNullException(key));
        }

        public void Run(string verb)
        {

        }

        public void Dispose()
        {
            content.SaveChanges();
            content.Dispose();
        }

        private static string Substitute(string key)
        {
            if (substitutions.TryGetValue(key, out var result))
            {
                return result;
            }

            if (key == "") return key;

            if (char.IsUpper(key[0]))
            {
                return $"//dc:{key.ToLowerInvariant()}";
            }

            return key;
        }

        private class LeafTraversable : ITraversable
        {
            readonly ContentModel content;
            readonly string key;

            public LeafTraversable(ContentModel content, string key)
            {
                this.content = content;
                this.key = key;
            }

            public object Value
            {
                get => content[key];
                set
                {
                    content[key] = Convert.ToString(value);
                    content.SaveChanges();
                }
            }

            public ITraversable Child(string key) => null;

            public void Run(string verb)
            {

            }

            public void Dispose()
            {
            }
        }
    }
}
