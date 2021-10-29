using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Chieftain's thematic now includes native american myths/sprits
/// The bleed is now the 'Vulture curse', which has 3 levels, which increase with applications, any further applications increase capacity
/// melee attacks will rip the curse out of the player, and heal the Chieftain for however much damage the curse had done
/// </summary>

public class PC_Chieftain : MonoBehaviour
{
    public DynamicMap chief_DynM;
    public PlayableCharacter chief_PC;
    public PC_Chieftain chief;
    public int trapCountdown;
    public int trapsRemaining;
    public List<Tile_Properties> trappedTiles;
    public List<Tile_Properties> trappableTiles;
    public bool trapMode = false;
    public Text ButtonText;
    public Button TrapEnableButton;
    public Button TrapDisableButton;

    public void initializeChief(PlayableCharacter playable, PC_Chieftain chieftain, DynamicMap M)
    {
        chief_PC = playable;
        chief = chieftain;
        chief_DynM = M;
        trapsRemaining = 3;
        chief_PC.MAX_Health = 35;
        chief_PC.DamageBonus = 1;
        chief_PC.AttackRange = 3;
    }

    public void disableTrapButtons() {
        if (chief_PC.playerActionMenuMode || chief_PC.attackMenuMode || chief_PC.moveMenuMode) {
            TrapEnableButton.gameObject.SetActive(false);
            TrapDisableButton.gameObject.SetActive(false);
        }
    }
    public void enableTrapButtons()
    {
        if (((!chief_PC.playerActionMenuMode) || (!chief_PC.attackMenuMode) || (!chief_PC.moveMenuMode)) && (!trapMode))
        {
            TrapEnableButton.gameObject.SetActive(true);
        }
        else if (((!chief_PC.playerActionMenuMode) || (!chief_PC.attackMenuMode) || (!chief_PC.moveMenuMode)) && trapMode)
        {
            TrapDisableButton.gameObject.SetActive(true);
        }
    }

    public void Chieftain_StartTurnConditions()
    {
        TrapEnableButton.gameObject.SetActive(true);
        TrapDisableButton.gameObject.SetActive(false);
        if (trapCountdown != 0)
        {
            trapCountdown--;
            if (trapCountdown == 0)
            {
                trapExpire();
                trapsRemaining = 3;
            }
        }
    }

    public void Chieftain_EndTurnConditions()
    {
        TrapEnableButton.gameObject.SetActive(false);
        TrapDisableButton.gameObject.SetActive(false);
        if (trappedTiles.Count != 0)
        {
            trapsRemaining = 0;
            ButtonText.text = trapsRemaining.ToString();
        }
        if (trapMode)
        {
            chief_PC.Deselect();
        }
    }

    public void TrapModeEnable() //Make buttons that trigger character abilities on visible on that character's turn
    {
        chief_PC.Deselect();
        chief_PC.AttackButton.gameObject.SetActive(false);
        chief_PC.MoveButton.gameObject.SetActive(false);
        chief_PC.EndTurnButton.gameObject.SetActive(false);
        chief_PC.MineButton.gameObject.SetActive(false);
        chief_PC.HealButton.gameObject.SetActive(false);
        chief_PC.UpgradeButton.gameObject.SetActive(false);
        if (!trapMode && trapsRemaining != 0)
        {
            trapMode = true;
            TrapDisableButton.gameObject.SetActive(true);
            TrapSet();
            
        }
        else if (!trapMode && trapsRemaining == 0)
        {
            trapMode = true;
            TrapDisableButton.gameObject.SetActive(true);
            for (int i = 0; i < trappedTiles.Count; i++)
            {
                trappedTiles[i].Srend.color = new Color(0, 1f, 0f, 0.7f);
            }
        }
    }

    void TrapSet() {
        for (int i = 0; i < chief_DynM.gridY; i++)
        {
            for (int j = 0; j < chief_DynM.gridX; j++)
            {
                if (!(chief_DynM.XY_TilePropArray[j, i].tType == tileType.Inactive || chief_DynM.XY_TilePropArray[j, i].tType == tileType.Base || chief_DynM.XY_TilePropArray[j, i].tType == tileType.BaseWall || chief_DynM.XY_TilePropArray[j, i].tType == tileType.Highland || chief_DynM.XY_TilePropArray[j, i].tType == tileType.Mine))
                {
                    if (chief_DynM.XY_TilePropArray[j, i].rangeTo <= 3)
                    {
                        if (chief_DynM.XY_TilePropArray[j, i].whoIsOnTile == isOnTile.empty)
                        {
                            bool mineNeighbor = false;
                            if (chief_DynM.XY_TilePropArray[j, i].tType == tileType.Lowland)
                            {
                                for (int k = 0; k < 6; k++)
                                {
                                    if (chief_DynM.XY_TilePropArray[j, i].individualNeighborsList[k].tType == tileType.Mine)
                                    {
                                        mineNeighbor = true;
                                    }
                                }
                            }

                            if (!mineNeighbor && !chief_DynM.XY_TilePropArray[j, i].trapOnTile)
                            {
                                trappableTiles.Add(chief_DynM.XY_TilePropArray[j, i]);
                                chief_DynM.XY_TilePropArray[j, i].tempPlayableCharacter = chief_PC;
                                chief_DynM.XY_TilePropArray[j, i].Srend.color = new Color(0, 1, 0f, 0.7f); //Changes the color to visually distinguish what tiles you CAN select
                                chief_DynM.XY_TilePropArray[j, i].coll2D.enabled = true;
                            }

                        }

                    }
                }
            }
        }
    }

    public void TrapModeDisable() {
        for (int i = 0; i < trappableTiles.Count; i++)
        {
            trappableTiles[i].tempPlayableCharacter = null;
            trappableTiles[i].Srend.color = trappableTiles[i].tileDefaultColor;
            trappableTiles[i].coll2D.enabled = false;
        }
        if (trapMode && trapsRemaining != 0)
        {
            trapMode = false;
            TrapEnableButton.gameObject.SetActive(true);
            TrapDisableButton.gameObject.SetActive(false);
        }
        else if (trapMode && trapsRemaining == 0) {
            for (int i = 0; i < trappedTiles.Count; i++)
            {
                trappedTiles[i].Srend.color = trappedTiles[i].tileDefaultColor;
            }
            trapMode = false;
            TrapEnableButton.gameObject.SetActive(true);
            TrapDisableButton.gameObject.SetActive(false);
        }
        trappableTiles = new List<Tile_Properties>();
        chief_PC.AttackButton.gameObject.SetActive(true);
        chief_PC.MoveButton.gameObject.SetActive(true);
        chief_PC.EndTurnButton.gameObject.SetActive(true);
        chief_PC.MineButton.gameObject.SetActive(true);
        chief_PC.HealButton.gameObject.SetActive(true);
        chief_PC.UpgradeButton.gameObject.SetActive(true);
    }

    public void trapExpire()
    {
        for (int i = 0; i < trappedTiles.Count; i++)
        {
            trappedTiles[i].trapOnTile = false;
        }
        trapsRemaining = 3;
        //ButtonText.text = "Traps: " + trapsRemaining;
        // RUBEN: I changed this to match changes to the UI
        ButtonText.text = trapsRemaining.ToString();
        Debug.Log("Traps have expired");
    }

    public void Chieftain_Attack(PlayableCharacter Victim)
    {
        if (chief_PC.ActionPoints >= chief_PC.AttackActionCost)
        {
            chief_PC.DamageBonus = chief_PC.DamageBonus + chief_PC.specialDamageBonus;
            switch (Victim.currentTile.rangeTo)
            {
                case (1):
                    {
                        //RNG Set
                        chief_PC.RNGeezus.numberOfDice = 2;
                        chief_PC.RNGeezus.diceMax = 4;
                        chief_PC.RNGeezus.missOn = 2;
                        if (Victim.Snared)
                        {
                            chief_PC.RNGeezus.trueDamage = true;
                        }
                        else
                        {
                            chief_PC.RNGeezus.trueDamage = false;
                        }
                        break;
                    }
                case (2):
                    {
                        //RNG Set
                        chief_PC.RNGeezus.numberOfDice = 1;
                        chief_PC.RNGeezus.diceMax = 8;
                        chief_PC.RNGeezus.missOn = 2;
                        if (Victim.Snared)
                        {
                            chief_PC.RNGeezus.armorPierce = true;
                        }
                        else
                        {
                            chief_PC.RNGeezus.armorPierce = false;
                        }
                        break;
                    }
                case (3):
                    {
                        //RNG Set
                        chief_PC.RNGeezus.numberOfDice = 1;
                        chief_PC.RNGeezus.diceMax = 4;
                        chief_PC.RNGeezus.missOn = 1;
                        if (Victim.Snared)
                        {
                            chief_PC.RNGeezus.armorPierce = true;
                        }
                        else
                        {
                            chief_PC.RNGeezus.armorPierce = false;
                        }
                        break;
                    }
            }
            //RNG call
            chief_PC.RNGeezus.Damage_Roll();
            //Set attack properties
           
            int startHealth = Victim.Health;
            //Damage calculation
            chief_PC.Target_ArchetypeSwitch(Victim, chief_PC);
            if (chief_PC.RNGeezus.isHit)
            {
                if (Victim.currentTile.rangeTo == 1)
                {
                    if (Victim.Cursed)
                    {
                        chief_PC.overhealCheck(Victim.CurseStorage);
                        Victim.Cursed = false;
                        Victim.CurseLevel = 0;
                        Victim.CurseStorage = 0;
                    }
                    if (!chief_PC.RNGeezus.isChip)
                    {
                        if (!Victim.Bleeding)
                        {
                            Victim.Bleeding = true;
                            Debug.Log("Target is now Bleeding");
                        }
                        else
                        {
                            chief_PC.overhealCheck((Victim.BleedSeverity) / 2);
                            Debug.Log("The target's bleeding worsens");
                        }
                        Victim.BleedSeverity = Victim.BleedSeverity + 5;
                    }
                }
                else if(Victim.currentTile.rangeTo > 1)
                {
                    if (!chief_PC.RNGeezus.isChip)
                    {
                        if (!Victim.Cursed)
                        {
                            Victim.Cursed = true;
                            Victim.CurseStorage = 0;
                            Victim.CurseLevel = 1;
                            Debug.Log("You are cursed");
                        }
                        else
                        {
                            Victim.CurseLevel++;
                            Debug.Log("The Curse worsens");
                        }
                    }
                }
            }
            //Action point deduction
            chief_PC.DamageBonus = chief_PC.DamageBonus - chief_PC.specialDamageBonus;
            if (chief_PC.ActionPoints == 0)
            {
                chief_PC.Deselect();
            }
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }
}