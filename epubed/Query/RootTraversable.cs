using System;

namespace epubed.Query
{
    public class RootTraversable : ITraversable
    {
        readonly string name;
        readonly EpubRoot root;

        public RootTraversable(string epubName, EpubRoot epubRoot)
        {
            name = epubName ?? throw new ArgumentNullException(nameof(epubName));
            root = epubRoot ?? throw new ArgumentNullException(nameof(epubRoot));
        }

        public object Value
        {
            get
            {
                return name;
            }
            set
            {
                // cannot set root name
            }
        }

        public ITraversable Child(string key)
        {
            if (key == "Content")
            {
                return new ContentTraversable(root.Content);
            }
            if (key == "CanLoadContent")
            {
                return new CanLoadContentTraversable(this);
            }
            else
            {
                return null;
            }
        }

        public void Run(string verb)
        {
            switch (verb.ToLower())
            {
                case "fix-content-length":
                    using (var stream = root.GetStream(KnownPaths.ContentFile))
                    {
                        stream.SetLength(stream.Length - 1);
                    }
                    break;
            }
        }

        public void Dispose()
        {
            root.Dispose();
        }

        #region Fake Children

        private class CanLoadContentTraversable : ITraversable
        {
            private RootTraversable parent;

            public CanLoadContentTraversable(RootTraversable parent)
            {
                this.parent = parent;
            }

            public object Value
            {
                get
                {
                    try
                    {
                        using (parent.root.Content)
                        {
                            return "true";
                        }
                    }
                    catch
                    {
                        return "false";
                    }
                }
                set { }
            }

            public ITraversable Child(string key)
            {
                return null;
            }

            public void Dispose()
            {
                
            }

            public void Run(string verb)
            {
                
            }
        }

        #endregion
    }
}
