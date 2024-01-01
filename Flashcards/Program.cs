
using Microsoft.Data.SqlClient;

namespace Flashcards
{
    class Program
    {
        private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";

        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    table.CommandText = @"CREATE TABLE IF NOT EXISTS coding_tracker(
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Date TEXT, 
                                        StartTime TEXT NOT NULL,
                                        EndTime TEXT NOT NULL,
                                        Duration INTEGER,
                                        CHECK (StartTime < EndTime))";

                    tableCmd.ExecuteNonQuery();


                    Console.WriteLine("Connected to LocalDB successfully.");

                    // Close the connection when done
                    connection.Close();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during connection
                    Console.WriteLine("Error: " + ex.Message);
                }
            } 
        }
    }
}