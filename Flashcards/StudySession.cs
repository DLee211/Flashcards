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

        string stackName = Console.ReadLine();
        
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

                    ChooseStackForStudy(query, stackName);
                }
                else
                {
                    Console.WriteLine("Stack name does not exist. Press enter to continue");
                    Console.ReadLine();
                    Study();
                }

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

            StartStudySession(stackId,stackName);

            connection.Close();
        }
    }

    private static void StartStudySession(int stackId, string stackName)
    {
        int numCorrectAnswers = 0;
        
        List<FlashcardDTO> flashcards = GetRandomFlashCards(stackId,out string numFlashcards);

        foreach (var flashcard in flashcards)
        {
            Console.WriteLine($"Q:{flashcard.Question}");
            Console.WriteLine("Input your answer:");
            string userAnswer = Console.ReadLine();

            if (userAnswer == flashcard.Answer)
            {
                Console.WriteLine("Your answer was correct");
                numCorrectAnswers++;
            }
            else
            {
                Console.WriteLine("Your answer was incorrect");
                Console.WriteLine($"Your answer:{userAnswer} | Correct answer:{flashcard.Answer}");
            }
        }
        Console.WriteLine($"You got {numCorrectAnswers} out of {numFlashcards} flashcards correct!");
        Console.WriteLine("Press any key to continue");
        Console.ReadLine();

        StudySessionRepository.InsertStudySessionRecords(stackId,stackName, numCorrectAnswers, int.Parse(numFlashcards));

    }

private static List<FlashcardDTO> GetRandomFlashCards(int stackId, out string numFlashcards)
    {
        Console.WriteLine("Input how many flashcards you want to study");
        numFlashcards = Console.ReadLine();
        
        int numCorrectAnswers = 0;
        
        string userAnswer;
        string query =
            $"SELECT TOP {numFlashcards} FlashcardId, Question, Answer FROM Flashcards WHERE StackId = {stackId} ORDER BY NEWID()";
        
        List<FlashcardDTO> flashcards = new List<FlashcardDTO>();
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FlashcardDTO flashcard = new FlashcardDTO
                        {
                            Question = reader.GetString(1),
                            Answer = reader.GetString(2)
                        };

                        flashcards.Add(flashcard);
                    }
                }
            }
            connection.Close();
        }
        return flashcards;
    }
}