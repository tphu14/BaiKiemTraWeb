using Microsoft.AspNetCore.Mvc;

using WebApplication20.Models;

namespace WebApplication20.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(string MaSV)
        {
            if (string.IsNullOrEmpty(MaSV))
            {
                ModelState.AddModelError("", "Vui lòng nhập mã sinh viên");
                return View();
            }

            var sinhVien = _context.SinhVien.FirstOrDefault(sv => sv.MaSV == MaSV);
            if (sinhVien == null)
            {
                ModelState.AddModelError("", "Mã sinh viên không tồn tại");
                return View();
            }

            // Lưu mã sinh viên vào session
            HttpContext.Session.SetString("MaSV", MaSV);

            // Chuyển hướng đến trang danh sách h��c phần đã đăng ký
            return RedirectToAction("DaDangKy", "HocPhan");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }
}