using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace epubed
{
    public class ContentModel : IDisposable
    {
        readonly OpfModel opf;

        public ContentModel(OpfModel opf)
        {
            this.opf = opf;
        }
        
        public string this[string path]
        {
            get => XPath(path)?.Value;
            set
            {
                var node = XPath(path);
                if (node != null) node.Value = value;
            }
        }

        public void SaveChanges()
        {
            opf.SaveChanges();
        }

        private XElement XPath(string path)
        {
            return opf.Document.XPathSelectElement(path, opf.NamespaceManager);
        }

        public void Dispose()
        {
            opf.Dispose();
        }
    }
}
