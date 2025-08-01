using Bai_moi_3.Models;
using Bai_moi_3.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// using VKIT_Lab04_Chieu.Repositories; // <-- Đã xóa hoặc comment nếu không sử dụng
// using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database; // <-- Đã xóa hoặc comment nếu không sử dụng

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký các Repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>(); // Đảm bảo bạn có Product model/repo nếu sử dụng
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>(); // Đảm bảo bạn có Category model/repo nếu sử dụng
builder.Services.AddScoped<IUserRepository, EFUserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép truy cập các file tĩnh trong wwwroot

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();

