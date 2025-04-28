using Johnson.Common.Models.Base;

namespace Johnson.Common.Models.DataStorage;

public class EventAuditLog : EventBase
{
    public string? EventName { get; set; }
    public EventOutcome Outcome { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime LoggedAt { get; set; }
}
