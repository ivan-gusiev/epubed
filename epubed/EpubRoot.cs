using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace epubed
{
    public class EpubRoot : IDisposable
    {
        readonly Stream underlyingStream;
        readonly ZipArchive archive;
        readonly string originalPath = null;

        public EpubRoot(string path) : this(new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
        {
            originalPath = path;
        }

        public EpubRoot(Stream stream)
        {
            underlyingStream = stream;
            archive = new ZipArchive(stream, ZipArchiveMode.Update);
        }

        public OpfModel GetOpf(string path)
        {
            var stream = GetStream(path);

            try
            {
                return new OpfModel(stream);
            }
            catch
            {
                stream.Close(); // do not leave the stream hanging
                throw;
            }
        }

        public Stream GetStream(string path)
        {
            var contentEntry = archive.GetEntry(path);
            if (contentEntry == null)
            {
                throw new KeyNotFoundException($"Entry [{path}] not found in EPUB archive.");
            }
            
            return contentEntry.Open();
        }

        public ContentModel Content
        {
            get
            {
                return new ContentModel(GetOpf(KnownPaths.ContentFile));
            }
        }

        public void Dispose()
        {
            archive.Dispose();
            underlyingStream.Dispose();
        }


    }
}
