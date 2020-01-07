using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RePKG.Core.Package;
using RePKG.Core.Package.Interfaces;

namespace RePKG.Application
{
    public class PackageWriter : IPackageWriter
    {
        public void WriteToStream(Core.Package.Package package, Stream stream)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            if (package.Entries.Count == 0)
                throw new Exception("Package entries is empty");
            
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.WriteStringI32Size(package.Magic);
                WriteEntriesHeader(package.Entries, writer);
                WriteBody(package.Entries, writer);
            }
        }

        private static void WriteEntriesHeader(ICollection<PackageEntry> entries, BinaryWriter writer)
        {
            writer.Write(entries.Count);

            var currentOffset = 0;
            
            foreach (var entry in entries)
            {
                if (entry == null)
                    throw new NullReferenceException("Entry is null");
                
                if (string.IsNullOrWhiteSpace(entry.FullPath))
                    throw new NullReferenceException($"Entry property `{nameof(entry.FullPath)}` is null or empty");
                
                if (entry.Bytes == null)
                    throw new NullReferenceException($"Entry property `{nameof(entry.Bytes)}` is null");
                
                writer.WriteStringI32Size(entry.FullPath);

                writer.Write(currentOffset);
                writer.Write(entry.Bytes.Length);

                entry.Offset = currentOffset;
                entry.Length = entry.Bytes.Length;

                currentOffset += entry.Bytes.Length;
            }
        }

        private static void WriteBody(ICollection<PackageEntry> entries, BinaryWriter writer)
        {
            foreach (var entry in entries)
            {
                writer.Write(entry.Bytes);
            }
        }
    }
}