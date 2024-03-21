
public interface IDisplayable
{
    public void SetDisplayable(bool displayable);
}

public interface ISpectatable: IDisplayable
{
    public void SetSpectatable(bool spectable);
}

public interface IControllable: ISpectatable
{
    public void SetControllable(bool controllable);
}

public interface IBrainable: IControllable
{
    public void SetBrainable(bool brainable);
}
