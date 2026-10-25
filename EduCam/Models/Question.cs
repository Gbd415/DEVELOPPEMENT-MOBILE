namespace EduCam.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectIndex { get; set; }
    public string Subject { get; set; } = string.Empty; // Maths, Français, Histoire
}
