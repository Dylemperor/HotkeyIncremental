

public class UpgradeData
{
    public string name;
    public int level;
    public double effect;
    public double cost;
    public double costScaleMult;

    public UpgradeData(string name, int level, double effect, double cost, double costScale)
    {
        this.name = name;
        this.level = level;
        this.effect = effect;
        this.cost = cost;
        this.costScaleMult = costScale;
    }
    public void Upgrade()
    {
        level++;
        effect *= 1.5;
        cost *= costScaleMult;
    }
}