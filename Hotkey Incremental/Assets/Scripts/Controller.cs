using System;
using UnityEngine;
public class Controller
{
    GameData gameData = new GameData();
     

    void Update()
    {
        double deltaTime = Time.deltaTime;

        foreach (var pair in gameData.letters)
        {
            string letter = pair.Key;
            LetterData letterA = pair.Value;

            double production = deltaTime * Math.Pow((letterA.GetMultiplier()), letterA.GetExponent());
            letterA.amount += production;        
        }     
    }
}
