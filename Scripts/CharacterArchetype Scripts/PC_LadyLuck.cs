using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_LadyLuck : MonoBehaviour
{
    public DynamicMap lady_DynM;
    public PlayableCharacter lady_PC;
    public PC_LadyLuck lady;
    public bool firstChamber = false;
    public bool secondChamber = false;
    public bool thirdChamber = false;
    public bool fourthChamber = false;
    public bool fifthChamber = false;
    public bool sixthChamber = false;
    public bool seventhChamber = false;
    public bool eighthChamber = false;

    public bool MissFortune = false;

    public void initializeLadyLuck(PlayableCharacter playable, PC_LadyLuck ladyluck, DynamicMap M)
    {
        lady_PC = playable;
        lady = ladyluck;
        lady_DynM = M;
        lady_PC.AttackRange = 2;
        lady_PC.HealActionCost = 2;
        lady_PC.MAX_Health = 35;
        lady_PC.Health = 35;
        lady_PC.Armor = 1;
        lady_PC.HealthUpgradeCost = 100;
        lady_PC.DamageUpgradeCost = 200;
        lady_PC.ArmorUpgradeCost = 300;
    }


    public void LadyLuck_StartTurnConditions()
    {

    }
    public void LadyLuck_EndTurnConditions()
    {

    }

    public void RouletteCall(PlayableCharacter Target)
    {
        if (!(firstChamber && secondChamber && thirdChamber && fourthChamber && fifthChamber && sixthChamber && seventhChamber && eighthChamber))
        {
            Debug.Log("you havent selected any numbers");
        }
        else
        {
            lady_PC.RNGeezus.Roulette(lady);
            Target.Target_ArchetypeSwitch(Target, lady_PC);
            /*if(lady_PC.Bleeding)
            {
                lady_PC.bleedTick();
            }*/
            lady_PC.curseTick();
        }

    }

    public void LadyLuck_Attack(PlayableCharacter Victim)
    {
        if (lady_PC.ActionPoints >= lady_PC.AttackActionCost)
        {
            lady_PC.DamageBonus = lady_PC.DamageBonus + lady_PC.specialDamageBonus;
            //RNG Set
            lady_PC.RNGeezus.numberOfDice = 1;
            lady_PC.RNGeezus.diceMax = 8;
            //RNG call
            lady_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            if (lady_PC.RNGeezus.Result == 8 && !MissFortune)
            {
                lady_PC.RNGeezus.Result = 16;
            }
            lady_PC.RNGeezus.missOn = 2;
            lady_PC.RNGeezus.armorPierce = false;
            //Damage calculation
            Victim.Target_ArchetypeSwitch(Victim, lady_PC);
            if (lady_PC.RNGeezus.isHit)
            {
                lady_PC.playerMoney = lady_PC.playerMoney + 50;
                if (lady_PC.RNGeezus.Result == 8 && !MissFortune)
                {
                    lady_PC.playerMoney = lady_PC.playerMoney + 50;
                }
            }
            lady_PC.DamageBonus = lady_PC.DamageBonus - lady_PC.specialDamageBonus;
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }
    public void FirstChamber()
    {
        if (!firstChamber)
        {
            firstChamber = true;
        }
        else
        {
            firstChamber = false;
        }
    }
    public void SecondChamber()
    {
        if (!secondChamber)
        {
            secondChamber = true;
        }
        else
        {
            secondChamber = false;
        }
    }
    public void ThirdChamber()
    {
        if (!thirdChamber)
        {
            thirdChamber = true;
        }
        else
        {
            thirdChamber = false;
        }
    }
    public void FourthChamber()
    {
        if (!fourthChamber)
        {
            fourthChamber = true;
        }
        else
        {
            fourthChamber = false;
        }
    }
    public void FifthChamber()
    {
        if (!fifthChamber)
        {
            fifthChamber = true;
        }
        else
        {
            fifthChamber = false;
        }
    }
    public void SixthChamber()
    {
        if (!sixthChamber)
        {
            sixthChamber = true;
        }
        else
        {
            sixthChamber = false;
        }
    }
    public void SeventhChamber()
    {
        if (!seventhChamber)
        {
            seventhChamber = true;
        }
        else
        {
            seventhChamber = false;
        }
    }
    public void EighthChamber()
    {
        if (!eighthChamber)
        {
            eighthChamber = true;
        }
        else
        {
            eighthChamber = false;
        }
    }
}
