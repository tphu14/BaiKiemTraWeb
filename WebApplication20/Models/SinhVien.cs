using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication20.Models;

namespace WebApplication20.Models
{
    public class SinhVien
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã sinh viên")]
        public string MaSV { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [StringLength(5)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Column(TypeName = "Date")]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [StringLength(50)]
        [Display(Name = "Hình ảnh")]
        public string? Hinh { get; set; }

        [ForeignKey("NganhHoc")]
        [Required(ErrorMessage = "Vui lòng chọn ngành học")]
        [Display(Name = "Ngành học")]
        public string MaNganh { get; set; }
        public virtual NganhHoc? NganhHoc { get; set; }
    }
}
