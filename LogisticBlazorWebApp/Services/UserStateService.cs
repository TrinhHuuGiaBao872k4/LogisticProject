public class UserStateService
{
    public string TenDangNhap { get; set; } = "";
    public bool IsLoggedIn => !string.IsNullOrEmpty(TenDangNhap);

    public event Action? OnChange;

    public void SetUser(string tenDangNhap)
    {
        TenDangNhap = tenDangNhap;
        NotifyStateChanged();
    }

    public void Logout()
    {
        TenDangNhap = "";
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
