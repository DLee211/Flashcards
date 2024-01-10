using Microsoft.Data.SqlClient;

namespace Flashcards;

public class Validation
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";

    public static void CheckIfStackNameExists(string? stackName, out bool exists)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = $"SELECT 1 FROM Stacks WHERE StackName = '{stackName}'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StackName", stackName);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    exists = true;
                }
                else
                {
                    exists = false;
                }
            }
        }
    }

    public static void CheckIfFlashCardExists(int continuousFlashcardId, int stackId, out bool exists)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Check if continuousFlashcardId already exists
            string checkIfExistsQuery = $@"
        SELECT 1
        FROM (
            SELECT FlashcardId, ROW_NUMBER() OVER (ORDER BY FlashcardId) AS ContinuousFlashcardId
            FROM Flashcards
            WHERE StackId = '{stackId}'
        ) AS NumberedFlashcards
        WHERE ContinuousFlashcardId = '{continuousFlashcardId}'
    ";

            using (SqlCommand command = new SqlCommand(checkIfExistsQuery, connection))
            {
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    exists = true;
                }
                else
                {
                    exists = false;
                }
            }
        }
    }
}