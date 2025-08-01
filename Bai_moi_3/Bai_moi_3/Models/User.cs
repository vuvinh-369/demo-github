using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Thêm namespace này cho List

namespace Bai_moi_3.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải dài từ 6 đến 100 ký tự.")]
        public string Password { get; set; } // Nhắc lại: Cần hash mật khẩu trước khi lưu vào DB

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        public string Role { get; set; } // Ví dụ: Admin, User, Manager

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; } // Ngày sinh

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string Address { get; set; } // Địa chỉ

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; } // Số điện thoại

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo tài khoản

        public string? ImageUrl1 { get; set; } // Đường dẫn ảnh đại diện

        // Đây có vẻ là một mối quan hệ One-to-Many với UserImage (ví dụ: một user có nhiều ảnh)
        // Nếu UserImage là các ảnh khác của user, thì nó đúng.
        // Nếu bạn chỉ muốn 1 ảnh chính (ImageUrl1) thì có thể bỏ List này đi.
        public List<UserImage>? Images1 { get; set; }
    }
}