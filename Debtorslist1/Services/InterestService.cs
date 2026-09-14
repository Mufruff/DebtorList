using System;
using Debtorslist1.Models;

namespace Debtorslist1.Services;

public class InterestService
{
    /// <summary>
    /// Начисляет проценты на CurrentDebt за период просрочки.
    /// Возвращает true, если долг изменился.
    /// </summary>
    public bool Recalculate(Debtor debtor, DateTime today)
    {
        if (debtor.IsPaid) return false;
        if (debtor.CurrentDebt <= 0) return false;
        if (today <= debtor.DueDate) return false;

        // С какого момента считаем: от последнего начисления или от DueDate
        var startDate = debtor.LastInterestDate > debtor.DueDate
            ? debtor.LastInterestDate
            : debtor.DueDate;

        var days = (today - startDate).Days;
        if (days <= 0) return false;

        var interest = debtor.CurrentDebt * (debtor.InterestRate / 100m) * (days / 30m);
        interest = Math.Round(interest, 2);

        if (interest <= 0) return false;

        debtor.CurrentDebt += interest;
        debtor.LastInterestDate = today;
        return true;
    }
}