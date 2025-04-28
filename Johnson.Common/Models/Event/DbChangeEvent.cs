using Johnson.Common.Models.Base;

namespace Johnson.Common.Models.Event;

public class DbChangeEvent : EventBase
{
    public DbChangeType ChangeType { get; set; }
    public Guid EffectedDataId { get; set; }
}
