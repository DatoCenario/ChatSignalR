namespace WebApplication2.Handlers.Interfaces;

public interface IPaginationRequest
{
    public int Offset { get; set; }
    public int Count { get; set; }
}