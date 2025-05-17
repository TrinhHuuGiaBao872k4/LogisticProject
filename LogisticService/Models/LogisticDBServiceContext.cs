using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LogisticService.Models;

public partial class LogisticDBServiceContext : DbContext
{
    public LogisticDBServiceContext()
    {
    }

    public LogisticDBServiceContext(DbContextOptions<LogisticDBServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietPhieuNhapXuat> ChiTietPhieuNhapXuats { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonHangCungCap> DonHangCungCaps { get; set; }

    public virtual DbSet<DonViVanChuyen> DonViVanChuyens { get; set; }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    public virtual DbSet<KhoHang> KhoHangs { get; set; }

    public virtual DbSet<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; }

    public virtual DbSet<LoaiHangHoa> LoaiHangHoas { get; set; }

    public virtual DbSet<LoaiKhoHang> LoaiKhoHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhieuNhapXuat> PhieuNhapXuats { get; set; }

    public virtual DbSet<PhieuVanChuyen> PhieuVanChuyens { get; set; }

    public virtual DbSet<TinhTrangDonHangChiTiet> TinhTrangDonHangChiTiets { get; set; }

    public virtual DbSet<TonKho> TonKhos { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaHangHoa, e.MaDonHang });

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDH_DonHang");

            entity.HasOne(d => d.MaHangHoaNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaHangHoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDH_HangHoa");
        });

        modelBuilder.Entity<ChiTietPhieuNhapXuat>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuNhapXuat, e.MaHangHoa, e.MaKhoHang });

            entity.ToTable("ChiTietPhieuNhapXuat");

            entity.Property(e => e.MaPhieuNhapXuat)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKhoHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaHangHoaNavigation).WithMany(p => p.ChiTietPhieuNhapXuats)
                .HasForeignKey(d => d.MaHangHoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPNX_HangHoa");

            entity.HasOne(d => d.MaKhoHangNavigation).WithMany(p => p.ChiTietPhieuNhapXuats)
                .HasForeignKey(d => d.MaKhoHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPNX_KhoHang");

            entity.HasOne(d => d.MaPhieuNhapXuatNavigation).WithMany(p => p.ChiTietPhieuNhapXuats)
                .HasForeignKey(d => d.MaPhieuNhapXuat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPNX_PhieuNhapXuat");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang);

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayDenDuKien).HasColumnType("datetime");
            entity.Property(e => e.NgayKhoiTao).HasColumnType("datetime");
            entity.Property(e => e.NgayVanChuyen).HasColumnType("datetime");
            entity.Property(e => e.TenDonHang).HasMaxLength(30);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_DonHang_NguoiDung");
        });

        modelBuilder.Entity<DonHangCungCap>(entity =>
        {
            entity.HasKey(e => new { e.MaHangHoa, e.MaNhaCungCap });

            entity.ToTable("DonHangCungCap");

            entity.Property(e => e.MaHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNhaCungCap)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayCungCap).HasColumnType("datetime");

            entity.HasOne(d => d.MaHangHoaNavigation).WithMany(p => p.DonHangCungCaps)
                .HasForeignKey(d => d.MaHangHoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHangCungCap_HangHoa");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.DonHangCungCaps)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHangCungCap_NhaCungCap");
        });

        modelBuilder.Entity<DonViVanChuyen>(entity =>
        {
            entity.HasKey(e => e.MaDonVi);

            entity.ToTable("DonViVanChuyen");

            entity.Property(e => e.MaDonVi)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenDonVi).HasMaxLength(30);
        });

        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.MaHangHoa);

            entity.ToTable("HangHoa");

            entity.Property(e => e.MaHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.MaLoaiHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgaySanXuat).HasColumnType("datetime");
            entity.Property(e => e.TenHangHoa).HasMaxLength(30);

            entity.HasOne(d => d.MaLoaiHangHoaNavigation).WithMany(p => p.HangHoas)
                .HasForeignKey(d => d.MaLoaiHangHoa)
                .HasConstraintName("FK_HangHoa_LoaiHangHoa");
        });

        modelBuilder.Entity<KhoHang>(entity =>
        {
            entity.HasKey(e => e.MaKhoHang);

            entity.ToTable("KhoHang");

            entity.Property(e => e.MaKhoHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiaChi).HasMaxLength(30);
            entity.Property(e => e.MaLoaiKhoHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenKhoHang).HasMaxLength(20);

            entity.HasOne(d => d.MaLoaiKhoHangNavigation).WithMany(p => p.KhoHangs)
                .HasForeignKey(d => d.MaLoaiKhoHang)
                .HasConstraintName("FK_KhoHang_LoaiKhoHang");
        });

        modelBuilder.Entity<LichSuTrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("PK_LichSuTrangThai");

            entity.ToTable("LichSuTrangThaiDonHang");

            entity.Property(e => e.MaLichSu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaTrangThai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.LichSuTrangThaiDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK_LichSu_DonHang");

            entity.HasOne(d => d.MaTrangThaiNavigation).WithMany(p => p.LichSuTrangThaiDonHangs)
                .HasForeignKey(d => d.MaTrangThai)
                .HasConstraintName("FK_LichSu_TrangThai");
        });

        modelBuilder.Entity<LoaiHangHoa>(entity =>
        {
            entity.HasKey(e => e.MaLoaiHangHoa);

            entity.ToTable("LoaiHangHoa");

            entity.Property(e => e.MaLoaiHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenLoaiHangHoa).HasMaxLength(30);
        });

        modelBuilder.Entity<LoaiKhoHang>(entity =>
        {
            entity.HasKey(e => e.MaLoaiKhoHang);

            entity.ToTable("LoaiKhoHang");

            entity.Property(e => e.MaLoaiKhoHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenLoaiKhoHang).HasMaxLength(30);
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung);

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.Cccd, "UQ_NguoiDung_CCCD").IsUnique();

            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(30);
            entity.Property(e => e.HoTen).HasMaxLength(30);
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenDanhNhap).HasMaxLength(30);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK_NguoiDung_VaiTro");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap);

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.MaNhaCungCap)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(30);
        });

        modelBuilder.Entity<PhieuNhapXuat>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhapXuat);

            entity.ToTable("PhieuNhapXuat");

            entity.Property(e => e.MaPhieuNhapXuat)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GhiChu).HasMaxLength(50);
            entity.Property(e => e.LoaiPhieu).HasMaxLength(20);
            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayLapPhieu).HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhieuNhapXuats)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_PhieuNhapXuat_NguoiDung");
        });

        modelBuilder.Entity<PhieuVanChuyen>(entity =>
        {
            entity.HasKey(e => e.MaPhieuVanChuyen);

            entity.ToTable("PhieuVanChuyen");

            entity.Property(e => e.MaPhieuVanChuyen)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaDonVi)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayVanChuyen).HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.PhieuVanChuyens)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK_PhieuVC_DonHang");

            entity.HasOne(d => d.MaDonViNavigation).WithMany(p => p.PhieuVanChuyens)
                .HasForeignKey(d => d.MaDonVi)
                .HasConstraintName("FK_PhieuVC_DonVi");
        });

        modelBuilder.Entity<TinhTrangDonHangChiTiet>(entity =>
        {
            entity.HasKey(e => e.MaTinhTrangChiTiet).HasName("PK__TinhTran__33B947B60E8D0B51");

            entity.ToTable("TinhTrangDonHangChiTiet");

            entity.Property(e => e.MaTinhTrangChiTiet)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NoiDung).HasMaxLength(255);
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.TinhTrangDonHangChiTiets)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK_TinhTrangChiTiet_DonHang");
        });

        modelBuilder.Entity<TonKho>(entity =>
        {
            entity.HasKey(e => new { e.MaKhoHang, e.MaHangHoa });

            entity.ToTable("TonKho");

            entity.Property(e => e.MaKhoHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaHangHoa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaHangHoaNavigation).WithMany(p => p.TonKhos)
                .HasForeignKey(d => d.MaHangHoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TonKho_HangHoa");

            entity.HasOne(d => d.MaKhoHangNavigation).WithMany(p => p.TonKhos)
                .HasForeignKey(d => d.MaKhoHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TonKho_KhoHang");
        });

        modelBuilder.Entity<TrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.MaTrangThai);

            entity.ToTable("TrangThaiDonHang");

            entity.Property(e => e.MaTrangThai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenTrangThai).HasMaxLength(20);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro);

            entity.ToTable("VaiTro");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenVaiTro).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
