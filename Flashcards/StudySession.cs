using Microsoft.Data.SqlClient;

namespace Flashcards;

public class StudySession
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";
    static int stackId;
    public static void Study()
    {
        Console.Clear();
        Stack.ViewStacks();
        Console.WriteLine("Choose stack to study.");
        
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

                ChooseStackForStudy(query, stackName);
                break;
        }
    }

    private static void ChooseStackForStudy(string query, string? stackName)
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

            StartStudySession(stackId);
            
            connection.Close();
        }
    }

    private static void StartStudySession(int stackId)
    {
        Console.WriteLine("Input how many flashcards you want to study");
        string numFlashcards = Console.ReadLine();
        
        int numCorrectAnswers = 0;

        string question;
        string answer;

        string userAnswer;
        string query =
            $"SELECT TOP {numFlashcards} FlashcardId, Question, Answer FROM Flashcards WHERE StackId = {stackId} ORDER BY NEWID()";
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        question = reader.GetString(1); // Assuming StackId is in the first column
                        answer = reader.GetString(2);
                        
                        Console.Clear();
                        Console.WriteLine($"Q:{question}");
                        Console.WriteLine("Input your answer:");
                        userAnswer = Console.ReadLine();

                        if (userAnswer == answer)
                        {
                            Console.WriteLine("Your answer was correct");
                            numCorrectAnswers++;
                        }
                        else
                        {
                            Console.WriteLine("Your answer was incorrect");
                            Console.WriteLine($"Your answer:{userAnswer} | Correct answer:{answer}");
                        }
                        
                        Console.WriteLine("Press any key to continue");
                        Console.ReadLine();
                    }
                }
                Console.WriteLine($"You got {numCorrectAnswers} out of {numFlashcards} flashcards correct!");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }
            connection.Close();
        }
    }
}