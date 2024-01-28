using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ItsMyDoliprane.Repository;

public abstract class AbstractRepository
{
    private readonly string _connectionString;
    private static readonly List<string> InitializedDatabase = new();
    private static readonly object InitializeLock = new();

    protected AbstractRepository(IConfiguration configuration) {
        _connectionString = GetConnectionString(configuration);
        Initialize();
    }

    private static string GetConnectionString(IConfiguration configuration) {
        return configuration["Repository:ConnectionString"];
    }

    private void Initialize() {
        if (CanUpdateDatabase())
            lock (InitializeLock) {
                if (CanUpdateDatabase()) {
                    IEnumerable<string> scripts = GetAllScriptsForUpdate();
                    UpdateDatabase(scripts);
                }
            }
    }

    private bool CanUpdateDatabase() {
        return !InitializedDatabase.Contains(_connectionString);
    }

    private IEnumerable<string> GetAllScriptsForUpdate() {
        int nextVersionDatabase = GetVersionDatabase() + 1;
        Assembly assembly = Assembly.GetExecutingAssembly();
        string[] allResourceNames = assembly.GetManifestResourceNames();
        IEnumerable<string> resourceNames = allResourceNames.Where(r => GetVersionScriptByResourceName(r) >= nextVersionDatabase);
        return resourceNames.OrderBy(r => r).Select(GetScriptByResourceName);
    }

    private int GetVersionDatabase() {
        using SqliteConnection sqliteConnection = CreateConnectionAndOpen();
        using SqliteCommand command = sqliteConnection.CreateCommand();
        command.CommandText = "PRAGMA user_version";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static int GetVersionScriptByResourceName(string resourceName) {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string assemblyName = assembly.FullName?[..assembly.FullName.IndexOf(',')] ?? "";
        int startIndex = assemblyName.Length + ".Scripts.".Length;
        string scriptName = resourceName[startIndex..];
        string version = scriptName[..scriptName.IndexOf('.')];
        return Convert.ToInt32(version);
    }

    private static string GetScriptByResourceName(string resourceName) {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException();
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private void UpdateDatabase(IEnumerable<string> scripts) {
        using SqliteConnection sqliteConnection = CreateConnectionAndOpenWithForeignKeys();
        using SqliteCommand command = sqliteConnection.CreateCommand();
        foreach (string script in scripts) {
            using SqliteTransaction sqliteTransaction = sqliteConnection.BeginTransaction();
            try {
                command.Transaction = sqliteTransaction;
                command.CommandText = script;
                command.ExecuteNonQuery();
                sqliteTransaction.Commit();
            }
            catch (Exception ex) {
                sqliteTransaction.Rollback();
                throw new Exception($"Update Database : {ex.Message}");
            }
        }
        DatabaseIsInitialized();
    }

    private void DatabaseIsInitialized() {
        InitializedDatabase.Add(_connectionString);
    }

    protected SqliteConnection CreateConnectionAndOpen() {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    protected SqliteConnection CreateConnectionAndOpenWithForeignKeys() {
        SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON";
        command.ExecuteNonQuery();
        return connection;
    }
}
