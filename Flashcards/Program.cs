
using Microsoft.Data.SqlClient;

namespace Flashcards
{
    class Program
    {
        private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";

        static string createStacksTableSql = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stacks')" +
                                      "   CREATE TABLE Stacks (" +
                                      "       StackId INT PRIMARY KEY," +
                                      "       StackName NVARCHAR(50))";

        // Define the SQL command to create Flashcards table if it doesn't exist
        static string createFlashcardsSql = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Flashcards')" +
                                          "   CREATE TABLE Flashcards (" +
                                          "       FlashcardId INT PRIMARY KEY," +
                                          "       Question NVARCHAR(255)," +
                                          "       Answer NVARCHAR(255)," +
                                          "       StackId INT FOREIGN KEY REFERENCES Stacks(StackId))";

        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(createStacksTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Stack table created successfully");
                    }
                    
                    using (SqlCommand command = new SqlCommand(createFlashcardsSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Flashcards table created successfully!");
                    }

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