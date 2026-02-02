using Microsoft.AspNetCore.Mvc;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;
using ConsultationAPI.Data; // Добавляем using для SharedData

namespace ConsultationAPI.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private static readonly List<Notification> _notifications = new();
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ILogger<NotificationsController> logger)
        {
            _logger = logger;
        }

        // 3.1. Отправка напоминания
        [HttpPost("reminders")]
        public IActionResult SendReminder([FromBody] SendReminderRequest request)
        {
            try
            {
                // Используем SharedData.Bookings вместо _bookings
                var booking = SharedData.Bookings.FirstOrDefault(b => b.Id == request.BookingId);
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
                    SentAt = DateTime.UtcNow,
                    DeliveryStatus = NotificationDeliveryStatus.Sent
                };

                // Имитация отправки уведомлений
                var sentTo = new Dictionary<string, bool>();
                foreach (var recipient in request.Recipients)
                {
                    var sent = SendNotificationToRecipient(recipient, booking, request.CustomMessage);
                    sentTo[recipient.ToString().ToLower()] = sent;

                    // Определяем recipientId в зависимости от типа получателя
                    string? recipientId = null;
                    string recipientType = recipient.ToString().ToLower();
                    
                    if (recipient == NotificationRecipient.Student)
                    {
                        recipientId = booking.StudentId;
                    }
                    else if (recipient == NotificationRecipient.Teacher)
                    {
                        // Находим слот, чтобы получить teacherId
                        var slot = SharedData.Slots.FirstOrDefault(s => s.Id == booking.SlotId);
                        recipientId = slot?.TeacherId ?? "unknown_teacher";
                    }

                    // Сохраняем историю для каждого получателя
                    _notifications.Add(new Notification
                    {
                        BookingId = request.BookingId,
                        Recipients = new List<NotificationRecipient> { recipient },
                        CustomMessage = request.CustomMessage,
                        Type = NotificationType.Reminder,
                        RecipientId = recipientId,
                        RecipientType = recipientType,
                        Message = request.CustomMessage ?? $"Напоминание о консультации: {booking.Topic}",
                        SentAt = DateTime.UtcNow,
                        DeliveryStatus = sent ? NotificationDeliveryStatus.Delivered : NotificationDeliveryStatus.Failed
                    });
                }

                var response = new SendReminderResponse
                {
                    NotificationId = notification.Id,
                    SentTo = sentTo,
                    SentAt = notification.SentAt,
                    NextReminder = DateTime.UtcNow.AddHours(24) // Следующее напоминание через 24 часа
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке напоминания");
                return BadRequest("Невалидные параметры получателей");
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
                    query = query.Where(n => n.RecipientId == userId);
                }

                if (!string.IsNullOrEmpty(notificationType))
                {
                    if (Enum.TryParse<NotificationType>(notificationType, true, out var type))
                    {
                        query = query.Where(n => n.Type == type);
                    }
                    else
                    {
                        return BadRequest("Неверный тип уведомления");
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
                        RecipientId = n.RecipientId ?? string.Empty,
                        RecipientType = n.RecipientType ?? string.Empty,
                        Message = n.Message,
                        SentAt = n.SentAt,
                        DeliveryStatus = n.DeliveryStatus.ToString().ToLower()
                    })
                    .ToList();

                var response = new NotificationHistoryResponse
                {
                    Notifications = notifications,
                    TotalCount = _notifications.Count
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении истории уведомлений");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // 3.3. Получение уведомлений по ID бронирования (новый метод для удобства)
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
                        n.Message,
                        n.SentAt,
                        DeliveryStatus = n.DeliveryStatus.ToString()
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

        private bool SendNotificationToRecipient(NotificationRecipient recipient, Booking booking, string? customMessage)
        {
            try
            {
                // Имитация отправки уведомления
                _logger.LogInformation($"Отправка напоминания {recipient} для бронирования {booking.Id}");
                
                var message = customMessage ?? $"Напоминание о консультации: {booking.Topic}";
                
                // Дополнительная информация о получателе
                if (recipient == NotificationRecipient.Student)
                {
                    _logger.LogInformation($"Отправка студенту {booking.StudentName} ({booking.ContactInfo?.Email ?? booking.ContactInfo?.TelegramId ?? "без контактов"})");
                }
                else if (recipient == NotificationRecipient.Teacher)
                {
                    var slot = SharedData.Slots.FirstOrDefault(s => s.Id == booking.SlotId);
                    _logger.LogInformation($"Отправка преподавателю {slot?.TeacherName ?? "unknown"} для бронирования {booking.Id}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при отправке уведомления {recipient}");
                return false;
            }
        }

        // Метод для отладки - получить все уведомления
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
                    n.RecipientId,
                    n.RecipientType,
                    n.SentAt,
                    n.DeliveryStatus
                })
            });
        }
    }
}