using System.Net.NetworkInformation;
using Microsoft.Data.SqlClient;

namespace Flashcards;

public class Flashcard
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";
    static int stackId;
    public static void ManageFlashCards()
    {
        Console.Clear();
        Stack.ViewStacks();
        Console.WriteLine("Choose a stack of flashcards to interact with: ");
        Console.WriteLine("\n");
        Console.WriteLine("-------------------------------");
        Console.WriteLine("Input a current stack name");
        Console.WriteLine("or Input 0 to exit input");
        Console.WriteLine("-------------------------------");
        string stackName= Console.ReadLine();
        
        string query = $"SELECT StackId FROM Stacks WHERE StackName ='{stackName}'";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StackName", stackName);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stackId = reader.GetInt32(0); // Assuming StackId is in the first column
                        Console.WriteLine($"StackId for {stackName}: {stackId}");
                        Console.ReadLine();
                    }
                }
            }

            currentStackFlashCards(stackName, stackId);
            
            connection.Close();
        }
        
        UserInput.Input();
    }

    private static void currentStackFlashCards(string stackName, int stackId)
    {
        Console.Clear();
        Console.WriteLine($"Current working stack: {stackName}");
        Console.WriteLine("-------------------------------");
        Console.WriteLine("0 to return to main menu");
        Console.WriteLine("x to change current stack");
        Console.WriteLine("v to view all flashcards in stack");
        Console.WriteLine("a to view x amount of cards in stack");
        Console.WriteLine("c to create a flashcard in current stack");
        Console.WriteLine("e to edit a flashcard");
        Console.WriteLine("d to delete a flashcard");
        Console.WriteLine("-------------------------------");

        string input = Console.ReadLine();
    }
}