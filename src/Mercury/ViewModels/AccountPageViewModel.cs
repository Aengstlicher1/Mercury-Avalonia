namespace Mercury.ViewModels;

public class AccountPageViewModel : ViewModelBase
{
    public string LoginUrl { get; } =
        "https://accounts.google.com/ServiceLogin?service=youtube&continue=https%3A%2F%2Fmusic.youtube.com%2F";
}