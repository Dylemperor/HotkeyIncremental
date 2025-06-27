

using System;

public class GeneratorData
{
    public int count;
    public double baseProduction;
    public double productionMultiplier;
    public double productionExponent;

    public GeneratorData(int count, double baseProd, double multi, double exponent)
    {
        this.count = count;
        this.baseProduction = baseProd;
        this.productionMultiplier = multi;
        this.productionExponent = exponent;
    }
    public double GetProduction()
    {
        return Math.Pow((count * baseProduction * productionMultiplier), productionExponent);
    }
}