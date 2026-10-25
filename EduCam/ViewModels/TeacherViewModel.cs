using System.Collections.ObjectModel;
using System.Windows.Input;
using EduCam.Models;
using EduCam.Services;

namespace EduCam.ViewModels;

public class TeacherViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Quiz> Quizzes { get; } = new();

    private string _newTitle = string.Empty;
    public string NewTitle
    {
        get => _newTitle;
        set => SetProperty(ref _newTitle, value);
    }

    private string _newSubject = "Maths";
    public string NewSubject
    {
        get => _newSubject;
        set => SetProperty(ref _newSubject, value);
    }

    private string _newDescription = string.Empty;
    public string NewDescription
    {
        get => _newDescription;
        set => SetProperty(ref _newDescription, value);
    }

    // Temporary questions while creating
    public ObservableCollection<Question> NewQuestions { get; } = new();

    private string _qText = string.Empty;
    public string QText
    {
        get => _qText;
        set => SetProperty(ref _qText, value);
    }

    private string _opt1 = string.Empty, _opt2 = string.Empty, _opt3 = string.Empty, _opt4 = string.Empty;
    public string Opt1 { get => _opt1; set => SetProperty(ref _opt1, value); }
    public string Opt2 { get => _opt2; set => SetProperty(ref _opt2, value); }
    public string Opt3 { get => _opt3; set => SetProperty(ref _opt3, value); }
    public string Opt4 { get => _opt4; set => SetProperty(ref _opt4, value); }

    private int _correctIndex = 0;
    public int CorrectIndex
    {
        get => _correctIndex;
        set => SetProperty(ref _correctIndex, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddQuestionCommand { get; }
    public ICommand SaveQuizCommand { get; }
    public ICommand DeleteQuizCommand { get; }
    public ICommand ClearFormCommand { get; }

    public TeacherViewModel(DatabaseService db)
    {
        _db = db;
        Title = "Espace Enseignant";
        LoadCommand = new Command(async () => await LoadAsync());
        AddQuestionCommand = new Command(OnAddQuestion);
        SaveQuizCommand = new Command(async () => await OnSaveQuizAsync());
        DeleteQuizCommand = new Command<Quiz>(async (q) => await OnDeleteAsync(q));
        ClearFormCommand = new Command(ClearForm);
    }

    public async Task LoadAsync()
    {
        var list = await _db.GetAllQuizzesAsync();
        Quizzes.Clear();
        foreach (var q in list)
            Quizzes.Add(q);
    }

    private void OnAddQuestion()
    {
        if (string.IsNullOrWhiteSpace(QText) || string.IsNullOrWhiteSpace(Opt1) || string.IsNullOrWhiteSpace(Opt2))
        {
            Application.Current?.MainPage?.DisplayAlert("Attention", "Remplis au moins la question et 2 options.", "OK");
            return;
        }

        var options = new List<string> { Opt1, Opt2 };
        if (!string.IsNullOrWhiteSpace(Opt3)) options.Add(Opt3);
        if (!string.IsNullOrWhiteSpace(Opt4)) options.Add(Opt4);

        if (CorrectIndex < 0 || CorrectIndex >= options.Count)
            CorrectIndex = 0;

        NewQuestions.Add(new Question
        {
            Text = QText.Trim(),
            Options = options,
            CorrectIndex = CorrectIndex,
            Subject = NewSubject
        });

        // Reset question form
        QText = Opt1 = Opt2 = Opt3 = Opt4 = string.Empty;
        CorrectIndex = 0;
    }

    private async Task OnSaveQuizAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle) || NewQuestions.Count == 0)
        {
            await Application.Current!.MainPage!.DisplayAlert("Attention", "Titre + au moins une question requis.", "OK");
            return;
        }

        var quiz = new Quiz
        {
            Title = NewTitle.Trim(),
            Subject = NewSubject,
            Description = NewDescription.Trim(),
            Questions = NewQuestions.ToList(),
            CreatedAt = DateTime.Now,
            IsDefault = false
        };

        await _db.SaveQuizAsync(quiz);
        ClearForm();
        await LoadAsync();
        await Application.Current!.MainPage!.DisplayAlert("Succès", "Questionnaire enregistré !", "OK");
    }

    private async Task OnDeleteAsync(Quiz quiz)
    {
        if (quiz.IsDefault)
        {
            await Application.Current!.MainPage!.DisplayAlert("Impossible", "Les questionnaires par défaut ne peuvent pas être supprimés.", "OK");
            return;
        }

        bool confirm = await Application.Current!.MainPage!.DisplayAlert("Confirmation",
            $"Supprimer « {quiz.Title} » ?", "Oui", "Non");
        if (!confirm) return;

        await _db.DeleteQuizAsync(quiz.Id);
        await LoadAsync();
    }

    private void ClearForm()
    {
        NewTitle = NewDescription = string.Empty;
        NewSubject = "Maths";
        NewQuestions.Clear();
        QText = Opt1 = Opt2 = Opt3 = Opt4 = string.Empty;
        CorrectIndex = 0;
    }
}
