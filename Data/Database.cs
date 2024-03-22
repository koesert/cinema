using System.Data;
using System.Data.SQLite;

public class Database
{
    private static readonly string _databasePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "../../../db/cinema.db"
    );

    public static SQLiteConnection? Connection { get; private set; }

    public static void OpenConnection()
    {
        if (Connection == null || Connection.State != ConnectionState.Open)
        {
            Connection = new SQLiteConnection($"Data Source={_databasePath}");
            Connection.Open();
        }
    }

    public static void CloseConnection()
    {
        if (Connection?.State == ConnectionState.Open)
        {
            Connection.Close();
            Connection = null; // Set to null after closing for clarity
        }
    }

    public static DataTable ExecuteQuery(string query)
    {
        try
        {
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing query: {ex.Message}");
            return null;
        }
        finally
        {
            CloseConnection();
        }
    }
}