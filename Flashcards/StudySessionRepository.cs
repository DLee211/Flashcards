using ConsoleTableExt;
using Microsoft.Data.SqlClient;

namespace Flashcards;

public class StudySessionRepository
{
    private static string connectionString = @"";
    
    public static void InsertStudySessionRecords(int stackId, string stackName, int correctAnswers, int totalAnswers)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string insertQuery = "INSERT INTO StudySessions (StackId, StackName, CorrectAnswers, TotalAnswers, Date) VALUES (@StackId, @StackName, @correctAnswers,@totalAnswers,@Date)";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@StackId", stackId);
                command.Parameters.AddWithValue("@StackName", stackName);
                command.Parameters.AddWithValue("@correctAnswers", correctAnswers);
                command.Parameters.AddWithValue("@totalAnswers", totalAnswers);
                command.Parameters.AddWithValue("@Date", DateTime.Now);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public static void ViewRecords()
    {
        string GetRecords = "SELECT * FROM StudySessions;";

        SqlCommand command;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (command = new SqlCommand(GetRecords, connection))
            {
                command.ExecuteNonQuery();
            }

            var tableData = new List<List<object>> { };
            List<object> rowData = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        rowData = new List<object>
                        {
                            reader.GetString(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4),
                            reader.GetDateTime(5)
                        };

                        tableData.Add(rowData);
                    }
                }
                else
                {
                    Console.WriteLine("No data found!");
                }
            }

            ConsoleTableBuilder
                .From(tableData)
                .WithTitle("Records", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                .WithColumn( "Stack Name", "Correct Answers", "TotalAnswers", "Date")
                .ExportAndWriteLine(TableAligntment.Left);
            
            Console.WriteLine("Press enter to continue:");
            Console.ReadLine();
            
            UserInput.Input();
        }
    }

    public static void DeleteStudySession(string? stackNameToDelete)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            int stackId;
            connection.Open();

            string query = "SELECT StackId FROM Stacks WHERE StackName = @StackName";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StackName", stackNameToDelete);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    stackId = Convert.ToInt32(result);
                    string deleteStudySessionsQuery = $"DELETE FROM StudySessions WHERE StackId = {stackId}";
                    using (SqlCommand deleteStudyCommand = new SqlCommand(deleteStudySessionsQuery, connection))
                    {
                        deleteStudyCommand.Parameters.AddWithValue("@StackId", stackId);
                        deleteStudyCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    Console.WriteLine($"No stack found with the name '{stackNameToDelete}'.");
                }
            }
            connection.Close();
        }
    }
}
