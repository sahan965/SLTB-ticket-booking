using System.ComponentModel.DataAnnotations;

namespace bus.Models
{
    public class BookingDetails
    {
        [Key]
        public int BookingId { get; set; } // New primary key
        public string IdNumber { get; set; } // ID number
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime BookingDate { get; set; }
        public List<int> SelectedSeats { get; set; }
    }
}
