using ConsultationAPI.Enums;

namespace ConsultationAPI.Models
{
    public class Booking
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SlotId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string? Email { get; set; }  // Просто строка, не отдельный класс
        public string? TelegramId { get; set; }  // Просто строка
        public string? AdditionalNotes { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelledBy { get; set; }
        public string? TeacherNotes { get; set; }
        public string? MeetingLink { get; set; }
    }

    public class CreateBookingRequest
    {
        public string SlotId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? TelegramId { get; set; }
        public string? AdditionalNotes { get; set; }
    }

    public class CreateBookingResponse
    {
        public string BookingId { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public string SlotId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ConfirmBookingRequest
    {
        public string? TeacherNotes { get; set; }
        public string? MeetingLink { get; set; }
        public bool SendNotification { get; set; } = true;
    }

    public class ConfirmBookingResponse
    {
        public string BookingId { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public string? MeetingLink { get; set; }
        public bool NotificationSent { get; set; }
    }

    public class CancelBookingRequest
    {
        public CancelledBy CancelledBy { get; set; }
        public string? Reason { get; set; }
        public bool NotifyOtherParty { get; set; } = true;
    }

    public class CancelBookingResponse
    {
        public string BookingId { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public DateTime CancelledAt { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public bool NotificationSent { get; set; }
    }

    public class TeacherBooking
    {
        public string Id { get; set; } = string.Empty;
        public string SlotId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Email { get; set; }
        public string? TelegramId { get; set; }
    }
}