using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace epubed.Query
{
    internal class SafeConsoleQueryExecutor : QueryExecutor
    {
        public SafeConsoleQueryExecutor(EpubQuery query, IEnumerable<ITraversable> inputs) : base(query, inputs)
        {
        }

        protected override bool DisposeAfterUse => true;

        protected override bool CatchExceptions => false;

        protected override void OnItemException(ITraversable item, Exception e)
        {
            if (Debugger.IsAttached) Debugger.Break();

            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine($"ERROR while working on {item.Value}.");
            Console.WriteLine(e);
        }
    }
}
