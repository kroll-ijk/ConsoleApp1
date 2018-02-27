using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace ConsoleApp1
{
    class Shifrator
    {
        private List<String> Files; // many write
        int files_count;
        private List<Result> Results; //many write
        private Thread FilesObserver;
        List<Thread> Comps;
        private string Path; // 1 read
        int files_writing;
        int files_comp;
        private bool EndWorkObserver; //1 poinr write many read

        private System.Object lockThis = new System.Object();
        private int workingcomps;
        DataRecord dataRecord;

        public Shifrator(string _path, int _countThreads, DataRecord _dataRecord)
        {
            dataRecord = _dataRecord;
            Path = _path;
            Files = new List<string>();
            files_count = 0;
            files_comp = 0;
            Results = new List<Result>();
            Comps = new List<Thread>();
            Thread Log = new Thread(new ThreadStart(Logout));
            FilesObserver = new Thread(new ThreadStart(GetAllFiles));
            if (Directory.Exists(Path))
            {
                FilesObserver.Start();                             
                
                for (int i = 0; i < _countThreads; i++)
                {
                    Thread Comp = new Thread(new ThreadStart(Compute));
                    Comps.Add(Comp);

                    Comp.Start();
                }
                Log.Start();

                Thread Writer = new Thread(new ThreadStart(Write));
                Writer.Start();
            }
            else
            {
                Console.WriteLine("Директория не найдена");
                Console.ReadKey();
            }

        }

        void Compute()
        {
            lock (lockThis)
            {
                workingcomps++;
            }
            int i = 0;
            while (!EndWorkObserver || Files.Count > 0)
            {
                ///Critical Section Start
                string F = "";
                lock (Files)
                {
                    if (Files.Count > 0)
                    {
                        F = Files.ElementAt(0);
                        Files.RemoveAt(0);
                      
                    }
                }

                ///Critical Section End
                if (F != "")
                {
                    Result Res = ComputeOne(F);
                    i++;
                    ///
                    ///Critical Section Start
                    lock (Results)
                    {
                        Results.Add(Res);
                        files_comp++;  //Запись только внутри этой Кс
                    }
                    ///Critical Section End
                    ///
                }
                
            }    
             
              
            


            lock (lockThis)
            {
                workingcomps--;
            }


        }

        void GetAllFiles()
        {
            EndWorkObserver = false;

            string[] subdirs = Directory.GetDirectories(Path);

            string[] Filess = Directory.GetFiles(Path);

            foreach (string F in Filess)
            {
                lock (Files)
                {
                    Files.Add(F);
                    files_count++;
                }
            }

            foreach (string S in subdirs)
            {
                GetFiles(S);
            }

            EndWorkObserver = true;
        }

        void GetFiles(string _path)
        {
            string[] subdirs =  Directory.GetDirectories(_path) ;

            string[] Filess =   Directory.GetFiles(_path);

            foreach (string F in Filess)
            {
                lock (Files)
                {
                    Files.Add(F);
                    files_count++;
                }

            }

            foreach (string S in subdirs)
            {
                GetFiles(S);
            }

            
        }

        Result ComputeOne(string path)
        {            
            FileStream fs = File.OpenRead(path);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fileData = new byte[fs.Length];
            fs.Read(fileData, 0, (int)fs.Length);
            byte[] checkSum = md5.ComputeHash(fileData);
            return new Result(path, BitConverter.ToString(checkSum).Replace("-", String.Empty));
      
        }


        void Logout()
        {
            
            do
            {
                Thread.Sleep(100);
                string Message = "Progress:";
                Console.Write(Message);
                Message = "   Files found:" + files_count;
                Console.Write(Message);
                Message = "   Files coded:" + files_comp;
                Console.Write(Message);
                Message = "   Data writing:" + files_writing;// workingcomps 
                Console.WriteLine(Message);



            } while ((workingcomps != 0 || !EndWorkObserver || Results.Count > 0) && dataRecord.connect == true);
            Console.Read();

        }

        void Write()
        {
            string Query = "INSERT Into Borisov_test.dbo.Session (Root, Start) values ('" + Path + "', GETDATE())";
            if (dataRecord.connect == true)
            {
                if (dataRecord.DataWrite(Query))
                {
                    string Session = dataRecord.ReadSingle("select max( log_id ) from Borisov_test.dbo.Session");
                    WriteResult(Session);
                }
            }
            else
            {
                Close();
            }

        }

        void WriteResult(string Session)
        {
            files_writing = 0;
            while ((workingcomps != 0 || !EndWorkObserver || Results.Count > 0) && dataRecord.connect == true)
            {
                if (Results.Count > 0)
                {
                    Result R;
                    files_writing++;
                    lock (Results)
                    {
                        R = Results.ElementAt(0);
                        Results.RemoveAt(0);
                    }
                    if (R != null)
                    {
                        string Query = "INSERT Into Borisov_test.dbo.Hashes (LogId, FileName, hash) values ('" + Session + "','" + R.Path + "','" + R.Hash + "')";
                        if (!dataRecord.DataWrite(Query))
                            files_writing--;
                    }
                }
            }

        }

        void Close()
        {
            foreach (Thread C in Comps)
            {
                C.Abort();
                FilesObserver.Abort();
                workingcomps = 0;
                EndWorkObserver = true;
            }
        }

    }
}
