namespace Flashcards;

public class UserInput
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";
    public static void Input()
    {
        bool closeApp = false;   
        Console.Clear();
        Console.WriteLine("\nMain Menu");
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine("\n0: Exit");
        Console.WriteLine("1: Manage Stacks ");
        Console.WriteLine("2: Manage FlashCards");
        Console.WriteLine("3: Study");
        Console.WriteLine("4: View study session data");
        Console.WriteLine("\n--------------------------------------------");

        string command = Console.ReadLine();

        while (closeApp == false)
        {
            if (int.TryParse(command, out int userInput))
            {
                switch (userInput)
                {
                    case 0:
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case 1:
                        Stack.ManageStacks();
                        break;
                    case 2:
                        Flashcard.ManageFlashCards();
                        break;
                    case 3:
                        StudySession.Study();
                        break;
                    case 4:
                        Console.Clear();
                        StudySessionRepository.ViewRecords();
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 4.");
                        command = Console.ReadLine();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid Input, Please enter a valid integer");
                command = Console.ReadLine();
            }
        }
    }
}