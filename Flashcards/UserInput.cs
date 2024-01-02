namespace Flashcards;

public class UserInput
{
    private static string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security=True";
    public static bool closeApp = false;    
    public static void Input()
    {
        Console.Clear();
        Console.WriteLine("\nMenu");
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

            switch (command)
            {
                case "0":
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    Stack.ManageStacks();
                    break;
                // case "2":
                //     ManageFlashCards();
                //     break;
                // case "3":
                //     Study();
                //     break;
                // case "4":
                //     ViewStudyData();
                //     break;
            }
        }
    }
}