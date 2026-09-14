using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Debtorslist1.Models;
using Debtorslist1.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debtorslist1.ViewModels
{
    public partial class CreateDebtorViewModel : ViewModelBase
    {
        private readonly DebtorService _debtorService;

        public CreateDebtorViewModel(DebtorService debtorService)
        {
            _debtorService = debtorService;
        }

        [ObservableProperty] private string _fullName = "";
        [ObservableProperty] private string _currentDebtText = "";
        [ObservableProperty] private string _interestRateText = "";
        [ObservableProperty] private DateTimeOffset? _dateIssued = DateTimeOffset.Now;
        [ObservableProperty] private DateTimeOffset? _dueDate = DateTimeOffset.Now.AddMonths(1);
        [ObservableProperty] private string _notes = "";
        [ObservableProperty] private string _statusMessage = "";

        [RelayCommand]
        private void Save()
        {
            // --- Валидация ---
            if (string.IsNullOrWhiteSpace(FullName))
            {
                StatusMessage = "Укажите ФИО";
                return;
            }

            if (!decimal.TryParse(CurrentDebtText, out var debt) || debt <= 0)
            {
                StatusMessage = "Сумма долга должна быть положительным числом";
                return;
            }

            if (!decimal.TryParse(InterestRateText, out var rate) || rate < 0)
            {
                StatusMessage = "Процентная ставка должна быть неотрицательным числом";
                return;
            }

            if (DateIssued is null || DueDate is null)
            {
                StatusMessage = "Укажите даты";
                return;
            }

            if (DueDate.Value.Date < DateIssued.Value.Date)
            {
                StatusMessage = "Срок возврата не может быть раньше даты выдачи";
                return;
            }

            // --- Создаём должника ---
            var debtor = new Debtor
            {
                FullName = FullName.Trim(),
                CurrentDebt = debt,
                OriginalDebt = debt,
                InterestRate = rate,
                DateIssued = DateIssued.Value.DateTime,
                DueDate = DueDate.Value.DateTime,
                LastInterestDate = DateIssued.Value.DateTime,
                Notes = Notes.Trim()
            };

            // Проверяем флаги (на случай, если сумма уже 0 — не наш случай, но пусть будет)
            _debtorService.RecalculateFlags(debtor);

            // Сохраняем (внутри также создастся первая запись в историю)
            _debtorService.Add(debtor);

            // --- Очищаем форму ---
            FullName = "";
            CurrentDebtText = "";
            InterestRateText = "";
            DateIssued = DateTimeOffset.Now;
            DueDate = DateTimeOffset.Now.AddMonths(1);
            Notes = "";

            StatusMessage = $"Должник сохранён (Id: {debtor.Id.ToString()[..8]}…)";
        }
    }
}
