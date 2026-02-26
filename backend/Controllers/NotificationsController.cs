using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;
using ConsultationAPI.Data;

namespace ConsultationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private static readonly List<Notification> _notifications = new();
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ApplicationDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 3.1. Отправка напоминания
        [HttpPost("reminders")]
        public async Task<IActionResult> SendReminder([FromBody] SendReminderRequest request)
        {
            try
            {
                // Проверяем существование бронирования в БД
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == request.BookingId);
                    
                if (booking == null)
                {
                    return NotFound("Бронирование не найдено");
                }

                if (request.Recipients == null || !request.Recipients.Any())
                {
                    return BadRequest("Необходимо указать хотя бы одного получателя");
                }

                // Проверяем, что бронирование не отменено
                if (booking.Status == BookingStatus.Cancelled)
                {
                    return BadRequest("Нельзя отправить напоминание для отмененного бронирования");
                }

                var notification = new Notification
                {
                    BookingId = request.BookingId,
                    Recipients = request.Recipients,
                    CustomMessage = request.CustomMessage,
                    Type = NotificationType.Reminder,
                    SentAt = DateTime.UtcNow
                };

                // Получаем информацию о преподавателе из слота
                var slot = await _context.Slots.FirstOrDefaultAsync(s => s.Id == booking.SlotId);

                // Имитация отправки уведомлений
                var sentTo = new Dictionary<string, bool>();
                foreach (var recipient in request.Recipients)
                {
                    var sent = SendNotificationToRecipient(recipient, booking, slot, request.CustomMessage);
                    sentTo[recipient.ToString().ToLower()] = sent;
                }

                _notifications.Add(notification);

                var response = new SendReminderResponse
                {
                    NotificationId = notification.Id,
                    SentTo = sentTo,
                    SentAt = notification.SentAt,
                    NextReminder = DateTime.UtcNow.AddHours(24)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке напоминания");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // 3.2. Получение истории уведомлений
        [HttpGet("history")]
        public IActionResult GetNotificationHistory(
            [FromQuery] string? userId = null,
            [FromQuery] string? notificationType = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] int limit = 50)
        {
            try
            {
                var query = _notifications.AsQueryable();

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(n => n.BookingId == userId); // упрощенно
                }

                if (!string.IsNullOrEmpty(notificationType))
                {
                    if (Enum.TryParse<NotificationType>(notificationType, true, out var type))
                    {
                        query = query.Where(n => n.Type == type);
                    }
                }

                if (dateFrom.HasValue)
                {
                    query = query.Where(n => n.SentAt >= dateFrom.Value);
                }

                var notifications = query
                    .OrderByDescending(n => n.SentAt)
                    .Take(limit)
                    .Select(n => new NotificationHistoryItem
                    {
                        Id = n.Id,
                        Type = n.Type.ToString().ToLower(),
                        BookingId = n.BookingId,
                        Message = n.CustomMessage ?? "Напоминание",
                        SentAt = n.SentAt,
                        Recipients = n.Recipients.Select(r => r.ToString()).ToList()
                    })
                    .ToList();

                return Ok(new NotificationHistoryResponse
                {
                    Notifications = notifications,
                    TotalCount = _notifications.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении истории уведомлений");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // 3.3. Получение уведомлений по ID бронирования
        [HttpGet("booking/{bookingId}")]
        public IActionResult GetNotificationsByBooking(string bookingId)
        {
            try
            {
                var notifications = _notifications
                    .Where(n => n.BookingId == bookingId)
                    .OrderByDescending(n => n.SentAt)
                    .Select(n => new
                    {
                        n.Id,
                        Type = n.Type.ToString(),
                        Recipients = n.Recipients.Select(r => r.ToString()),
                        n.CustomMessage,
                        n.SentAt
                    })
                    .ToList();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении уведомлений по бронированию");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        private bool SendNotificationToRecipient(NotificationRecipient recipient, Booking booking, Slot? slot, string? customMessage)
        {
            try
            {
                // Имитация отправки уведомления
                _logger.LogInformation($"Отправка напоминания {recipient} для бронирования {booking.Id}");
                
                if (recipient == NotificationRecipient.Student)
                {
                    _logger.LogInformation($"→ Тема: {booking.Topic}");
                }
                else if (recipient == NotificationRecipient.Teacher && slot != null)
                {
                    _logger.LogInformation($"→ Преподавателю: {slot.TeacherName}");
                    _logger.LogInformation($"→ Студент: {booking.StudentName}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при отправке уведомления {recipient}");
                return false;
            }
        }

        // Метод для отладки
        [HttpGet("debug")]
        public IActionResult GetDebugInfo()
        {
            return Ok(new 
            {
                TotalNotifications = _notifications.Count,
                Notifications = _notifications.Select(n => new 
                {
                    n.Id,
                    n.BookingId,
                    n.Type,
                    Recipients = n.Recipients.Select(r => r.ToString()),
                    n.SentAt
                })
            });
        }
    }
}