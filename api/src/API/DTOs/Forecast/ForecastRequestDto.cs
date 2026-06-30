using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Forecast;

public class ForecastRequestDto
{
    [Required]
    public string BloodType { get; set; } = null!;

    [Required]
    public string Hospital { get; set; } = null!;

    [Range(1, 90)]
    public int DaysAhead { get; set; } = 7;
}