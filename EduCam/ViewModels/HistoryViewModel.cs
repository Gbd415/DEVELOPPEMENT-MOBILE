using System.Collections.ObjectModel;
using System.Windows.Input;
using EduCam.Models;
using EduCam.Services;

namespace EduCam.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<QuizResult> Results { get; } = new();

    public ICommand LoadCommand { get; }
    public ICommand ClearCommand { get; }

    public HistoryViewModel(DatabaseService db)
    {
        _db = db;
        Title = "Historique des résultats";
        LoadCommand = new Command(async () => await LoadAsync());
        ClearCommand = new Command(async () => await ClearAsync());
    }

    public async Task LoadAsync()
    {
        var list = await _db.GetAllResultsAsync();
        Results.Clear();
        foreach (var r in list)
            Results.Add(r);
    }

    private async Task ClearAsync()
    {
        bool confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Confirmation", "Effacer tout l'historique ?", "Oui", "Non");
        if (!confirm) return;

        await _db.ClearResultsAsync();
        Results.Clear();
    }
}
