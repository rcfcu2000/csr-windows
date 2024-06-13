using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Install.Common
{
    public static class ZipHelper
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="extractPath"></param>
        /// <param name="progressEvent"></param>
        public static void Decompress(byte[] zipFile, string extractPath, Action<long, long> progressEvent)
        {
            extractPath = extractPath.TrimEnd('/') + "//";
            byte[] data = new byte[1024 * 1024];
            long maxSize = 0;
            long accumSize = 0;
            using (ZipInputStream s = new ZipInputStream(new System.IO.MemoryStream(zipFile)))
            {
                ZipEntry entry;
                while ((entry = s.GetNextEntry()) != null)
                {
                    maxSize += entry.Size;
                }
                progressEvent?.Invoke(accumSize, maxSize);
            }
            using (ZipInputStream s = new ZipInputStream(new System.IO.MemoryStream(zipFile)))
            {
                ZipEntry entry;
                while ((entry = s.GetNextEntry()) != null)
                {
                    try
                    {
                        accumSize += entry.Size;
                        string directoryName = Path.GetDirectoryName(entry.Name);
                        string fileName = Path.GetFileName(entry.Name);

                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(extractPath + directoryName);
                        }
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(extractPath + entry.Name.Replace("/", "//")))
                            {
                                while (true)
                                {
                                    int count = s.Read(data, 0, data.Length);
                                    if (count > 0)
                                    {
                                        streamWriter.Write(data, 0, count);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        progressEvent?.Invoke(accumSize, maxSize);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
