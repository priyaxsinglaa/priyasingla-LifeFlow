using System;
using System.Collections.Generic;

namespace API.Models;

public partial class DemandForecast
{
    public int Id { get; set; }

    public string BloodType { get; set; } = null!;

    public string Hospital { get; set; } = null!;

    public DateTime ForecastDate { get; set; }

    public int PredictedUnits { get; set; }
}
