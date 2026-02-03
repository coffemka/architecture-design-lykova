using ConsultationAPI.Models;

namespace ConsultationAPI.Data
{
    public static class SharedData
    {
        public static readonly List<Slot> Slots = new();
        public static readonly List<Booking> Bookings = new();
    }
}