using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace epubed.Query
{
    public interface ITraversable : IDisposable
    {
        object Value { get; set; }
        ITraversable Child(string key);
        void Run(string verb);
    }

    public static class Traversable
    {
        public static ITraversable Find(this ITraversable item, EpubPath path)
        {
            ITraversable result = item;
            while (path.ContainsKeys && result != null)
            {
                result = result.Child(path.FirstKey);
                path = path.Rest;
            }
            return result;
        }

        public static object SafeGet(this ITraversable item, EpubPath path)
        {
            object safeGetImpl(ITraversable current, EpubPath currentPath)
            {
                if (currentPath.IsEmpty)
                {
                    return current.Value;
                }

                using (var child = current.Child(currentPath.FirstKey))
                {
                    return safeGetImpl(child, currentPath.Rest);
                }
            }

            // will ensure that all intermediate traversables are disposed
            return safeGetImpl(item, path);
        }

        public static void SafeExecute(this ITraversable item, EpubPath path, Action<ITraversable> action)
        {
            void safeExecuteImpl(ITraversable current, EpubPath currentPath)
            {
                if (currentPath.IsEmpty)
                {
                    action(current);
                    return;
                }

                using (var child = current.Child(currentPath.FirstKey))
                {
                    safeExecuteImpl(child, currentPath.Rest);
                }
            }

            // will ensure that all intermediate traversables are disposed
            safeExecuteImpl(item, path);
        }

        public static T SafeExecute<T>(this ITraversable item, EpubPath path, Func<ITraversable, T> action)
        {
            T safeExecuteImpl(ITraversable current, EpubPath currentPath)
            {
                if (currentPath.IsEmpty)
                {
                    return action(current);
                }

                using (var child = current.Child(currentPath.FirstKey))
                {
                    return safeExecuteImpl(child, currentPath.Rest);
                }
            }

            // will ensure that all intermediate traversables are disposed
            return safeExecuteImpl(item, path);
        }

        public static void SafeRun(this ITraversable item, EpubPath path, string verb)
        {
            SafeExecute(item, path, x => x.Run(verb));
        }
    }
}
