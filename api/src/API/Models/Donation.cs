using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Donation
{
    public int Id { get; set; }

    public string BloodType { get; set; } = null!;

    public int Units { get; set; }

    public DateTime DonationDate { get; set; }

    public string? DonorName { get; set; }

    public string? Contact { get; set; }

    public string Hospital { get; set; } = null!;

    public string? Notes { get; set; }
}
