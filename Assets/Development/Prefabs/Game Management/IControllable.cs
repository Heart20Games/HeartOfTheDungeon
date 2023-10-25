
public interface IDisplayable
{
    public void SetDisplayable(bool displayable);
}

public interface IControllable: IDisplayable
{
    public void SetControllable(bool controllable);
}

public interface IBrainable: IControllable
{
    public void SetBrainable(bool brainable);
}
