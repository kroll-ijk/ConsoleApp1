using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string Path = "";
            int ThreadCount = 0;
            XmlDocument Setts = new XmlDocument();
            try
            {
                Setts.Load("Settings.xml");
            }
            catch (Exception)
            {
                Console.WriteLine("Файл настроек не найден");
                Console.ReadKey();
                return;
            }

            SqlConnectionStringBuilder SCSB = new SqlConnectionStringBuilder();
            SCSB.PersistSecurityInfo = true;
            SCSB.IntegratedSecurity = false;
            foreach (XmlNode FirstN in Setts.ChildNodes)
            {
                if ( FirstN.Name == "Document")
                {
                    foreach (XmlNode XN in FirstN.ChildNodes)
                    {
                        switch (XN.Name)
                        {
                            case "Directory":
                                {
                                    Path = XN.InnerText;
                                    break;
                                }
                            case "ThreadCount":
                                {
                                    try
                                    {
                                        ThreadCount = Convert.ToInt32(XN.InnerText);
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("Количество потоков не число");
                                        Console.ReadKey();
                                        return;
                                    }
                                    break;
                                }

                            case "Connection":
                                {
                                    foreach (XmlNode C in XN.ChildNodes)
                                    {
                                        switch (C.Name)
                                        {
                                            case "ServerName":
                                                {
                                                    SCSB.DataSource = C.InnerText;
                                                    break;
                                                }
                                            case "DataBase":
                                                {
                                                    SCSB.InitialCatalog = C.InnerText;
                                                    break;
                                                }
                                            case "User":
                                                {
                                                    SCSB.UserID = C.InnerText;
                                                    break;
                                                }
                                            case "Password":
                                                {
                                                    SCSB.Password = C.InnerText;
                                                    break;
                                                }
                                        }

                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
                        
            DataRecord dataRecord = new DataRecord( SCSB.ConnectionString);

            Shifrator Sh = new Shifrator(Path, ThreadCount, dataRecord);
        }

    }
}
