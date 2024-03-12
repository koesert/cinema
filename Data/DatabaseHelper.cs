using System.Data.SQLite;
using System.Data;
using System.IO;

public class DatabaseHelper
{
    private SQLiteConnection _connection;
    private string _databasePath;

    public DatabaseHelper(string databasePath)
    {
        _databasePath = databasePath;

        // Controleer of het databasebestand bestaat
        if (!Directory.Exists("../../../db"))
        {
            Directory.CreateDirectory("../../../db");
        }

        if (!File.Exists(databasePath))
        {
            SQLiteConnection.CreateFile(databasePath);
        }

        // Maak een nieuwe SQLite-verbinding met het opgegeven databasepad
        _connection = new SQLiteConnection($"Data Source={databasePath}");
    }

    // Methode om de verbinding met de database te openen
    public void OpenConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    // Methode om de verbinding met de database te sluiten
    public void CloseConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Closed)
        {
            _connection.Close();
        }
    }

    // Methode om een SQL-query uit te voeren en het resultaat als een DataTable terug te geven
    public DataTable ExecuteQuery(string query)
    {
        SQLiteCommand command = new SQLiteCommand(query, _connection);
        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
        DataTable dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }

    // Voer een SQL-query uit voor tables
        public void CreateTables()
    {
        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Movies (
                MovieId INT PRIMARY KEY     NOT NULL,
                Title           TEXT    NOT NULL,
                Description     TEXT,
                Duration        INT,
                Year            INT,
                Genres          TEXT,
                Cast            TEXT
            );";
        ExecuteQuery(createTableQuery);
    }
}
