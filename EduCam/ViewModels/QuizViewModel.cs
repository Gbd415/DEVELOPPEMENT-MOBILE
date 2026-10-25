using System.Windows.Input;
using EduCam.Models;
using EduCam.Services;

namespace EduCam.ViewModels;

[QueryProperty(nameof(Quiz), "Quiz")]
public class QuizViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private Quiz? _quiz;
    private int _currentIndex;
    private int _score;
    private bool _isFinished;
    private string? _selectedOption;
    private bool _hasAnswered;

    public Quiz? Quiz
    {
        get => _quiz;
        set
        {
            SetProperty(ref _quiz, value);
            if (value != null)
            {
                Title = value.Title;
                CurrentIndex = 0;
                Score = 0;
                IsFinished = false;
                HasAnswered = false;
                SelectedOption = null;
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(ProgressText));
                OnPropertyChanged(nameof(Progress));
            }
        }
    }

    public Question? CurrentQuestion => Quiz?.Questions.ElementAtOrDefault(CurrentIndex);

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            SetProperty(ref _currentIndex, value);
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(IsLastQuestion));
        }
    }

    public int Score
    {
        get => _score;
        set => SetProperty(ref _score, value);
    }

    public bool IsFinished
    {
        get => _isFinished;
        set => SetProperty(ref _isFinished, value);
    }

    public string? SelectedOption
    {
        get => _selectedOption;
        set => SetProperty(ref _selectedOption, value);
    }

    public bool HasAnswered
    {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }

    public string ProgressText => Quiz is null ? "" : $"Question {CurrentIndex + 1} / {Quiz.Questions.Count}";
    public double Progress => Quiz is null || Quiz.Questions.Count == 0 ? 0 : (double)(CurrentIndex + 1) / Quiz.Questions.Count;
    public bool IsLastQuestion => Quiz is not null && CurrentIndex >= Quiz.Questions.Count - 1;

    public string FinalMessage
    {
        get
        {
            if (Quiz is null) return "";
            var pct = (double)Score / Quiz.Questions.Count * 100;
            if (pct >= 80) return "Excellent travail ! 🎉";
            if (pct >= 60) return "Bien joué ! Continue comme ça.";
            if (pct >= 40) return "Pas mal, tu peux encore progresser.";
            return "Courage ! Révise et réessaie.";
        }
    }

    public ICommand SelectOptionCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand FinishCommand { get; }
    public ICommand RestartCommand { get; }
    public ICommand GoHomeCommand { get; }

    public QuizViewModel(DatabaseService db)
    {
        _db = db;
        SelectOptionCommand = new Command<string>(OnSelectOption);
        NextCommand = new Command(OnNext);
        FinishCommand = new Command(async () => await OnFinishAsync());
        RestartCommand = new Command(OnRestart);
        GoHomeCommand = new Command(async () => await Shell.Current.GoToAsync("//HomePage"));
    }

    private void OnSelectOption(string option)
    {
        if (HasAnswered || CurrentQuestion is null) return;
        SelectedOption = option;
        HasAnswered = true;

        var correct = CurrentQuestion.Options[CurrentQuestion.CorrectIndex];
        if (option == correct)
            Score++;
    }

    private void OnNext()
    {
        if (!HasAnswered) return;
        if (IsLastQuestion)
        {
            IsFinished = true;
            OnPropertyChanged(nameof(FinalMessage));
            return;
        }
        CurrentIndex++;
        SelectedOption = null;
        HasAnswered = false;
    }

    private async Task OnFinishAsync()
    {
        if (Quiz is null) return;

        var result = new QuizResult
        {
            QuizTitle = Quiz.Title,
            Subject = Quiz.Subject,
            Score = Score,
            TotalQuestions = Quiz.Questions.Count,
            CompletedAt = DateTime.Now,
            StudentName = "Élève"
        };
        await _db.SaveResultAsync(result);
        IsFinished = true;
        OnPropertyChanged(nameof(FinalMessage));
    }

    private void OnRestart()
    {
        CurrentIndex = 0;
        Score = 0;
        IsFinished = false;
        HasAnswered = false;
        SelectedOption = null;
        OnPropertyChanged(nameof(CurrentQuestion));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(Progress));
    }
}
