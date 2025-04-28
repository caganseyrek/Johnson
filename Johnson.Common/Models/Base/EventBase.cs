namespace Johnson.Common.Models.Base;

public class EventBase
{
    public Guid Id { get; set; }
    public DateTime? EventPublishedAt { get; set; }
    public required string OriginService { get; set; }
}