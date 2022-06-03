﻿using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

//
//  2020-03-18  Mark Stega
//              Created (See attribution below)
//

namespace ICEBG.DataTier.HelperClasses;

/// Author: Sander van de Velde
/// Reference: https://sandervandevelde.wordpress.com/2017/12/20/zip-and-unzip-a-string-of-data-in-memory/
/// Git: https://github.com/sandervandevelde/ZipHelper
/// 
/// <summary>
/// This is a C# .Net Standard library which enables zipping and unzipping strings in memory.
/// Zip and Unzip in memory using System.IO.Compression.
/// </summary>
public static class ZipHelper
{
    /// <summary>
    /// Zips a string into a zipped byte array.
    /// </summary>
    /// <param name="textToZip">The text to be zipped.</param>
    /// <returns>byte[] representing a zipped stream</returns>
    public static byte[] Zip(string textToZip)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var demoFile = zipArchive.CreateEntry("zipped.txt");

                using (var entryStream = demoFile.Open())
                {
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(textToZip);
                    }
                }
            }

            return memoryStream.ToArray();
        }
    }

    /// <summary>
    /// Unzip a zipped byte array into a string.
    /// </summary>
    /// <param name="zippedBuffer">The byte array to be unzipped</param>
    /// <returns>string representing the original stream</returns>
    public static string Unzip(byte[] zippedBuffer)
    {
        using (var zippedStream = new MemoryStream(zippedBuffer))
        {
            using (var archive = new ZipArchive(zippedStream))
            {
                var entry = archive.Entries.FirstOrDefault();

                if (entry != null)
                {
                    using (var unzippedEntryStream = entry.Open())
                    {
                        using (var ms = new MemoryStream())
                        {
                            unzippedEntryStream.CopyTo(ms);
                            var unzippedArray = ms.ToArray();

                            return Encoding.Default.GetString(unzippedArray);
                        }
                    }
                }

                return null;
            }
        }
    }
}

