using System;
using System.IO;
using System.Text.Json;
using Debtorslist1.Models;
using MySqlConnector;

namespace Debtorslist1.Services;

public class DbService
{
    public string ConnectionString { get; }

    public DbService()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(configPath))
            throw new FileNotFoundException(
                $"Не найден файл конфигурации: {configPath}. " +
                "Убедись, что appsettings.json копируется в выходной каталог.");

        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<AppConfig>(json)
                     ?? throw new InvalidOperationException("Не удалось прочитать appsettings.json");

        ConnectionString = config.ConnectionStrings.MariaDB;
    }

    /// <summary>Создаёт новое подключение к базе (уже открытое).</summary>
    public MySqlConnection CreateOpenConnection()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}