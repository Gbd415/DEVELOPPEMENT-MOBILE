using EduCam.Views;

namespace EduCam;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));
        Routing.RegisterRoute(nameof(TeacherPage), typeof(TeacherPage));
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
    }
}
