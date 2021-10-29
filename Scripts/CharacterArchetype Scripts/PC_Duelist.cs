using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum duelMode {defense, mobile, offense}

public class PC_Duelist : MonoBehaviour {
    public DynamicMap duel_DynM;
    public PlayableCharacter duel_PC;
    public PC_Duelist duel;
    public Tile_Properties attackDestination;
    public Tile_Properties destinationCurrTileProps;
    public bool selectingAttackDestination = false;
    public duelMode DuelMode;
    public Button Btn_ShiftD;
    public Button Btn_ShiftO;
    public Button Btn_ShiftM;
    public bool shifted = false;


    //make him roll a 1d4 for hitchance, and ladyluck 
    //put deflect/counter in TakeDamage, and set missOn to 0 in the function

    public void initializeDuelist(PlayableCharacter playable, PC_Duelist duelist, DynamicMap M)
    {
        duel_PC = playable;
        duel = duelist;
        duel_DynM = M;
        DuelMode = duelMode.defense;
        duel_PC.AttackRange = 1;
        duel_PC.MAX_Health = 40;
        duel_PC.Health = 40;
        duel_PC.HealthUpgradeCost = 150;
        duel_PC.DamageUpgradeCost = 250;
        duel_PC.ArmorUpgradeCost = 350;
    }

    public void ShiftInto_Offense() {
        duel_PC.AttackRange = 1;
        DuelMode = duelMode.offense;
        shifted = true;
        disableShiftButtons();
        //EnableRiposte 25%
    }

    public void ShiftInto_Defense() {
        duel_PC.AttackRange = 1;
        DuelMode = duelMode.defense;
        shifted = true;
        disableShiftButtons();
        //EnableRiposte 50%
    }

    public void ShiftInto_Mobile() {
        duel_PC.AttackRange = 2;
        DuelMode = duelMode.mobile;
        shifted = true;
        disableShiftButtons();
        //Disable riposte
    }

    public void Duelist_StartTurnConditions()
    {
        shifted = false;
        enableShiftButtons();
    }

    public void Duelist_EndTurnConditions()
    {
        Btn_ShiftD.gameObject.SetActive(false);
        Btn_ShiftM.gameObject.SetActive(false);
        Btn_ShiftO.gameObject.SetActive(false);
    }

    public void disableShiftButtons()
    {
        if ((duel_PC.playerActionMenuMode || duel_PC.attackMenuMode || duel_PC.moveMenuMode) || (shifted))
        {
            Btn_ShiftD.gameObject.SetActive(false);
            Btn_ShiftM.gameObject.SetActive(false);
            Btn_ShiftO.gameObject.SetActive(false);
        }
    }
    public void enableShiftButtons()
    {
        if (((!duel_PC.playerActionMenuMode) || (!duel_PC.attackMenuMode) || (!duel_PC.moveMenuMode)))
        {
            switch (DuelMode) {
                case duelMode.defense:
                    {
                        if (!shifted) {
                            Btn_ShiftM.gameObject.SetActive(true);
                            Btn_ShiftO.gameObject.SetActive(true);
                        }
                        break;
                    }
                case duelMode.offense:
                    {
                        if (!shifted)
                        {
                            Btn_ShiftM.gameObject.SetActive(true);
                            Btn_ShiftD.gameObject.SetActive(true);
                        }
                        break;
                    }
                case duelMode.mobile:
                    {
                        if (!shifted)
                        {
                            Btn_ShiftO.gameObject.SetActive(true);
                            Btn_ShiftD.gameObject.SetActive(true);
                        }
                        break;
                    }
            }
        }
    }

    public void duel_AgressorModeSwitch(PlayableCharacter Agro, PlayableCharacter Tgt) {
        switch (DuelMode)
        {
            case (duelMode.defense):
                {
                    Jab_Attack(Tgt);
                    break;
                }
            case (duelMode.mobile):
                {
                    if (Tgt.currentTile.rangeTo == 2)
                    {
                        int tempDir = Tgt.currentTile.relativeDirection;
                        if (duel_PC.currentTile.individualNeighborsList[tempDir].whoIsOnTile == isOnTile.empty)
                        {
                            Tile_Properties temp;
                            temp = duel.duel_PC.currentTile;
                            duel_PC.tileChosen = duel_PC.currentTile.individualNeighborsList[tempDir];
                            duel_PC.MoveFunctionPt1();
                            temp.distanceTo = int.MaxValue;
                            temp.Srend.color = temp.tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
                            temp.tempPlayableCharacter = null;
                            temp.Movable = true;
                            if (duel_PC.currentTile != null)
                            {
                                duel_PC.currentTile.Movable = false;
                            }
                            duel_PC.ActionPoints = duel_PC.ActionPoints + duel_PC.MovementCost;
                        }
                        Lunge_Attack(Tgt);
                    }
                    else {
                        if (attackDestination == null) {
                            attackDestination = duel_PC.currentTile;
                        }
                        Lunge_Attack(Tgt);
                    }
                    break;
                }
            case (duelMode.offense):
                {
                    if (attackDestination == null)
                    {
                        attackDestination = Tgt.currentTile;
                    }
                    Suplex_Attack(Tgt);
                    break;
                }
        }
    }

    public void Suplex_Attack(PlayableCharacter Victim)
    {
        if (duel_PC.ActionPoints >= duel_PC.AttackActionCost)
        {
            duel_PC.DamageBonus = duel_PC.DamageBonus + duel_PC.specialDamageBonus + Victim.Armor + Victim.specialArmor;
            //RNG Set
            duel_PC.RNGeezus.numberOfDice = 3;
            duel_PC.RNGeezus.diceMax = 4;
            //RNG call
            duel_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            duel_PC.RNGeezus.missOn = 0;
            duel_PC.RNGeezus.armorPierce = false;
            //Damage calculation
            Victim.Target_ArchetypeSwitch(Victim, duel_PC);
            duel_PC.DamageBonus = duel_PC.DamageBonus - (duel_PC.specialDamageBonus + Victim.Armor + Victim.specialArmor);
            if (duel_PC.RNGeezus.isHit)
            {
                Tile_Properties temp;
                temp = Victim.currentTile;
                Victim.tileChosen = attackDestination;
                Victim.MoveFunctionPt1();
                temp.distanceTo = int.MaxValue;
                temp.Srend.color = temp.tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
                temp.tempPlayableCharacter = null;
                temp.Movable = true;
                if (Victim.currentTile != null)
                {
                    Victim.currentTile.Movable = false;
                }
                Victim.ActionPoints = Victim.ActionPoints + Victim.MovementCost;
                PurgeDestinationNeighbors(destinationCurrTileProps);
                attackDestination.Srend.color = attackDestination.tileDefaultColor;
            }
            destinationCurrTileProps = null;
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    public void Lunge_Attack(PlayableCharacter Victim)
    {
        if (duel_PC.ActionPoints >= duel_PC.AttackActionCost)
        {
            duel_PC.DamageBonus = duel_PC.DamageBonus + duel_PC.specialDamageBonus;
            //RNG Set
            duel_PC.RNGeezus.numberOfDice = 1;
            duel_PC.RNGeezus.diceMax = 4;
            //RNG call
            duel_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            duel_PC.RNGeezus.missOn = 0;
            duel_PC.RNGeezus.armorPierce = false;
            //Damage calculation
            Victim.Target_ArchetypeSwitch(Victim, duel_PC);
            duel_PC.DamageBonus = duel_PC.DamageBonus - duel_PC.specialDamageBonus;
            if (duel_PC.RNGeezus.isHit)
            {
                duel_PC.ActionPoints++;
                if (!duel_PC.Snared) {
                    Tile_Properties temp;
                    temp = duel_PC.currentTile;
                    duel_PC.tileChosen = attackDestination;
                    duel_PC.MoveFunctionPt1();
                    temp.distanceTo = int.MaxValue;
                    temp.Srend.color = temp.tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
                    temp.tempPlayableCharacter = null;
                    temp.Movable = true;
                    if (duel_PC.currentTile != null)
                    {
                        duel_PC.currentTile.Movable = false;
                    }
                    duel_PC.ActionPoints = duel_PC.ActionPoints + duel_PC.MovementCost;
                    PurgeDestinationNeighbors(destinationCurrTileProps);
                    attackDestination.Srend.color = attackDestination.tileDefaultColor;
                }
                
            }
           
            destinationCurrTileProps = null;
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    public void Jab_Attack(PlayableCharacter Victim)
    {
        if (duel_PC.ActionPoints >= duel_PC.AttackActionCost)
        {
            int damageStore = duel_PC.DamageBonus;
            duel_PC.DamageBonus =  duel_PC.specialDamageBonus;
            //RNG Set
            duel_PC.RNGeezus.numberOfDice = 2;
            duel_PC.RNGeezus.diceMax = 4;
            //RNG call
            duel_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            duel_PC.RNGeezus.missOn = 0;
            duel_PC.RNGeezus.armorPierce = false;
            //Damage calculation
            Victim.Target_ArchetypeSwitch(Victim, duel_PC);
            duel_PC.DamageBonus = damageStore;
            if (duel_PC.RNGeezus.isHit)
            {
                duel_PC.RNGeezus.braceRoll(duel_PC, 3); //if it's too strong reduce to 2, worst case scenario make it a flat specialArmor increase
                duel_PC.specialArmor += duel_PC.RNGeezus.Result;
            }
            
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    public void duel_SightLineSet() {
        
        duel_PC.RangePurge();
        duel_PC.attackableTiles = new List<Tile_Properties>();
        
        for (int i = 0; i < 6; i++)
        {
            Tile_Properties tempProps = duel_PC.currentTile.individualNeighborsList[i];
            while (tempProps != null)
            {
                if (tempProps.tType == tileType.Inactive)
                {
                    break;
                }
                else
                {
                    if (tempProps.rangeTo <= duel_PC.AttackRange)
                    {
                        tempProps.Sightline = true;
                        duel_PC.attackableTiles.Add(tempProps);
                    }
                    if (tempProps.whoIsOnTile != isOnTile.empty)
                    {
                        break;
                    }
                    tempProps = tempProps.individualNeighborsList[i]; //this MUST be outside the IF, because if it can't look for the next tile the loop runs forever, thus not allowing the game to continue...
                }
            }
        }
        duel_PC.checkAttackableTiles();
    }

    public void SetDestinationNeighborsToClickable(Tile_Properties tprop)
    {
        for (int i = 0; i < 6; i++)
        {
            if (tprop.individualNeighborsList[i] != null)
            {
                if (tprop.individualNeighborsList[i].Movable)
                {
                    if (DuelMode == duelMode.mobile)
                    {
                        if ((tprop.individualNeighborsList[i].ownedBy == OwnedBy.NONE) || (duel_PC == destinationCurrTileProps.individualNeighborsList[i].getOwner()))
                        {
                            tprop.individualNeighborsList[i].Srend.color = new Color(0f, 0.7f, 1f, 1f); //sets the color of a valid neighbor to a lightblue color to indicate that you can continue making a path but you won't be able to attack from there
                            tprop.individualNeighborsList[i].tempPlayableCharacter = duel_PC;
                            tprop.individualNeighborsList[i].coll2D.enabled = true;
                            selectingAttackDestination = true;
                        }
                    }
                    else if (DuelMode == duelMode.offense)
                    {
                        if ((tprop.individualNeighborsList[i].ownedBy == OwnedBy.NONE) || (duel_PC.selectedTarget_PC == duel_PC.currentTile.individualNeighborsList[i].getOwner()))
                        {
                            tprop.individualNeighborsList[i].Srend.color = new Color(0f, 0.7f, 1f, 1f); //sets the color of a valid neighbor to a lightblue color to indicate that you can continue making a path but you won't be able to attack from there
                            tprop.individualNeighborsList[i].tempPlayableCharacter = duel_PC;
                            tprop.individualNeighborsList[i].coll2D.enabled = true;
                            selectingAttackDestination = true;
                        }
                    }
                }

            }
        }
    }
    public void PurgeDestinationNeighbors(Tile_Properties tprop)
    {
        for (int i = 0; i < 6; i++)
        {
            if (tprop.individualNeighborsList[i] != null)
            {
                if (tprop.individualNeighborsList[i].Movable)
                {
                    tprop.individualNeighborsList[i].Srend.color = tprop.individualNeighborsList[i].tileDefaultColor; //sets the color of a valid neighbor to a lightblue color to indicate that you can continue making a path but you won't be able to attack from there
                    tprop.individualNeighborsList[i].tempPlayableCharacter = null;
                    tprop.individualNeighborsList[i].coll2D.enabled = false;
                }

            }
        }
        if ((attackDestination != null) && (selectingAttackDestination))
        {
            attackDestination.Srend.color = new Color(0f, 0.7f, 1f, 1f);
            selectingAttackDestination = false;
        }
        else if ((attackDestination != null) && (!selectingAttackDestination))
        {
            attackDestination.Srend.color = attackDestination.tileDefaultColor;
        }
        selectingAttackDestination = false;
        //enemyCurrTileProps = null;

    }

    public void Duelist_TakeDamage(PlayableCharacter Agressor)
    {
        int storeArmor = duel_PC.Armor;
        //store armor
        //switch
        //offensive: turn off armor, run riposte
        //defense: run riposte at double power
        //mobile: break
        duel_PC.TakeDamage(Agressor);
        //restore armor
    }

}
