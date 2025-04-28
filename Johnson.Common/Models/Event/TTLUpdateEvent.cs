using Johnson.Common.Models.Base;

namespace Johnson.Common.Models.Event;

public class TTLUpdateEvent : EventBase
{
    public Guid EffectedDataId { get; set; }
}