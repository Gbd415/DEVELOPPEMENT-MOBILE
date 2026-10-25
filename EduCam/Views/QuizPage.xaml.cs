using EduCam.ViewModels;

namespace EduCam.Views;

public partial class QuizPage : ContentPage
{
    public QuizPage(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
