
public interface ISpectatable
{
    public void SetSpectatable(bool spectable);
}

public interface IControllable: ISpectatable
{
    public bool Alive { get; }
    public bool Controllable { get; set; }
}