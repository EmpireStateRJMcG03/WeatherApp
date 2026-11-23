namespace WeatherStation2.Models;

public class PlannedActivity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Today;
    public string LocationName { get; set; } = "";
    public bool IsDone { get; set; }
}
