using Microsoft.EntityFrameworkCore;
using ConsultationAPI.Data;
using ConsultationAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Добавляем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Получаем строку подключения из переменных окружения
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // Если нет строки подключения - ошибка, мы в Docker
    throw new Exception("Connection string not found. Please set ConnectionStrings__DefaultConnection environment variable.");
}

// Используем только PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

Console.WriteLine($"🟢 Используется PostgreSQL: {connectionString}");

var app = builder.Build();

// Создаем базу данных и добавляем тестовые данные
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Применяем миграции или создаем базу
    dbContext.Database.EnsureCreated();
    Console.WriteLine("✅ База данных создана/проверена");
    
    // Добавляем тестовые данные, если таблица пуста
    if (!dbContext.Slots.Any())
    {
        dbContext.Slots.AddRange(new[]
        {
            new Slot
            {
                TeacherId = "teacher1",
                TeacherName = "Иван Петров",
                StartTime = DateTime.UtcNow.AddDays(1).AddHours(10),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(11),
                MeetingType = ConsultationAPI.Enums.MeetingType.Online,
                Description = "Консультация по математике",
                Status = ConsultationAPI.Enums.SlotStatus.Free
            },
            new Slot
            {
                TeacherId = "teacher2",
                TeacherName = "Мария Сидорова",
                StartTime = DateTime.UtcNow.AddDays(1).AddHours(14),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(15),
                MeetingType = ConsultationAPI.Enums.MeetingType.Offline,
                Description = "Консультация по физике",
                Status = ConsultationAPI.Enums.SlotStatus.Free
            }
        });
        
        dbContext.SaveChanges();
        Console.WriteLine("✅ Тестовые данные добавлены в базу");
    }
    
    Console.WriteLine($"📊 Всего слотов: {dbContext.Slots.Count()}");
}

app.UseCors("AllowAll");
app.MapControllers();

Console.WriteLine("🚀 API запущен");
app.Run();