namespace Johnson.Common;

public enum DbChangeType
{
    Create,
    Update,
    Delete,
}

public enum EventOutcome
{
    Success,
    Failure,
    CriticalFailure,
}

public enum FailureType
{
    Error,
    CriticalError,
    FatalError,
}
