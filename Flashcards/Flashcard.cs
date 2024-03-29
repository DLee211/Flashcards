﻿using System.Linq.Expressions;
using System.Net.NetworkInformation;
using ConsoleTableExt;
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
        
        Validation.CheckIfStackNameExists(stackName, out bool exists);

        switch (stackName)
        {
            case "0":
                UserInput.Input();
                break;
            
            default:
                if (exists)
                {
                    string query = $"SELECT StackId FROM Stacks WHERE StackName ='{stackName}'";
                    ChooseStack(query, stackName);
                }
                else
                {
                    Console.WriteLine("Stack doesn't exist. Press Enter to continue.");
                    Console.ReadLine();
                    ManageFlashCards();
                }
                break;
        }
    }

    public static void ChooseStack(string query, string? stackName)
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

        while (true)
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
                    UserInput.Input();
                    break;

                case "x":
                    Console.Clear();
                    ManageFlashCards();
                    break;

                case "v":
                    Console.Clear();
                    ViewAllFlashCards(stackName, stackId);
                    Console.ReadLine();
                    currentStackFlashCards(stackName, stackId);
                    break;

                case "a":
                    Console.Clear();
                    ViewXAmountOfCards(stackId);
                    break;

                case "c":
                    Console.Clear();
                    CreateFlashcard(stackName, stackId);
                    break;

                case "e":
                    Console.Clear();
                    ViewAllFlashCards(stackName, stackId);
                    EditFlashcard(stackId);
                    break;

                case "d":
                    Console.Clear();
                    ViewAllFlashCards(stackName,stackId);
                    DeleteFlashcard( stackId);
                    break;
                
                default:
                    Console.WriteLine("Invalid input. Please enter a valid input. Press enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void EditFlashcard(int stackId)
    {
        int Id;
        Console.WriteLine("Input the Id of the flashcard you want to edit:");

        string continuousFlashcardId = Console.ReadLine();
        
        while (!int.TryParse(continuousFlashcardId, out Id))
        {
            Console.WriteLine("Id has to be an integer!");
            continuousFlashcardId = Console.ReadLine();
        }
        
        Validation.CheckIfFlashCardExists(Id, stackId, out bool exists);

        if (exists)
        {

            Console.WriteLine("Input in the new question:");

            string newQuestion = Console.ReadLine();

            Console.WriteLine("Input the new answer:");

            string newAnswer = Console.ReadLine();

            string query =
                $"DECLARE @stackId INT = {stackId}; DECLARE @continuousFlashcardId INT = {Id}; UPDATE Flashcards SET Question = '{newQuestion}', Answer = '{newAnswer}' WHERE FlashcardId = (SELECT FlashcardId FROM(SELECT FlashcardId, ROW_NUMBER() OVER (ORDER BY FlashcardId) AS ContinuousFlashcardId FROM Flashcards WHERE StackId =  @stackId) AS NumberedFlashcards WHERE ContinuousFlashcardId = @continuousFlashcardId) AND StackId = @stackId;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        else
        {
            Console.WriteLine("Flashcard does not exist. Press press enter to continue");
            Console.ReadLine();
        }
    }

    private static void ViewXAmountOfCards(int stackId)
    {
        string query = $"SELECT COUNT(*) FROM Flashcards WHERE StackId = {stackId} ";
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    int rowCount = (int)command.ExecuteScalar();
                    
                    Console.WriteLine($"There are {rowCount} cards in this stack.");
                    Console.WriteLine("Press Enter to continue:");
                    Console.ReadLine();
                }
            }
            connection.Close();
        }
    }

    private static void DeleteFlashcard(int stackId)
    {
        Console.WriteLine("Input the id of the flashcard you want to delete:");
        string id = Console.ReadLine();

        int Id;
        
        while (!int.TryParse(id, out Id))
        {
            Console.WriteLine("Id has to be an integer!");
            id = Console.ReadLine();
        }
        
        Validation.CheckIfFlashCardExists(Id,stackId, out bool exists);
        if (exists)
        {
            string query =
                $"DECLARE @YourFlashcardId INT; SELECT @YourFlashcardId = FlashcardId FROM (SELECT FlashcardId, ROW_NUMBER() OVER (ORDER BY FlashcardId) AS ContinuousFlashcardId FROM Flashcards WHERE StackId = {stackId}) AS NumberedFlashcards WHERE ContinuousFlashcardId = {Id}; DELETE FROM Flashcards WHERE FlashcardId = @YourFlashcardId AND StackId = {stackId}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        else
        {
            Console.WriteLine("Flashcard does not exist! Press enter to continue");
            Console.ReadLine();
        }
    }

    private static void ViewAllFlashCards(string stackName, int stackId)
    {
        string query = $"WITH NumberedFlashcards AS (SELECT FlashcardId, Question, Answer, ROW_NUMBER() OVER (ORDER BY FlashcardId) AS ContinuousFlashcardId FROM Flashcards WHERE StackId = {stackId})SELECT ContinuousFlashcardId, Question, Answer FROM NumberedFlashcards;";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var tableData = new List<List<object>> { };
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            List<object> rowData = new List<object>
                            {
                                reader.GetInt64(0),
                                reader.GetString(1),
                                reader.GetString(2)
                            };
                            tableData.Add(rowData);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found!");
                    }
                    
                    Console.Clear();

                    ConsoleTableBuilder
                        .From(tableData)
                        .WithTitle($"{stackName} Flashcards", ConsoleColor.Yellow, ConsoleColor.DarkGray)
                        .WithColumn("FlashcardId", "Question","Answer")
                        .WithFormat(ConsoleTableBuilderFormat.Alternative)
                        .ExportAndWriteLine(TableAligntment.Center);
                }
            }
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

    public static void DeleteStackFlashcards(string? stackNameToDelete)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            int stackId;
            connection.Open();

            string query = $"SELECT StackId FROM Stacks WHERE StackName = '{stackNameToDelete}'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    stackId = Convert.ToInt32(result);
                    string deleteFlashcardsQuery = $"DELETE FROM Flashcards WHERE StackId = {stackId}";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteFlashcardsQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@StackId", stackId);
                        deleteCommand.ExecuteNonQuery();
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
