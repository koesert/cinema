using System;
using System.Data;

class Program
{
    static void Main(string[] args)
    {
        // Maak een nieuwe instantie van de DatabaseHelper klasse
        DatabaseHelper databaseHelper = new DatabaseHelper("../../../db/cinema.db");

        // Open de verbinding met de database
        databaseHelper.OpenConnection();

        // maakt tables
        databaseHelper.CreateTables();

        // Sluit de verbinding met de database
        databaseHelper.CloseConnection();
    }
}
