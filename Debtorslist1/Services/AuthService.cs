using Debtorslist1.Models;
using MySqlConnector;

namespace Debtorslist1.Services;

public class AuthService
{
    private const string DefaultPassword = "1234";
    private readonly DbService _db;

    public AuthService(DbService db)
    {
        _db = db;
        EnsureConfigRow();
    }

    /// <summary>Проверяет, совпадает ли введённый пароль с хранимым в БД.</summary>
    public bool CheckPassword(string input)
    {
        return GetPassword() == input;
    }

    /// <summary>Возвращает текущий пароль из таблицы config.</summary>
    public string GetPassword()
    {
        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand("SELECT password FROM config WHERE id = 1", conn);
        var result = cmd.ExecuteScalar();
        return result?.ToString() ?? DefaultPassword;
    }

    /// <summary>
    /// Если в таблице config нет строки с id=1 — создаёт её с паролем по умолчанию.
    /// </summary>
    private void EnsureConfigRow()
    {
        using var conn = _db.CreateOpenConnection();

        using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM config WHERE id = 1", conn))
        {
            var count = System.Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0) return;
        }

        using var insertCmd = new MySqlCommand(
            "INSERT INTO config (id, password) VALUES (1, @pwd)", conn);
        insertCmd.Parameters.AddWithValue("@pwd", DefaultPassword);
        insertCmd.ExecuteNonQuery();
    }
}