namespace EduCam.Models;

public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Question> Questions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsDefault { get; set; } = false;
}
