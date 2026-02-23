using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;
using ConsultationAPI.Data;

namespace ConsultationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SlotsController> _logger;

        public SlotsController(ApplicationDbContext context, ILogger<SlotsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/slots/available
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableSlots()
        {
            try
            {
                var availableSlots = await _context.Slots
                    .Where(s => s.Status == SlotStatus.Free)
                    .Select(s => new AvailableSlot
                    {
                        Id = s.Id,
                        TeacherId = s.TeacherId,
                        TeacherName = s.TeacherName,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        MeetingType = s.MeetingType,
                        Description = s.Description
                    })
                    .ToListAsync();

                return Ok(new AvailableSlotsResponse { Slots = availableSlots });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении слотов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // POST: api/slots
        [HttpPost]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotRequest request)
        {
            try
            {
                // Проверка корректности времени
                if (request.StartTime >= request.EndTime)
                {
                    return BadRequest("Время начала должно быть раньше времени окончания");
                }

                // Проверка конфликтов
                var hasConflict = await _context.Slots.AnyAsync(s =>
                    s.TeacherId == request.TeacherId &&
                    s.Status != SlotStatus.Cancelled &&
                    request.StartTime < s.EndTime &&
                    request.EndTime > s.StartTime);

                if (hasConflict)
                {
                    return BadRequest("Это время уже занято");
                }

                var slot = new Slot
                {
                    TeacherId = request.TeacherId,
                    TeacherName = $"Преподаватель {request.TeacherId}",
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    MeetingType = request.MeetingType,
                    Description = request.Description,
                    Status = SlotStatus.Free
                };

                _context.Slots.Add(slot);
                await _context.SaveChangesAsync();

                var response = new CreateSlotResponse
                {
                    Id = slot.Id,
                    Status = slot.Status,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    CreatedAt = slot.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании слота");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}