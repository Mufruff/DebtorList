using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Debtorslist1.Models;

public partial class Debtor : ObservableObject
{
    [ObservableProperty] private Guid _id = Guid.NewGuid();
    [ObservableProperty] private string _fullName = "";
    [ObservableProperty] private decimal _currentDebt;
    [ObservableProperty] private decimal _originalDebt;
    [ObservableProperty] private decimal _interestRate;
    [ObservableProperty] private DateTime _dateIssued = DateTime.Now;
    [ObservableProperty] private DateTime _dueDate = DateTime.Now.AddMonths(1);
    [ObservableProperty] private DateTime _lastInterestDate = DateTime.Now;
    [ObservableProperty] private bool _isPaid;
    [ObservableProperty] private bool _markedForDeletion;
    [ObservableProperty] private string _notes = "";
}