#include <iostream>
#include <fstream>
#include <string>
#include <map>
#include <filesystem>

using namespace std;
namespace FS = filesystem;

const string ACCOUNTS_DIR = "registered_accounts/";
const string PASSWORDS_DIR = "passwords/";
const string ACCOUNTS_FILE = "accounts.txt";

void createFolder()
{
    if (!FS::exists(PASSWORDS_DIR) && !FS::create_directory(PASSWORDS_DIR))
    {
        cout << "Error creating the 'passwords/' directory." << endl;
        return;
    }
}

void loadAccounts(map<string, string>& accounts)
{
    ifstream file(ACCOUNTS_DIR + ACCOUNTS_FILE);

    if (file.is_open())
    {
        string username, password;
        while (file >> username >> password)
        {
            accounts[username] = password;
        }
        file.close();
    }
}

void saveAccounts(const map<string, string>& accounts)
{
    if (!FS::exists(ACCOUNTS_DIR) && !FS::create_directory(ACCOUNTS_DIR))
    {
        cout << "Error creating the 'registered_accounts/' directory." << endl;
        return;
    }

    ofstream file(ACCOUNTS_DIR + ACCOUNTS_FILE);

    if (file.is_open())
    {
        for (const auto& entry : accounts)
        {
            file << entry.first << " " << entry.second << endl;
        }
        file.close();
    }
    else
    {
        cout << "Error opening the file to save accounts." << endl;
    }
}


bool login(const map<string, string>& accounts, string& loggedInUser)
{
    string username, password;

    try {
        cout << "Enter your username: ";
        cin >> username;

        cout << "Enter your password: ";
        cin >> password;

        auto user = accounts.find(username);
        if (user != accounts.end() && user->second == password)
        {
            cout << "Login successful. Welcome, " << username << endl;
            loggedInUser = username;
            return true;
        }
        else
        {
            cout << "Login failed. Incorrect username or password." << endl;
            return false;
        }
    }
    catch (const exception& exc)
    {
        cerr << "Error during login: " << exc.what() << endl;
        return false;
    }
}

void registerAccount(map<string, string>& accounts)
{
    string username, password;

    try {
        cout << "Enter a new username: ";
        cin >> username;

        if (accounts.find(username) != accounts.end())
        {
            cout << "Username already exists. Please choose a different username." << endl;
            return;
        }

        cout << "Enter a new password: ";
        cin >> password;

        accounts[username] = password;
        cout << "Registration successful. Welcome, " << username << endl;
        saveAccounts(accounts);
    }
    catch (const exception& exc)
    {
        cerr << "Error during registration: " << exc.what() << endl;
    }
}

void savePassword(const string& loggedInUser)
{
    string accountUsername, password;

    try {
        createFolder();  // Ensure "passwords" directory exists

        cout << "Enter the account name: ";
        cin >> accountUsername;

        cout << "Enter the password: ";
        cin >> password;

        string userPasswordDir = PASSWORDS_DIR + loggedInUser + "/";

        if (!FS::exists(userPasswordDir) && !FS::create_directory(userPasswordDir))
        {
            cout << "Error creating the user's password directory." << endl;
            return;
        }

        ofstream file(userPasswordDir + loggedInUser + "_" + accountUsername + "_password.txt");
        if (file.is_open())
        {
            file << "Password for account " << accountUsername << ": " << password << endl;
            file.close();
            cout << "Password saved for account " << accountUsername << "." << endl;
        }
        else
        {
            cout << "Error opening the file to save the password" << endl;
        }
    }
    catch (const exception& exc)
    {
        cerr << "Error during saving password: " << exc.what() << endl;
    }
}

void displayPasswords(const string& loggedInUser)
{
    cout << "Passwords for " << loggedInUser << ":\n";

    string userPasswordDir = PASSWORDS_DIR + loggedInUser + "/";

    if (!FS::exists(userPasswordDir))
    {
        cout << "No passwords saved for " << loggedInUser << endl;
        return;
    }

    for (const auto& entry : FS::directory_iterator(userPasswordDir))
    {
        ifstream file(entry.path());
        if (file.is_open())
        {
            string line;
            while (getline(file, line))
            {
                cout << line << endl;
            }
            file.close();
        }
        else
        {
            cout << "Error opening file: " << entry.path() << endl;
        }
    }
}

int main()
{
    map<string, string> accounts;
    string loggedInUser;

    createFolder();

    loadAccounts(accounts);

    int choice;
    do {
        cout << "1. Log In\n";
        cout << "2. Register\n";
        cout << "0. Exit\n";
        cout << "Choose an option: ";

        while (!(cin >> choice))
        {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Invalid input. Please enter a number: ";
        }

        if (choice == 1)
        {
            if (login(accounts, loggedInUser))
            {
                int loggedInChoice;
                do {
                    cout << "\nLogged In Menu(enter a number 0-2):\n";
                    cout << "1. Save Password\n";
                    cout << "2. Display Passwords\n";
                    cout << "0. Log Out\n";
                    cout << "Choose an option: ";

                    while (!(cin >> loggedInChoice))
                    {
                        cin.clear();
                        cin.ignore(numeric_limits<streamsize>::max(), '\n');
                        cout << "Invalid input. Please enter a number: ";
                    }

                    if (loggedInChoice == 1)
                    {
                        savePassword(loggedInUser);
                    }
                    else if (loggedInChoice == 2)
                    {
                        displayPasswords(loggedInUser);
                    }
                    else if (loggedInChoice == 0)
                    {
                        cout << "Logged out\n";
                    }
                    else
                    {
                        cout << "Invalid option" << endl;
                    }
                } while (loggedInChoice != 0);
            }
        }
        else if (choice == 2)
        {
            registerAccount(accounts);
        }
        else if (choice == 0)
        {
            saveAccounts(accounts);
            return 0;
        }
        else
        {
            cout << "Invalid option" << endl;
        }

    } while (choice != 0);

    return 0;
}
    
