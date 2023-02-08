using System;

namespace Gnutella
{
    class FileSystem
    {
        string mypath = "/home/leon/Documents/Gnutella_Files/";
        string[] files;

        InformationBox alerts;

        public FileSystem(InformationBox alerts)
        {
            //updating the stored files
            files = Directory.GetFiles(mypath);
            this.alerts = alerts;
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
            //removing the file first if it exists to make sure that they dont merge
            if (File.Exists(mypath + filename))
            {
                File.Delete(mypath + filename);
            }
            else
            {
                using var writer = new BinaryWriter(File.OpenWrite(mypath + filename));
                writer.Write(data);
                Console.WriteLine("File created!");
                alerts.ShowInfo("File created! \n(" + filename + ")");
            }
        }
    }
}