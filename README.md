# User Account Management System

This C++ program implements a simple user account management system. It allows users to register new accounts, log in, save and display passwords for their accounts. The program uses file I/O and the c++ filesystem library to manage user data.

## Features

1. **Account Registration:**
   - New users can register by providing a unique username and password.

2. **User Authentication:**
   - Users can log in with a username and password.
   - Passwords are stored securely in a map.

3. **Password Management:**
   - Logged-in users can save passwords for different accounts.
   - Passwords are stored in separate files using the filesystem library.

4. **File I/O:**
   - User account information is loaded and saved from/to files.
   - Passwords are stored in individual files for each user.

## File Structure

- The `registered_accounts/` directory stores the account information in the file `accounts.txt`.
- Passwords are saved in the `passwords/` directory, with each user having their own subdirectory.

## Usage

1. Run the program and choose between logging in, registering a new account, or exiting.
2. If logging in, users can save passwords or display saved passwords.
3. Passwords are saved in separate files for each account.

## Dependencies

- C++ Standard Library
- C++ Filesystem Library

## How to Run

Compile the program using a C++ compiler:

```bash
g++ -o account_management account_management.cpp
