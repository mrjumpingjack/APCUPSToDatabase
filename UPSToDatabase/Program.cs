using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace UPSToDatabase
{
    internal class Program
    {

        private static string connectionString;
        static void Main(string[] args)
        {

            // Load the JSON configuration file
            string configPath = "Config.conf";
            string json = File.ReadAllText(configPath);
            dynamic config = JsonConvert.DeserializeObject(json);

            // Build the MySQL connection string
            connectionString = string.Format(
                "server={0};user={1};password={2};database={3};",
                config.DatabaseUri,
                config.DatabaseUser,
                config.DatabasePassword,
                config.DatabaseName
            );





            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "OnBattery":
                        InsertDataIntoData("UPS", new Dictionary<string, string>() { { "Name", "Battery" }, { "Value", "On" } });
                        break;

                    case "OffBattery":
                        InsertDataIntoData("UPS", new Dictionary<string, string>() { { "Name", "Battery" }, { "Value", "Off" } });
                        break;



                    case "ConnectionEst":
                        InsertDataIntoData("UPS", new Dictionary<string, string>() { { "Name", "Connection" }, { "Value", "Est" } });
                        break;

                    case "ConnectionLost":
                        InsertDataIntoData("UPS", new Dictionary<string, string>() { { "Name", "Connection" }, { "Value", "Lost" } });
                        break;



                    case "SelfTestOK":
                        InsertDataIntoData("UPS", new Dictionary<string, string>() { { "Name", "SelfTest" }, { "Value", "OK" } });
                        break;

                    case "SelfTestFailed":
                        InsertDataIntoData("UPS", new Dictionary<string, string>() { { "Name", "SelfTest" }, { "Value", "Failed" } });
                        break;
                }
            }
        }

        public static void InsertDataIntoData(String Table, Dictionary<string, string> data)
        {

            string query = "";

            try
            {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {

                    query = "INSERT INTO " + Table + " (";

                    foreach (var dp in data)
                    {
                        query += "`" + dp.Key + "`, ";
                    }
                    query = query.Trim(' ').Trim(',');

                    query += ") VALUES (";

                    foreach (var dp in data)
                    {
                        query += "'" + dp.Value + "', ";
                    }

                    query = query.Trim(' ').Trim(',');

                    query += ");";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();                       
                    }

                    Console.WriteLine(query);
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}