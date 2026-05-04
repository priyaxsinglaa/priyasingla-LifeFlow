using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string FirstName { get; set; }
        [MaxLength(50)]
        public required string LastName { get; set; }
        public required DateTime DOB { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public required string PhoneNumber { get; set; }
        public required string EmergencyContact { get; set; }

        public string? MedicalBaseline { get; set; }

        [Range(40, 200)]
        public double Weight { get; set; }
        [MaxLength(3)]
        public required string BloodGroup { get; set; }

        public required string Password { get; set; }

        public string Role { get; set; } = "User"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
}
