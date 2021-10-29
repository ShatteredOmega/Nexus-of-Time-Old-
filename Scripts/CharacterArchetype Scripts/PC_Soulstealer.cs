using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC_Soulstealer : MonoBehaviour
{
    public DynamicMap soul_DynM;
    public PlayableCharacter soul_PC;
    public PC_Soulstealer soul;
    public Tile_Properties pullDestination;
    public Tile_Properties destinationCurrTileProps;
    public bool grappleMode = false;
    public bool grappledThisTurn =false;
    public bool selectingPullDestination = false;
    public bool yeet = false;
    public bool yoink = false;
    public int stolenArmor = 0;
    public int stolenDamage = 0;
    public bool hasYeeted = false;
    public bool hasYoinked = false;
    public PlayableCharacter grappletarget;
    public PlayableCharacter statTheftVictim;
    public Button Btn_Yeet;
    public Button Btn_Yoink;

    public void initializeSoulstealer(PlayableCharacter playable, PC_Soulstealer soulstealer, DynamicMap M)
    {
        soul_PC = playable;
        soul = soulstealer;
        soul_DynM = M;
        soul_PC.MAX_Health = 40;
        soul_PC.Health = 40;
        soul_PC.AttackRange = 1;
        soul_PC.HealthUpgradeCost = 150;
        soul_PC.DamageUpgradeCost = 250;
        soul_PC.ArmorUpgradeCost = 350;
    }

    public void Soulstealer_StartTurnConditions()
    {
        grappledThisTurn = false;
        enableGrappleButtons();
    }

    public void Soulstealer_EndTurnConditions()
    {
        Btn_Yeet.gameObject.SetActive(false);
        Btn_Yoink.gameObject.SetActive(false);
    }

    public void enableGrappleButtons()
    {
        if (((!soul_PC.playerActionMenuMode) || (!soul_PC.attackMenuMode) || (!soul_PC.moveMenuMode)) && (!grappleMode) && (!grappledThisTurn))
        {
            if (!soul_PC.Snared) {
                Btn_Yeet.gameObject.SetActive(true);
            }
            Btn_Yoink.gameObject.SetActive(true);
        }
    }

    public void disableGrappleButtons() {
        Btn_Yeet.gameObject.SetActive(false);
        Btn_Yoink.gameObject.SetActive(false);
    }

    public void grappleModeEnable() //Make buttons that trigger character abilities on visible on that character's turn
    {
        soul_PC.Deselect();
        soul_PC.AttackRange = 3;
        soul_PC.AttackButton.gameObject.SetActive(false);
        soul_PC.MoveButton.gameObject.SetActive(false);
        soul_PC.EndTurnButton.gameObject.SetActive(false);
        soul_PC.MineButton.gameObject.SetActive(false);
        soul_PC.HealButton.gameObject.SetActive(false);
        soul_PC.UpgradeButton.gameObject.SetActive(false);
        grappleMode = true;
        soul_PC.PathPurge();
        soul_PC.RangeSet();
        soul_PC.RangefinderStart();
    }

    public void grappleModeDisable()
    {
        if (destinationCurrTileProps == null) {
            soul_PC.RangePurge();

            Debug.Log("rangeShould be purged");
        } else {
            //PurgePullDestinationNeighbors(destinationCurrTileProps);
            Debug.Log("else");
        }
        soul_PC.Untarget();
        Debug.Log("Untargeted");
        grappleMode = false;
        yeet = false;
        yoink = false;
        soul_PC.AttackRange = 1;
        soul_PC.AttackButton.gameObject.SetActive(true);
        soul_PC.MoveButton.gameObject.SetActive(true);
        soul_PC.EndTurnButton.gameObject.SetActive(true);
        soul_PC.MineButton.gameObject.SetActive(true);
        soul_PC.HealButton.gameObject.SetActive(true);
        soul_PC.UpgradeButton.gameObject.SetActive(true);
        enableGrappleButtons();
    }

    public void YeetAction()
    {
        if (soul_PC.ActionPoints >= 1) {
            if (!grappleMode)
            {
                yeet = true;
                grappleModeEnable();
                Btn_Yoink.gameObject.SetActive(false);
            }
            if ((grappleMode) && (soul_PC.selectedTarget_PC != null))
            {
                if (soul.pullDestination != null)
                {
                    YeetMove();
                    soul_PC.selectedTarget = false;
                    soul_PC.selectedTarget_PC.self_Srend.color = soul_PC.selectedTarget_PC.normalColor;
                    soul_PC.selectedTarget_PC = null;
                    yeet = false;
                    grappledThisTurn = true;
                    Btn_Yeet.gameObject.SetActive(false);
                    grappleModeDisable();
                    //soul_PC.Deselect();
                }
                else
                {
                    Debug.Log("Please select a destination");
                }

            }
            else
            {
                Debug.Log("Please select a Target");
            }
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }

    }
    public void YeetMove()
    {
        if (!soul_PC.Snared)
        {
            Tile_Properties temp;
            temp = soul_PC.currentTile;
            soul_PC.tileChosen = pullDestination;
            soul_PC.MoveFunctionPt1();
            temp.distanceTo = int.MaxValue;
            temp.Srend.color = temp.tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
            temp.tempPlayableCharacter = null;
            temp.Movable = true;
            if (soul_PC.currentTile != null)
            {
                soul_PC.currentTile.Movable = false;
            }
            soul_PC.ActionPoints = soul_PC.ActionPoints + soul_PC.MovementCost;
            soul_PC.ActionPoints--;
            PurgePullDestinationNeighbors(destinationCurrTileProps);
            pullDestination.Srend.color = pullDestination.tileDefaultColor;
            destinationCurrTileProps = null;
        }
    }
    public void YoinkAction()
    {
        if (soul_PC.ActionPoints >= 1)
        {
            if (!grappleMode)
            {
                yoink = true;
                grappleModeEnable();
                Btn_Yeet.gameObject.SetActive(false);
            }
            if ((grappleMode) && (soul_PC.selectedTarget_PC != null))
            {
                if (soul.pullDestination != null)
                {
                    YoinkMove();
                    soul_PC.selectedTarget = false;
                    soul_PC.selectedTarget_PC.self_Srend.color = soul_PC.selectedTarget_PC.normalColor;
                    soul_PC.selectedTarget_PC = null;
                    yoink = false;
                    grappledThisTurn = true;
                    Btn_Yoink.gameObject.SetActive(false);
                    grappleModeDisable();
                    //soul_PC.Deselect();
                }
                else
                {
                    Debug.Log("Please select a destination");
                }

            }
            else
            {
                Debug.Log("Please select a Target");
            }
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    void YoinkMove() {
        Tile_Properties temp;
        temp = soul_PC.selectedTarget_PC.currentTile;
        soul_PC.selectedTarget_PC.tileChosen = pullDestination;
        soul_PC.selectedTarget_PC.MoveFunctionPt1();
        temp.distanceTo = int.MaxValue;
        temp.Srend.color = temp.tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
        temp.tempPlayableCharacter = null;
        temp.Movable = true;
        if (soul_PC.selectedTarget_PC.currentTile != null)
        {
            soul_PC.selectedTarget_PC.currentTile.Movable = false;
        }
        soul_PC.selectedTarget_PC.ActionPoints = soul_PC.selectedTarget_PC.ActionPoints + soul_PC.selectedTarget_PC.MovementCost;
        soul_PC.ActionPoints--;
        PurgePullDestinationNeighbors(destinationCurrTileProps);
        pullDestination.Srend.color = pullDestination.tileDefaultColor;
        destinationCurrTileProps = null;
    }

    public void SetPullDestinationNeighborsToClickable(Tile_Properties tprop)
    {
        for (int i = 0; i < 6; i++)
        {
            if (tprop.individualNeighborsList[i] != null)
            {
                if (tprop.individualNeighborsList[i].Movable)
                {
                    if (yeet)
                    {
                        if ((tprop.individualNeighborsList[i].ownedBy == OwnedBy.NONE) || (soul_PC == destinationCurrTileProps.individualNeighborsList[i].getOwner()))
                        {
                            tprop.individualNeighborsList[i].Srend.color = new Color(0f, 0.7f, 1f, 1f); //sets the color of a valid neighbor to a lightblue color to indicate that you can continue making a path but you won't be able to attack from there
                            tprop.individualNeighborsList[i].tempPlayableCharacter = soul_PC;
                            tprop.individualNeighborsList[i].coll2D.enabled = true;
                            selectingPullDestination = true;
                        }
                    }
                    else if (yoink)
                    {
                        if ((tprop.individualNeighborsList[i].ownedBy == OwnedBy.NONE) || (soul_PC.selectedTarget_PC == soul_PC.currentTile.individualNeighborsList[i].getOwner()))
                        {
                            tprop.individualNeighborsList[i].Srend.color = new Color(0f, 0.7f, 1f, 1f); //sets the color of a valid neighbor to a lightblue color to indicate that you can continue making a path but you won't be able to attack from there
                            tprop.individualNeighborsList[i].tempPlayableCharacter = soul_PC;
                            tprop.individualNeighborsList[i].coll2D.enabled = true;
                            selectingPullDestination = true;
                        }
                    }
                }

            }
        }
    }
    public void PurgePullDestinationNeighbors(Tile_Properties tprop)
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
        soul_PC.RangePurge();
        if ((pullDestination != null) && (selectingPullDestination))
        {
            pullDestination.Srend.color = new Color(0f, 0.7f, 1f, 1f);
            selectingPullDestination = false;
        }
        else if ((pullDestination != null) && (!selectingPullDestination))
        {
            pullDestination.Srend.color = pullDestination.tileDefaultColor;
        }
        selectingPullDestination = false;
        //enemyCurrTileProps = null;
        //grappleModeDisable();
        

    }

    public void checkPullTiles() //[Test Pending]
    {
        int EffectiveRange = 0;
        EffectiveRange = ((soul_PC.ActionPoints - soul_PC.AttackActionCost) / soul_PC.MovementCost); //the effective range calculation simply allows you to see what you could target if you moved by that amount
        if (EffectiveRange > 0)
        {
            for (int i = 0; i < soul_PC.attackableTiles.Count; i++)
            {
                
                if (soul_PC.attackableTiles[i].rangeTo <= (soul_PC.AttackRange + EffectiveRange)) //Tiles that WOULD be in direct attack range, should you move, are colored pink and have disabled the targeting component
                {
                    if (soul_PC.attackableTiles[i].tType != tileType.Base && soul_PC.attackableTiles[i].rangeTo > 0)
                    {
                        //soul_PC.attackableTiles[i].Srend.color = new Color(1f, 0.7f, 1f, 1f);
                        if (soul_PC.attackableTiles[i].whoIsOnTile != isOnTile.empty)
                        {
                            //soul_PC.attackableTiles[i].Srend.color = new Color(1, 0.7f, 1, 0.8f);
                        }

                    }
                }
                if (soul_PC.attackableTiles[i].rangeTo <= soul_PC.AttackRange)
                {
                    if (soul_PC.attackableTiles[i].tType != tileType.Base)
                    {
                        if (soul_PC.attackableTiles[i].rangeTo > 0)
                        {
                            if (soul_PC.attackableTiles[i].Sightline)
                            {
                                soul_PC.attackableTiles[i].Srend.color = new Color(0f, 1, 0, 0.6f);

                                if ((soul_PC.attackableTiles[i].whoIsOnTile != isOnTile.empty))
                                {
                                    soul_PC.occupiedBy(soul_PC.attackableTiles[i].tile_self);
                                    soul_PC.attackableTiles[i].Srend.color = new Color(0, 1, 0, 0.4f);
                                }
                            }
                            else if ((!soul_PC.attackableTiles[i].Sightline) && (soul_PC.attackableTiles[i].tType != tileType.Mine))
                            {
                                soul_PC.attackableTiles[i].Srend.color = new Color(0f, 1, 0.0f, 0.6f);

                                if ((soul_PC.attackableTiles[i].whoIsOnTile != isOnTile.empty))
                                {
                                    soul_PC.occupiedBy(soul_PC.attackableTiles[i].tile_self);
                                    soul_PC.attackableTiles[i].Srend.color = new Color(0, 1, 0f, 0.4f);
                                }
                            }
                        }
                    }
                }

            }
        }
        else
        {
            for (int i = 0; i < soul_PC.attackableTiles.Count; i++)
            {
                if (soul_PC.attackableTiles[i].tType != tileType.Base)
                {
                    if (soul_PC.attackableTiles[i].rangeTo > 0)
                    {
                        if (soul_PC.attackableTiles[i].Sightline)
                        {
                            soul_PC.attackableTiles[i].Srend.color = new Color(0f, 1, 0, 0.6f);

                            if (soul_PC.attackableTiles[i].whoIsOnTile != isOnTile.empty)
                            {
                                soul_PC.occupiedBy(soul_PC.attackableTiles[i].tile_self);
                                soul_PC.attackableTiles[i].Srend.color = new Color(0, 1, 0.6f, 0.4f);
                            }
                        }
                        else if ((!soul_PC.attackableTiles[i].Sightline) && (soul_PC.attackableTiles[i].tType != tileType.Mine))
                        {
                            soul_PC.attackableTiles[i].Srend.color = new Color(0f, 1, 0.6f, 0.6f);

                            if (soul_PC.attackableTiles[i].whoIsOnTile != isOnTile.empty)
                            {
                                soul_PC.occupiedBy(soul_PC.attackableTiles[i].tile_self);
                                soul_PC.attackableTiles[i].Srend.color = new Color(0, 1, 0.6f, 0.4f);
                            }
                        }
                    }
                }
            }
        }

    }

}
