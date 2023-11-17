using System;
using System.Collections.Generic;
using System.IO;

class PasswordSaver
{
    const string ACCOUNTS_FILE = "accounts.txt";
    const string ACCOUNTS_DIR = "registered_accounts/";
    const string PASSWORDS_DIR = "passwords/";

    static void LoadAccounts(Dictionary<string, string> accounts)
    {
        try
        {
            using (StreamReader file = new StreamReader(Path.Combine(ACCOUNTS_DIR, ACCOUNTS_FILE)))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 2)
                    {
                        accounts[parts[0]] = parts[1];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading accounts: {ex.Message}");
        }
    }

    static void SaveAccounts(Dictionary<string, string> accounts)
    {
        try
        {
            if (!Directory.Exists(ACCOUNTS_DIR))
            {
                Directory.CreateDirectory(ACCOUNTS_DIR);
            }

            using (StreamWriter file = new StreamWriter(Path.Combine(ACCOUNTS_DIR, ACCOUNTS_FILE)))
            {
                foreach (var entry in accounts)
                {
                    file.WriteLine($"{entry.Key} {entry.Value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving accounts: {ex.Message}");
        }
    }

    static bool Login(Dictionary<string, string> accounts, ref string loggedInUser)
    {
        string username, password;

        try
        {
            Console.Write("Enter your username: ");
            username = Console.ReadLine();

            Console.Write("Enter your password: ");
            password = Console.ReadLine();

            if (accounts.TryGetValue(username, out string storedPassword) && storedPassword == password)
            {
                Console.WriteLine($"Login successful. Welcome, {username}");
                loggedInUser = username;
                return true;
            }
            else
            {
                Console.WriteLine("Login failed. Incorrect username or password.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            return false;
        }
    }

    static void RegisterAccount(Dictionary<string, string> accounts)
    {
        string username, password;

        try
        {
            Console.Write("Enter a new username: ");
            username = Console.ReadLine();

            if (accounts.ContainsKey(username))
            {
                Console.WriteLine("Username already exists. Please choose a different username.");
                return;
            }

            Console.Write("Enter a new password: ");
            password = Console.ReadLine();

            accounts[username] = password;
            Console.WriteLine($"Registration successful. Welcome, {username}");
            SaveAccounts(accounts);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during registration: {ex.Message}");
        }
    }

    static void SavePassword(string loggedInUser)
    {
        string accountName, password;

        try
        {
            Console.Write("Enter the account name: ");
            accountName = Console.ReadLine();

            Console.Write("Enter the password: ");
            password = Console.ReadLine();

            string userPasswordDir = Path.Combine(PASSWORDS_DIR, loggedInUser) + Path.DirectorySeparatorChar;

            if (!Directory.Exists(userPasswordDir))
            {
                Directory.CreateDirectory(userPasswordDir);
            }

            string filePath = Path.Combine(userPasswordDir, $"{loggedInUser}_{accountName}_password.txt");
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.WriteLine($"Password for account {accountName}: {password}");
            }

            Console.WriteLine($"Password saved for account {accountName}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during saving password: {ex.Message}");
        }
    }

    static void DisplayPasswords(string loggedInUser)
    {
        Console.WriteLine($"Passwords for {loggedInUser}:");

        string userPasswordDir = Path.Combine(PASSWORDS_DIR, loggedInUser) + Path.DirectorySeparatorChar;

        if (!Directory.Exists(userPasswordDir))
        {
            Console.WriteLine($"No passwords saved for {loggedInUser}.");
            return;
        }

        foreach (string filePath in Directory.GetFiles(userPasswordDir))
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {filePath}. {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {filePath}. {ex.Message}");
            }
        }
    }

    static void Main(string[] args)
    {
        Dictionary<string, string> accounts = new Dictionary<string, string>();
        string loggedInUser = "";

        LoadAccounts(accounts);

        int choice;
        do
        {
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Register");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            while (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a valid number: ");
            }

            if (choice == 1)
            {
                if (Login(accounts, ref loggedInUser))
                {
                    int loggedInChoice;
                    do
                    {
                        Console.WriteLine("\nLogged In Menu (enter a number 0-2):");
                        Console.WriteLine("1. Save Password");
                        Console.WriteLine("2. Display Passwords");
                        Console.WriteLine("0. Log Out");
                        Console.Write("Choose an option: ");

                        while (!int.TryParse(Console.ReadLine(), out loggedInChoice))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number: ");
                        }

                        if (loggedInChoice == 1)
                        {
                            SavePassword(loggedInUser);
                        }
                        else if (loggedInChoice == 2)
                        {
                            DisplayPasswords(loggedInUser);
                        }
                        else if (loggedInChoice == 0)
                        {
                            Console.WriteLine("Logged out");
                        }
                        else
                        {
                            Console.WriteLine("Invalid option");
                        }
                    } while (loggedInChoice != 0);
                }
            }
            else if (choice == 2)
            {
                RegisterAccount(accounts);
            }
            else if (choice == 0)
            {
                SaveAccounts(accounts);
                return;
            }
            else
            {
                Console.WriteLine("Invalid option");
            }

        } while (choice != 0);
    }
}
