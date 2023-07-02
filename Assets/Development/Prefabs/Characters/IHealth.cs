
public interface IHealth : IDamageable
{
    public void SetHealthTotal(int amount);
    public void SetHealthBase(int amount, int total);
    public void SetHealth(int amount);
    public void HealDamage(int amount);
}
