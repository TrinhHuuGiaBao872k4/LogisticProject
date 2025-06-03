using System.Net.Http;
using System.Net.Http.Json;

public class ProductStateService(HttpClient http)
{
    private string _message = "Hello";
    public string Message => _message;

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void SetMessage(string message)
    {
        _message = message;
        NotifyStateChanged();
    }

    // Place your HTTP methods below
}