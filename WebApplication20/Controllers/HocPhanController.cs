
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication20.Attributes;
using WebApplication20.Models;
using WebApplication20;

namespace WebApplication20.Controllers
{
    [Authorize]
    public class HocPhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocPhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách học phần
        public async Task<IActionResult> ListHP()
        {
            var hocPhans = await _context.HocPhan.ToListAsync();
            return View(hocPhans);
        }

        // Xử lý đăng ký học phần
        [HttpPost]
        public async Task<IActionResult> DangKy(string maHP)
        {
            // Lấy mã sinh viên từ session
            string maSV = HttpContext.Session.GetString("MaSV");

            // Kiểm tra bản ghi đăng ký đã tồn tại chưa
            var dangKy = await _context.DangKy
                .FirstOrDefaultAsync(d => d.MaSV == maSV && d.NgayDK == DateTime.Today);

            if (dangKy == null)
            {
                // Tạo mới đăng ký
                dangKy = new DangKy
                {
                    MaSV = maSV,
                    NgayDK = DateTime.Today
                };
                _context.DangKy.Add(dangKy);
                await _context.SaveChangesAsync();
            }

            // Thêm vào bảng ChiTietDangKy nếu chưa có
            bool daDangKy = await _context.ChiTietDangKy
                .AnyAsync(c => c.MaDK == dangKy.MaDK && c.MaHP == maHP);

            if (!daDangKy)
            {
                var chiTiet = new ChiTietDangKy
                {
                    MaDK = dangKy.MaDK,
                    MaHP = maHP
                };
                _context.ChiTietDangKy.Add(chiTiet);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký học phần thành công!";
            }
            else
            {
                TempData["Error"] = "Bạn đã đăng ký học phần này!";
            }

            return RedirectToAction(nameof(ListHP));
        }

        // Hiển thị danh sách học phần đã đăng ký
        public async Task<IActionResult> DaDangKy()
        {
            string maSV = HttpContext.Session.GetString("MaSV");

            var dangKyList = await _context.DangKy
                .Include(d => d.ChiTietDangKys)
                    .ThenInclude(c => c.HocPhan)
                .Where(d => d.MaSV == maSV)
                .ToListAsync();

            return View(dangKyList);
        }

        // Thêm action xóa đăng ký học phần
        [HttpPost]
        public async Task<IActionResult> XoaDangKy(int maDK, string maHP)
        {
            var chiTietDangKy = await _context.ChiTietDangKy
                .FirstOrDefaultAsync(c => c.MaDK == maDK && c.MaHP == maHP);

            if (chiTietDangKy != null)
            {
                _context.ChiTietDangKy.Remove(chiTietDangKy);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã hủy đăng ký học phần thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy thông tin đăng ký học phần!";
            }

            return RedirectToAction(nameof(DaDangKy));
        }
    }
}
