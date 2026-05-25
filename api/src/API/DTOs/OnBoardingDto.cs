using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class OnBoardingDto
{
        public required float Weight { get; set; }
        [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "Invalid Blood Group")]
        public required string BloodGroup { get; set; }
        public string? RhFactor { get; set; }
        [MaxLength(2000, ErrorMessage = "Diseases description cannot exceed 2000 characters.")]
        public string? Diseases { get; set; }
}