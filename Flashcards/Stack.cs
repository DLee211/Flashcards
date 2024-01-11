using System.Xml;
using ConsoleTableExt;
using Microsoft.Data.SqlClient;

namespace Flashcards;

public class Stack
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";

    public static void AddStacks()
    {
        Console.Clear();

        ViewStacks();

        Console.WriteLine("---------------------------");
        Console.WriteLine("Input new stack name to add");
        Console.WriteLine("Or input 0 to exit input.");
        Console.WriteLine("---------------------------");

        string input = Console.ReadLine();

        Validation.CheckIfStackNameExists(input, out bool exists);

        if (exists)
        {
            Console.WriteLine("Stack name already exists! Press enter to continue");
            Console.ReadLine();
            AddStacks();
        }
        else
        {
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
        }
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
            List<object> rowData = null;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        rowData = new List<object>
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
            }

            ConsoleTableBuilder
                .From(tableData)
                .WithTitle("Stacks ", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                .WithColumn("Name")
                .ExportAndWriteLine(TableAligntment.Left);

            rowData.Clear();
            tableData.Clear();
        }
    }

    public static void ManageStacks()
    {
        bool flag = true;
        while (flag)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------");
            Console.WriteLine("0 to return to main menu");
            Console.WriteLine("v to view all stacks");
            Console.WriteLine("c to create a new stack");
            Console.WriteLine("d to delete a stack");
            Console.WriteLine("-------------------------------");

            string input = Console.ReadLine();

            switch (input)
            {
                case "0":
                    UserInput.Input();
                    break;

                case "v":
                    Console.Clear();
                    ViewStacks();
                    Console.ReadLine();
                    break;

                case "c":
                    Console.Clear();
                    AddStacks();
                    break;

                case "d":
                    Console.Clear();
                    DeleteStack();
                    break;
            }
        }
    }

    private static void DeleteStack()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            bool flag = true;

            while (true)
            {
                ViewStacks();
                Console.WriteLine("Input the stack you want to delete");
                Console.WriteLine("Or input 0 to exit input.");
                string stackNameToDelete = Console.ReadLine();

                switch (stackNameToDelete)
                {
                    case "0":
                        ManageStacks();
                        break;

                    default:
                        Console.Clear();
                        Validation.CheckIfStackNameExists(stackNameToDelete, out bool exists);
                        if (exists)
                        {
                            flag = false;
                            StudySessionRepository.DeleteStudySession(stackNameToDelete);

                            Flashcard.DeleteStackFlashcards(stackNameToDelete);

                            string query = $"DELETE FROM Stacks WHERE StackName = '{stackNameToDelete}'";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@StackName", stackNameToDelete);

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine(
                                        $"Stack '{stackNameToDelete}' deleted successfully. Press enter to continue");
                                    Console.ReadLine();
                                }
                                else
                                {
                                    Console.WriteLine(
                                        $"No stack found with the name '{stackNameToDelete}'. Press enter to continue");
                                    Console.ReadLine();
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Stack does not exist! Press enter to continue:");
                            Console.ReadLine();
                        }
                        break;
                }
            }
        }
    }
}