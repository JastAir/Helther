namespace Helther.Shared.Entity;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int Status { get; set; }
    public int RateInSec { get; set; }
    public DateTime LastUpdateDateTime { get; set; }
    public ICollection<Check> History { get; set; } = new List<Check>();
}