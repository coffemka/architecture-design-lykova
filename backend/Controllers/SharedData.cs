using ConsultationAPI.Models;
using ConsultationAPI.Enums;

namespace ConsultationAPI.Data
{
    public static class SharedData
    {
        public static List<Slot> Slots { get; } = new List<Slot>();
        public static List<Booking> Bookings { get; } = new List<Booking>();
        
        public static void Initialize()
        {
            if (!Slots.Any())
            {
                Slots.AddRange(new[]
                {
                    new Slot
                    {
                        Id = Guid.NewGuid().ToString(),
                        TeacherId = "teacher1",
                        TeacherName = "Иван Петров",
                        StartTime = DateTime.UtcNow.AddDays(1).AddHours(10),
                        EndTime = DateTime.UtcNow.AddDays(1).AddHours(11),
                        MeetingType = MeetingType.Online,
                        Description = "Консультация по математике",
                        Status = SlotStatus.Free
                    },
                    new Slot
                    {
                        Id = Guid.NewGuid().ToString(),
                        TeacherId = "teacher2",
                        TeacherName = "Мария Сидорова",
                        StartTime = DateTime.UtcNow.AddDays(1).AddHours(14),
                        EndTime = DateTime.UtcNow.AddDays(1).AddHours(15),
                        MeetingType = MeetingType.Offline,
                        Description = "Консультация по физике",
                        Status = SlotStatus.Free
                    }
                });
            }
        }
    }
}