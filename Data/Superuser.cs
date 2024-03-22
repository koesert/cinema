using System.Data;
using System.Data.SQLite;

public class Superuser // This class represents a superuser account with limited instances
{
    public string Email { get; set; } // Email address of the superuser
    public string Password { get; set; } // Password for the superuser account
    public static List<Superuser> Superusers = new List<Superuser>(); // Stores a list of all created superuser accounts

    public Superuser(string email, string password) // Constructor to create a new superuser
    {
        Email = email;
        Password = password;
    }

    private static void CreateAdminTable()
    {
        string createAdminTableQuery = @"
            CREATE TABLE IF NOT EXISTS Superusers (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Email TEXT NOT NULL,
                Password TEXT NOT NULL
            );";
        Database.OpenConnection();
        Database.ExecuteQuery(createAdminTableQuery);
        Database.CloseConnection();
    }

    private static void AddAdminToDB()
    {
        try
        {
            string checkForAdmin = @"SELECT 1 FROM Superusers LIMIT 1";
            Database.OpenConnection();
            DataTable existingAdmin = Database.ExecuteQuery(checkForAdmin);
            Database.CloseConnection();

            if (existingAdmin.Rows.Count == 0)
            {
                string createAdminQuery = @"INSERT INTO Superusers (Email, Password) VALUES ('marcel@bioscoop.nl', 'admin123')";
                Database.OpenConnection();
                Database.ExecuteQuery(createAdminQuery);
                Database.CloseConnection();
            }
        }
        catch (SQLiteException ex)
        {
            Console.WriteLine("Error encountered while adding admin to database: " + ex.Message);
        }
    }

    private static void AddAdminToList()
    {
        Superusers.Add(new Superuser("marcel@bioscoop.nl", "admin123"));
    }

    public static void InitializeSuperuser()
    {
        CreateAdminTable();
        AddAdminToDB();
        AddAdminToList();
    }
}