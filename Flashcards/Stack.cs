using ConsoleTableExt;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace Flashcards;

public class Stack
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";

    public static void ManageStacks()
    {
        Console.Clear();

        ViewStacks();

        Console.WriteLine("---------------------------");
        Console.WriteLine("Input a current stack name");
        Console.WriteLine("Or input 0 to exit input.");
        Console.WriteLine("---------------------------");

        string input = Console.ReadLine();
        
        string insertSQL = $"INSERT INTO Stacks(StackName) Values('{input}')";

        switch (input)
        {
            case "0":
                break;

            default:
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(insertSQL, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Stack Created!");
                        Console.WriteLine("Press Enter to continue:");
                        Console.ReadLine();
                    }

                    connection.Close();
                }
                break;
        }
        UserInput.Input();
    }

    public static void ViewStacks()
    {
        string GetStackNames = "SELECT StackName FROM Stacks;";

        SqlCommand command;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (command = new SqlCommand(GetStackNames, connection))
            {
                command.ExecuteNonQuery();
            }

            var tableData = new List<List<object>> { };
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        List<object> rowData = new List<object>
                        {
                            reader.GetString(0)
                        };

                        tableData.Add(rowData);
                    }
                }
                else
                {
                    Console.WriteLine("No data found!");
                }
                
                ConsoleTableBuilder
                    .From(tableData)
                    .WithTitle("Stacks ", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                    .WithColumn("Name")
                    .WithFormat(ConsoleTableBuilderFormat.Alternative)
                    .ExportAndWriteLine(TableAligntment.Left);
            }
        }
    }
}