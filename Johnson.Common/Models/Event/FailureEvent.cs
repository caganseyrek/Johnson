using Johnson.Common.Models.Base;

namespace Johnson.Common.Models.Event;

public class FailureEvent : EventBase
{
    public FailureType FailureType { get; set; }
    public required string FailureMessage { get; set; }
}
