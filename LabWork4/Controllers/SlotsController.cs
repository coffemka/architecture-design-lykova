using Microsoft.AspNetCore.Mvc;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;
using ConsultationAPI.Data; 

namespace ConsultationAPI.Controllers
{
    [ApiController]
    [Route("api/slots")]
    public class SlotsController : ControllerBase
    {
        // private static readonly List<Slot> _slots = new();
        private readonly ILogger<SlotsController> _logger;

        public SlotsController(ILogger<SlotsController> logger)
        {
            _logger = logger;
        }

        // 1.1. Создание нового слота
        [HttpPost]
        public IActionResult CreateSlot([FromBody] CreateSlotRequest request)
        {
            try
            {
                // Валидация
                if (request.StartTime >= request.EndTime)
                {
                    return BadRequest("EndTime должен быть позже StartTime");
                }

                // Проверка конфликта времени (теперь используем SharedData.Slots)
                var hasConflict = SharedData.Slots.Any(s => 
                    s.TeacherId == request.TeacherId &&
                    s.Status != SlotStatus.Cancelled &&
                    request.StartTime < s.EndTime && 
                    request.EndTime > s.StartTime);

                if (hasConflict)
                {
                    return BadRequest("Слот пересекается с существующим слотом");
                }

                // Имитация интеграции с календарем
                var calendarEventId = Guid.NewGuid().ToString();
                var publicUrl = $"https://calendar.yandex.ru/event/{calendarEventId}";

                var slot = new Slot
                {
                    TeacherId = request.TeacherId,
                    TeacherName = $"Преподаватель {request.TeacherId}", // Временно
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    MeetingType = request.MeetingType,
                    Description = request.Description,
                    Metadata = request.Metadata,
                    Status = SlotStatus.Free,
                    PublicUrl = publicUrl,
                    CalendarEventId = calendarEventId,
                    CreatedAt = DateTime.UtcNow
                };

                // Добавляем в общий список
                SharedData.Slots.Add(slot);

                var response = new CreateSlotResponse
                {
                    Id = slot.Id,
                    Status = slot.Status,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    PublicUrl = slot.PublicUrl,
                    CalendarEventId = slot.CalendarEventId,
                    CreatedAt = slot.CreatedAt
                };

                return CreatedAtAction(nameof(CreateSlot), response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании слота");
                return StatusCode(503, "Ошибка интеграции с календарем");
            }
        }

        // 1.2. Получение списка доступных слотов
        [HttpGet("available")]
        public IActionResult GetAvailableSlots(
            [FromQuery] string? teacherId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var query = SharedData.Slots.Where(s => s.Status == SlotStatus.Free);

                if (!string.IsNullOrEmpty(teacherId))
                {
                    query = query.Where(s => s.TeacherId == teacherId);
                }

                if (dateFrom.HasValue)
                {
                    query = query.Where(s => s.StartTime >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    query = query.Where(s => s.EndTime <= dateTo.Value);
                }

                var availableSlots = query.Select(s => new AvailableSlot
                {
                    Id = s.Id,
                    TeacherId = s.TeacherId,
                    TeacherName = s.TeacherName,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MeetingType = s.MeetingType,
                    Description = s.Description
                }).ToList();

                var response = new AvailableSlotsResponse
                {
                    Slots = availableSlots
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении слотов");
                return StatusCode(500, "Ошибка базы данных");
            }
        }

        // 1.3. Удаление слота
        [HttpDelete("{slotId}")]
        public IActionResult DeleteSlot(string slotId)
        {
            try
            {
                var slot = SharedData.Slots.FirstOrDefault(s => s.Id == slotId);
                if (slot == null)
                {
                    return NotFound("Слот не найден");
                }

                // Проверка прав
                // if (slot.TeacherId != currentUserId) return Forbid();

                // Имитация удаления из календаря
                try
                {
                    // calendarService.DeleteEvent(slot.CalendarEventId);
                }
                catch
                {
                    return StatusCode(503, "Ошибка при удалении события из календаря");
                }

                slot.Status = SlotStatus.Cancelled;
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении слота");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // Метод для отладки - получить все слоты
        [HttpGet("debug")]
        public IActionResult GetDebugSlots()
        {
            return Ok(new 
            {
                TotalSlots = SharedData.Slots.Count,
                Slots = SharedData.Slots.Select(s => new 
                {
                    s.Id,
                    s.TeacherId,
                    s.TeacherName,
                    s.StartTime,
                    s.EndTime,
                    s.Status,
                    s.MeetingType
                })
            });
        }
    }
}