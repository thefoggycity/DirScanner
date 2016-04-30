using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DirScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string Dir = Environment.CurrentDirectory;
            StreamWriter Result = new StreamWriter(Dir + "\\FileScanResult", false, Encoding.Unicode);
            DirectoryInfo Root = new DirectoryInfo(Dir);
            Result.WriteLine("DirScannerResult {0} {1}", Environment.MachineName, DateTime.Now);
            Result.WriteLine("{0:s}", Root);
            EnumFile(Root, 0, Result);
            Result.Dispose();
        }

        static void EnumFile(DirectoryInfo DirInfo, int Depth, StreamWriter R)
        {
            const string PFUNIT = "-";
            string Prefix = "";
            for (int i = 0; i < Depth; i++)
                Prefix += PFUNIT;
            Prefix += "";

            List<DirectoryInfo> DirEntries = new List<DirectoryInfo>();
            List<FileInfo> FileEntries = new List<FileInfo>();
            DirEntries.Clear();
            FileEntries.Clear();
            try
            {
                DirEntries.AddRange(DirInfo.EnumerateDirectories());
                FileEntries.AddRange(DirInfo.EnumerateFiles());
            }
            catch (System.UnauthorizedAccessException) { };

            foreach (DirectoryInfo d in DirEntries)
            {
                R.WriteLine(" {0:s} {1:s} >", Prefix, d.Name);
                EnumFile(d, Depth + 1, R);
            }
            foreach (FileInfo f in FileEntries)
            {
                if (Depth != 0 || f.Name != "FileScanResult")
                    R.WriteLine(" {0:s} [{1:s}] {2:s}", Prefix, SizeConvert(f.Length), f.Name);
            }
            R.Flush();

            return;
        }

        static string SizeConvert(long SizeInByte)
        {
            const long
                GB = 1073741824,
                MB = 1048576,
                KB = 1024;
            if (SizeInByte > GB)
                return String.Format("{0:f}GB", ((float)SizeInByte / GB));
            else if (SizeInByte > MB)
                return String.Format("{0:f}MB", ((float)SizeInByte / MB));
            else if (SizeInByte > KB)
                return String.Format("{0:f}KB", ((float)SizeInByte / KB));
            else
                return String.Format("{0:d}B", ((int)SizeInByte));
        }
    }
}
