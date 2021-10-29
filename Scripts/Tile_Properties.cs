using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// So here's the rundown on how this system works:
/// Each tile has two major defining properties. 
/// 1.What their type is. i.e. "tileType" (which is also reflected in their game object tag)
/// 2.Who is occupying the tile. i.e. "isOnTile"
/// </summary>


public enum tileType { Lowland, Midland, Highland, Mine, Base, Sanctuary, Quarry, BaseWall, Inactive };
public enum isOnTile { empty, p1, p2, p3, p4, p5, p6 };
public enum OwnedBy { NONE, p1, p2, p3, p4, p5, p6 }

public class Tile_Properties : MonoBehaviour
{

    [Header("Set Dynamically:")]
    public int X = int.MaxValue;
    public int Y = int.MaxValue;
    public int Z = int.MaxValue;
    public Vector3Int individualTileCoords;

    public int TileIndex_X;
    public int TileIndex_Y;
    public GameObject N_Neighbor; //the tile that is both North and adjacent to the current one
    public GameObject S_Neighbor; //the tile that is both South and adjacent to the current one
    public GameObject NE_Neighbor; //the tile that is both North-East and adjacent to the current one
    public GameObject SW_Neighbor; //the tile that is both South-East and adjacent to the current one
    public GameObject NW_Neighbor; //the tile that is both North-West and adjacent to the current one
    public GameObject SE_Neighbor; //the tile that is both South-West and adjacent to the current one
    public List<Tile_Properties> individualNeighborsList;
    public tileType tType; //gets manually set
    public OwnedBy ownedBy;
    public isOnTile whoIsOnTile; //For some reason enums need to be defined twice... ¯\_(ツ)_/¯
    public PolygonCollider2D coll2D; //holds access to the Polygon Collider properties in this object
    public GameObject GO_Self; //A variable that is a reference to the instance that has this code attached to it (useful for a variety of reasons)
    public Tile_Properties tile_self;
    public bool trapOnTile = false; //does this tile have a Chieftain Trap?
    public bool droneOnTile = false; //does this tile have a sniper DroneTrap in it?
    public bool droneCrashed = false; //is this the tile where the Drone crashed in?
    public SpriteRenderer Srend; //used for color and visual modifications through code
    public Color tileDefaultColor;
    public PlayableCharacter tempPlayableCharacter; //holds the access to the "Playable Character" code (used for movement)
    public PlayableCharacter playerOnTile;
    public bool Movable = false;
    public bool EnemyOn = false;
    public int rangeTo = 0; //Used by player to calculate Attack range
    //public int tempRangeTo = int.MaxValue;
    public int distanceTo = 0; //Used by player to calculate movement range
    public bool Sightline = false;
    public bool checkedbyPathfinder = false;
    public bool checkedbyRangefinder = false;
    public bool selectedForMovement = false;
    public int relativeDirection = int.MaxValue;

    private PlayableCharacter playerOwner;

    public PlayableCharacter getOwner() {
        return playerOwner;
    }

    public void setOwner(PlayableCharacter owner) {
        playerOwner = owner;
    }

    //Didn't use Start() because I ran into rush cases, so I'm manually setting the initialization sequence, you'll find that the map calls this function (which is why it is also a 'public void')
    public void TilePropertiesDefault()
    { //starts the tiles with their default properties
        GO_Self = this.gameObject; //Sets the self reference
        tile_self = GO_Self.GetComponent<Tile_Properties>();
        whoIsOnTile = isOnTile.empty; //Defaults who is occupying the tile to 'empty'
        ownedBy = OwnedBy.NONE;
        Srend = GO_Self.GetComponent<SpriteRenderer>(); //gets access to the Spriterenderer in this object
        tileDefaultColor = GO_Self.GetComponent<Tile_Properties>().Srend.color;
        coll2D = this.GetComponent<PolygonCollider2D>(); //gets access to the Polygon Collider in this object
        coll2D.enabled = false; //Disables the collider for ease of input reasons
    }

    public void HideInactiveTiles()
    {
        if (this.tag == "Inactive")
        {
            GO_Self.SetActive(false);
        }
        else
        {
            Movable = true;
        }
    }

    public void Pathfinder(PlayableCharacter originPlayer)
    {
        for (int i = 0; i < 6; i++)
        {
            if (individualNeighborsList[i] != null)
            {
                if (individualNeighborsList[i].Movable)
                {
                    if (!individualNeighborsList[i].checkedbyPathfinder)
                    {
                        if ((individualNeighborsList[i].ownedBy == OwnedBy.NONE) || (originPlayer == individualNeighborsList[i].getOwner()))
                        {
                            individualNeighborsList[i].distanceTo = distanceTo + 1;
                            originPlayer.pathfindQueue.Add(individualNeighborsList[i]);
                            originPlayer.movableTiles.Add(individualNeighborsList[i]);
                            individualNeighborsList[i].tempPlayableCharacter = originPlayer;

                            //Enables me to see what tiles I could move into and have enough AP to attack from
                            /*
                            if ((originPlayer.ActionPoints - (distanceTo * originPlayer.MovementCost)) > originPlayer.AttackActionCost)
                            {
                                individualNeighborsList[i].Srend.color = new Color(0.5f, 0f, 0.5f, 1f);
                            }
                            else {
                                individualNeighborsList[i].Srend.color = new Color(0f, 0f, 0.6f, 1f);
                            }*/

                            individualNeighborsList[i].Srend.color = new Color(0f, 0f, 0.7f, 1f); //Enabled for uniform visuals
                            individualNeighborsList[i].checkedbyPathfinder = true;
                        }
                    }
                }
            }

        }
        checkedbyPathfinder = true;
        originPlayer.checkAttackableTiles();
        originPlayer.Pathfinder();
    }

    public void Rangefinder(PlayableCharacter originPlayer)
    {
        int EffectiveRange = ((originPlayer.ActionPoints - originPlayer.AttackActionCost) / originPlayer.MovementCost); //the effective range calculation simply allows you to see what you could target if you moved by that amount
        if (EffectiveRange >= 0)
        {
            if (rangeTo < originPlayer.AttackRange + EffectiveRange)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (individualNeighborsList[i] != null)
                    {
                        if (!individualNeighborsList[i].checkedbyRangefinder)
                        {
                            originPlayer.rangefindQueue.Add(individualNeighborsList[i]);
                            originPlayer.attackableTiles.Add(individualNeighborsList[i]);
                            individualNeighborsList[i].tempPlayableCharacter = originPlayer;
                            individualNeighborsList[i].checkedbyRangefinder = true;
                        }

                    }

                }
            }
            /*
            if (rangeTo <= originPlayer.AttackRange) //Tiles that ARE within direct attack range are colored the same as always
            {
                if (tType != tileType.Base)
                {
                    if (Sightline)
                    {
                        Srend.color = new Color(1f, 0, 0, 0.8f);

                        if (whoIsOnTile != isOnTile.empty && rangeTo != 0)
                        {
                            originPlayer.occupiedBy(tile_self);
                            Srend.color = new Color(1, 0, 0, 0.6f);
                        }
                    }
                    else if ((!Sightline) && (tType != tileType.Mine))
                    {
                        Srend.color = new Color(1f, 0, 0, 0.8f);

                        if (whoIsOnTile != isOnTile.empty && rangeTo != 0)
                        {
                            originPlayer.occupiedBy(tile_self);
                            Srend.color = new Color(1, 0, 0, 0.6f);
                        }
                    }

                }
            }
            else //Tiles that WOULD be in direct attack range, should you move, are colored pink and have disabled the targeting component
            {
                if (tType != tileType.Base)
                {
                    Srend.color = new Color(1f, 0.7f, 1f, 1f);
                    if (whoIsOnTile != isOnTile.empty && rangeTo != 0)
                    {
                        Srend.color = new Color(1, 0.7f, 1, 0.8f);
                    }
                }
            }
            //Debug.Log("effective range ran");
            */
        } //same range check but taking into account movement stretching the range
        else //old range check doesn't account for movement stretch because it either CAN'T stretch due to action points, or a bug
        {
            if (rangeTo < originPlayer.AttackRange)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (individualNeighborsList[i] != null)
                    {
                        if (!individualNeighborsList[i].checkedbyRangefinder)
                        {
                            originPlayer.rangefindQueue.Add(individualNeighborsList[i]);
                            originPlayer.attackableTiles.Add(individualNeighborsList[i]);
                            individualNeighborsList[i].tempPlayableCharacter = originPlayer;
                            individualNeighborsList[i].checkedbyRangefinder = true;
                        }

                    }

                }
            }
            /*
            if (tType != tileType.Base)
            {
                if (Sightline)
                {
                    Srend.color = new Color(1f, 0, 0, 0.8f);

                    if (whoIsOnTile != isOnTile.empty && rangeTo != 0)
                    {
                        originPlayer.occupiedBy(tile_self);
                        Srend.color = new Color(1, 0, 0, 0.6f);
                    }
                }
                else if ((!Sightline) && (tType != tileType.Mine))
                {
                    Srend.color = new Color(1f, 0, 0, 0.8f);

                    if (whoIsOnTile != isOnTile.empty && rangeTo != 0)
                    {
                        originPlayer.occupiedBy(tile_self);
                        Srend.color = new Color(1, 0, 0, 0.6f);
                    }
                }

            }
            */
        }
        checkedbyRangefinder = true;
        originPlayer.Rangefinder();

    }

    public void SetColorIfEnemy()
    {
        Srend.color = new Color(Srend.color.r + 1f, Srend.color.g - 1f, Srend.color.b - 1, 0.6f); //Changes the color to visually distinguish what tiles you CAN select
        EnemyOn = true;
    }

    void OnMouseOver() //Unity event Handler for when the mouse touches a collision box
    {
        if (Input.GetMouseButtonDown(0)) //if left-click WHILE over the collision box
        {
            if (tempPlayableCharacter.chief != null)
            {
                chieftainTileClickConditions();
            }
            else if (tempPlayableCharacter.huntress != null)
            {
                headhuntressTileClickConditions();
            }
            else if (tempPlayableCharacter.duel != null)
            {
                duelistTileClickConditions();
            }
            else if (tempPlayableCharacter.soul != null)
            {
                soulstealerTileClickConditions();
            }

            else if ((tempPlayableCharacter != null))//seeing that the following code requires access to "PlayableCharacter", the IF is to prevent errors in the case the reference happens to be empty
            {
                ManualPathfinding();
            }
        }
    }

    void ManualPathfinding()
    { //[NEEDS SOME FINE TUNING]

        if (!selectedForMovement)
        {
            if (tempPlayableCharacter.tentativeActionPoints >= tempPlayableCharacter.MovementCost)
            {
                selectedForMovement = true;
                tempPlayableCharacter.tentativeActionPoints = tempPlayableCharacter.tentativeActionPoints - tempPlayableCharacter.MovementCost;
                tempPlayableCharacter.NeighborPurge();
                tempPlayableCharacter.tilesSelectedForMovement.Add(tile_self);
                if (tempPlayableCharacter.tentativeActionPoints >= tempPlayableCharacter.MovementCost)
                {
                    if (tempPlayableCharacter.tentativeActionPoints >= tempPlayableCharacter.AttackActionCost)
                    {
                        Srend.color = new Color(1f, 0f, 0.35f, 0.8f);
                        //tempPlayableCharacter.stepByStepAttackableTiles(tempPlayableCharacter.tilesSelectedForMovement.Count);
                    }
                    else
                    {
                        Srend.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
                    }
                    tempPlayableCharacter.SetNeighborsToClickable(tile_self);
                }
                else {
                    Srend.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }

            }
            else
            {
                tempPlayableCharacter.NeighborPurge();
            }

        }
        else
        {
            //tempPlayableCharacter.MoveOnPath();
            Debug.Log("Click on the Move Button again");
        }
    }

    void chieftainTileClickConditions()
    {
        if (tempPlayableCharacter.chief.trapMode && tempPlayableCharacter.chief.trapsRemaining != 0 && !trapOnTile)
        {
            trapOnTile = true;
            tempPlayableCharacter.chief.trappedTiles.Add(tile_self);
            tile_self.Srend.color = tile_self.tileDefaultColor;
            tile_self.coll2D.enabled = false;
            tempPlayableCharacter.chief.trapsRemaining--;

            //tempPlayableCharacter.chief.ButtonText.text = "Traps: " + tempPlayableCharacter.chief.trapsRemaining;
            // RUBEN: I changed this to match changes to the UI
            tempPlayableCharacter.chief.ButtonText.text = tempPlayableCharacter.chief.trapsRemaining.ToString();
            tempPlayableCharacter.chief.trapCountdown = 3;
            tempPlayableCharacter.chief.TrapModeDisable();
            Debug.Log("Trap set");
        }
        else if (tempPlayableCharacter.chief.trapMode && trapOnTile)
        {
            Debug.Log("There's already a trap here....Dumbass");
        }
        else if (tempPlayableCharacter.chief.trapMode && tempPlayableCharacter.chief.trapsRemaining == 0)
        {
            Debug.Log("Dumbass");
        }
        else if (!tempPlayableCharacter.chief.trapMode)//seeing that the following code requires access to "PlayableCharacter", the IF is to prevent errors in the case the reference happens to be empty
        {
            ManualPathfinding();

        }
    }
    void headhuntressTileClickConditions()
    {
        if (tempPlayableCharacter.huntress.droneMode && !droneOnTile)
        {
            tempPlayableCharacter.huntress.DeployDrone(tile_self);
            tempPlayableCharacter.huntress.DroneModeDisable();
            Debug.Log("Drone deployed");
        }
        else if (tempPlayableCharacter.huntress.droneMode && droneOnTile)
        {
            Debug.Log("There's already a drone here....Dumbass");
        }
        else if (!tempPlayableCharacter.huntress.droneMode)//seeing that the following code requires access to "PlayableCharacter", the IF is to prevent errors in the case the reference happens to be empty
        {
            ManualPathfinding();

        }
    }
    void duelistTileClickConditions()
    {
        if (tempPlayableCharacter.attackMenuMode && tempPlayableCharacter.duel.selectingAttackDestination)
        {
            tile_self.Srend.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            tile_self.coll2D.enabled = false;
            tempPlayableCharacter.duel.attackDestination = tile_self;
            tempPlayableCharacter.duel.PurgeDestinationNeighbors(tempPlayableCharacter.duel.destinationCurrTileProps);
            //tempPlayableCharacter.duel.selectingAttackDestination = false;
            
        }
        else if (tempPlayableCharacter.attackMenuMode && (!tempPlayableCharacter.duel.selectingAttackDestination))
        {
            Debug.Log("You already selected a destination");
        }
        else if (!tempPlayableCharacter.attackMenuMode)//seeing that the following code requires access to "PlayableCharacter", the IF is to prevent errors in the case the reference happens to be empty
        {
            ManualPathfinding();

        }
    }
    void soulstealerTileClickConditions()
    {
        if (tempPlayableCharacter.soul.grappleMode && tempPlayableCharacter.soul.selectingPullDestination)
        {
            tile_self.Srend.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            tile_self.coll2D.enabled = false;
            tempPlayableCharacter.soul.pullDestination = tile_self;
            tempPlayableCharacter.soul.PurgePullDestinationNeighbors(tempPlayableCharacter.soul.destinationCurrTileProps);
        }
        else if (!tempPlayableCharacter.soul.grappleMode)//seeing that the following code requires access to "PlayableCharacter", the IF is to prevent errors in the case the reference happens to be empty
        {
            ManualPathfinding();
        }
    }
    //Made it its own function for cleanlyness
    public void characterSwitch(PlayableCharacter playableCharacter)
    { //Checks the Character that moved and sets the enum to its proper value
        switch (playableCharacter.playerNumber)
        {
            case PlayerNumber.Player1:
                {
                    playableCharacter.currentTile.whoIsOnTile = isOnTile.p1;
                    playerOnTile = playableCharacter;
                    break;
                }
            case PlayerNumber.Player2:
                {
                    playableCharacter.currentTile.whoIsOnTile = isOnTile.p2;
                    playerOnTile = playableCharacter;
                    break;
                }
            case PlayerNumber.Player3:
                {
                    playableCharacter.currentTile.whoIsOnTile = isOnTile.p3;
                    playerOnTile = playableCharacter;
                    break;
                }
            case PlayerNumber.Player4:
                {
                    playableCharacter.currentTile.whoIsOnTile = isOnTile.p4;
                    playerOnTile = playableCharacter;
                    break;
                }
            case PlayerNumber.Player5:
                {
                    playableCharacter.currentTile.whoIsOnTile = isOnTile.p5;
                    playerOnTile = playableCharacter;
                    break;
                }
            case PlayerNumber.Player6:
                {
                    playableCharacter.currentTile.whoIsOnTile = isOnTile.p6;
                    playerOnTile = playableCharacter;
                    break;
                }
            default:
                {
                    playableCharacter = null;
                    break;
                }
        }

    }
}
