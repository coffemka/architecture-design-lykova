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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateSlotRequest
    {
        public string TeacherId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MeetingType MeetingType { get; set; }
        public string? Description { get; set; }
    }

    public class CreateSlotResponse
    {
        public string Id { get; set; } = string.Empty;
        public SlotStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AvailableSlot
    {
        public string Id { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MeetingType MeetingType { get; set; }
        public string? Description { get; set; }
    }

    public class AvailableSlotsResponse
    {
        public List<AvailableSlot> Slots { get; set; } = new();
    }
}