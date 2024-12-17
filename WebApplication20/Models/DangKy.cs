
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication20.Models;
namespace WebApplication20
{
    public class DangKy
    {
        [Key]
        public int MaDK { get; set; }

        [Column(TypeName = "Date")]
        public DateTime NgayDK { get; set; }

        [ForeignKey("SinhVien")]
        public string MaSV { get; set; }
        public virtual SinhVien SinhVien { get; set; }

        // Khởi tạo List để tránh null
        public List<ChiTietDangKy> ChiTietDangKys { get; set; } = new List<ChiTietDangKy>();
    }
}
