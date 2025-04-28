using Johnson.Common.Models.Base;

namespace Johnson.Common.Models.DataStorage;

public class RegistryServiceEntry : ServiceDetailsBase
{
    public Guid Id { get; set; }
    public required string URL { get; set; }
    public required string IP { get; set; }
    public int Port { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsHealthy { get; set; }
}
