using System;
using System.Collections.Generic;

namespace API.Models;

public partial class ShortageAlert
{
    public int Id { get; set; }

    public string Severity { get; set; } = null!;

    public string BloodType { get; set; } = null!;

    public string Hospital { get; set; } = null!;

    public int Units { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }
}
