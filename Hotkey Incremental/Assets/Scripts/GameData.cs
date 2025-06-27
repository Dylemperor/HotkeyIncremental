using System;
using System.Collections.Generic;
using UnityEngine;
public class GameData
{
    public Dictionary<string, LetterData> letters = new Dictionary<string, LetterData>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>();

    public void InitializeDefaults()
    {
        letters["A"] = new LetterData("A");
        letters["B"] = new LetterData("B");
        // all letters add later


    }
}
