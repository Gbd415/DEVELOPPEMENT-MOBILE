namespace EduCam.Models;

public class QuizResult
{
    public int Id { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage => TotalQuestions == 0 ? 0 : (double)Score / TotalQuestions * 100;
    public DateTime CompletedAt { get; set; } = DateTime.Now;
    public string StudentName { get; set; } = "Élève";
}
