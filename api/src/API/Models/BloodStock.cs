using System;
using System.Collections.Generic;

namespace API.Models;

public partial class BloodStock
{
    public string BloodType { get; set; } = null!;

    public int AvailableUnits { get; set; }

    public int PredictedDemand { get; set; }

    public int SupplyLevel { get; set; }

    public string Status { get; set; } = null!;
}
