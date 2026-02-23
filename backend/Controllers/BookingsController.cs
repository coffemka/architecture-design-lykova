using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;
using ConsultationAPI.Data;

namespace ConsultationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(ApplicationDbContext context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 2.1. Создание бронирования
        [HttpPost]
public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
{
    try
    {
        // Находим слот
        var slot = await _context.Slots.FirstOrDefaultAsync(s => s.Id == request.SlotId);
        if (slot == null)
        {
            return NotFound("Слот не найден");
        }

        if (slot.Status != SlotStatus.Free)
        {
            return BadRequest("Слот уже занят");
        }

        // Проверяем, не забронировал ли уже этот студент этот слот
        var existingBooking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.SlotId == request.SlotId && 
                b.StudentId == request.StudentId && 
                b.Status != BookingStatus.Cancelled);
        
        if (existingBooking != null)
        {
            return Conflict("Студент уже забронировал этот слот");
        }

        // Создаем бронирование
        var booking = new Booking
        {
            SlotId = request.SlotId,
            StudentId = request.StudentId,
            StudentName = request.StudentName,
            Topic = request.Topic,
            Email = request.Email,
            TelegramId = request.TelegramId,
            AdditionalNotes = request.AdditionalNotes,
            Status = BookingStatus.Pending
        };

        // Обновляем статус слота
        slot.Status = SlotStatus.Pending;

        // Сохраняем
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var response = new CreateBookingResponse
        {
            BookingId = booking.Id,
            Status = booking.Status,
            SlotId = booking.SlotId,
            StudentId = booking.StudentId,
            Topic = booking.Topic,
            CreatedAt = booking.CreatedAt
        };

        return Ok(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ошибка при создании бронирования");
        return StatusCode(500, "Внутренняя ошибка сервера");
    }
}
        // 2.2. Подтверждение бронирования
        [HttpPut("{bookingId}/confirm")]
        public async Task<IActionResult> ConfirmBooking(string bookingId, [FromBody] ConfirmBookingRequest request)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == bookingId);
                    
                if (booking == null)
                {
                    return NotFound("Бронирование не найдено");
                }

                if (booking.Status != BookingStatus.Pending)
                {
                    return BadRequest("Бронирование уже подтверждено или отменено");
                }

                // Обновляем статус бронирования
                booking.Status = BookingStatus.Confirmed;
                booking.ConfirmedAt = DateTime.UtcNow;
                booking.TeacherNotes = request.TeacherNotes;
                booking.MeetingLink = request.MeetingLink;

                // Обновляем статус слота
                var slot = await _context.Slots.FirstOrDefaultAsync(s => s.Id == booking.SlotId);
                if (slot != null)
                {
                    slot.Status = SlotStatus.Booked;
                }

                await _context.SaveChangesAsync();

                bool notificationSent = false;
                if (request.SendNotification)
                {
                    // Имитация отправки уведомления
                    notificationSent = true;
                    _logger.LogInformation($"Уведомление отправлено для бронирования {booking.Id}");
                }

                var response = new ConfirmBookingResponse
                {
                    BookingId = booking.Id,
                    Status = booking.Status,
                    ConfirmedAt = booking.ConfirmedAt.Value,
                    MeetingLink = booking.MeetingLink,
                    NotificationSent = notificationSent
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при подтверждении бронирования");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // 2.3. Отмена бронирования
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(string bookingId, [FromBody] CancelBookingRequest request)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == bookingId);
                    
                if (booking == null)
                {
                    return NotFound("Бронирование не найдено");
                }

                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTime.UtcNow;
                booking.CancelledBy = request.CancelledBy.ToString();

                // Освобождаем слот
                var slot = await _context.Slots.FirstOrDefaultAsync(s => s.Id == booking.SlotId);
                if (slot != null)
                {
                    slot.Status = SlotStatus.Free;
                }

                await _context.SaveChangesAsync();

                bool notificationSent = false;
                if (request.NotifyOtherParty)
                {
                    notificationSent = true;
                    _logger.LogInformation($"Уведомление об отмене отправлено для бронирования {booking.Id}");
                }

                var response = new CancelBookingResponse
                {
                    BookingId = booking.Id,
                    Status = booking.Status,
                    CancelledAt = booking.CancelledAt.Value,
                    CancelledBy = booking.CancelledBy,
                    NotificationSent = notificationSent
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отмене бронирования");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // 2.4. Получение бронирований преподавателя
        [HttpGet("teachers/{teacherId}")]
        public async Task<IActionResult> GetTeacherBookings(
            string teacherId,
            [FromQuery] BookingStatus? status = null)
        {
            try
            {
                // Получаем слоты преподавателя
                var teacherSlots = await _context.Slots
                    .Where(s => s.TeacherId == teacherId)
                    .ToListAsync();
                    
                var slotIds = teacherSlots.Select(s => s.Id).ToList();

                // Получаем бронирования для этих слотов
                var bookingsQuery = _context.Bookings
                    .Where(b => slotIds.Contains(b.SlotId));

                if (status.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Status == status.Value);
                }

                var bookings = await bookingsQuery.ToListAsync();

                var teacherBookings = bookings.Select(b =>
                {
                    var slot = teacherSlots.FirstOrDefault(s => s.Id == b.SlotId);
                    return new TeacherBooking
                    {
                        Id = b.Id,
                        SlotId = b.SlotId,
                        StudentName = b.StudentName,
                        Topic = b.Topic,
                        Status = b.Status,
                        StartTime = slot?.StartTime ?? DateTime.MinValue,
                        EndTime = slot?.EndTime ?? DateTime.MinValue,
                        CreatedAt = b.CreatedAt,
                    };
                }).ToList();

                return Ok(teacherBookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении бронирований преподавателя");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}