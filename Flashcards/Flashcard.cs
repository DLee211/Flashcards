using System.Linq.Expressions;
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

        switch (stackName)
        {
            case "0":
                UserInput.Input();
                break;
            
            default:
                string query = $"SELECT StackId FROM Stacks WHERE StackName ='{stackName}'";

                ChooseStack(query, stackName);
                break;
        }
    }

    private static void ChooseStack(string query, string? stackName)
    {
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

        switch (input)
        {
            case "0":
                break;
            
            case "x":
                Console.Clear();
                Stack.ViewStacks();
                Console.WriteLine("Change to which Stack?");
                
                string newstackName= Console.ReadLine();
        
                string query = $"SELECT StackId FROM Stacks WHERE StackName ='{newstackName}'";

                ChooseStack(query, newstackName);
                break;
            
            /*case "v":
                ViewAllFlashCards();
                break;*/
            
            /*case "a":
                ViewXAmountOfCards():
                break;*/
            
            case "c":
                CreateFlashcard(stackName, stackId);
                break;
            
            /*case "e":
                EditFlashcard(stackId);
                break;*/
            
            /*case "d":
                DeleteFlashcard(stackId);
                break;*/
        }
    }

    private static void CreateFlashcard(string stackName, int stackId)
    {
        Console.Clear();
        Console.WriteLine("Input the question here:");
        string Question = Console.ReadLine();
        
        Console.WriteLine("Input the answer here:");
        string Answer = Console.ReadLine();

        string query = $"INSERT INTO Flashcards (Question, Answer, StackId) VALUES ('{Question}', '{Answer}', {stackId});";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        
        currentStackFlashCards(stackName, stackId);
    }
}