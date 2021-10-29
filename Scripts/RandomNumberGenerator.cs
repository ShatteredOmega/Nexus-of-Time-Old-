using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Need to create 1d6 for Quarry mine
/// Need to create 1d18 for Sanctuary heal
/// </summary>

public class RandomNumberGenerator : MonoBehaviour
{

    public int[] diceResult;
    int i;
    public int Result;
    public int numberOfDice;
    public int diceMax;
    public int missOn;
    public bool isChip = false;
    public bool isHit = false;
    public bool armorPierce = false;
    public bool trueDamage = false;


    public void mineRoll(int numDice)
    {
        i = 0;
        diceResult = new int[numDice];
        Result = 0;
        do
        {
            int Roll = Random.Range(1, 3);
            Result += Roll;
            diceResult[i] = Roll;
            numDice -= 1;
            i++;
            Debug.Log("You Rolled a: " + Roll);
        }
        while (numDice >= 1);
        Result = Result * 50;
    }

    public void healRoll(int numDice)
    {
        i = 0;
        Result = 0;
        diceResult = new int[numDice];
        do 
        {
            int Roll = Random.Range(1, 7);
            Result += Roll;
            diceResult[i] = Roll;
            numDice -= 1;
            i++;
            Debug.Log("You Rolled a: " + Roll);
        }
        while (numDice >= 1);
    }
    public void braceRoll(PlayableCharacter Defender, int numDice)
    {
        i = 0;
        diceResult = new int[numDice];
        Result = 0;
        do
        {
            int Roll = Random.Range(1, 3);
            Result += Roll;
            diceResult[i] = Roll;
            numDice -= 1;
            i++;
            Debug.Log("You Rolled a: " + Roll);
        }
        while (numDice >= 1);
    }

    //public void Damage_Roll(PlayableCharacter Attacker, int Dmax, int nDie, int APierce, int MissOn)
    //^^Alternative Call
    public void Damage_Roll()
    {
        armorPierce = false;
        isChip = false;
        isHit = false;
        trueDamage = false;
        Result = 0;
        diceResult = new int[numberOfDice];
        i = 0;
        int numDie = numberOfDice;
        do
        {
            int Roll = Random.Range(1, (diceMax + 1));
            Result += Roll;
            diceResult[i] = Roll;
            numDie -= 1;
            i++;
            Debug.Log("You Rolled a: " + Roll);

        }
        while (numDie >= 1);
    }

    public void Roulette(PC_LadyLuck LadyLuck)
    {
        Result = 0;
        diceResult = new int[numberOfDice];
        i = 0;
        int numDie = 9;
        if (LadyLuck.firstChamber)
        {
            numDie--;
        }
        if (LadyLuck.secondChamber)
        {
            numDie--;
        }
        if (LadyLuck.thirdChamber)
        {
            numDie--;
        }
        if (LadyLuck.fourthChamber)
        {
            numDie--;
        }
        if (LadyLuck.fifthChamber)
        {
            numDie--;
        }
        if (LadyLuck.sixthChamber)
        {
            numDie--;
        }
        if (LadyLuck.seventhChamber)
        {
            numDie--;
        }
        if (LadyLuck.eighthChamber)
        {
            numDie--;
        }
        int minRoll = numDie;
        int RouletteRoll = Random.Range(1, (9));
        if ((RouletteRoll == 1 && LadyLuck.firstChamber) || (RouletteRoll == 2 && LadyLuck.secondChamber) || (RouletteRoll == 3 && LadyLuck.thirdChamber) || (RouletteRoll == 4 && LadyLuck.fourthChamber) || (RouletteRoll == 5 && LadyLuck.fifthChamber) || (RouletteRoll == 6 && LadyLuck.sixthChamber) || (RouletteRoll == 7 && LadyLuck.seventhChamber) || (RouletteRoll == 8 && LadyLuck.eighthChamber))
        {
            do
            {
                int Roll = Random.Range(1, (9));
                if (Roll > (minRoll))
                {
                    Result += Roll;
                    diceResult[i] = Roll;
                }
                else
                {
                    Result += minRoll;
                    diceResult[i] = minRoll;
                }
                numDie -= 1;
                i++;
                Debug.Log("You Rolled a: " + Roll);
            }
            while (numDie >= 1);
        }
        else
        {
            Result = 0;
            LadyLuck.MissFortune = true;
            Debug.Log("Miss!");
        }
    }
}
