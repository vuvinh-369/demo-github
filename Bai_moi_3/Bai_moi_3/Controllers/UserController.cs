using Bai_moi_3.Models;
using Bai_moi_3.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Có thể không cần nếu không dùng SelectList
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Thêm namespace này cho DbUpdateConcurrencyException

namespace Bai_moi_3.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Hiển thị danh sách người dùng
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        // Hiển thị form thêm người dùng mới
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm người dùng mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(User user, IFormFile imageUrl1)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl1 != null && imageUrl1.Length > 0)
                {
                    try
                    {
                        // Truyền biến imageUrl1 vào hàm SaveImage (tham số là 'image')
                        user.ImageUrl1 = await SaveImage(imageUrl1);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Lỗi khi lưu ảnh: " + ex.Message);
                        return View(user);
                    }
                }
                else
                {
                    user.ImageUrl1 = null;
                }

                await _userRepository.AddAsync(user);
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // **Hàm SaveImage đã được sửa lại: Tham số là 'image' (số ít) và dùng tên này bên trong hàm**
        private async Task<string> SaveImage(IFormFile image) // <-- Đã đổi lại thành 'image'
        {
            // Đường dẫn đến thư mục wwwroot/images
            // **LƯU Ý QUAN TRỌNG: ĐẢM BẢO THƯ MỤC wwwroot/images TỒN TẠI TRƯỚC KHI CHẠY**
            // Nếu thư mục không tồn tại, bạn sẽ gặp lỗi.
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Tên file sẽ giữ nguyên tên gốc của ảnh
            // **CẨN THẬN: CÓ THỂ BỊ GHI ĐÈ NẾU TÊN FILE TRÙNG NHAU**
            string fileName = image.FileName; // <-- Đã đổi thành 'image.FileName'
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu file vào đường dẫn đã tạo
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream); // <-- Đã đổi thành 'image.CopyToAsync'
            }
            // Trả về đường dẫn tương đối để lưu vào cơ sở dữ liệu và hiển thị trên web
            return "/images/" + fileName;
        }

        // Hiển thị thông tin chi tiết người dùng
        public async Task<IActionResult> Display(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Hiển thị form cập nhật người dùng
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Xử lý cập nhật người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, User user, IFormFile newImageUrl1)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.Remove("Password");
            }

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Address = user.Address;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.CreatedAt = user.CreatedAt;
            existingUser.ImageUrl1 = user.ImageUrl1;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                // TODO: PHẢI HASH MẬT KHẨU Ở ĐÂY TRƯỚC KHI LƯU VÀO DB!
                existingUser.Password = user.Password;
            }

            // Xử lý cập nhật hình ảnh
            if (newImageUrl1 != null && newImageUrl1.Length > 0)
            {
                try
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingUser.ImageUrl1))
                    {
                        // Đường dẫn xóa ảnh cũ phải đúng với cách lưu ảnh đó
                        // Nếu trước đây bạn lưu vào /images/users/, thì phải xóa từ đó.
                        // Nếu bây giờ lưu vào /images/, thì chỉ cần xóa từ /images/.
                        // Hàm TrimStart('/') là quan trọng để Path.Combine làm việc đúng.
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingUser.ImageUrl1.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Gọi hàm SaveImage đã sửa (tham số là 'image')
                    existingUser.ImageUrl1 = await SaveImage(newImageUrl1);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật ảnh: " + ex.Message);
                    return View(user);
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _userRepository.UpdateAsync(existingUser);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật người dùng: " + ex.Message);
            }

            return View(user);
        }

        // Hiển thị form xác nhận xóa người dùng
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Xử lý xóa người dùng
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Xóa hình ảnh của người dùng khỏi server trước khi xóa user khỏi DB
            if (!string.IsNullOrEmpty(user.ImageUrl1))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ImageUrl1.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting old image: {ex.Message}");
                    }
                }
            }

            await _userRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}