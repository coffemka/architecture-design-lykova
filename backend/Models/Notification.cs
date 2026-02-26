using ConsultationAPI.Enums;

namespace ConsultationAPI.Models
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BookingId { get; set; } = string.Empty;
        public List<NotificationRecipient> Recipients { get; set; } = new();
        public string? CustomMessage { get; set; }
        public NotificationType Type { get; set; }
        public DateTime SentAt { get; set; }
    }

    public enum NotificationType
    {
        Reminder,
        Confirmation,
        Cancellation
    }

    public class SendReminderRequest
    {
        public string BookingId { get; set; } = string.Empty;
        public List<NotificationRecipient> Recipients { get; set; } = new();
        public string? CustomMessage { get; set; }
    }

    public class SendReminderResponse
    {
        public string NotificationId { get; set; } = string.Empty;
        public Dictionary<string, bool> SentTo { get; set; } = new();
        public DateTime SentAt { get; set; }
        public DateTime? NextReminder { get; set; }
    }

    public class NotificationHistoryResponse
    {
        public List<NotificationHistoryItem> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class NotificationHistoryItem
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public List<string> Recipients { get; set; } = new();
    }
}