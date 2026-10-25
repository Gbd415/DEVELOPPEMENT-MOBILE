using SQLite;
using EduCam.Models;

namespace EduCam.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "educam.db3");
    }

    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<QuizEntity>();
        await _database.CreateTableAsync<QuestionEntity>();
        await _database.CreateTableAsync<QuizResultEntity>();
    }

    // ===== QUIZZES =====
    public async Task<List<Quiz>> GetAllQuizzesAsync()
    {
        await InitAsync();
        var entities = await _database!.Table<QuizEntity>().ToListAsync();
        var quizzes = new List<Quiz>();

        foreach (var e in entities)
        {
            var questions = await _database.Table<QuestionEntity>()
                .Where(q => q.QuizId == e.Id)
                .ToListAsync();

            quizzes.Add(new Quiz
            {
                Id = e.Id,
                Title = e.Title,
                Subject = e.Subject,
                Description = e.Description,
                CreatedAt = e.CreatedAt,
                IsDefault = e.IsDefault,
                Questions = questions.Select(q => new Question
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.OptionsJson.Split('|').ToList(),
                    CorrectIndex = q.CorrectIndex,
                    Subject = e.Subject
                }).ToList()
            });
        }
        return quizzes;
    }

    public async Task<Quiz?> GetQuizByIdAsync(int id)
    {
        var all = await GetAllQuizzesAsync();
        return all.FirstOrDefault(q => q.Id == id);
    }

    public async Task<int> SaveQuizAsync(Quiz quiz)
    {
        await InitAsync();

        var entity = new QuizEntity
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Subject = quiz.Subject,
            Description = quiz.Description,
            CreatedAt = quiz.CreatedAt,
            IsDefault = quiz.IsDefault
        };

        if (quiz.Id == 0)
        {
            await _database!.InsertAsync(entity);
            quiz.Id = entity.Id;
        }
        else
        {
            await _database!.UpdateAsync(entity);
            // Delete old questions
            await _database.Table<QuestionEntity>()
                .DeleteAsync(q => q.QuizId == quiz.Id);
        }

        // Insert questions
        foreach (var q in quiz.Questions)
        {
            await _database.InsertAsync(new QuestionEntity
            {
                QuizId = quiz.Id,
                Text = q.Text,
                OptionsJson = string.Join("|", q.Options),
                CorrectIndex = q.CorrectIndex
            });
        }

        return quiz.Id;
    }

    public async Task DeleteQuizAsync(int id)
    {
        await InitAsync();
        await _database!.Table<QuestionEntity>().DeleteAsync(q => q.QuizId == id);
        await _database.DeleteAsync<QuizEntity>(id);
    }

    // ===== RESULTS =====
    public async Task SaveResultAsync(QuizResult result)
    {
        await InitAsync();
        await _database!.InsertAsync(new QuizResultEntity
        {
            QuizTitle = result.QuizTitle,
            Subject = result.Subject,
            Score = result.Score,
            TotalQuestions = result.TotalQuestions,
            CompletedAt = result.CompletedAt,
            StudentName = result.StudentName
        });
    }

    public async Task<List<QuizResult>> GetAllResultsAsync()
    {
        await InitAsync();
        var entities = await _database!.Table<QuizResultEntity>()
            .OrderByDescending(r => r.CompletedAt)
            .ToListAsync();

        return entities.Select(e => new QuizResult
        {
            Id = e.Id,
            QuizTitle = e.QuizTitle,
            Subject = e.Subject,
            Score = e.Score,
            TotalQuestions = e.TotalQuestions,
            CompletedAt = e.CompletedAt,
            StudentName = e.StudentName
        }).ToList();
    }

    public async Task ClearResultsAsync()
    {
        await InitAsync();
        await _database!.DeleteAllAsync<QuizResultEntity>();
    }

    // ===== SEED DATA =====
    public async Task SeedDefaultQuizzesAsync()
    {
        await InitAsync();
        var count = await _database!.Table<QuizEntity>().CountAsync();
        if (count > 0) return;

        // Maths
        await SaveQuizAsync(new Quiz
        {
            Title = "Mathématiques - Niveau 1",
            Subject = "Maths",
            Description = "Opérations de base et calcul mental",
            IsDefault = true,
            Questions = new List<Question>
            {
                new() { Text = "Combien font 12 + 8 ?", Options = new() { "18", "20", "22", "19" }, CorrectIndex = 1 },
                new() { Text = "Quelle est la moitié de 36 ?", Options = new() { "16", "18", "20", "12" }, CorrectIndex = 1 },
                new() { Text = "7 × 6 = ?", Options = new() { "40", "42", "48", "36" }, CorrectIndex = 1 },
                new() { Text = "Combien font 100 - 37 ?", Options = new() { "63", "73", "67", "53" }, CorrectIndex = 0 },
                new() { Text = "Quel est le carré de 5 ?", Options = new() { "10", "15", "25", "20" }, CorrectIndex = 2 }
            }
        });

        // Français
        await SaveQuizAsync(new Quiz
        {
            Title = "Français - Grammaire",
            Subject = "Français",
            Description = "Accord, conjugaison et orthographe",
            IsDefault = true,
            Questions = new List<Question>
            {
                new() { Text = "Quelle est la bonne orthographe ?", Options = new() { "Les enfants joue", "Les enfants jouent", "Les enfants joues", "Les enfant jouent" }, CorrectIndex = 1 },
                new() { Text = "Conjugue le verbe 'manger' à la 1ère personne du pluriel au présent :", Options = new() { "Je mange", "Nous mangeons", "Ils mangent", "Tu manges" }, CorrectIndex = 1 },
                new() { Text = "Quel est le féminin de 'acteur' ?", Options = new() { "Acteuse", "Actrice", "Acteure", "Acteuse" }, CorrectIndex = 1 },
                new() { Text = "Choisis la bonne phrase :", Options = new() { "Il a mangé une pomme.", "Il a manger une pomme.", "Il a mangée une pomme.", "Il a mangés une pomme." }, CorrectIndex = 0 },
                new() { Text = "Quel est le pluriel de 'cheval' ?", Options = new() { "Chevals", "Chevaux", "Chevales", "Cheveaux" }, CorrectIndex = 1 }
            }
        });

        // Histoire
        await SaveQuizAsync(new Quiz
        {
            Title = "Histoire - Afrique",
            Subject = "Histoire",
            Description = "Quelques repères historiques africains",
            IsDefault = true,
            Questions = new List<Question>
            {
                new() { Text = "Qui était le fondateur de l'empire du Mali ?", Options = new() { "Soundiata Keïta", "Mansa Moussa", "Samory Touré", "Askia Mohamed" }, CorrectIndex = 0 },
                new() { Text = "En quelle année le Cameroun a-t-il obtenu son indépendance ?", Options = new() { "1958", "1960", "1962", "1972" }, CorrectIndex = 1 },
                new() { Text = "Quel grand empire africain était célèbre pour sa richesse en or ?", Options = new() { "Empire Songhaï", "Empire du Ghana", "Empire du Mali", "Royaume du Kongo" }, CorrectIndex = 2 },
                new() { Text = "Qui était Nelson Mandela ?", Options = new() { "Un roi zoulou", "Un président sud-africain et symbole de la lutte anti-apartheid", "Un explorateur", "Un musicien" }, CorrectIndex = 1 },
                new() { Text = "La ville de Tombouctou était célèbre pour :", Options = new() { "Ses mines de diamant", "Ses universités et son savoir", "Ses ports maritimes", "Ses pyramides" }, CorrectIndex = 1 }
            }
        });
    }
}

// Entities for SQLite
public class QuizEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDefault { get; set; }
}

public class QuestionEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = string.Empty;
    public int CorrectIndex { get; set; }
}

public class QuizResultEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime CompletedAt { get; set; }
    public string StudentName { get; set; } = string.Empty;
}
