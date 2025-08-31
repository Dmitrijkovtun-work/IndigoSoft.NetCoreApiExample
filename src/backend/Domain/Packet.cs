using System.Timers;

namespace Example.Domain
{
    public struct Packet
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public List<int> Slice { get; set; }
    }
}
