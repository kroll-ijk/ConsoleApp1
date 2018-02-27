using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class DataRecord
    {
        SqlConnection connection;
        bool connected;
        public bool connect { get{ return connected; } }
         

        public DataRecord(string connectionstring)
        {
            connection = new SqlConnection(connectionstring);           
            try
            {
                connection.Open();
                connected = true;
            }
            catch (SqlException SE)
            {

                connected = false;
                Console.WriteLine("Подключение к серверу неуспешно: ");
                Console.WriteLine(SE.Message);

                Console.ReadKey();
            }
            
        }
        public bool DataWrite(string Querry)
        {
            if (connected == true)
            {
                try
                {
                    new SqlCommand(Querry, connection).ExecuteNonQuery();
                    return true;
                }
                catch (SqlException SE)
                {
                    connected = false;
                    Console.WriteLine("Ошибка при выполнении запроса: ");
                    Console.WriteLine(SE.Message);

                    Console.ReadKey();
                }

            }

            return false;
        }
        public string ReadSingle(string Querry)
        {
            if (connected == true)
            {
                try
                {
                    return new SqlCommand(Querry, connection).ExecuteScalar().ToString();
                    
                }
                catch (SqlException SE)
                {
                    connected = false;
                    Console.WriteLine("Ошибка при выполнении запроса: ");
                    Console.WriteLine(SE.Message);

                    Console.ReadKey();
                }

            }
            return "";
        }

    }
}
