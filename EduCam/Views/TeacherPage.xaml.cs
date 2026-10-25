using EduCam.ViewModels;

namespace EduCam.Views;

public partial class TeacherPage : ContentPage
{
    private readonly TeacherViewModel _vm;

    public TeacherPage(TeacherViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
