using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Debtorslist1.Services;
using Debtorslist1.ViewModels;
using Debtorslist1.Views;
using System;

namespace Debtorslist1;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 1. Создаём контейнер сервисов
        var services = new ServiceCollection();

        // 2. Сервисы — один экземпляр на всё приложение
        services.AddSingleton<DbService>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<DebtorService>();
        services.AddSingleton<InterestService>();

        // 3. ViewModel — новый экземпляр при каждом запросе
        services.AddTransient<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<CreateDebtorViewModel>();
        services.AddTransient<DebtorViewViewModel>();
        services.AddTransient<HistoryViewModel>();

        // 4. Собираем контейнер
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Стартовое окно — логин
            var loginVm = Services.GetRequiredService<LoginViewModel>();
            desktop.MainWindow = new LoginWindow(loginVm);
        }

        base.OnFrameworkInitializationCompleted();
    }
}