using ConsultationAPI.Enums;

namespace ConsultationAPI.Models
{
    public class Slot
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MeetingType MeetingType { get; set; }
        public string? Description { get; set; }
        public SlotStatus Status { get; set; } = SlotStatus.Free;
        public Dictionary<string, object>? Metadata { get; set; }
        public string? PublicUrl { get; set; }
        public string? CalendarEventId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateSlotRequest
    {
        public required string TeacherId { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public required MeetingType MeetingType { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class CreateSlotResponse
    {
        public required string Id { get; set; }
        public required SlotStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public required string PublicUrl { get; set; }
        public required string CalendarEventId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AvailableSlotsResponse
    {
        public required List<AvailableSlot> Slots { get; set; }
    }

    public class AvailableSlot
    {
        public required string Id { get; set; }
        public required string TeacherId { get; set; }
        public required string TeacherName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MeetingType MeetingType { get; set; }
        public string? Description { get; set; }
    }
}