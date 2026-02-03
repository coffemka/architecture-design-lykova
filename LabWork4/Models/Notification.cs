using ConsultationAPI.Enums;

namespace ConsultationAPI.Models
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string BookingId { get; set; }
        public List<NotificationRecipient> Recipients { get; set; } = new();
        public string? CustomMessage { get; set; }
        public NotificationType Type { get; set; }
        public string? RecipientId { get; set; }
        public string? RecipientType { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public NotificationDeliveryStatus DeliveryStatus { get; set; } = NotificationDeliveryStatus.Pending;
    }

    public enum NotificationType
    {
        Reminder,
        Confirmation,
        Cancellation,
        Update
    }

    public enum NotificationDeliveryStatus
    {
        Pending,
        Sent,
        Delivered,
        Failed
    }

    public class SendReminderRequest
    {
        public required string BookingId { get; set; }
        public required List<NotificationRecipient> Recipients { get; set; }
        public string? CustomMessage { get; set; }
    }

    public class SendReminderResponse
    {
        public required string NotificationId { get; set; }
        public required Dictionary<string, bool> SentTo { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? NextReminder { get; set; }
    }

    public class NotificationHistoryResponse
    {
        public required List<NotificationHistoryItem> Notifications { get; set; }
        public int TotalCount { get; set; }
    }

    public class NotificationHistoryItem
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required string RecipientId { get; set; }
        public required string RecipientType { get; set; }
        public required string Message { get; set; }
        public DateTime SentAt { get; set; }
        public required string DeliveryStatus { get; set; }
    }

    public enum NotificationRecipient
{
    Student,
    Teacher
}
}