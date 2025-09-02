using Example.Domain.Entities;
namespace Example.Application.Services
{
    public class SliceProcessor
    {
        public Task<ProcessedPacket?> ProcessAsync( Packet packet)
        {
            return Task.FromResult<ProcessedPacket?>(new ProcessedPacket
            {
                Id = packet.Id,
                Timestamp = packet.Timestamp,
                Max = packet.Slice.Max(),
                Status = "Processed"
            });
        }
    }
}
