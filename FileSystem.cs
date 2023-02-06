using System;
using System.Collections.Generic;
using System.IO;

namespace Gnutella
{
    class FileSystem
    {
        string mypath = "/home/leon/Documents/Gnutella_Files/";
        string[] files;

        public FileSystem()
        {
            files = Directory.GetFiles(mypath);
        }

        public bool CheckForFile(string filename)
        {
            files = Directory.GetFiles(mypath);
            Console.WriteLine("Checking for file: " + filename);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Replace(mypath, "").ToString().Contains(filename))
                {
                    return true;
                }
            }
            return false;
        }

        public byte[] GetFile(string filename)
        {
            return File.ReadAllBytes(mypath + filename);
        }

        public void CreateFile(string filename, byte[] data)
        {
            using var writer = new BinaryWriter(File.OpenWrite(mypath + filename));
            writer.Write(data);
            Console.WriteLine("File created!");
        }
    }
}