namespace Debtorslist1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public CreateDebtorViewModel CreateDebtorVm { get; }
    public DebtorViewViewModel DebtorViewVm { get; }
    public HistoryViewModel HistoryVm { get; }

    public MainViewModel(CreateDebtorViewModel createDebtorVm,
                         DebtorViewViewModel debtorViewVm,
                         HistoryViewModel historyVm)
    {
        CreateDebtorVm = createDebtorVm;
        DebtorViewVm = debtorViewVm;
        HistoryVm = historyVm;
    }
}