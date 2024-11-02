
/*
 * These interfaces define the shape of spectable and player controlled objects.
 */

public interface ISpectatable
{
    public void SetSpectatable(bool spectable);
}

public interface IControllable: ISpectatable
{
    public bool PlayerControlled { get; set; }
    public void SetPlayerControlled(bool playerControlled, bool ignoreDeath=false);
}