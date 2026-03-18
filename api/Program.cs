using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            // Giữ nguyên DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 1. Cấu hình CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Ép chạy cổng 5266
            builder.WebHost.UseUrls("http://0.0.0.0:5266");

            var app = builder.Build();

            // 2. Kích hoạt CORS NGAY LẬP TỨC sau khi Build
            app.UseCors("AllowAll");

            // 3. Cho phép chạy Swagger cả ở môi trường Production (để Thành dễ debug trên Pi)
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Gym API V1");
                c.RoutePrefix = string.Empty; // Vào thẳng IP của Pi là thấy Swagger
            });

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}