using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Debtorslist1.Models;
using Debtorslist1.Services;

namespace Debtorslist1.ViewModels;

public partial class DebtorViewViewModel : ViewModelBase
{
    private readonly DebtorService _debtorService;
    private readonly InterestService _interestService;

    public DebtorViewViewModel(DebtorService debtorService, InterestService interestService)
    {
        _debtorService = debtorService;
        _interestService = interestService;
        LoadDebtors();
    }

    /// <summary>Список всех должников для выбора.</summary>
    public ObservableCollection<Debtor> Debtors { get; } = new();

    /// <summary>Выбранный должник (редактируется напрямую).</summary>
    [ObservableProperty]
    private Debtor? _selectedDebtor;

    /// <summary>Сообщение внизу экрана.</summary>
    [ObservableProperty]
    private string _statusMessage = "";

    /// <summary>Сумма долга на момент выбора — чтобы понять, изменилась ли она.</summary>
    private decimal _originalDebt;

    // Генерируется автоматически при изменении SelectedDebtor
    partial void OnSelectedDebtorChanged(Debtor? value)
    {
        if (value != null)
            _originalDebt = value.CurrentDebt;

        OnPropertyChanged(nameof(SelectedDateIssued));
        OnPropertyChanged(nameof(SelectedDueDate));
    }

    private void LoadDebtors()
    {
        Debtors.Clear();
        foreach (var d in _debtorService.GetAll())
            Debtors.Add(d);
    }

    private void ReloadPreservingSelection()
    {
        var id = SelectedDebtor?.Id;
        LoadDebtors();
        if (id.HasValue)
            SelectedDebtor = Debtors.FirstOrDefault(d => d.Id == id);
    }

    [RelayCommand]
    private void Save()
    {
        if (SelectedDebtor is null)
        {
            StatusMessage = "Выберите должника";
            return;
        }

        _debtorService.RecalculateFlags(SelectedDebtor);
        _debtorService.Update(SelectedDebtor);

        if (SelectedDebtor.CurrentDebt != _originalDebt)
        {
            _debtorService.AddHistoryRecord(SelectedDebtor.Id, SelectedDebtor.CurrentDebt);
            _originalDebt = SelectedDebtor.CurrentDebt;
        }

        StatusMessage = "Сохранено";
        ReloadPreservingSelection();
    }

    [RelayCommand]
    private void AccrueInterest()
    {
        if (SelectedDebtor is null)
        {
            StatusMessage = "Выберите должника";
            return;
        }

        var changed = _interestService.Recalculate(SelectedDebtor, DateTime.Now);

        _debtorService.RecalculateFlags(SelectedDebtor);
        _debtorService.Update(SelectedDebtor);

        if (changed)
        {
            _debtorService.AddHistoryRecord(SelectedDebtor.Id, SelectedDebtor.CurrentDebt);
            _originalDebt = SelectedDebtor.CurrentDebt;
        }

        StatusMessage = changed
            ? $"Проценты начислены. Текущий долг: {SelectedDebtor.CurrentDebt:N2} ₽"
            : "Проценты не начислены (нет просрочки)";

        ReloadPreservingSelection();
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedDebtor is null)
        {
            StatusMessage = "Выберите должника";
            return;
        }

        if (!SelectedDebtor.MarkedForDeletion)
        {
            StatusMessage = "Сначала отметьте должника к удалению (галочка в списке)";
            return;
        }

        _debtorService.Delete(SelectedDebtor.Id);
        StatusMessage = "Должник удалён";

        LoadDebtors();
        SelectedDebtor = null;
    }
    public DateTimeOffset? SelectedDateIssued
    {
        get => SelectedDebtor?.DateIssued is DateTime dt ? new DateTimeOffset(dt) : null;
        set
        {
            if (SelectedDebtor != null && value.HasValue)
                SelectedDebtor.DateIssued = value.Value.DateTime;
        }
    }

    public DateTimeOffset? SelectedDueDate
    {
        get => SelectedDebtor?.DueDate is DateTime dt ? new DateTimeOffset(dt) : null;
        set
        {
            if (SelectedDebtor != null && value.HasValue)
                SelectedDebtor.DueDate = value.Value.DateTime;
        }
    }


    [RelayCommand]
    private void Refresh()
    {
        ReloadPreservingSelection();
        StatusMessage = "Список обновлён";
    }
}