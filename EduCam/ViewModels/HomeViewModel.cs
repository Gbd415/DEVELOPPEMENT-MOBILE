using System.Collections.ObjectModel;
using System.Windows.Input;
using EduCam.Models;
using EduCam.Services;
using EduCam.Views;

namespace EduCam.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Quiz> Quizzes { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand OpenQuizCommand { get; }
    public ICommand OpenTeacherCommand { get; }
    public ICommand OpenHistoryCommand { get; }

    public HomeViewModel(DatabaseService db)
    {
        _db = db;
        Title = "EduCam";
        RefreshCommand = new Command(async () => await LoadQuizzesAsync());
        OpenQuizCommand = new Command<Quiz>(async (q) => await OpenQuizAsync(q));
        OpenTeacherCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(TeacherPage)));
        OpenHistoryCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(HistoryPage)));
    }

    public async Task LoadQuizzesAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            await _db.SeedDefaultQuizzesAsync();
            var list = await _db.GetAllQuizzesAsync();
            Quizzes.Clear();
            foreach (var q in list)
                Quizzes.Add(q);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OpenQuizAsync(Quiz quiz)
    {
        if (quiz is null) return;
        var navigationParameter = new Dictionary<string, object>
        {
            { "Quiz", quiz }
        };
        await Shell.Current.GoToAsync(nameof(QuizPage), navigationParameter);
    }
}
