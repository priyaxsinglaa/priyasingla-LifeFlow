namespace API.DTOs.BloodStock;

public class BloodStockDto
{
    public string BloodType { get; set; } = null!;
    public int AvailableUnits { get; set; }
    public int PredictedDemand { get; set; }
    public int SupplyLevel { get; set; }
    public string Status { get; set; } = null!;
}

public class UpdateBloodStockDto
{
    public int AvailableUnits { get; set; }
    public int PredictedDemand { get; set; }
}