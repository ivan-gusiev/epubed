using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace epubed.Query
{
    public class QueryExecutor
    {
        public EpubQuery Query { get; }

        public IEnumerable<ITraversable> Inputs { get; }

        public QueryExecutor(EpubQuery query, IEnumerable<ITraversable> inputs)
        {
            Query = query;
            Inputs = inputs;
        }

        public List<EpubQuery.Result> Execute()
        {
            var resultSet = new List<EpubQuery.Result>();

            foreach (var item in Inputs)
            {
                try
                {
                    if (!ApplyFilters(item)) continue;

                    var result = new EpubQuery.Result(item.Value);

                    foreach (var path in Query.Getters)
                    {
                        result.GetResuts.Add((path, item.SafeGet(path)));
                    }

                    foreach (var (path, newValue) in Query.Setters)
                    {
                        var resultTuple = item.SafeExecute(path, leaf =>
                        {
                            var oldValue = leaf.Value;
                            leaf.Value = newValue;
                            return (path, oldValue, newValue);
                        });

                        result.SetResuts.Add(resultTuple);
                    }

                    foreach (var (path, verb) in Query.Verbs)
                    {
                        item.SafeRun(path, verb);
                    }

                    resultSet.Add(result);
                }
                catch (Exception e) when (CatchExceptions)
                {
                    OnItemException(item, e);
                }
                finally
                {
                    if (DisposeAfterUse) item.Dispose();
                }
            }

            return resultSet;
        }

        private bool ApplyFilters(ITraversable item)
        {
            foreach (var (path, target) in Query.Filters)
            {
                // use object.Equals to achieve magic filters
                if (!Equals(target, item.SafeGet(path))) return false;
            }

            return true;
        }

        protected virtual bool DisposeAfterUse
        {
            get
            {
                return false;
            }
        }

        protected virtual bool CatchExceptions
        {
            get
            {
                return false;
            }
        }

        protected virtual void OnItemException(ITraversable item, Exception e)
        {

        }
    }
}
