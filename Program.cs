using System;
using System.Data;

class Program
{
    static void Main(string[] args)
    {
        // Create a new instance of the DatabaseHelper class
        DatabaseHelper databaseHelper = new DatabaseHelper();
        
        // Initialize database with tables and movies from specified folder
        databaseHelper.InitializeDatabase();
    }
}
