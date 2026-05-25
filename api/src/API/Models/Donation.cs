using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Donation
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime? DonationDate { get; set; }

    public string? Location { get; set; }

    public string? Notes { get; set; }

    public virtual User? User { get; set; }
}
