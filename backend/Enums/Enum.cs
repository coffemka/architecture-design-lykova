namespace ConsultationAPI.Enums
{
    public enum MeetingType
    {
        Online,
        Offline
    }

    public enum BookingStatus
    {
        Pending,      // Ожидает подтверждения
        Confirmed,    // Подтверждено
        Cancelled,    // Отменено
        Completed     // Завершено
    }

    public enum SlotStatus
    {
        Free,         // Свободно
        Pending,      // Ожидает подтверждения
        Booked,       // Занято
        Cancelled     // Отменено
    }

    public enum NotificationRecipient
    {
        Student,
        Teacher
    }

    public enum CancelledBy
    {
        Student,
        Teacher
    }
}