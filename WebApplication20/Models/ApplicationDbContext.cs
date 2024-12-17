using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApplication20.Models;

namespace WebApplication20.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Các DbSet đại diện cho bảng trong cơ sở dữ liệu
        public DbSet<NganhHoc> NganhHoc { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<HocPhan> HocPhan { get; set; }
        public DbSet<DangKy> DangKy { get; set; }
        public DbSet<ChiTietDangKy> ChiTietDangKy { get; set; }

        // Cấu hình khóa chính kết hợp cho ChiTietDangKy trong OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính kết hợp cho ChiTietDangKy
            modelBuilder.Entity<ChiTietDangKy>()
                .HasKey(c => new { c.MaDK, c.MaHP });  // Khóa chính kết hợp từ MaDK và MaHP

            modelBuilder.Entity<ChiTietDangKy>()
                .HasOne(c => c.DangKy)
                .WithMany(d => d.ChiTietDangKys)
                .HasForeignKey(c => c.MaDK);

            modelBuilder.Entity<ChiTietDangKy>()
                .HasOne(c => c.HocPhan)
                .WithMany()
                .HasForeignKey(c => c.MaHP);

            // Nếu bạn cần các cấu hình khác, có thể tiếp tục ở đây
        }
    }
}
