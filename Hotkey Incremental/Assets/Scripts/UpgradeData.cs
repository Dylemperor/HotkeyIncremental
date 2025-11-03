

public class UpgradeData
{
    public string name;
    public int level;
    public double effect;
    public double cost;
    public double costScaleMult;
    
    // Effect scaling system
    public bool useAdditiveEffect = false;
    public double effectIncrement = 0; // Amount to add per level
    
    // Complex scaling system
    public bool useComplexScaling = false;
    public double baseCost;
    public int[] scalingPhases; // Level thresholds for different phases
    public double[] scalingMultipliers; // Cost multipliers for each phase

    public UpgradeData(string name, int level, double effect, double cost, double costScale)
    {
        this.name = name;
        this.level = level;
        this.effect = effect;
        this.cost = cost;
        this.costScaleMult = costScale;
        this.baseCost = cost;
    }
    
    // Constructor for complex scaling
    public UpgradeData(string name, int level, double effect, double baseCost, int[] phases, double[] multipliers)
    {
        this.name = name;
        this.level = level;
        this.effect = effect;
        this.cost = baseCost;
        this.baseCost = baseCost;
        this.useComplexScaling = true;
        this.scalingPhases = phases;
        this.scalingMultipliers = multipliers;
        
        // Calculate initial cost based on current level
        CalculateCostForLevel(level);
    }
    
    // Constructor for additive effects
    public UpgradeData(string name, int level, double effect, double cost, double costScale, bool additive, double increment)
    {
        this.name = name;
        this.level = level;
        this.effect = effect;
        this.cost = cost;
        this.costScaleMult = costScale;
        this.baseCost = cost;
        this.useAdditiveEffect = additive;
        this.effectIncrement = increment;
    }
    
    // Constructor for complex scaling with additive effects
    public UpgradeData(string name, int level, double effect, double baseCost, int[] phases, double[] multipliers, bool additive, double increment)
    {
        this.name = name;
        this.level = level;
        this.effect = effect;
        this.cost = baseCost;
        this.baseCost = baseCost;
        this.useComplexScaling = true;
        this.scalingPhases = phases;
        this.scalingMultipliers = multipliers;
        this.useAdditiveEffect = additive;
        this.effectIncrement = increment;
        
        // Calculate initial cost based on current level
        CalculateCostForLevel(level);
    }
    
    public void Upgrade()
    {
        level++;
        
        if (useAdditiveEffect)
        {
            effect += effectIncrement;
        }
        else
        {
            effect *= 1.5;
        }
        
        if (useComplexScaling)
        {
            CalculateCostForLevel(level);
        }
        else
        {
            cost *= costScaleMult;
        }
    }
    
    private void CalculateCostForLevel(int targetLevel)
    {
        if (!useComplexScaling || scalingPhases == null || scalingMultipliers == null)
            return;
            
        double currentCost = baseCost;
        int currentLevel = 1;
        
        // Apply scaling for each level up to target level
        for (int i = 0; i < targetLevel - 1; i++)
        {
            double multiplier = GetMultiplierForLevel(currentLevel);
            currentCost *= multiplier;
            currentLevel++;
        }
        
        cost = currentCost;
    }
    
    private double GetMultiplierForLevel(int level)
    {
        if (scalingPhases == null || scalingMultipliers == null)
            return costScaleMult;
            
        for (int i = 0; i < scalingPhases.Length; i++)
        {
            if (level <= scalingPhases[i])
            {
                return scalingMultipliers[i];
            }
        }
        
        // If level exceeds all phases, use the last multiplier
        return scalingMultipliers[scalingMultipliers.Length - 1];
    }
} 