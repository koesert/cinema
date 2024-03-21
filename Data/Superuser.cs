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

    private static void AddAdminToDB(string email, string password)
    {   
        DatabaseHelper databaseHelper = new DatabaseHelper();
        string createAdminTableQuery =
            @"
                CREATE TABLE IF NOT EXISTS Superusers (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Email TEXT NOT NULL,
                Password TEXT NOT NULL,
            );";
        
        databaseHelper.ExecuteQuery(createAdminTableQuery);

        string createAdminQuery = @"INSERT INTO Superusers (Email, Password) VALUES ({email}, @password)";

        using (SQLiteCommand command = new SQLiteCommand(insertQuery, databaseHelper._connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@Description", movie.Description);
                    command.Parameters.AddWithValue("@Duration", movie.Duration);
                    command.Parameters.AddWithValue("@Year", movie.Year);
                    command.Parameters.AddWithValue("@Genres", string.Join(",", movie.Genres));
                    command.Parameters.AddWithValue("@Cast", string.Join(",", movie.Cast));

                    // Open the database connection
                    OpenConnection();

                    // Execute the command
                    command.ExecuteNonQuery();

                    // Close the database connection
                    CloseConnection();
                }
        databaseHelper.ExecuteQuery(createAdminQuery);
    }

    private static void AddAdminList()
    {   
        DatabaseHelper databaseHelper = new DatabaseHelper();
        string createAdminTableQuery = @"SELECT * FROM Superusers LIMIT 1";
        DataTable admins = databaseHelper.ExecuteQuery(createAdminTableQuery);

        foreach (DataRow item in admins.Rows)
        {
            string ?email = item["Email"].ToString();
            string ?password = item["Password"].ToString();
            Superusers.Add(new Superuser(email, password));
        }
    }

    public static List<Superuser> GetSuperusers()
    {
        AddAdminToDB("marcel@bioscoop.nl", "admin123");

        AddAdminList();

        return Superusers;
    }
}