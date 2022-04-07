namespace Helther.Shared.Entity;

public class Check
{
    public int Id { get; set; }
    public Service Service { get; set; }
    public int Status { get; set; }
    public DateTime CheckDateTime { get; set; }
}