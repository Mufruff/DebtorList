using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Debtorslist1.Models;
using Debtorslist1.Services;

namespace Debtorslist1.ViewModels;

public partial class HistoryViewModel : ViewModelBase
{
    private readonly DebtorService _debtorService;

    public HistoryViewModel(DebtorService debtorService)
    {
        _debtorService = debtorService;
        LoadDebtors();
    }

    public ObservableCollection<Debtor> Debtors { get; } = new();
    public ObservableCollection<HistoryRecord> History { get; } = new();

    [ObservableProperty] private Debtor? _selectedDebtor;
    [ObservableProperty] private string _statusMessage = "";

    partial void OnSelectedDebtorChanged(Debtor? value)
    {
        LoadHistory();
    }

    private void LoadDebtors()
    {
        Debtors.Clear();
        foreach (var d in _debtorService.GetAll())
            Debtors.Add(d);
    }

    private void LoadHistory()
    {
        History.Clear();
        if (SelectedDebtor is null) return;
        foreach (var h in _debtorService.GetHistory(SelectedDebtor.Id))
            History.Add(h);
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadDebtors();
        LoadHistory();
        StatusMessage = "Обновлено";
    }

    [RelayCommand]
    private void ClearHistory()
    {
        if (SelectedDebtor is null)
        {
            StatusMessage = "Выберите должника";
            return;
        }

        _debtorService.ClearHistory(SelectedDebtor.Id);
        History.Clear();
        StatusMessage = "История очищена";
    }
}