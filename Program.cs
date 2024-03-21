using System;
using System.Collections.Generic;
using System.Data;
using Sharprompt;

class Program
{
    static void Main()
    {
        PresentLogin admin = new PresentLogin();
        // Initialize database with tables and movies from specified folder
        DatabaseHelper.InitializeDatabase();

        ChoiceMethods choiceMethods = new ChoiceMethods();
        choiceMethods.GetMovies();

        Choice currentChoice = Choice.Login;

        while (currentChoice != Choice.Exit)
        {
            currentChoice = Prompt.Select<Choice>("What would you like to do?");

            switch (currentChoice)
            {
                case Choice.ListMovies:
                    choiceMethods.ListMovies();
                    break;
                case Choice.Login:
                    PresentLogin.Start();
                    break;
                default:
                    break;
            }
        }
    }
}
