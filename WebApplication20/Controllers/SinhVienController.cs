using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using WebApplication20.Attributes;

using WebApplication20.Models;


namespace WebApplication20.Controllers
{
    [Authorize]
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SinhVienController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Trang danh sách sinh viên
        public IActionResult Index()
        {
            var sinhViens = _context.SinhVien.Include(s => s.NganhHoc).ToList();
            return View(sinhViens);
        }

        // Trang thêm sinh viên
        public IActionResult Create()
        {
            ViewData["NganhHocList"] = new SelectList(_context.NganhHoc, "MaNganh", "TenNganh");
            return View();
        }

        // Xử lý thêm sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien, IFormFile? Hinh)
        {
            if (await _context.SinhVien.AnyAsync(s => s.MaSV == sinhVien.MaSV))
            {
                ModelState.AddModelError("MaSV", "Mã sinh viên đã tồn tại.");
                ViewData["NganhHocList"] = new SelectList(_context.NganhHoc, "MaNganh", "TenNganh", sinhVien.MaNganh);
                return View(sinhVien);
            }

            if (Hinh != null && Hinh.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinh.FileName);
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Tạo thư mục nếu không tồn tại
                Directory.CreateDirectory(uploadPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Hinh.CopyToAsync(stream);
                }

                sinhVien.Hinh = "images/" + uniqueFileName;
            }
            else
            {
                sinhVien.Hinh = "images/default.jpg";
            }

            _context.Add(sinhVien);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Trang sửa sinh viên
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = _context.SinhVien.Find(id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            ViewData["NganhHocList"] = new SelectList(_context.NganhHoc, "MaNganh", "TenNganh", sinhVien.MaNganh);
            return View(sinhVien);
        }

        // Xử lý sửa sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SinhVien sinhVien, IFormFile? Hinh)
        {
            if (id != sinhVien.MaSV)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm sinh viên hiện tại từ CSDL
                    var sinhVienInDb = await _context.SinhVien.AsNoTracking().FirstOrDefaultAsync(s => s.MaSV == id);

                    if (sinhVienInDb == null)
                    {
                        return NotFound();
                    }

                    // Xử lý ảnh mới
                    if (Hinh != null && Hinh.Length > 0)
                    {
                        // Xóa ảnh cũ nếu không phải ảnh mặc định
                        if (!string.IsNullOrEmpty(sinhVienInDb.Hinh) && sinhVienInDb.Hinh != "images/default.jpg")
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, sinhVienInDb.Hinh);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Hinh.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var filePath = Path.Combine(uploadPath, uniqueFileName);

                        Directory.CreateDirectory(uploadPath);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Hinh.CopyToAsync(stream);
                        }

                        sinhVien.Hinh = "images/" + uniqueFileName;
                    }
                    else
                    {
                        // Giữ lại ảnh cũ nếu không có ảnh mới
                        sinhVien.Hinh = sinhVienInDb.Hinh;
                    }

                    // Cập nhật thông tin sinh viên
                    _context.Entry(sinhVien).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSV))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewData["NganhHocList"] = new SelectList(_context.NganhHoc, "MaNganh", "TenNganh", sinhVien.MaNganh);
            return View(sinhVien);
        }

        // Trang xóa sinh viên
        // Trang chi tiết sinh viên
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = _context.SinhVien
                .Include(s => s.NganhHoc)
                .FirstOrDefault(m => m.MaSV == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        public IActionResult Delete(string id)
        {
            var sinhVien = _context.SinhVien.Include(s => s.NganhHoc).FirstOrDefault(s => s.MaSV == id);
            if (sinhVien == null)
                return NotFound();

            return View(sinhVien);
        }

        // Xử lý xóa sinh viên
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Kiểm tra sinh viên có tồn tại không
            var sinhVien = await _context.SinhVien.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            // Xóa ảnh nếu có
            if (!string.IsNullOrEmpty(sinhVien.Hinh) && sinhVien.Hinh != "images/default.jpg")
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, sinhVien.Hinh);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Xóa sinh viên khỏi cơ sở dữ liệu
            _context.SinhVien.Remove(sinhVien);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }


        private bool SinhVienExists(string id)
        {
            return _context.SinhVien.Any(e => e.MaSV == id);
        }
    }
}
