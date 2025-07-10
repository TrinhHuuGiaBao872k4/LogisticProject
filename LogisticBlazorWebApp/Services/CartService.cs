public class CartService
{
    public List<CartItemVM> Items = new();
    public event Action OnChange;
    public void AddToCart(HangHoaVM hang)
    {
        var existing = Items.FirstOrDefault(x => x.MaHangHoa == hang.MaHangHoa);
        if (existing != null)
        {
            existing.SoLuong++;
        }
        else
        {
            Items.Add(new CartItemVM
            {
                MaHangHoa = hang.MaHangHoa,
                TenHangHoa = hang.TenHangHoa,
                DonGia = hang.GiaHangHoa,
                HinhAnh = hang.HinhAnh,
                SoLuong = 1
            });
        }
        NotifyStateChanged();
    }

    public void UpdateSoLuong(string MaHangHoa, int soLuong)
    {
        var item = Items.FirstOrDefault(x => x.MaHangHoa == MaHangHoa);
        if (item != null)
        {
            item.SoLuong = Math.Max(1, soLuong);
            NotifyStateChanged();
        }
    }

    public void RemoveItem(string MaHangHoa)
    {
        var item = Items.FirstOrDefault(x => x.MaHangHoa == MaHangHoa);
        if (item != null)
        {
            Items.Remove(item);
            NotifyStateChanged();
        }
    }

    public int GetTongSoLuong() => Items.Sum(x => x.SoLuong);
    public decimal? GetTongTien() => Items.Sum(x => x.SoLuong * x.DonGia);

    private void NotifyStateChanged() => OnChange?.Invoke();
}
