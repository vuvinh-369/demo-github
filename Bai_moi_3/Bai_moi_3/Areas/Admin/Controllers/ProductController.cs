﻿using Bai_moi_3.Models;
using Bai_moi_3.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Bai_moi_3.Areas.Admin.Controllers
{
    [Area("Admin")] // Chỉ định đây là Controller thuộc khu vực "Admin"
    [Authorize(Roles = SD.Role_Admin)] // Yêu cầu người dùng phải có vai trò "Admin" để truy cập
    public class ProductController : Controller
        {
            private readonly IProductRepository _productRepository;
            private readonly ICategoryRepository _categoryRepository;
            public ProductController(IProductRepository productRepository,
    ICategoryRepository categoryRepository)
            {
                _productRepository = productRepository;
                _categoryRepository = categoryRepository;
            }

            // Hiển thị danh sách sản phẩm 
            public async Task<IActionResult> Index()
            {
                var products = await _productRepository.GetAllAsync();
                return View(products);
            }
            // Hiển thị form thêm sản phẩm mới 
            public async Task<IActionResult> Add()
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                return View();
            }

            // Xử lý thêm sản phẩm mới 
            [HttpPost]
            public async Task<IActionResult> Add(Product product, IFormFile
    imageUrl)
            {
                if (ModelState.IsValid)
                {
                    if (imageUrl != null)
                    {
                        // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage 
                        product.ImageUrl = await SaveImage(imageUrl);
                    }

                    await _productRepository.AddAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập 
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(product);
            }

            // Viết thêm hàm SaveImage (tham khảo bài 02) 

            private async Task<string> SaveImage(IFormFile image)
            {
                //Thay đổi đường dẫn theo cấu hình của bạn 
                var savePath = Path.Combine("wwwroot/images", image.FileName);
                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                return "/images/" + image.FileName; // Trả về đường dẫn tương đối 
            }

            //Nhớ tạo folder images trong wwwroot
            // Hiển thị thông tin chi tiết sản phẩm 
            public async Task<IActionResult> Display(int id)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            // Hiển thị form cập nhật sản phẩm 
            public async Task<IActionResult> Update(int id)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name",
    product.CategoryId);
                return View(product);
            }
            // Xử lý cập nhật sản phẩm 
            [HttpPost]
            public async Task<IActionResult> Update(int id, Product product,
    IFormFile imageUrl)
            {
                ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
                if (id != product.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {

                    var existingProduct = await _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync 

                    // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                    if (imageUrl == null)
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }
                    else
                    {
                        // Lưu hình ảnh mới 
                        product.ImageUrl = await SaveImage(imageUrl);
                    }
                    // Cập nhật các thông tin khác của sản phẩm 
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.ImageUrl = product.ImageUrl;
                    await _productRepository.UpdateAsync(existingProduct);
                    return RedirectToAction(nameof(Index));
                }
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(product);
            }
            public async Task<IActionResult> Delete(int id)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            // Xử lý xóa sản phẩm 
            [HttpPost, ActionName("DeleteConfirmed")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _productRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
    }

