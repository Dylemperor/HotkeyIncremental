using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveDataWrapper
{
    public int saveVersion = 1;
    public List<LetterSaveData> letters = new List<LetterSaveData>();
    public List<NumberSaveData> numbers = new List<NumberSaveData>();
    public float automationSpeed;
    public int automationSpeedLevel;
    // Add time or other metadata if needed
}

[System.Serializable]
public class LetterSaveData
{
    public string letter;
    public double amount;
    public bool isUnlocked;
    public List<UpgradeSaveData> upgrades = new List<UpgradeSaveData>();
}

[System.Serializable]
public class NumberSaveData
{
    public int number;
    public double amount;
    public bool isUnlocked;
    public List<UpgradeSaveData> upgrades = new List<UpgradeSaveData>();
}

[System.Serializable]
public class UpgradeSaveData
{
    public string id;
    public int level;
    public double effect;
    public double cost;
}
