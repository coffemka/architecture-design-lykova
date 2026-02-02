using ConsultationAPI.Enums;

namespace ConsultationAPI.Models
{
    public class Booking
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string SlotId { get; set; }
        public required string StudentId { get; set; }
        public required string StudentName { get; set; }
        public required string Topic { get; set; }
        public required ContactInfo ContactInfo { get; set; }
        public string? AdditionalNotes { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelledBy { get; set; }
        public string? Reason { get; set; }
        public string? TeacherNotes { get; set; }
        public string? MeetingLink { get; set; }
        public DateTime ConfirmationDeadline { get; set; }
    }

    public class ContactInfo
    {
        public string? TelegramId { get; set; }
        public string? Email { get; set; }
    }

    public class CreateBookingRequest
    {
        public required string SlotId { get; set; }
        public required string StudentId { get; set; }
        public required string StudentName { get; set; }
        public required string Topic { get; set; }
        public required ContactInfo ContactInfo { get; set; }
        public string? AdditionalNotes { get; set; }
    }

    public class CreateBookingResponse
    {
        public required string BookingId { get; set; }
        public required BookingStatus Status { get; set; }
        public required string SlotId { get; set; }
        public required string StudentId { get; set; }
        public required string Topic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ConfirmationDeadline { get; set; }
    }

    public class ConfirmBookingRequest
    {
        public string? TeacherNotes { get; set; }
        public string? MeetingLink { get; set; }
        public bool SendNotification { get; set; } = true;
    }

    public class ConfirmBookingResponse
    {
        public required string BookingId { get; set; }
        public required BookingStatus Status { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public string? MeetingLink { get; set; }
        public bool NotificationSent { get; set; }
        public BookingStatistics? Statistics { get; set; }
    }

    public class CancelBookingRequest
    {
        public required CancelledBy CancelledBy { get; set; }
        public string? Reason { get; set; }
        public bool NotifyOtherParty { get; set; } = true;
    }

    public class CancelBookingResponse
    {
        public required string BookingId { get; set; }
        public required BookingStatus Status { get; set; }
        public DateTime CancelledAt { get; set; }
        public required string CancelledBy { get; set; }
        public required SlotStatus SlotStatus { get; set; }
        public bool NotificationSent { get; set; }
    }

    public class TeacherBookingsResponse
    {
        public required List<TeacherBooking> Bookings { get; set; }
        public required BookingsSummary Summary { get; set; }
    }

    public class TeacherBooking
    {
        public required string Id { get; set; }
        public required string SlotId { get; set; }
        public required string StudentName { get; set; }
        public required string Topic { get; set; }
        public required BookingStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public required ContactInfo ContactInfo { get; set; }
    }

    public class BookingsSummary
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
        public int Completed { get; set; }
    }

    public class BookingStatistics
    {
        public int TotalBookings { get; set; }
        public int ConfirmedThisMonth { get; set; }
    }
}