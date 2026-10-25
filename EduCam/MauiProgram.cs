using Microsoft.Extensions.Logging;
using EduCam.Services;
using EduCam.ViewModels;
using EduCam.Views;
using EduCam.Converters;

namespace EduCam;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<DatabaseService>();

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<QuizViewModel>();
        builder.Services.AddTransient<TeacherViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();

        // Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<QuizPage>();
        builder.Services.AddTransient<TeacherPage>();
        builder.Services.AddTransient<HistoryPage>();

        return builder.Build();
    }
}
