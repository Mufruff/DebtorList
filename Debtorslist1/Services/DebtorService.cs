using System;
using System.Collections.Generic;
using Debtorslist1.Models;
using MySqlConnector;

namespace Debtorslist1.Services;

public class DebtorService
{
    private readonly DbService _db;

    public DebtorService(DbService db)
    {
        _db = db;
    }

    // ---------- ДОЛЖНИКИ ----------

    public List<Debtor> GetAll()
    {
        var result = new List<Debtor>();

        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand(
            @"SELECT id, full_name, original_debt, current_debt, interest_rate,
       date_issued, due_date, last_interest_date,
       is_paid, marked_for_deletion, notes
FROM debtors", conn);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(ReadDebtor(reader));
        }

        return result;
    }

    public Debtor? GetById(Guid id)
    {
        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand(
            @"SELECT id, full_name, original_debt, current_debt, interest_rate,
       date_issued, due_date, last_interest_date,
       is_paid, marked_for_deletion, notes
FROM debtors WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id.ToString());

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReadDebtor(reader) : null;
    }

    public void Add(Debtor debtor)
    {
        using (var conn = _db.CreateOpenConnection())
        using (var cmd = new MySqlCommand(
            @"INSERT INTO debtors
              (id, full_name, current_debt, original_debt, interest_rate,
               date_issued, due_date, last_interest_date,
               is_paid, marked_for_deletion, notes)
              VALUES
              (@id, @full_name, @current_debt, @original_debt, @interest_rate,
               @date_issued, @due_date, @last_interest_date,
               @is_paid, @marked_for_deletion, @notes)", conn))
        {
            cmd.Parameters.AddWithValue("@id", debtor.Id.ToString());
            cmd.Parameters.AddWithValue("@full_name", debtor.FullName);
            cmd.Parameters.AddWithValue("@current_debt", debtor.CurrentDebt);
            cmd.Parameters.AddWithValue("@original_debt", debtor.OriginalDebt);
            cmd.Parameters.AddWithValue("@interest_rate", debtor.InterestRate);
            cmd.Parameters.AddWithValue("@date_issued", debtor.DateIssued);
            cmd.Parameters.AddWithValue("@due_date", debtor.DueDate);
            cmd.Parameters.AddWithValue("@last_interest_date", debtor.LastInterestDate);
            cmd.Parameters.AddWithValue("@is_paid", debtor.IsPaid ? 1 : 0);
            cmd.Parameters.AddWithValue("@marked_for_deletion", debtor.MarkedForDeletion ? 1 : 0);
            cmd.Parameters.AddWithValue("@notes", debtor.Notes);

            cmd.ExecuteNonQuery();
        }

        AddHistoryRecord(debtor.Id, debtor.CurrentDebt);
    }

    public void Update(Debtor debtor)
    {
        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand(
            @"UPDATE debtors SET
                full_name = @full_name,
                current_debt = @current_debt,
                original_debt = @original_debt,
                interest_rate = @interest_rate,
                date_issued = @date_issued,
                due_date = @due_date,
                last_interest_date = @last_interest_date,
                is_paid = @is_paid,
                marked_for_deletion = @marked_for_deletion,
                notes = @notes
              WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("@id", debtor.Id.ToString());
        cmd.Parameters.AddWithValue("@full_name", debtor.FullName);
        cmd.Parameters.AddWithValue("@current_debt", debtor.CurrentDebt);
        cmd.Parameters.AddWithValue("@original_debt", debtor.OriginalDebt);
        cmd.Parameters.AddWithValue("@interest_rate", debtor.InterestRate);
        cmd.Parameters.AddWithValue("@date_issued", debtor.DateIssued);
        cmd.Parameters.AddWithValue("@due_date", debtor.DueDate);
        cmd.Parameters.AddWithValue("@last_interest_date", debtor.LastInterestDate);
        cmd.Parameters.AddWithValue("@is_paid", debtor.IsPaid ? 1 : 0);
        cmd.Parameters.AddWithValue("@marked_for_deletion", debtor.MarkedForDeletion ? 1 : 0);
        cmd.Parameters.AddWithValue("@notes", debtor.Notes);

        cmd.ExecuteNonQuery();
    }

    public void Delete(Guid id)
    {
        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand("DELETE FROM debtors WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id.ToString());
        cmd.ExecuteNonQuery();
    }

    // ---------- ИСТОРИЯ ----------

    public List<HistoryRecord> GetHistory(Guid debtorId)
    {
        var result = new List<HistoryRecord>();

        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand(
            @"SELECT id, debtor_id, amount, change_date
              FROM history
              WHERE debtor_id = @debtor_id
              ORDER BY change_date ASC", conn);
        cmd.Parameters.AddWithValue("@debtor_id", debtorId.ToString());

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new HistoryRecord
            {
                Id = reader.GetGuid("id"),
                DebtorId = reader.GetGuid("debtor_id"),
                Amount = reader.GetDecimal("amount"),
                ChangeDate = reader.GetDateTime("change_date")
            });
        }

        return result;
    }

    public void AddHistoryRecord(Guid debtorId, decimal amount)
    {
        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand(
            @"INSERT INTO history (id, debtor_id, amount, change_date)
              VALUES (@id, @debtor_id, @amount, @change_date)", conn);

        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@debtor_id", debtorId.ToString());
        cmd.Parameters.AddWithValue("@amount", amount);
        cmd.Parameters.AddWithValue("@change_date", DateTime.Now);

        cmd.ExecuteNonQuery();
    }
    /// <summary>Удаляет всю историю указанного должника.</summary>
    public void ClearHistory(Guid debtorId)
    {
        using var conn = _db.CreateOpenConnection();
        using var cmd = new MySqlCommand(
            "DELETE FROM history WHERE debtor_id = @debtor_id", conn);
        cmd.Parameters.AddWithValue("@debtor_id", debtorId.ToString());
        cmd.ExecuteNonQuery();
    }

    // ---------- ЛОГИКА ФЛАГОВ ----------

    public void RecalculateFlags(Debtor debtor)
    {
        debtor.IsPaid = debtor.CurrentDebt <= 0;

        if (debtor.IsPaid)
            debtor.MarkedForDeletion = true;
    }

    // ---------- ВСПОМОГАТЕЛЬНЫЙ МЕТОД ----------

    private static Debtor ReadDebtor(MySqlDataReader reader)
    {
        return new Debtor
        {
            Id = reader.GetGuid("id"),
            FullName = reader.GetString("full_name"),
            CurrentDebt = reader.GetDecimal("current_debt"),
            OriginalDebt = reader.GetDecimal("original_debt"),
            InterestRate = reader.GetDecimal("interest_rate"),
            DateIssued = reader.GetDateTime("date_issued"),
            DueDate = reader.GetDateTime("due_date"),
            LastInterestDate = reader.GetDateTime("last_interest_date"),
            IsPaid = reader.GetBoolean("is_paid"),
            MarkedForDeletion = reader.GetBoolean("marked_for_deletion"),
            Notes = reader.IsDBNull(reader.GetOrdinal("notes"))
                    ? ""
                    : reader.GetString("notes")
        };
    }
}