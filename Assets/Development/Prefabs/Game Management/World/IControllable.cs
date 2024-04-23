
public interface ISpectatable
{
    public void SetSpectatable(bool spectable);
}

public interface IControllable: ISpectatable
{
    public bool Controllable { get; set; }
}