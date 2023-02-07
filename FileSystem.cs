using System;

namespace Gnutella
{
    class FileSystem
    {
        string mypath = "/home/leon/Documents/Gnutella_Files/";
        string[] files;

        public FileSystem()
        {
            //updating the stored files
            files = Directory.GetFiles(mypath);
        }

        public bool CheckForFile(string filename)
        {
            //updating the stored files
            files = Directory.GetFiles(mypath);

            //checking if any of the stored files meet the requirements (filename)
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
            //return file in byte-format
            return File.ReadAllBytes(mypath + filename);
        }

        public void CreateFile(string filename, byte[] data)
        {
            //creating a file from given bytes
            using var writer = new BinaryWriter(File.OpenWrite(mypath + filename));
            writer.Write(data);
            Console.WriteLine("File created!");
        }
    }
}