namespace Example.Domain.Entities
{
    public struct ProcessedPacket
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public List<int> Slice { get; set; }
        public int Max { get; set; }
        public string? Status { get; set; }
    }
}
