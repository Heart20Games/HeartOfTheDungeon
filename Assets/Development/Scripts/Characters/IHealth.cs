
public interface IHealth : IDamageReceiver
{
    public void SetHealthTotal(int amount);
    public void SetHealthBase(int total);
    public void SetHealthBase(int amount, int total);
    public void SetHealth(int amount);
    public void HealDamage(int amount);
}
