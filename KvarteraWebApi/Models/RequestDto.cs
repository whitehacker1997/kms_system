namespace KvarteraWebApi.Models;

public class RequestDto
{
    public List<MonthlyOutcome> Outcomes { get; set; } = new();
}
