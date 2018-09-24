using System.Collections.Generic;

namespace epubed.Query
{
    public class EpubQuery
    {
        public class Result
        {
            public object Key { get; }

            public List<(EpubPath, object value)> GetResuts { get; }
                = new List<(EpubPath, object value)>();

            public List<(EpubPath, object oldValue, object newValue)> SetResuts { get; }
                = new List<(EpubPath, object oldValue, object newValue)>();

            public Result(object key)
            {
                Key = key;
            }
        }

        public List<EpubPath> Getters { get; } = new List<EpubPath>();

        public List<(EpubPath, object)> Filters { get; } = new List<(EpubPath, object)>();

        public List<(EpubPath, object)> Setters { get; } = new List<(EpubPath, object)>();

        public List<(EpubPath, string)> Verbs { get; } = new List<(EpubPath, string verb)>();
    }
}
