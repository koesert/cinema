using System;
using System.Collections.Generic;
using System.Data;
using Sharprompt;

class Program
{
    static void Main()
    {
        // Create a new instance of the DatabaseHelper class
        DatabaseHelper databaseHelper = new DatabaseHelper();
        PresentLogin admin = new PresentLogin();
        // Initialize database with tables and movies from specified folder
        databaseHelper.InitializeDatabase();

        ChoiceMethods.GetMovies();

        ProgramChoice currentChoice = ProgramChoice.Login;

        while (currentChoice != ProgramChoice.Exit)
        {
            currentChoice = Prompt.Select<ProgramChoice>("What would you like to do?");

            switch (currentChoice)
            {
                case ProgramChoice.ListMovies:
                    ChoiceMethods.ListMovies();
                    break;
                case ProgramChoice.Login:
                    PresentLogin.Start();
                    break;
                default:
                    break;
            }
        }
    }
}
