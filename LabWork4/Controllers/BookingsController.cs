using Microsoft.AspNetCore.Mvc;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;
using ConsultationAPI.Data; 

namespace ConsultationAPI.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(ILogger<BookingsController> logger)
        {
            _logger = logger;
        }

        // 2.1. Создание бронирования
        [HttpPost]
        public IActionResult CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                // Находим слот в общем списке
                var slot = SharedData.Slots.FirstOrDefault(s => s.Id == request.SlotId);
                if (slot == null || slot.Status != SlotStatus.Free)
                {
                    return Conflict($"Слот не существует или уже занят. Slot ID: {request.SlotId}, Status: {slot?.Status}");
                }

                // Проверяем, не забронировал ли уже этот студент этот слот
                var existingBooking = SharedData.Bookings.FirstOrDefault(b => 
                    b.SlotId == request.SlotId && 
                    b.StudentId == request.StudentId && 
                    b.Status != BookingStatus.Cancelled);
                
                if (existingBooking != null)
                {
                    return Conflict("Студент уже забронировал этот слот");
                }

                var booking = new Booking
                {
                    SlotId = request.SlotId,
                    StudentId = request.StudentId,
                    StudentName = request.StudentName,
                    Topic = request.Topic,
                    ContactInfo = request.ContactInfo,
                    AdditionalNotes = request.AdditionalNotes,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    ConfirmationDeadline = DateTime.UtcNow.AddDays(2) // 2 дня на подтверждение
                };

                // Обновляем статус слота
                slot.Status = SlotStatus.Pending;

                // Добавляем в общий список
                SharedData.Bookings.Add(booking);

                var response = new CreateBookingResponse
                {
                    BookingId = booking.Id,
                    Status = booking.Status,
                    SlotId = booking.SlotId,
                    StudentId = booking.StudentId,
                    Topic = booking.Topic,
                    CreatedAt = booking.CreatedAt,
                    ConfirmationDeadline = booking.ConfirmationDeadline
                };

                return CreatedAtAction(nameof(CreateBooking), response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании бронирования");
                return BadRequest("Невалидные данные");
            }
        }

        // 2.2. Подтверждение бронирования
        [HttpPut("{bookingId}/confirm")]
        public IActionResult ConfirmBooking(string bookingId, [FromBody] ConfirmBookingRequest request)
        {
            try
            {
                var booking = SharedData.Bookings.FirstOrDefault(b => b.Id == bookingId);
                if (booking == null)
                {
                    return NotFound("Бронирование не найдено");
                }

                if (booking.Status != BookingStatus.Pending)
                {
                    return BadRequest("Бронирование уже подтверждено или отменено");
                }

                var slot = SharedData.Slots.FirstOrDefault(s => s.Id == booking.SlotId);
                if (slot != null)
                {
                    slot.Status = SlotStatus.Booked;
                }

                booking.Status = BookingStatus.Confirmed;
                booking.ConfirmedAt = DateTime.UtcNow;
                booking.TeacherNotes = request.TeacherNotes;
                booking.MeetingLink = request.MeetingLink;

                bool notificationSent = false;
                if (request.SendNotification)
                {
                    // Имитация отправки уведомления
                    notificationSent = SendNotification(booking, "confirmed");
                }

                var statistics = new BookingStatistics
                {
                    TotalBookings = SharedData.Bookings.Count(b => b.StudentId == booking.StudentId),
                    ConfirmedThisMonth = SharedData.Bookings.Count(b => 
                        b.StudentId == booking.StudentId && 
                        b.Status == BookingStatus.Confirmed &&
                        b.ConfirmedAt.HasValue &&
                        b.ConfirmedAt.Value.Month == DateTime.UtcNow.Month)
                };

                var response = new ConfirmBookingResponse
                {
                    BookingId = booking.Id,
                    Status = booking.Status,
                    ConfirmedAt = booking.ConfirmedAt.Value,
                    MeetingLink = booking.MeetingLink,
                    NotificationSent = notificationSent,
                    Statistics = statistics
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
        public IActionResult CancelBooking(string bookingId, [FromBody] CancelBookingRequest request)
        {
            try
            {
                var booking = SharedData.Bookings.FirstOrDefault(b => b.Id == bookingId);
                if (booking == null)
                {
                    return NotFound("Бронирование не найдено");
                }

                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTime.UtcNow;
                booking.CancelledBy = request.CancelledBy.ToString();
                booking.Reason = request.Reason;

                // Освобождаем слот
                var slot = SharedData.Slots.FirstOrDefault(s => s.Id == booking.SlotId);
                var slotStatus = SlotStatus.Cancelled;
                if (slot != null)
                {
                    slot.Status = SlotStatus.Free;
                    slotStatus = slot.Status;
                }

                bool notificationSent = false;
                if (request.NotifyOtherParty)
                {
                    notificationSent = SendNotification(booking, "cancelled");
                }

                var response = new CancelBookingResponse
                {
                    BookingId = booking.Id,
                    Status = booking.Status,
                    CancelledAt = booking.CancelledAt.Value,
                    CancelledBy = booking.CancelledBy,
                    SlotStatus = slotStatus,
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
        [HttpGet("teachers/{teacherId}/bookings")]
        public IActionResult GetTeacherBookings(
            string teacherId,
            [FromQuery] BookingStatus? status = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                // Получаем слоты преподавателя
                var teacherSlots = SharedData.Slots.Where(s => s.TeacherId == teacherId).ToList();
                var slotIds = teacherSlots.Select(s => s.Id).ToList();

                // Получаем бронирования для этих слотов
                var bookingsQuery = SharedData.Bookings.Where(b => slotIds.Contains(b.SlotId));

                if (status.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Status == status.Value);
                }

                if (dateFrom.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.CreatedAt >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    bookingsQuery = bookingsQuery.Where(b => b.CreatedAt <= dateTo.Value);
                }

                var teacherBookings = bookingsQuery.Select(b =>
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
                        ContactInfo = b.ContactInfo
                    };
                }).ToList();

                var summary = new BookingsSummary
                {
                    Total = teacherBookings.Count,
                    Pending = teacherBookings.Count(b => b.Status == BookingStatus.Pending),
                    Confirmed = teacherBookings.Count(b => b.Status == BookingStatus.Confirmed),
                    Cancelled = teacherBookings.Count(b => b.Status == BookingStatus.Cancelled),
                    Completed = teacherBookings.Count(b => b.Status == BookingStatus.Completed)
                };

                var response = new TeacherBookingsResponse
                {
                    Bookings = teacherBookings,
                    Summary = summary
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении бронирований преподавателя");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // Метод для отладки - получить все данные
        [HttpGet("debug")]
        public IActionResult GetDebugInfo()
        {
            return Ok(new 
            {
                TotalSlots = SharedData.Slots.Count,
                TotalBookings = SharedData.Bookings.Count,
                Slots = SharedData.Slots.Select(s => new 
                {
                    s.Id,
                    s.Status,
                    s.TeacherId,
                    s.StartTime,
                    s.EndTime
                }),
                Bookings = SharedData.Bookings.Select(b => new 
                {
                    b.Id,
                    b.SlotId,
                    b.Status,
                    b.StudentId
                })
            });
        }

        private bool SendNotification(Booking booking, string notificationType)
        {
            // Имитация отправки уведомления
            _logger.LogInformation($"Отправка уведомления {notificationType} для бронирования {booking.Id}");
            return true;
        }
    }
}