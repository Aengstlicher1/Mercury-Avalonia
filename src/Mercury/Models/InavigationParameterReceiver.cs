namespace Mercury.Models;

public interface INavigationParameterReceiver<in TParam> where TParam : INavigationParameter
{
    void OnNavigatedTo(TParam parameter);
}