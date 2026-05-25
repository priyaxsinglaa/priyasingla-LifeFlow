using System;
using System.Collections.Generic;

namespace API.Models;

public partial class HealthProfile
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public double? Weight { get; set; }

    public string? BloodGroup { get; set; }

    public string? RhFactor { get; set; }

    public string? Diseases { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
