using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace epubed
{
    public class OpfModel : IDisposable
    {
        readonly Stream fileStream;
        public XDocument Document { get; }
        public XmlNamespaceManager NamespaceManager { get; } = new XmlNamespaceManager(new NameTable());

        public OpfModel(Stream stream)
        {
            fileStream = stream;
            Document = XDocument.Load(stream);
            NamespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
        }

        public void SaveChanges()
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.SetLength(0);
            Document.Save(fileStream);
        }

        public void Dispose()
        {
            fileStream.Dispose();
        }
    }
}
