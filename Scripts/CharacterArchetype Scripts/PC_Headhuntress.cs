using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC_Headhuntress : MonoBehaviour
{
    public DynamicMap hHuntress_DynM;
    public PlayableCharacter hHuntress_PC;
    public PC_Headhuntress hHuntress;

    public Button DroneEnableButton;
    public Button DroneDisableButton;
    public List<Tile_Properties> droneFriendlyTiles;

    public bool droneMode = false;
    public Tile_Properties dronedTile;
    public PlayableCharacter dronedPlayer;
    public bool activeDrone = false;
    public int droneCost = 3;
    public int droneEnergy = 10;

    public bool sprinting = false;

    public bool focused = false;

    public bool overcharged = false;
    public int overloadCountdown = 0;
    public bool overloaded = false;
    public int overloadTurnsRemaining = 0;

    public int abilityCost = 2;

    public void initializeHeadhuntress(PlayableCharacter playable, PC_Headhuntress Headhuntress, DynamicMap M)
    {
        hHuntress_PC = playable;
        hHuntress = Headhuntress;
        hHuntress_DynM = M;
        hHuntress_PC.AttackActionCost = 3;
        hHuntress_PC.MAX_Health = 30;
        hHuntress_PC.Health = 30;
        hHuntress_PC.DamageBonus = 2;
        droneCost = 3;
        hHuntress_PC.AttackRange = 1000; //Careful we don't want her attacking people in bases
        activeDrone = true;
    }

    public void Headhuntress_StartTurnConditions()
    {
        DroneEnableButton.gameObject.SetActive(true);
        DroneDisableButton.gameObject.SetActive(false);
        if (dronedPlayer != null && dronedPlayer != hHuntress_PC)
        {
            droneEnergy = droneEnergy - 3;
            if (droneEnergy == 0)
            {
                activeDrone = false;
                dronedTile = dronedPlayer.currentTile;
                dronedTile.droneCrashed = true;
                dronedPlayer = null;
            }
        }
        if (dronedTile != null)
        {
            droneEnergy = droneEnergy - 1;
            if (droneEnergy == 1)
            {
                dronedTile = null;
                dronedPlayer = hHuntress_PC;
                hHuntress_PC.Droned = true;
                droneEnergy = 10;
            }

        }
    }
    public void Headhuntress_EndTurnConditions()
    {
        DroneEnableButton.gameObject.SetActive(false);
        DroneDisableButton.gameObject.SetActive(false);
    }

    public void disableDroneButtons()
    {
        if (hHuntress_PC.playerActionMenuMode || hHuntress_PC.attackMenuMode || hHuntress_PC.moveMenuMode)
        {
            DroneEnableButton.gameObject.SetActive(false);
            DroneDisableButton.gameObject.SetActive(false);
        }
    }
    public void enableDroneButtons()
    {
        if (((!hHuntress_PC.playerActionMenuMode) || (!hHuntress_PC.attackMenuMode) || (!hHuntress_PC.moveMenuMode)) && (!droneMode))
        {
            DroneEnableButton.gameObject.SetActive(true);
        }
        else if (((!hHuntress_PC.playerActionMenuMode) || (!hHuntress_PC.attackMenuMode) || (!hHuntress_PC.moveMenuMode)) && droneMode)
        {
            DroneDisableButton.gameObject.SetActive(true);
        }
    }

    void DroneSet()
    {
        for (int i = 0; i < hHuntress_DynM.gridY; i++)
        {
            for (int j = 0; j < hHuntress_DynM.gridX; j++)
            {
                if (!(hHuntress_DynM.XY_TilePropArray[j, i].tType == tileType.Inactive || hHuntress_DynM.XY_TilePropArray[j, i].tType == tileType.Base || hHuntress_DynM.XY_TilePropArray[j, i].tType == tileType.BaseWall))
                {
                    droneFriendlyTiles.Add(hHuntress_DynM.XY_TilePropArray[j, i]);
                    hHuntress_DynM.XY_TilePropArray[j, i].tempPlayableCharacter = hHuntress_PC;
                    hHuntress_DynM.XY_TilePropArray[j, i].Srend.color = new Color(0, 1, 0f, 0.7f); //Changes the color to visually distinguish what tiles you CAN select
                    hHuntress_DynM.XY_TilePropArray[j, i].coll2D.enabled = true;
                }
            }
        }
        for (int i = 0; i < hHuntress_PC.turnSys.PCs.Length; i++)
        {
            hHuntress_PC.turnSys.PCs[i].coll2D.enabled = false;
        }
    }

    public void DroneModeEnable() //Make buttons that trigger character abilities on visible on that character's turn
    {

        if (sprinting) //Perhaps moving the drone while sprinting might not be a bad idea.
        {
            Debug.Log("You can't do that, all power has been diverted to your legs.");
            return;
        }
        if (!activeDrone)
        {
            Debug.Log("Your drone's damaged, you need to retrieve it first");
            return;
        }
        if (hHuntress_PC.ActionPoints < droneCost)
        {
            Debug.Log("Not enough action points");
            return;
        }
        hHuntress_PC.Deselect();
        hHuntress_PC.AttackButton.gameObject.SetActive(false);
        hHuntress_PC.MoveButton.gameObject.SetActive(false);
        hHuntress_PC.EndTurnButton.gameObject.SetActive(false);
        hHuntress_PC.MineButton.gameObject.SetActive(false);
        hHuntress_PC.HealButton.gameObject.SetActive(false);
        hHuntress_PC.UpgradeButton.gameObject.SetActive(false);
        if (!droneMode && activeDrone)
        {
            droneMode = true;
            DroneDisableButton.gameObject.SetActive(true);
            DroneSet();
        }
        else if (!droneMode && !activeDrone)
        {
            droneMode = true;
            DroneDisableButton.gameObject.SetActive(true);
            for (int i = 0; i < droneFriendlyTiles.Count; i++)
            {
                dronedTile.Srend.color = new Color(0, 1f, 0f, 0.7f);
            }
        }
    }
    public void DroneModeDisable()
    {
        hHuntress_PC.AttackButton.gameObject.SetActive(true);
        hHuntress_PC.MoveButton.gameObject.SetActive(true);
        hHuntress_PC.EndTurnButton.gameObject.SetActive(true);
        hHuntress_PC.MineButton.gameObject.SetActive(true);
        hHuntress_PC.HealButton.gameObject.SetActive(true);
        hHuntress_PC.UpgradeButton.gameObject.SetActive(true);
        for (int i = 0; i < droneFriendlyTiles.Count; i++)
        {
            droneFriendlyTiles[i].tempPlayableCharacter = null;
            droneFriendlyTiles[i].Srend.color = droneFriendlyTiles[i].tileDefaultColor;
            droneFriendlyTiles[i].coll2D.enabled = false;
        }
        if (droneMode && activeDrone)
        {
            droneMode = false;
            DroneEnableButton.gameObject.SetActive(true);
            DroneDisableButton.gameObject.SetActive(false);
        }
        else if (droneMode && !activeDrone)
        {
            droneMode = false;
            DroneEnableButton.gameObject.SetActive(true);
            DroneDisableButton.gameObject.SetActive(false);
        }
        droneFriendlyTiles = new List<Tile_Properties>();
        for (int i = 0; i < hHuntress_PC.turnSys.PCs.Length; i++)
        {
            hHuntress_PC.turnSys.PCs[i].coll2D.enabled = true;
        }
    }

    public void DeployDrone(Tile_Properties Destination)
    {
        if (hHuntress_PC.Droned) //when DeployDrone() is called when the drone is ON you
        {
            hHuntress_PC.Droned = false;
            dronedTile = Destination;
            dronedTile.droneOnTile = true;
        }
        else if (dronedTile != null) //when DeployDrone() is called and the drone is ON a tile (i.e. not on you)
        {
            dronedTile.droneOnTile = false;
            Destination.droneOnTile = true;
            dronedTile = Destination;

        }
        else if (dronedPlayer != null) // when DeployDrone() is called and the drone is ON someone else
        {
            dronedPlayer.Droned = false;
            dronedPlayer = null;
            Destination.droneOnTile = true;
            dronedTile = Destination;
        }
        if (Destination.playerOnTile != null)
        {
            dronedTile = null;
            dronedPlayer = Destination.playerOnTile;
        }
        droneEnergy = 10;
        hHuntress_PC.ActionPoints = hHuntress_PC.ActionPoints - 3;
        //apply the following code:
        //dronedPlayer = Hhuntress_PC.turnSys.PCs[i];
        //dronedPlayer.Droned = true;

    }

    public void Sprint()
    {
        if (hHuntress_PC.Silenced)
        {
            Debug.Log("Now now, subject, didn't I tell you to be silent?");
            return;
        }
        if (overloaded)
        {
            Debug.Log("You can't do that, your systems are still overloaded.");
            return;
        }
        Debug.Log("Diverting all power to legs.");
        hHuntress_PC.AttackButton.gameObject.SetActive(false);
        hHuntress_PC.MoveButton.gameObject.SetActive(false);
        hHuntress_PC.MineButton.gameObject.SetActive(false);
        hHuntress_PC.HealButton.gameObject.SetActive(false);
        hHuntress_PC.UpgradeButton.gameObject.SetActive(false);
        disableDroneButtons();
        hHuntress_PC.ActionPoints = 10;
    }
    //If this buff expires at the end of the turn, only make it available during attacking, and a first turn action
    //If this buff expires when moving, then put code in Move to do so
    public void Focus()
    {
        if (hHuntress_PC.Silenced)
        {
            Debug.Log("Now now, subject, didn't I tell you to be silent?");
        }
        if (overloaded)
        {
            Debug.Log("You can't do that, your systems are still overloaded.");
            return;
        }
        if (sprinting)
        {
            Debug.Log("You can't do that, all power has been diverted to your legs.");
            return;
        }
        Debug.Log("Scanning armor for weak points.");
        focused = true;
    }

    public void Overcharge()
    {
        if (hHuntress_PC.Silenced)
        {
            Debug.Log("Now now, subject, didn't I tell you to be silent?");
            return;
        }
        if (overloaded)
        {
            Debug.Log("You can't do that, your systems are still overloaded.");
            return;
        }
        if (sprinting)
        {
            Debug.Log("You can't do that, all power has been diverted to your legs.");
            return;
        }
        overcharged = true;
    }

    public void Headhuntress_Attack(PlayableCharacter Victim)
    {
        if (overloaded)
        {
            Debug.Log("You can't do that, your systems are still overloaded.");
            return;
        }
        if (sprinting)
        {
            Debug.Log("You can't do that, all power has been diverted to your legs.");
            return;
        }
        if (hHuntress_PC.ActionPoints >= hHuntress_PC.AttackActionCost)
        {
            int armorStore = Victim.Armor;
            int specialArmorStore = Victim.specialArmor;
            hHuntress_PC.DamageBonus = hHuntress_PC.DamageBonus + hHuntress_PC.specialDamageBonus;
            //RNG Set
            hHuntress_PC.RNGeezus.numberOfDice = 4;
            hHuntress_PC.RNGeezus.diceMax = 6;
            //RNG call
            hHuntress_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            hHuntress_PC.RNGeezus.missOn = 2;
            if (focused)
            {
                hHuntress_PC.RNGeezus.armorPierce = true;
                if (Victim.currentTile.Sightline)
                {
                    hHuntress_PC.RNGeezus.trueDamage = true;
                }
            }
            if (overcharged)
            {
                hHuntress_PC.RNGeezus.Result = hHuntress_PC.RNGeezus.Result * 2;
                Victim.Armor = Victim.Armor * 2;
                Victim.specialArmor = Victim.specialArmor * 2;
            }
            if (Victim.Droned && Victim.currentTile.Sightline)
            {
                hHuntress_PC.RNGeezus.Result = hHuntress_PC.RNGeezus.Result * 2;
            }
            if (Victim.Punctured)
            {
                hHuntress_PC.RNGeezus.armorPierce = true;
                if (focused)
                {
                    hHuntress_PC.RNGeezus.trueDamage = true;
                    {
                        if (Victim.currentTile.Sightline)
                        {
                            Victim.Armor = 0;
                            Victim.specialArmor = 0;
                        }
                    }
                }
            }
            //Damage calculation
            Victim.Target_ArchetypeSwitch(Victim, hHuntress_PC);
            hHuntress_PC.DamageBonus = hHuntress_PC.DamageBonus - hHuntress_PC.specialDamageBonus;
            if (hHuntress_PC.RNGeezus.isHit && focused)
            {
                Victim.Punctured = true;
            }
            Victim.Armor = armorStore;
            Victim.specialArmor = specialArmorStore;
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }
}



