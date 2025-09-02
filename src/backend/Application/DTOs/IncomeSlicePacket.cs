using System.Timers;

namespace Example.Application.DTOs;

public struct IncomeSlicePacket
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public List<int> Slice { get; set; }
}
