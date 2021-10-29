using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [IMPORTANT NOTE]
/// In lieu of a Character Select Screen, I'm attaching CharacterArchetype scripts to the players in order of implementation priority
///
/// </summary>

public enum PlayerNumber { Player1, Player2, Player3, Player4, Player5, Player6 }
public enum CharacterArchetype { genericCharacter, ShapeShifter, Chieftain, Duelist, Headhuntress, Soulstealer, LadyLuck }

public class PlayableCharacter : MonoBehaviour
{

    [Header("Set Dynamically:")]
    public PlayerNumber playerNumber;
    public CharacterArchetype characterArchetype;
    public DynamicMap DynMap;
    public Map m; //Stores reference to the "Map" code;
    public RandomNumberGenerator RNGeezus;
    public PlayableCharacter PC; //Stores reference for its own "PlayableCharacter" code
    public TurnSystem turnSys; //Stores reference to the "TurnSystem" code
    public GameObject GO_Self; //Stores reference for its own GameObject
    public SpriteRenderer self_Srend;
    public CircleCollider2D coll2D;
    public Color normalColor;
    public Button AttackButton;
    public Button MoveButton;
    public Button MineButton;
    public Button EndTurnButton;
    public Button HealButton;
    public Button UpgradeButton;

    [Header("Set Dynamically: Archetype Accessor Vars")]
    public PC_Chieftain chief;
    public PC_Duelist duel;
    public PC_LadyLuck lady;
    public PC_ShapeShifter Sshifter;
    public PC_Soulstealer soul;
    public PC_Headhuntress huntress;

    [Header("Set Dynamically: Position")]
    public Tile_Properties currentTile; //Stores reference to the "TileProperties" code in the tile the character is currently ON
    public List<Tile_Properties> pathfindQueue; //This is consumed by the pathfinding process itself
    public List<Tile_Properties> movableTiles; //This is kept so that all tiles checked by the pathfinding are reset to normal then this list self destructs
    public List<Tile_Properties> tilesSelectedForMovement;
    public Tile_Properties tileChosen;


    [Header("Set Dynamically: MenuModes")]
    public bool playerActionMenuMode = false; //is the player selected and awaiting an action?
    public bool moveMenuMode = false;
    public bool attackMenuMode = false;

    //Abstract Stats
    [Header("Set Dynamically: Abstract Stats:")]
    public int MAX_ActionPoints = 5; //Default 5. Used as a limit for how many Action Points can a Character have at any given time
    public int tentativeActionPoints = 0;
    public int ActionPoints = 5; //Set to MAX at the end of every turn; 
    public int MAX_VictoryPoints = 25;
    public int VictoryPoints = 0;
    public int playerMoney = 0;
    public int HealthUpgradeCost = 100;
    public int DamageUpgradeCost = 200;
    public int ArmorUpgradeCost = 250;

    //BasePlayerStats
    [Header("Set Dynamically: Player Attributes:")]
    public int MAX_Health = 30;
    public int Health = 30;
    public int DamageBonus = 0;
    public int Armor = 0;
    public int AttackRange = 3; //0 = self, 1 = Melee/Adjacents, <2 range [Has NO effect or implementation] 

    //Special PlayerStats
    [Header("Set Dynamically: Special Attributes:")]
    public int specialHealth = 0; //AKA 'Grey Health' or 'Potential'
    public int specialDamageBonus = 0;
    public int specialArmor = 0;
    public bool trueArmor = false; // [NEEDS RESET CASES]
    public bool Invulnerable = true;
    public bool Snared = false;
    public int snaredTurns = 0;
    public bool Droned = false;
    public bool Punctured = false;
    public bool Silenced = false;
    public bool Cursed = false;
    public int CurseLevel = 0;
    public int CurseStorage = 0;
    public bool Bleeding = false;
    public int BleedSeverity = 0;
    public int BleedTurnsRemaining = 0;

    //Targeting bools
    [Header("Set Dynamically: Targeting vars")]
    public List<Tile_Properties> rangefindQueue;
    public List<Tile_Properties> attackableTiles;
    public List<PlayableCharacter> targetList;
    public PlayableCharacter selectedTarget_PC;
    public bool naturalSightLine; //[needs to be check]
    public bool targetedByP1 = false;
    public bool targetedByP2 = false;
    public bool targetedByP3 = false;
    public bool targetedByP4 = false;
    public bool targetedByP5 = false;
    public bool targetedByP6 = false;
    public bool selectedTarget = false;


    //Action dependent Cost Variables
    [Header("Set Dynamically: Action dependent Cost Variables")]
    public int MovementCost = 1; //due to character types/classes, this variable becomes relevant
    public int AttackActionCost = 2; //due to character types/classes, this variable becomes relevant
    public int HealActionCost = 3; //due to character types/classes, this variable becomes relevant
    public int MineActionCost = 2; //This is a strong MAYBE, most likely will never be used


    public void StartCharacters() //Called by "Map" [not using Start() due to rush cases]
    {
        RNGeezus = FindObjectOfType<RandomNumberGenerator>(); //looks for an object with a "RandomNumberGenerator" component and stores it
        turnSys = FindObjectOfType<TurnSystem>(); //looks for an object with a "TurnSystem" component and stores it 
        GO_Self = this.gameObject; //Sets its own GameObject reference
        PC = GO_Self.GetComponent<PlayableCharacter>(); //Sets its "Playable Character" component reference for other code to use directly
        self_Srend = GO_Self.GetComponent<SpriteRenderer>();
        normalColor = self_Srend.color;
        coll2D = this.GetComponent<CircleCollider2D>();
        if (FindObjectOfType<Map>() == null)
        {
            DynMap = FindObjectOfType<DynamicMap>();
            Dyn_PlayerNmbr_StartUpSwitch();
        }

    }

    public void findPlayerArchetype(PlayableCharacter pC)
    {
        if (pC.GetComponent<PC_ShapeShifter>() != null)
        {
            pC.Sshifter = pC.GetComponent<PC_ShapeShifter>();
            pC.characterArchetype = CharacterArchetype.ShapeShifter;
            pC.Sshifter.initializeShapeShifter(pC, pC.Sshifter, pC.DynMap);
        }
        else if (pC.GetComponent<PC_Chieftain>() != null)
        {
            pC.chief = pC.GetComponent<PC_Chieftain>();
            pC.characterArchetype = CharacterArchetype.Chieftain;
            pC.chief.initializeChief(pC, pC.chief, pC.DynMap);
        }
        else if (pC.GetComponent<PC_Duelist>() != null)
        {
            pC.duel = pC.GetComponent<PC_Duelist>();
            pC.characterArchetype = CharacterArchetype.Duelist;
            pC.duel.initializeDuelist(pC, pC.duel, pC.DynMap);
        }
        else if (pC.GetComponent<PC_Headhuntress>() != null)
        {
            pC.huntress = pC.GetComponent<PC_Headhuntress>();
            pC.characterArchetype = CharacterArchetype.Headhuntress;
            pC.huntress.initializeHeadhuntress(pC, pC.huntress, pC.DynMap);
        }
        else if (pC.GetComponent<PC_Soulstealer>() != null)
        {
            pC.soul = pC.GetComponent<PC_Soulstealer>();
            pC.characterArchetype = CharacterArchetype.Soulstealer;
            pC.soul.initializeSoulstealer(pC, pC.soul, pC.DynMap);
        }
        else if (pC.GetComponent<PC_LadyLuck>() != null)
        {
            pC.lady = pC.GetComponent<PC_LadyLuck>();
            pC.characterArchetype = CharacterArchetype.LadyLuck;
            pC.lady.initializeLadyLuck(pC, pC.lady, pC.DynMap);
        }
        else
        {
            pC.characterArchetype = CharacterArchetype.genericCharacter;
        }



    }
    //Oscar Update, removed ARM+SP ARM
    void OnMouseOver() //Unity event Handler for when the mouse touches a collision box
    {
        if (Input.GetMouseButtonDown(0))
        { //if left-click WHILE over the collision box
            DetermineCharacterClickedOn();
        }
    }

    void LateUpdate() //Right click Deselect
    {
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }

    void DetermineCharacterClickedOn() //checks for Whose's turn is it, and 'selects' the character IF it is its turn
    {
        if (turnSys.pTurn == PlayerTurns.p1 && this.tag == "Player1")
        {
            //PlayerClicked();
        }
        else if (turnSys.pTurn == PlayerTurns.p2 && this.tag == "Player2")
        {
            //PlayerClicked();
        }
        else if (turnSys.pTurn == PlayerTurns.p3 && this.tag == "Player3")
        {
            //PlayerClicked();
        }
        else if (turnSys.pTurn == PlayerTurns.p4 && this.tag == "Player4")
        {
            //PlayerClicked();
        }
        else if (turnSys.pTurn == PlayerTurns.p5 && this.tag == "Player5")
        {
            //PlayerClicked();
        }
        else if (turnSys.pTurn == PlayerTurns.p6 && this.tag == "Player6")
        {
            //PlayerClicked();
        }
        else //Handles the Check for attack Targets
        {
            if ((turnSys.pTurn == PlayerTurns.p1) && (targetedByP1))
            {
                if (turnSys.currentPlayer.playerNumber == PlayerNumber.Player1)
                {
                    if (!turnSys.currentPlayer.selectedTarget)
                    {
                        turnSys.currentPlayer.selectedTarget_PC = PC;
                        colorSelectedTargetPlayer();
                        if (turnSys.currentPlayer.duel != null)
                        {
                            if (turnSys.currentPlayer.duel.DuelMode == duelMode.mobile)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                            }
                            else if (turnSys.currentPlayer.duel.DuelMode == duelMode.offense)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = turnSys.currentPlayer.duel.duel_PC.currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.duel.duel_PC.currentTile);
                            }
                        }
                        if (turnSys.currentPlayer.soul != null)
                        {
                            if (!turnSys.currentPlayer.soul.grappledThisTurn) {
                                if (turnSys.currentPlayer.soul.yeet)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                                }
                                else if (turnSys.currentPlayer.soul.yoink)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = turnSys.currentPlayer.soul.soul_PC.currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.soul.soul_PC.currentTile);
                                }
                            }
                            
                        }
                        turnSys.currentPlayer.selectedTarget = true;
                    }
                    /*
                    else {
                        if (turnSys.currentPlayer.tilesSelectedForMovement != null)
                        {
                            turnSys.currentPlayer.MoveOnPath();
                            if (!turnSys.currentPlayer.Snared)
                            {
                                turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                                selectedTarget = false;
                                PC.self_Srend.color = PC.normalColor;
                            }
                        }
                        else
                        {
                            turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                            selectedTarget = false;
                            PC.self_Srend.color = PC.normalColor;
                        }
                    }
                    */
                }

            }
            else if ((turnSys.pTurn == PlayerTurns.p2) && (targetedByP2))
            {
                if (turnSys.currentPlayer.playerNumber == PlayerNumber.Player2)
                {
                    if (!turnSys.currentPlayer.selectedTarget)
                    {
                        turnSys.currentPlayer.selectedTarget_PC = PC;
                        colorSelectedTargetPlayer();
                        if (turnSys.currentPlayer.duel != null)
                        {
                            if (turnSys.currentPlayer.duel.DuelMode == duelMode.mobile)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                            }
                            else if (turnSys.currentPlayer.duel.DuelMode == duelMode.offense)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = turnSys.currentPlayer.duel.duel_PC.currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.duel.duel_PC.currentTile);
                            }
                        }
                        if (turnSys.currentPlayer.soul != null)
                        {
                            if (!turnSys.currentPlayer.soul.grappledThisTurn)
                            {
                                if (turnSys.currentPlayer.soul.yeet)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                                }
                                else if (turnSys.currentPlayer.soul.yoink)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = turnSys.currentPlayer.soul.soul_PC.currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.soul.soul_PC.currentTile);
                                }
                            }
                        }
                        turnSys.currentPlayer.selectedTarget = true;
                    }
                    /*
                    else
                    {
                        if (turnSys.currentPlayer.tilesSelectedForMovement != null)
                        {
                            turnSys.currentPlayer.MoveOnPath();
                            if (!turnSys.currentPlayer.Snared)
                            {
                                turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                                selectedTarget = false;
                                PC.self_Srend.color = PC.normalColor;
                            }
                        }
                        else
                        {
                            turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                            selectedTarget = false;
                            PC.self_Srend.color = PC.normalColor;
                        }
                    }
                    */
                }
            }
            else if ((turnSys.pTurn == PlayerTurns.p3) && (targetedByP3))
            {
                if (turnSys.currentPlayer.playerNumber == PlayerNumber.Player3)
                {
                    if (!turnSys.currentPlayer.selectedTarget)
                    {
                        turnSys.currentPlayer.selectedTarget_PC = PC;
                        colorSelectedTargetPlayer();
                        if (turnSys.currentPlayer.duel != null)
                        {
                            if (turnSys.currentPlayer.duel.DuelMode == duelMode.mobile)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                            }
                            else if (turnSys.currentPlayer.duel.DuelMode == duelMode.offense)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = turnSys.currentPlayer.duel.duel_PC.currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.duel.duel_PC.currentTile);
                            }
                        }
                        if (turnSys.currentPlayer.soul != null)
                        {
                            if (!turnSys.currentPlayer.soul.grappledThisTurn)
                            {
                                if (turnSys.currentPlayer.soul.yeet)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                                }
                                else if (turnSys.currentPlayer.soul.yoink)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = turnSys.currentPlayer.soul.soul_PC.currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.soul.soul_PC.currentTile);
                                }
                            }
                        }
                        turnSys.currentPlayer.selectedTarget = true;
                    }
                    /*
                    else
                    {
                        if (turnSys.currentPlayer.tilesSelectedForMovement != null)
                        {
                            turnSys.currentPlayer.MoveOnPath();
                            if (!turnSys.currentPlayer.Snared)
                            {
                                turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                                selectedTarget = false;
                                PC.self_Srend.color = PC.normalColor;
                            }
                        }
                        else {
                            turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                            selectedTarget = false;
                            PC.self_Srend.color = PC.normalColor;
                        }
                        
                    }
                    */
                }
            }
            else if ((turnSys.pTurn == PlayerTurns.p4) && (targetedByP4))
            {
                if (turnSys.currentPlayer.playerNumber == PlayerNumber.Player4)
                {
                    if (!turnSys.currentPlayer.selectedTarget)
                    {
                        turnSys.currentPlayer.selectedTarget_PC = PC;
                        colorSelectedTargetPlayer();
                        if (turnSys.currentPlayer.duel != null)
                        {
                            if (turnSys.currentPlayer.duel.DuelMode == duelMode.mobile)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                            }
                            else if (turnSys.currentPlayer.duel.DuelMode == duelMode.offense)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = turnSys.currentPlayer.duel.duel_PC.currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.duel.duel_PC.currentTile);
                            }
                        }
                        if (turnSys.currentPlayer.soul != null)
                        {
                            if (!turnSys.currentPlayer.soul.grappledThisTurn)
                            {
                                if (turnSys.currentPlayer.soul.yeet)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                                }
                                else if (turnSys.currentPlayer.soul.yoink)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = turnSys.currentPlayer.soul.soul_PC.currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.soul.soul_PC.currentTile);
                                }
                            }
                        }
                        turnSys.currentPlayer.selectedTarget = true;
                    }
                    /*
                    else
                    {
                        if (turnSys.currentPlayer.tilesSelectedForMovement != null)
                        {
                            turnSys.currentPlayer.MoveOnPath();
                        }
                        turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                        selectedTarget = false;
                        PC.self_Srend.color = PC.normalColor;
                    }
                    */
                }
            }
            else if ((turnSys.pTurn == PlayerTurns.p5) && (targetedByP5))
            {
                if (turnSys.currentPlayer.playerNumber == PlayerNumber.Player5)
                {
                    if (!turnSys.currentPlayer.selectedTarget)
                    {
                        turnSys.currentPlayer.selectedTarget_PC = PC;
                        colorSelectedTargetPlayer();
                        if (turnSys.currentPlayer.duel != null)
                        {
                            if (turnSys.currentPlayer.duel.DuelMode == duelMode.mobile)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                            }
                            else if (turnSys.currentPlayer.duel.DuelMode == duelMode.offense)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = turnSys.currentPlayer.duel.duel_PC.currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.duel.duel_PC.currentTile);
                            }
                        }
                        if (turnSys.currentPlayer.soul != null)
                        {
                            if (!turnSys.currentPlayer.soul.grappledThisTurn)
                            {
                                if (turnSys.currentPlayer.soul.yeet)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                                }
                                else if (turnSys.currentPlayer.soul.yoink)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = turnSys.currentPlayer.soul.soul_PC.currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.soul.soul_PC.currentTile);
                                }
                            }
                        }
                        turnSys.currentPlayer.selectedTarget = true;
                    }
                    /*
                    else
                    {
                        if (turnSys.currentPlayer.tilesSelectedForMovement != null)
                        {
                            turnSys.currentPlayer.MoveOnPath();
                        }
                        turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                        selectedTarget = false;
                        PC.self_Srend.color = PC.normalColor;
                    }
                    */
                }
            }
            else if ((turnSys.pTurn == PlayerTurns.p6) && (targetedByP6))
            {
                if (turnSys.currentPlayer.playerNumber == PlayerNumber.Player6)
                {
                    if (!turnSys.currentPlayer.selectedTarget)
                    {
                        turnSys.currentPlayer.selectedTarget_PC = PC;
                        colorSelectedTargetPlayer();
                        if (turnSys.currentPlayer.duel != null)
                        {
                            if (turnSys.currentPlayer.duel.DuelMode == duelMode.mobile)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                            }
                            else if (turnSys.currentPlayer.duel.DuelMode == duelMode.offense)
                            {
                                turnSys.currentPlayer.duel.destinationCurrTileProps = turnSys.currentPlayer.duel.duel_PC.currentTile;
                                turnSys.currentPlayer.duel.SetDestinationNeighborsToClickable(turnSys.currentPlayer.duel.duel_PC.currentTile);
                            }
                        }
                        if (turnSys.currentPlayer.soul != null)
                        {
                            if (!turnSys.currentPlayer.soul.grappledThisTurn)
                            {
                                if (turnSys.currentPlayer.soul.yeet)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.selectedTarget_PC.currentTile);
                                }
                                else if (turnSys.currentPlayer.soul.yoink)
                                {
                                    turnSys.currentPlayer.soul.destinationCurrTileProps = turnSys.currentPlayer.soul.soul_PC.currentTile;
                                    turnSys.currentPlayer.soul.SetPullDestinationNeighborsToClickable(turnSys.currentPlayer.soul.soul_PC.currentTile);
                                }
                            }
                        }
                        turnSys.currentPlayer.selectedTarget = true;
                    }
                    /*
                    else
                    {
                        if (turnSys.currentPlayer.tilesSelectedForMovement != null)
                        {
                            turnSys.currentPlayer.MoveOnPath();
                        }
                        turnSys.currentPlayer.Agressor_ArchetypeSwitch(turnSys.currentPlayer, PC);
                        selectedTarget = false;
                        PC.self_Srend.color = PC.normalColor;
                    }
                    */
                }
            }
            else
            {
                Debug.Log("You clicked on " + this.name + " but it's not this person's turn yet"); //Just letting you know 
            }
        }
    }
    void PlayerClicked() //Puts the character in "ActionMenuMode", which will eventually do a variety of things
    {
        /*
        if (!playerActionMenuMode) //if it isn't already in "ActionMenuMode"
        {
            playerActionMenuMode = true; //Enable "ActionMenuMode"
            RangePurge();
            PathPurge();
            if (ActionPoints >= MovementCost)
            {
                moveMenuMode = true;
                tentativeActionPoints = ActionPoints;
                //attackMenuMode = false;
                AttackButton.gameObject.SetActive(false);
                EndTurnButton.gameObject.SetActive(false);
                MineButton.gameObject.SetActive(false);
                HealButton.gameObject.SetActive(false);
                UpgradeButton.gameObject.SetActive(false);
                PathfinderStart();
                SetNeighborsToClickable(currentTile);
            }
            else if (ActionPoints == 0)
            {
                Debug.Log("You have no Action Points Remaining");
                Deselect(); //disables all the changes that came with the "FindValidMovement" interactions, and disables "ActionMenuMode" 
            }
        }
        */
        //Debug.Log("You clicked on " + this.name + " which you should be able to move");
    }

    void Dyn_PlayerNmbr_StartUpSwitch() //[using DynamicMap]sets the startup positions for all the characters on the map [not using Start() due to rush cases]
    {
        Tile_Properties temp_T_P; //Local variable that stores reference to the "TileProperties" code in the tile the character is going to be ON during initialization

        if (GO_Self.tag == "Player1")
        {
            playerNumber = PlayerNumber.Player1;
            GO_Self.name = "HumanPlayer 1"; //changes the Name of the instance (just a neat trick to know if the code was running properly)
            for (int i = 0; i < DynMap.Bases.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Bases[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.Base) && (temp_T_P.ownedBy == OwnedBy.p1)) //if 'Base' and player number MATCH then move the player there
                {
                    currentTile = DynMap.Bases[i].GetComponent<Tile_Properties>(); //sets the currentTile to the starting position
                    temp_T_P.whoIsOnTile = isOnTile.p1; //Tells that tile that the player is on it
                    currentTile.playerOnTile = PC;
                    temp_T_P.setOwner(PC);
                    currentTile.Movable = false;
                    GO_Self.transform.position = currentTile.transform.position; //Moves the player to it
                }

                temp_T_P = null; //empties the variable to prevent memory leaks
            }
            for (int i = 0; i < DynMap.Basewalls.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Basewalls[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.BaseWall) && (temp_T_P.ownedBy == OwnedBy.p1)) //if 'Base' and player number MATCH then move the player there
                {
                    temp_T_P.setOwner(PC);
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
        }
        else if (GO_Self.tag == "Player2")
        {
            playerNumber = PlayerNumber.Player2;
            GO_Self.name = "HumanPlayer 2"; //changes the Name of the instance (just a neat trick to know if the code was running properly)
            for (int i = 0; i < DynMap.Bases.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Bases[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.Base) && (temp_T_P.ownedBy == OwnedBy.p2)) //if 'Base' and player number MATCH then move the player there
                {
                    currentTile = DynMap.Bases[i].GetComponent<Tile_Properties>(); //sets the currentTile to the starting position
                    temp_T_P.whoIsOnTile = isOnTile.p2; //Tells that tile that the player is on it
                    currentTile.playerOnTile = PC;
                    temp_T_P.setOwner(PC);
                    currentTile.Movable = false;
                    GO_Self.transform.position = currentTile.transform.position; //Moves the player to it
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
            for (int i = 0; i < DynMap.Basewalls.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Basewalls[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.BaseWall) && (temp_T_P.ownedBy == OwnedBy.p2)) //if 'Base' and player number MATCH then move the player there
                {
                    temp_T_P.setOwner(PC);
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
        }
        else if (GO_Self.tag == "Player3")
        {
            playerNumber = PlayerNumber.Player3;
            GO_Self.name = "HumanPlayer 3"; //changes the Name of the instance (just a neat trick to know if the code was running properly)
            for (int i = 0; i < DynMap.Bases.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Bases[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.Base) && (temp_T_P.ownedBy == OwnedBy.p3)) //if 'Base' and player number MATCH then move the player there
                {
                    currentTile = DynMap.Bases[i].GetComponent<Tile_Properties>(); //sets the currentTile to the starting position
                    temp_T_P.whoIsOnTile = isOnTile.p3; //Tells that tile that the player is on it
                    currentTile.playerOnTile = PC;
                    temp_T_P.setOwner(PC);
                    currentTile.Movable = false;
                    GO_Self.transform.position = currentTile.transform.position; //Moves the player to it
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
            for (int i = 0; i < DynMap.Basewalls.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Basewalls[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.BaseWall) && (temp_T_P.ownedBy == OwnedBy.p3)) //if 'Base' and player number MATCH then move the player there
                {
                    temp_T_P.setOwner(PC);
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
        }
        else if (GO_Self.tag == "Player4")
        {
            playerNumber = PlayerNumber.Player4;
            GO_Self.name = "HumanPlayer 4"; //changes the Name of the instance (just a neat trick to know if the code was running properly)
            for (int i = 0; i < DynMap.Bases.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Bases[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.Base) && (temp_T_P.ownedBy == OwnedBy.p4)) //if 'Base' and player number MATCH then move the player there
                {
                    currentTile = DynMap.Bases[i].GetComponent<Tile_Properties>(); //sets the currentTile to the starting position
                    temp_T_P.whoIsOnTile = isOnTile.p4; //Tells that tile that the player is on it
                    temp_T_P.setOwner(PC);
                    currentTile.playerOnTile = PC;
                    currentTile.Movable = false;
                    GO_Self.transform.position = currentTile.transform.position; //Moves the player to it    
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
            for (int i = 0; i < DynMap.Basewalls.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Basewalls[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.BaseWall) && (temp_T_P.ownedBy == OwnedBy.p4)) //if 'Base' and player number MATCH then move the player there
                {
                    temp_T_P.setOwner(PC);
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
        }
        else if (GO_Self.tag == "Player5")
        {
            playerNumber = PlayerNumber.Player5;
            GO_Self.name = "HumanPlayer 5"; //changes the Name of the instance (just a neat trick to know if the code was running properly)
            for (int i = 0; i < DynMap.Bases.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Bases[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.Base) && (temp_T_P.ownedBy == OwnedBy.p5)) //if 'Base' and player number MATCH then move the player there
                {
                    currentTile = DynMap.Bases[i].GetComponent<Tile_Properties>(); //sets the currentTile to the starting position
                    temp_T_P.whoIsOnTile = isOnTile.p5; //Tells that tile that the player is on it
                    currentTile.playerOnTile = PC;
                    temp_T_P.setOwner(PC);
                    currentTile.Movable = false;
                    GO_Self.transform.position = currentTile.transform.position; //Moves the player to it
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
            for (int i = 0; i < DynMap.Basewalls.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Basewalls[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.BaseWall) && (temp_T_P.ownedBy == OwnedBy.p5)) //if 'Base' and player number MATCH then move the player there
                {
                    temp_T_P.setOwner(PC);
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
        }
        else if (GO_Self.tag == "Player6")
        {
            playerNumber = PlayerNumber.Player6;
            GO_Self.name = "HumanPlayer 6"; //changes the Name of the instance (just a neat trick to know if the code was running properly)
            for (int i = 0; i < DynMap.Bases.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Bases[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.Base) && (temp_T_P.ownedBy == OwnedBy.p6)) //if 'Base' and player number MATCH then move the player there
                {
                    currentTile = DynMap.Bases[i].GetComponent<Tile_Properties>(); //sets the currentTile to the starting position
                    temp_T_P.whoIsOnTile = isOnTile.p6; //Tells that tile that the player is on it
                    currentTile.playerOnTile = PC;
                    temp_T_P.setOwner(PC);
                    currentTile.Movable = false;
                    GO_Self.transform.position = currentTile.transform.position; //Moves the player to it
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
            for (int i = 0; i < DynMap.Basewalls.Count; i++) //looks through all the 'Base' tiles in the map, to find the corresponding one
            {
                temp_T_P = DynMap.Basewalls[i].GetComponent<Tile_Properties>(); //gets its properties
                if ((temp_T_P.tType == tileType.BaseWall) && (temp_T_P.ownedBy == OwnedBy.p6)) //if 'Base' and player number MATCH then move the player there
                {
                    temp_T_P.setOwner(PC);
                }
                temp_T_P = null; //empties the variable to prevent memory leaks
            }
        }
        else
        {
            Debug.Log(this.name + "ran into an unknown problem"); // there really shouldn't be a reason for this case to ever trigger... but I have it so that it softlocks rather than crash
        }
        findPlayerArchetype(PC);
    }

    public void NeighborPurge() //There is a bug with removing some of the color
    {
        if (tilesSelectedForMovement.Count > 0)
        {
            for (int i = 0; i < tilesSelectedForMovement.Count; i++)
            {
                for (int p = 0; p < 6; p++)
                {
                    if (!tilesSelectedForMovement[i].individualNeighborsList[p].selectedForMovement && tilesSelectedForMovement[i].individualNeighborsList[p].checkedbyPathfinder)
                    {
                        tilesSelectedForMovement[i].individualNeighborsList[p].coll2D.enabled = false;
                        tilesSelectedForMovement[i].individualNeighborsList[p].Srend.color = new Color(0f, 0f, 0.7f, 1f);
                    }

                }
                if (i == (tilesSelectedForMovement.Count - 1))
                {
                    tilesSelectedForMovement[i].coll2D.enabled = false;
                    tilesSelectedForMovement[i].Srend.color = new Color(0.19f, 0.2f, 0.3f + (i * 0.1f), 1f);
                }
            }
        }

    }

    public void PathPurge()
    {
        tentativeActionPoints = ActionPoints;
        for (int i = 0; i < movableTiles.Count; i++)
        {
            movableTiles[i].coll2D.enabled = false;
            movableTiles[i].distanceTo = int.MaxValue;
            movableTiles[i].Srend.color = movableTiles[i].tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
            movableTiles[i].tempPlayableCharacter = null;
            movableTiles[i].Movable = true;
            movableTiles[i].checkedbyPathfinder = false;
            movableTiles[i].selectedForMovement = false;
            tilesSelectedForMovement = new List<Tile_Properties>();

            if (currentTile != null)
            {
                currentTile.Movable = false;
            }
        }
        movableTiles = new List<Tile_Properties>();
    }

    void PathfinderStart()
    {
        PathPurge();
        pathfindQueue = new List<Tile_Properties>();
        movableTiles = new List<Tile_Properties>();
        currentTile.distanceTo = 0;
        pathfindQueue.Add(currentTile);
        movableTiles.Add(currentTile);
        currentTile.checkedbyPathfinder = true;
        pathfindQueue[0].Pathfinder(PC);
    }

    public void PathInvoke()
    {
        Invoke("Pathfinder", 0.1f);
    }

    public void Pathfinder()
    {
        int p = pathfindQueue.Count;
        pathfindQueue.Remove(pathfindQueue[0]);
        if (pathfindQueue.Count != 0)
        {
            if (pathfindQueue[0].distanceTo < (ActionPoints / MovementCost))
            {
                pathfindQueue[0].Pathfinder(PC);
            }
            else
            {
                Pathfinder();
            }
        }
    }

    public void SetNeighborsToClickable(Tile_Properties tprop)
    {
        for (int i = 0; i < 6; i++)
        {
            if (tprop.individualNeighborsList[i] != null)
            {
                if (tprop.individualNeighborsList[i].Movable)
                {
                    if (tprop.individualNeighborsList[i].checkedbyPathfinder && !tprop.individualNeighborsList[i].selectedForMovement)
                    {
                        if (ActionPoints - (tilesSelectedForMovement.Count * MovementCost) > AttackActionCost)
                        {
                            tprop.individualNeighborsList[i].Srend.color = new Color(0.6f, 0f, 0.6f, 1f); //sets the color of a valid neighbor to a purple-ish color to indicate that you can move there and still attack
                        }
                        else
                        {
                            tprop.individualNeighborsList[i].Srend.color = new Color(0f, 0.7f, 1f, 1f); //sets the color of a valid neighbor to a lightblue color to indicate that you can continue making a path but you won't be able to attack from there
                        }
                        tprop.individualNeighborsList[i].coll2D.enabled = true;
                    }
                }

            }

        }
    }

    public void RangeSet()
    {

        for (int i = 0; i < DynMap.gridY; i++)
        {
            for (int j = 0; j < DynMap.gridX; j++)
            {
                if (!(DynMap.XY_TilePropArray[j, i].tType == tileType.Inactive || DynMap.XY_TilePropArray[j, i].tType == tileType.Base))
                {
                    int difX = currentTile.X - DynMap.XY_TilePropArray[j, i].X;
                    int difY = currentTile.Y - DynMap.XY_TilePropArray[j, i].Y;
                    int difZ = currentTile.Z - DynMap.XY_TilePropArray[j, i].Z;
                    DynMap.XY_TilePropArray[j, i].rangeTo = (Mathf.Abs(difX) + Mathf.Abs(difY) + Mathf.Abs(difZ)) / 2;
                }
                if (DynMap.XY_TilePropArray[j, i].Sightline)
                {
                    DynMap.XY_TilePropArray[j, i].Sightline = false;
                    DynMap.XY_TilePropArray[j, i].relativeDirection = int.MaxValue;
                    DynMap.XY_TilePropArray[j, i].Srend.color = DynMap.XY_TilePropArray[j, i].tileDefaultColor;
                }

            }
        }
        SightlineSet();
    }

    public void SightlineSet()
    {
        if (turnSys.currentPlayer.huntress != null)
        {
            if (turnSys.currentPlayer.attackMenuMode)
            {
                RangePurge();
                attackableTiles = new List<Tile_Properties>();
            }
        }
        for (int i = 0; i < 6; i++)
        {
            Tile_Properties tempProps = currentTile.individualNeighborsList[i];
            while (tempProps != null)
            {
                if (tempProps.tType == tileType.Inactive)
                {
                    break;
                }
                else
                {
                    if (tempProps.rangeTo <= AttackRange)
                    {
                        tempProps.Sightline = true;
                        tempProps.relativeDirection = i;
                        if (turnSys.currentPlayer.huntress != null)
                        {

                            //tempProps.Srend.color = new Color(1f, 0.7f, 1f, 1f); //PeptoBismol Color
                            if (turnSys.currentPlayer.attackMenuMode)
                            {
                                attackableTiles.Add(tempProps);
                            }
                        }
                    }
                    tempProps = tempProps.individualNeighborsList[i]; //this MUST be outside the IF, because if it can't look for the next tile the loop runs forever, thus not allowing the game to continue...
                }
            }
        }
        if (turnSys.currentPlayer.huntress != null)
        {
            if (turnSys.currentPlayer.attackMenuMode)
            {
                checkAttackableTiles();
            }
        }
    }

    public void RangePurge()
    {
        for (int i = 0; i < attackableTiles.Count; i++)
        {
            attackableTiles[i].Srend.color = attackableTiles[i].tileDefaultColor; //validNeighborList[i].Srend.color = new Color(movableTiles[i].Srend.color.r, validNeighborList[i].Srend.color.g, validNeighborList[i].Srend.color.b, 1f);
            attackableTiles[i].tempPlayableCharacter = null;
            attackableTiles[i].checkedbyRangefinder = false;
        }
        attackableTiles = new List<Tile_Properties>();
    }

    public void RangefinderStart()
    {
        RangePurge();
        rangefindQueue = new List<Tile_Properties>();
        attackableTiles = new List<Tile_Properties>();
        rangefindQueue.Add(currentTile);
        attackableTiles.Add(currentTile);
        currentTile.checkedbyRangefinder = true;
        rangefindQueue[0].Rangefinder(PC);
    }

    public void RangeInvoke()
    {
        Invoke("Rangefinder", 0.1f);
    }

    public void Rangefinder()
    {
        rangefindQueue.Remove(rangefindQueue[0]);
        if (rangefindQueue.Count != 0)
        {
            rangefindQueue[0].Rangefinder(PC);
        }
        else
        {
            if (turnSys.currentPlayer.soul != null)
            {
                if (turnSys.currentPlayer.soul.grappleMode)
                {
                    turnSys.currentPlayer.soul.checkPullTiles();
                }
                else
                {
                    checkAttackableTiles();
                }
            }
            else
            {
                checkAttackableTiles();
            }
            
        }
    }

    /*
    //This one is the one that works on Movement, I currently disabled it's ability to confirm an attack
    public void stepByStepAttackableTiles(int movementStretch) //[BUGGY, WORK IN PROGRESS]
    {
        //targetList = new List<PlayableCharacter>();
        int EffectiveRange = 0;
        int AttackStretch = 0;
        EffectiveRange = ((ActionPoints - AttackActionCost) / MovementCost);
        AttackStretch = (movementStretch);
        if (EffectiveRange > 0)
        {
            for (int i = 0; i < attackableTiles.Count; i++)
            {
                if (attackableTiles[i].rangeTo <= (AttackRange + EffectiveRange)) //Tiles that WOULD be in direct attack range, should you move, are colored pink and have disabled the targeting component
                {
                    if (attackableTiles[i].tType != tileType.Base && attackableTiles[i].rangeTo > 0)
                    {
                        attackableTiles[i].Srend.color = new Color(1f, 0.7f, 1f, 1f);
                        if (attackableTiles[i].whoIsOnTile != isOnTile.empty)
                        {
                            attackableTiles[i].Srend.color = new Color(1, 0.7f, 1, 0.8f);
                        }

                    }
                }
                if (attackableTiles[i].rangeTo <= AttackRange + movementStretch) // this is the part that's killing me... I need to change RangeSet in order to do this correctly
                {
                    if (attackableTiles[i].tType != tileType.Base)
                    {
                        if (attackableTiles[i].rangeTo > 0)
                        {
                            if (attackableTiles[i].Sightline)
                            {
                                attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                                if ((attackableTiles[i].whoIsOnTile != isOnTile.empty))
                                {
                                    //occupiedBy(attackableTiles[i].tile_self);
                                    attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                                }
                            }
                            else if ((!attackableTiles[i].Sightline) && (attackableTiles[i].tType != tileType.Mine))
                            {
                                attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                                if ((attackableTiles[i].whoIsOnTile != isOnTile.empty))
                                {
                                    //occupiedBy(attackableTiles[i].tile_self);
                                    attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < attackableTiles.Count; i++)
            {
                if (attackableTiles[i].tType != tileType.Base)
                {
                    if (attackableTiles[i].rangeTo > 0)
                    {
                        if (attackableTiles[i].Sightline)
                        {
                            attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                            if (attackableTiles[i].whoIsOnTile != isOnTile.empty)
                            {
                                //occupiedBy(attackableTiles[i].tile_self);
                                attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                            }
                        }
                        else if ((!attackableTiles[i].Sightline) && (attackableTiles[i].tType != tileType.Mine))
                        {
                            attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                            if (attackableTiles[i].whoIsOnTile != isOnTile.empty)
                            {
                                //occupiedBy(attackableTiles[i].tile_self);
                                attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                            }
                        }
                    }
                }
            }
        }

    }
    */

    //This one is called when you use the "Attack" Button and is completely standard (seriously this one works OK don't fuck with it... unless I'm wrong XD)
    public void checkAttackableTiles() //[COMPLETE]
    {
        int EffectiveRange = 0;
        EffectiveRange = ((ActionPoints - AttackActionCost) / MovementCost); //the effective range calculation simply allows you to see what you could target if you moved by that amount
        if (EffectiveRange > 0)
        {
            for (int i = 0; i < attackableTiles.Count; i++)
            {
                if (attackableTiles[i].rangeTo <= (AttackRange + EffectiveRange)) //Tiles that WOULD be in direct attack range, should you move, are colored pink and have disabled the targeting component
                {
                    if (attackableTiles[i].tType != tileType.Base && attackableTiles[i].rangeTo > 0)
                    {
                        attackableTiles[i].Srend.color = new Color(1f, 0.7f, 1f, 1f);
                        if (attackableTiles[i].whoIsOnTile != isOnTile.empty)
                        {
                            attackableTiles[i].Srend.color = new Color(1, 0.7f, 1, 0.8f);
                        }

                    }
                }
                if (attackableTiles[i].rangeTo <= AttackRange)
                {
                    if (attackableTiles[i].tType != tileType.Base)
                    {
                        if (attackableTiles[i].rangeTo > 0)
                        {
                            if (attackableTiles[i].Sightline)
                            {
                                attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                                if ((attackableTiles[i].whoIsOnTile != isOnTile.empty))
                                {
                                    occupiedBy(attackableTiles[i].tile_self);
                                    attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                                }
                            }
                            else if ((!attackableTiles[i].Sightline) && (attackableTiles[i].tType != tileType.Mine))
                            {
                                attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                                if ((attackableTiles[i].whoIsOnTile != isOnTile.empty))
                                {
                                    occupiedBy(attackableTiles[i].tile_self);
                                    attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                                }
                            }
                        }
                    }
                }

            }
        }
        else
        {
            for (int i = 0; i < attackableTiles.Count; i++)
            {
                if (attackableTiles[i].tType != tileType.Base)
                {
                    if (attackableTiles[i].rangeTo > 0)
                    {
                        if (attackableTiles[i].Sightline)
                        {
                            attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                            if (attackableTiles[i].whoIsOnTile != isOnTile.empty)
                            {
                                occupiedBy(attackableTiles[i].tile_self);
                                attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                            }
                        }
                        else if ((!attackableTiles[i].Sightline) && (attackableTiles[i].tType != tileType.Mine))
                        {
                            attackableTiles[i].Srend.color = new Color(1f, 0, 0, 0.8f);

                            if (attackableTiles[i].whoIsOnTile != isOnTile.empty)
                            {
                                occupiedBy(attackableTiles[i].tile_self);
                                attackableTiles[i].Srend.color = new Color(1, 0, 0, 0.6f);
                            }
                        }
                    }
                }
            }
        }

    }

    void colorSelectedTargetPlayer()
    {
        PC.self_Srend.color = new Color(0.9f, 0.9f, 0.9f, 1);
    }


    public void MoveOnPath()
    {
        for (int i = 0; i < tilesSelectedForMovement.Count; i++)
        {

            if (!Snared)
            {
                tileChosen = null;
                tileChosen = tilesSelectedForMovement[i];
                MoveFunctionPt1();
            }
            else
            {
                break;
            }

        }
        MoveFunctionPt2();
        Deselect();
        if (ActionPoints == 0)
        {
            Debug.Log("You have no Action Points Remaining");
            Deselect();
        }

    }

    public void MoveFunctionPt1()
    {
        if (!Snared)
        {
            currentTile.whoIsOnTile = isOnTile.empty;
            currentTile.playerOnTile = null;
            currentTile = null;
            ActionPoints = ActionPoints - MovementCost;//ActionPoints = ActionPoints - (MovementCost * tileChosen.distanceTo);
            /*
            if (Bleeding)
            {
                if (Health - MovementCost >= 1)
                {
                    Health = Health - MovementCost;
                    Debug.Log(PC.name + " took " + MovementCost + " points of Bleed Damage");
                }
                else
                {
                    Health = 1;
                    Debug.Log(PC.name + " can't take any more points of Bleed Damage");
                }
            }
            */
            currentTile = tileChosen;
            if (tileChosen.tag != "Base")
            {
                Invulnerable = false;
            }
            else
            {
                Invulnerable = true;
                if (Cursed)
                {
                    Cursed = false;
                    CurseLevel = 0;
                    CurseStorage = 0;
                    Debug.Log("You have purged the curse afflicting you");
                }
                if (Bleeding)
                {
                    Bleeding = false;
                    BleedSeverity = 0;
                    Debug.Log("You are no longer bleeding.");
                }
            }
            if (tileChosen.tag != "Highland")
            {
                if ((specialDamageBonus - 2)>0) //it was previously set to 0 any Special Damge if you got moved out of the Highland, so I changed it to a substraction
                {
                    specialDamageBonus = specialDamageBonus - 2;
                }
                else if ((specialDamageBonus -2)<= 0) // if the subtraction is less than or equal to 0, make it 0
                {
                    specialDamageBonus = 0;
                }

                if ((specialArmor - 2)>0) //it was previously set to 0 any special defense if you got moved out of the Highland, so I changed it to a substraction
                {
                    specialArmor = specialArmor - 2;
                }
                else if ((specialArmor - 2) <= 0) // if the subtraction is less than or equal to 0, make it 0
                {
                    specialArmor = 0;
                }
            }
            else
            {
                if (specialDamageBonus <= 0) {
                    specialDamageBonus = specialDamageBonus + 2;
                }
                if (specialArmor <= 0) {
                    specialArmor = specialArmor + 2;
                }
                
            }
            if (tileChosen.trapOnTile)
            {
                Snared = true;
                currentTile.trapOnTile = false;
                snaredTurns = 2;
                MovementCost = int.MaxValue;
                if (!Bleeding)
                {
                    Bleeding = true;
                    BleedTurnsRemaining = 3;
                    BleedSeverity = 2;
                }
                else
                {
                    BleedTurnsRemaining = BleedTurnsRemaining + 2;
                    BleedSeverity++;
                }
                if (Cursed)
                {
                    Cursed = false;
                    Health = Health - (CurseStorage / 2);
                    //[Oscar note] heal chieftain for the other half
                }
                //Deselect();
                //Debug.Log("You are trapped. Not big surprise.");
            }
            tileChosen.characterSwitch(PC);
            this.transform.position = tileChosen.transform.position;
        }
        else
        {
            Debug.Log("You can't move, you're snared");
            Deselect();
        }
    }

    public void MoveFunctionPt2()
    {
        PathPurge();
        RangeSet();
        
    }

    public void Deselect()
    {
        turnSys.currentPlayer.archetypeAbilityButtonEnableSwitch();
        if (playerActionMenuMode || moveMenuMode || attackMenuMode)
        {
            PathPurge();
            RangePurge();
            if (targetList.Count > 0)
            {
                Untarget();
            }
            playerActionMenuMode = false;
            Debug.Log("Action Menu Mode disabled");
        }

        if (turnSys.currentPlayer.chief != null)
        {
            if (turnSys.currentPlayer.chief.trapMode)
            {
                turnSys.currentPlayer.chief.TrapModeDisable();
            }
        }
        else if (turnSys.currentPlayer.huntress != null)
        {
            if (turnSys.currentPlayer.huntress.droneMode)
            {
                turnSys.currentPlayer.huntress.DroneModeDisable();
            }
        }
        else if (turnSys.currentPlayer.duel != null)
        {
            if (turnSys.currentPlayer.duel.DuelMode != duelMode.defense)
            {
                if (turnSys.currentPlayer.selectedTarget_PC != null)
                {
                    turnSys.currentPlayer.duel.PurgeDestinationNeighbors(turnSys.currentPlayer.duel.destinationCurrTileProps);
                } 
            }
        }
        else if (turnSys.currentPlayer.soul != null)
        {
            if (turnSys.currentPlayer.soul.grappleMode)
            {
                turnSys.currentPlayer.soul.grappleModeDisable();
            }
        }
        else
        {
            //Debug.Log("Not in action menu mode anyways");
        }
        
        if (moveMenuMode)
        {
            moveMenuMode = false;
        }
        if (attackMenuMode)
        {
            attackMenuMode = false;
        }
        AttackButton.gameObject.SetActive(true);
        MoveButton.gameObject.SetActive(true);
        EndTurnButton.gameObject.SetActive(true);
        MineButton.gameObject.SetActive(true);
        HealButton.gameObject.SetActive(true);
        UpgradeButton.gameObject.SetActive(true);
    }

    public void Untarget()
    {
        //RangePurge();
        if (targetList != null)
        {
            for (int j = 0; j < targetList.Count; j++)
            {

                DisableTargetByPlayerSwitch(j);
                selectedTarget_PC = null;
                targetList[j] = null;
            }
        }
        //enemiesInRange = new List<Tile_Properties>();
        targetList = new List<PlayableCharacter>();
    }

    void DisableTargetByPlayerSwitch(int num)
    {
        switch (turnSys.pTurn)
        {
            case PlayerTurns.p1:
                {
                    targetList[num].targetedByP1 = false;
                    targetList[num].selectedTarget = false;
                    targetList[num].self_Srend.color = targetList[num].normalColor;
                    break;
                }
            case PlayerTurns.p2:
                {
                    targetList[num].targetedByP2 = false;
                    targetList[num].selectedTarget = false;
                    targetList[num].self_Srend.color = targetList[num].normalColor;
                    break;
                }
            case PlayerTurns.p3:
                {
                    targetList[num].targetedByP3 = false;
                    targetList[num].selectedTarget = false;
                    targetList[num].self_Srend.color = targetList[num].normalColor;
                    break;
                }
            case PlayerTurns.p4:
                {
                    targetList[num].targetedByP4 = false;
                    targetList[num].selectedTarget = false;
                    targetList[num].self_Srend.color = targetList[num].normalColor;
                    break;
                }
            case PlayerTurns.p5:
                {
                    targetList[num].targetedByP5 = false;
                    targetList[num].selectedTarget = false;
                    targetList[num].self_Srend.color = targetList[num].normalColor;
                    break;
                }
            case PlayerTurns.p6:
                {
                    targetList[num].targetedByP6 = false;
                    targetList[num].selectedTarget = false;
                    targetList[num].self_Srend.color = targetList[num].normalColor;
                    break;
                }
        }
    }

    void EnableTargetByPlayerSwitch(int num)
    {
        switch (turnSys.pTurn)
        {
            case PlayerTurns.p1:
                {
                    targetList[num].targetedByP1 = true;
                    break;
                }
            case PlayerTurns.p2:
                {
                    targetList[num].targetedByP2 = true;
                    break;
                }
            case PlayerTurns.p3:
                {
                    targetList[num].targetedByP3 = true;
                    break;
                }
            case PlayerTurns.p4:
                {
                    targetList[num].targetedByP4 = true;
                    break;
                }
            case PlayerTurns.p5:
                {
                    targetList[num].targetedByP5 = true;
                    break;
                }
            case PlayerTurns.p6:
                {
                    targetList[num].targetedByP6 = true;
                    break;
                }
        }
    }

    public void EndTurnConditions()
    {
        //EndTurn Brace [funnily enough we accidentally made it a 0 turn cooldown ability ^_^ ]
        if (ActionPoints != 0)
        {
            RNGeezus.braceRoll(PC, ActionPoints);
            specialArmor += RNGeezus.Result;
            if (Bleeding)
            {
                bleedTick();
            }
            ActionPoints = 0;
        }
        if (Bleeding)
        {
            if (BleedSeverity == 0)
            {
                Bleeding = false;
                Debug.Log("The bleeding has stopped");
            }
        }
        if (Snared)
        {
            snaredTurns--;
            if (snaredTurns == 0)
            {
                Snared = false;
                MovementCost = 1;
                if (Sshifter != null)
                {
                    if (!Sshifter.Predator)
                    {
                        MovementCost = 2;
                    }

                }
            }
        }
        EndTurnArchetypeSwitch();
        ActionPoints = MAX_ActionPoints; // transfer this to StartTurnConditions()
        if (playerActionMenuMode || moveMenuMode || attackMenuMode)
        {
            Deselect();
        }
        //outer ring, Lowland/Base Wall
        if ((currentTile.tType == tileType.Lowland) || (currentTile.tType == tileType.BaseWall))
        {
            if ((VictoryPoints + 1) >= MAX_VictoryPoints)
            {
                Debug.Log("You won the game!");
            }
            else
            {
                VictoryPoints++;
            }
        }
        //Sanctuary
        else if (currentTile.tType == tileType.Sanctuary)
        {
            if ((VictoryPoints + 1) >= MAX_VictoryPoints)
            {
                Debug.Log("You won the game!");
            }
            else
            {
                VictoryPoints++;
                if ((Health + 3) > MAX_Health)
                {
                    Debug.Log("You don't need healing");
                }
                else
                {
                    Health += 3;
                }

            }
        }
        //Quarry
        else if (currentTile.tType == tileType.Quarry)
        {
            if ((VictoryPoints + 1) >= MAX_VictoryPoints)
            {
                Debug.Log("You won the game!");
            }
            else
            {
                VictoryPoints++;
                playerMoney += 50;
            }
        }
        //Midland
        else if (currentTile.tType == tileType.Midland)
        {
            if ((VictoryPoints + 1) >= MAX_VictoryPoints)
            {
                Debug.Log("You won the game!");
            }
            else
            {
                VictoryPoints += 2;
            }
        }
        //Highland
        else if (currentTile.tType == tileType.Highland)
        {
            if ((VictoryPoints + 1) >= MAX_VictoryPoints)
            {
                Debug.Log("You won the game!");
            }
            else
            {
                VictoryPoints += 3;
            }
            RNGeezus.mineRoll(1);
            playerMoney = RNGeezus.Result;
            RNGeezus.healRoll(1);
            if ((Health + RNGeezus.Result) >= MAX_Health)
            {
                Health = MAX_Health;
                Debug.Log("You have been healed to" + Health + " + " + RNGeezus.Result);
            }
            else
            {
                Health = Health + RNGeezus.Result;
            }
        }
        else if ((currentTile.tType == tileType.Mine))
        {
            playerMoney = playerMoney + 100;
            Debug.Log("You find an additional 100 Resources in the mine");
        }
        else if ((currentTile.tag == "Base"))
        {
            overhealCheck(6);
        }
        self_Srend.color = normalColor;
    }

    public void EndTurnArchetypeSwitch()
    {
        switch (characterArchetype)
        {
            case (CharacterArchetype.ShapeShifter):
                {
                    Sshifter.Shapeshifter_EndTurnConditions();
                    break;
                }
            case (CharacterArchetype.Chieftain):
                {
                    chief.Chieftain_EndTurnConditions();
                    break;
                }
            case (CharacterArchetype.LadyLuck):
                {
                    lady.LadyLuck_EndTurnConditions();
                    break;
                }
            case (CharacterArchetype.Headhuntress):
                {
                    huntress.Headhuntress_EndTurnConditions();
                    break;
                }
            case (CharacterArchetype.Duelist):
                {
                    duel.Duelist_EndTurnConditions();
                    break;
                }
            case (CharacterArchetype.Soulstealer):
                {
                    soul.Soulstealer_EndTurnConditions();
                    break;
                }
        }
    }

    public void StartTurnConditions()
    {
        RangeSet();
        ActionPoints = MAX_ActionPoints;
        //Makes all buttons Active by default at the beginning of a turn
        AttackButton.gameObject.SetActive(true);
        EndTurnButton.gameObject.SetActive(true);
        MineButton.gameObject.SetActive(true);
        HealButton.gameObject.SetActive(true);
        UpgradeButton.gameObject.SetActive(true);
        if (PC.currentTile.tag != "Highland")
        {
            specialArmor = 0;
        }
        else
        {
            specialArmor = 2;
        }
        //self_Srend.color = new Color(1f, 0.5f, 0.5f, 1);
        StartTurnArchetypeSwitch();
        curseTick();
    }

    public void StartTurnArchetypeSwitch()
    {
        switch (characterArchetype)
        {
            case (CharacterArchetype.ShapeShifter):
                {
                    Sshifter.Shapeshifter_StartTurnConditions();
                    break;
                }
            case (CharacterArchetype.Chieftain):
                {
                    chief.Chieftain_StartTurnConditions();
                    break;
                }
            case (CharacterArchetype.LadyLuck):
                {
                    lady.LadyLuck_StartTurnConditions();
                    break;
                }
            case (CharacterArchetype.Headhuntress):
                {
                    huntress.Headhuntress_StartTurnConditions();
                    break;
                }
            case (CharacterArchetype.Duelist):
                {
                    duel.Duelist_StartTurnConditions();
                    break;
                }
            case (CharacterArchetype.Soulstealer):
                {
                    soul.Soulstealer_StartTurnConditions();
                    break;
                }
        }
    }

    public void AttackAction()
    {
        if (ActionPoints >= AttackActionCost)
        {
            switch (turnSys.pTurn)
            {
                case PlayerTurns.errorhandler:
                    {
                        Debug.Log("Not A valid Player turn");
                        break;
                    }
                case PlayerTurns.p1:
                    {
                        if (PC.playerNumber == PlayerNumber.Player1)
                        {
                            AttackCompressed();
                        }

                        break;
                    }
                case PlayerTurns.p2:
                    {
                        if (PC.playerNumber == PlayerNumber.Player2)
                        {
                            AttackCompressed();
                        }
                        break;
                    }
                case PlayerTurns.p3:
                    {
                        if (PC.playerNumber == PlayerNumber.Player3)
                        {
                            AttackCompressed();
                        }
                        break;
                    }
                case PlayerTurns.p4:
                    {
                        if (PC.playerNumber == PlayerNumber.Player4)
                        {
                            AttackCompressed();
                        }
                        break;
                    }
                case PlayerTurns.p5:
                    {
                        if (PC.playerNumber == PlayerNumber.Player5)
                        {
                            AttackCompressed();
                        }
                        break;
                    }
                case PlayerTurns.p6:
                    {
                        if (PC.playerNumber == PlayerNumber.Player6)
                        {
                            AttackCompressed();
                        }
                        break;
                    }
            }
        }
        else
        {
            Debug.Log("Not enough action points");
        }

    }
    void AttackCompressed() {
        if (duel == null)
        {
            if (!attackMenuMode)
            {
                AttackClean();
            }
            else if ((attackMenuMode) && (selectedTarget_PC != null))
            {
                Agressor_ArchetypeSwitch(PC, selectedTarget_PC);
                selectedTarget = false;
                selectedTarget_PC.self_Srend.color = selectedTarget_PC.normalColor;
                selectedTarget_PC = null;
                Deselect();
            }
            else
            {
                Debug.Log("Please select a Target");
            }
        }
        else {
            if (duel.DuelMode != duelMode.defense)
            {
                if (!attackMenuMode)
                {
                    AttackClean();
                }
                else if ((attackMenuMode) && (selectedTarget_PC != null))
                {
                    if (selectedTarget_PC.currentTile.rangeTo == 2)
                    {
                        if (duel.attackDestination != null)
                        {
                            Agressor_ArchetypeSwitch(PC, selectedTarget_PC);
                            selectedTarget = false;
                            selectedTarget_PC.self_Srend.color = selectedTarget_PC.normalColor;
                            selectedTarget_PC = null;
                            Deselect();
                        }
                        else
                        {
                            Debug.Log("Please select a tile in which to finish your attack");
                        }
                    }
                    else if (selectedTarget_PC.currentTile.rangeTo == 1)
                    {  
                        Agressor_ArchetypeSwitch(PC, selectedTarget_PC);
                        selectedTarget = false;
                        selectedTarget_PC.self_Srend.color = selectedTarget_PC.normalColor;
                        selectedTarget_PC = null;
                        Deselect();   
                    }
                   
                    
                }
                else
                {
                    Debug.Log("Please select a Target");
                }
            }
            else
            {
                if (!attackMenuMode)
                {
                    AttackClean();
                }
                else if ((attackMenuMode) && (selectedTarget_PC != null))
                {
                    Agressor_ArchetypeSwitch(PC, selectedTarget_PC);
                    selectedTarget = false;
                    selectedTarget_PC.self_Srend.color = selectedTarget_PC.normalColor;
                    selectedTarget_PC = null;
                    Deselect();
                }
                else
                {
                    Debug.Log("Please select a Target");
                }
            }
        }
    }

    void AttackClean()
    {
        PathPurge();
        attackMenuMode = true;
        moveMenuMode = false;
        archetypeAbilityButtonDisableSwitch();
        MoveButton.gameObject.SetActive(false);
        EndTurnButton.gameObject.SetActive(false);
        MineButton.gameObject.SetActive(false);
        HealButton.gameObject.SetActive(false);
        UpgradeButton.gameObject.SetActive(false);
        RangeSet();
        if (huntress == null)
        {
            if (duel == null)
            {
                RangefinderStart();
            }
            else
            {
                if (duel.DuelMode != duelMode.mobile)
                {
                    RangefinderStart();
                }
                else {
                    duel.duel_SightLineSet();
                }
                
            }
            
        }
        
            
        
    }

    public void MoveAction()
    {
        if (ActionPoints >= MovementCost)
        {
            switch (turnSys.pTurn)
            {
                case PlayerTurns.errorhandler:
                    {
                        Debug.Log("Not A valid Player turn");
                        break;
                    }
                case PlayerTurns.p1:
                    {
                        if (PC.playerNumber == PlayerNumber.Player1)
                        {
                            if (!moveMenuMode)
                            {
                                MoveClean();
                            }
                            else if ((moveMenuMode) && (tilesSelectedForMovement != null))
                            {
                                MoveOnPath();
                            }
                            else
                            {
                                Debug.Log("Please select a tile to move into then click the move button");
                            }
                        }

                        break;
                    }
                case PlayerTurns.p2:
                    {
                        if (PC.playerNumber == PlayerNumber.Player2)
                        {
                            if (!moveMenuMode)
                            {
                                MoveClean();
                            }
                            else if ((moveMenuMode) && (tilesSelectedForMovement != null))
                            {
                                MoveOnPath();
                            }
                            else
                            {
                                Debug.Log("Please select a tile to move into then click the move button");
                            }
                        }
                        break;
                    }
                case PlayerTurns.p3:
                    {
                        if (PC.playerNumber == PlayerNumber.Player3)
                        {
                            if (!moveMenuMode)
                            {
                                MoveClean();
                            }
                            else if ((moveMenuMode) && (tilesSelectedForMovement != null))
                            {
                                MoveOnPath();
                            }
                            else
                            {
                                Debug.Log("Please select a tile to move into then click the move button");
                            }
                        }
                        break;
                    }
                case PlayerTurns.p4:
                    {
                        if (PC.playerNumber == PlayerNumber.Player4)
                        {
                            if (!moveMenuMode)
                            {
                                MoveClean();
                            }
                            else if ((moveMenuMode) && (tilesSelectedForMovement != null))
                            {
                                MoveOnPath();
                            }
                            else
                            {
                                Debug.Log("Please select a tile to move into then click the move button");
                            }
                        }
                        break;
                    }
                case PlayerTurns.p5:
                    {
                        if (PC.playerNumber == PlayerNumber.Player5)
                        {
                            if (!moveMenuMode)
                            {
                                MoveClean();
                            }
                            else if ((moveMenuMode) && (tilesSelectedForMovement != null))
                            {
                                MoveOnPath();
                            }
                            else
                            {
                                Debug.Log("Please select a tile to move into then click the move button");
                            }
                        }
                        break;
                    }
                case PlayerTurns.p6:
                    {
                        if (PC.playerNumber == PlayerNumber.Player6)
                        {
                            if (!moveMenuMode)
                            {
                                MoveClean();
                            }
                            else if ((moveMenuMode) && (tilesSelectedForMovement != null))
                            {
                                MoveOnPath();
                            }
                            else
                            {
                                Debug.Log("Please select a tile to move into then click the move button");
                            }
                        }
                        break;
                    }
            }
        }
        else
        {
            Debug.Log("Not enough action points");
        }
    }
    void MoveClean()
    {
        moveMenuMode = true;
        attackMenuMode = false;
        tentativeActionPoints = ActionPoints;
        archetypeAbilityButtonDisableSwitch();
        AttackButton.gameObject.SetActive(false);
        EndTurnButton.gameObject.SetActive(false);
        MineButton.gameObject.SetActive(false);
        HealButton.gameObject.SetActive(false);
        UpgradeButton.gameObject.SetActive(false);
        PathfinderStart();
        SetNeighborsToClickable(currentTile);
    }

    public void MineAction()
    {
        if (playerActionMenuMode)
        {
            Deselect();
        }
        switch (turnSys.pTurn)
        {
            case PlayerTurns.errorhandler:
                {
                    Debug.Log("Not A valid Player turn");
                    break;
                }
            case PlayerTurns.p1:
                {
                    if (PC.playerNumber == PlayerNumber.Player1)
                    {
                        mineClean();
                    }

                    break;
                }
            case PlayerTurns.p2:
                {
                    if (PC.playerNumber == PlayerNumber.Player2)
                    {
                        mineClean();
                    }
                    break;
                }
            case PlayerTurns.p3:
                {
                    if (PC.playerNumber == PlayerNumber.Player3)
                    {
                        mineClean();
                    }
                    break;
                }
            case PlayerTurns.p4:
                {
                    if (PC.playerNumber == PlayerNumber.Player4)
                    {
                        mineClean();
                    }
                    break;
                }
            case PlayerTurns.p5:
                {
                    if (PC.playerNumber == PlayerNumber.Player5)
                    {
                        mineClean();
                    }
                    break;
                }
            case PlayerTurns.p6:
                {
                    if (PC.playerNumber == PlayerNumber.Player6)
                    {
                        mineClean();
                    }
                    break;
                }
        }


    }
    void mineClean()
    {
        if ((currentTile.tType == tileType.Highland) || (currentTile.tType == tileType.Base))
        {
            Debug.Log("You are not allowed to mine here");
        }
        else if ((currentTile.tType == tileType.Mine) && (ActionPoints >= MineActionCost))
        {
            RNGeezus.mineRoll(4);
            playerMoney = playerMoney + RNGeezus.Result;
            ActionPoints = ActionPoints - MineActionCost;
            /*if (Bleeding)
            {
                bleedTick(MineActionCost);
            }*/
        }
        else if ((currentTile.tType == tileType.Quarry) && (ActionPoints >= MineActionCost))
        {
            RNGeezus.mineRoll(3);
            playerMoney = playerMoney + RNGeezus.Result;
            ActionPoints = ActionPoints - MineActionCost;
            /*if (Bleeding)
            {
                bleedTick(MineActionCost);
            }*/
        }
        else if (((currentTile.tType == tileType.Sanctuary) || (currentTile.tType == tileType.Midland)) && (ActionPoints >= MineActionCost))
        {
            RNGeezus.mineRoll(1);
            playerMoney = playerMoney + RNGeezus.Result;
            ActionPoints = ActionPoints - MineActionCost;
            /*if (Bleeding)
            {
                bleedTick(MineActionCost);
            }*/
        }
        else if (ActionPoints >= MineActionCost)
        {
            RNGeezus.mineRoll(2);
            playerMoney = playerMoney + RNGeezus.Result;
            ActionPoints = ActionPoints - MineActionCost;
            /*if (Bleeding)
            {
                bleedTick(MineActionCost);
            }*/
        }
        else if ((ActionPoints < MineActionCost))
        {
            Debug.Log("Not enough Action Points");
        }
    }

    void archetypeAbilityButtonDisableSwitch()
    {
        switch (characterArchetype)
        {
            case (CharacterArchetype.Chieftain):
                {
                    if (chief != null)
                    {
                        chief.disableTrapButtons();
                    }
                    break;
                }
            case (CharacterArchetype.ShapeShifter):
                {
                    //put button disableButton function inside the character's script for more versatility
                    if (Sshifter != null)
                    {
                        Sshifter.disableTransformButton();
                    }
                    break;
                }
            case (CharacterArchetype.LadyLuck):
                {
                    //put button disable function inside the character's script for more versatility
                    break;
                }
            case (CharacterArchetype.Headhuntress):
                {
                    if (huntress != null)
                    {
                        huntress.disableDroneButtons();
                    }
                    break;
                }
            case (CharacterArchetype.Duelist):
                {
                    if (duel != null)
                    {
                        duel.disableShiftButtons();
                    }
                    break;
                }
            case (CharacterArchetype.Soulstealer):
                {
                    if (soul != null)
                    {
                        soul.disableGrappleButtons();
                    }
                    break;
                }
        }
    }
    void archetypeAbilityButtonEnableSwitch()
    {
        switch (characterArchetype)
        {
            case (CharacterArchetype.Chieftain):
                {
                    if (chief != null)
                    {
                        chief.enableTrapButtons();
                    }
                    break;
                }
            case (CharacterArchetype.ShapeShifter):
                {
                    if (Sshifter != null)
                    {
                        Sshifter.enableTransfromButton();//put button disable function inside the character's script for more versatility
                    }
                    break;
                }
            case (CharacterArchetype.LadyLuck):
                {
                    //put button disable function inside the character's script for more versatility
                    break;
                }
            case (CharacterArchetype.Headhuntress):
                {
                    if (huntress != null)
                    {
                        huntress.enableDroneButtons();
                    }
                    break;
                }
            case (CharacterArchetype.Duelist):
                {
                    if (duel != null) {
                        duel.enableShiftButtons();
                    }
                    break;
                }
            case (CharacterArchetype.Soulstealer):
                {
                    if (soul != null)
                    {
                        soul.enableGrappleButtons();
                    }
                    break;
                }
        }
    }


    public void HealAction()
    {
        if (playerActionMenuMode)
        {
            Deselect();
        }
        switch (turnSys.pTurn)
        {
            case PlayerTurns.errorhandler:
                {
                    Debug.Log("Not A valid Player turn");
                    break;
                }
            case PlayerTurns.p1:
                {
                    if (PC.playerNumber == PlayerNumber.Player1)
                    {
                        healClean();
                    }

                    break;
                }
            case PlayerTurns.p2:
                {
                    if (PC.playerNumber == PlayerNumber.Player2)
                    {
                        healClean();
                    }
                    break;
                }
            case PlayerTurns.p3:
                {
                    if (PC.playerNumber == PlayerNumber.Player3)
                    {
                        healClean();
                    }
                    break;
                }
            case PlayerTurns.p4:
                {
                    if (PC.playerNumber == PlayerNumber.Player4)
                    {
                        healClean();
                    }
                    break;
                }
            case PlayerTurns.p5:
                {
                    if (PC.playerNumber == PlayerNumber.Player5)
                    {
                        healClean();
                    }
                    break;
                }
            case PlayerTurns.p6:
                {
                    if (PC.playerNumber == PlayerNumber.Player6)
                    {
                        healClean();
                    }
                    break;
                }
        }
    }
    void healClean()
    {
        if (currentTile.tType == tileType.Highland)
        {
            Debug.Log("You are not allowed to heal here");
        }
        else if ((currentTile.tType == tileType.Base) && (ActionPoints >= HealActionCost)) //[there's a bug here, it always heals me to full]
        {
            RNGeezus.healRoll(4);
            overhealCheck(RNGeezus.Result);
            ActionPoints = ActionPoints - HealActionCost;
        }
        else if ((currentTile.tType == tileType.Sanctuary) && (ActionPoints >= HealActionCost))
        {
            RNGeezus.healRoll(3);
            overhealCheck(RNGeezus.Result);
            if (Cursed)
            {
                Cursed = false;
                CurseLevel = 0;
                CurseStorage = 0;
                Debug.Log("You have purged the curse afflicting you");
            }
            if (Bleeding)
            {
                BleedSeverity = 0;
                Bleeding = false;
                Debug.Log("You are no longer bleeding.");
            }
            ActionPoints = ActionPoints - HealActionCost;
        }
        else if (((currentTile.tType == tileType.Quarry || (currentTile.tType == tileType.Midland))) && (ActionPoints >= HealActionCost))
        {
            RNGeezus.healRoll(1);
            overhealCheck(RNGeezus.Result);
            ActionPoints = ActionPoints - HealActionCost;
            if (Bleeding)
            {
                bleedTaper();
            }

        }
        else if (ActionPoints >= HealActionCost)
        {
            RNGeezus.healRoll(2);
            overhealCheck(RNGeezus.Result);
            ActionPoints = ActionPoints - HealActionCost;
            if (Bleeding)
            {
                bleedTaper();
            }
        }
        else if ((ActionPoints < HealActionCost))
        {
            Debug.Log("Not enough Action Points");
        }
    }


    public void BuyHealthAction()
    {
        if (playerActionMenuMode)
        {
            Deselect();
        }
        switch (turnSys.pTurn)
        {
            case PlayerTurns.errorhandler:
                {
                    Debug.Log("Not A valid Player turn");
                    break;
                }
            case PlayerTurns.p1:
                {
                    if (PC.playerNumber == PlayerNumber.Player1)
                    {
                        BuyHealthClean();
                    }

                    break;
                }
            case PlayerTurns.p2:
                {
                    if (PC.playerNumber == PlayerNumber.Player2)
                    {
                        BuyHealthClean();
                    }
                    break;
                }
            case PlayerTurns.p3:
                {
                    if (PC.playerNumber == PlayerNumber.Player3)
                    {
                        BuyHealthClean();
                    }
                    break;
                }
            case PlayerTurns.p4:
                {
                    if (PC.playerNumber == PlayerNumber.Player4)
                    {
                        BuyHealthClean();
                    }
                    break;
                }
            case PlayerTurns.p5:
                {
                    if (PC.playerNumber == PlayerNumber.Player5)
                    {
                        BuyHealthClean();
                    }
                    break;
                }
            case PlayerTurns.p6:
                {
                    if (PC.playerNumber == PlayerNumber.Player6)
                    {
                        BuyHealthClean();
                    }
                    break;
                }
        }


    }
    void BuyHealthClean()
    {
        if (playerMoney >= HealthUpgradeCost)
        {
            playerMoney = playerMoney - HealthUpgradeCost;
            MAX_Health = MAX_Health + 1;
            Health = Health + 1;
        }
        else
        {
            Debug.Log("You don't have enough money for this");
        }
    }

    public void BuyDamageAction()
    {
        if (playerActionMenuMode)
        {
            Deselect();
        }
        switch (turnSys.pTurn)
        {
            case PlayerTurns.errorhandler:
                {
                    Debug.Log("Not A valid Player turn");
                    break;
                }
            case PlayerTurns.p1:
                {
                    if (PC.playerNumber == PlayerNumber.Player1)
                    {
                        BuyDamageClean();
                    }

                    break;
                }
            case PlayerTurns.p2:
                {
                    if (PC.playerNumber == PlayerNumber.Player2)
                    {
                        BuyDamageClean();
                    }
                    break;
                }
            case PlayerTurns.p3:
                {
                    if (PC.playerNumber == PlayerNumber.Player3)
                    {
                        BuyDamageClean();
                    }
                    break;
                }
            case PlayerTurns.p4:
                {
                    if (PC.playerNumber == PlayerNumber.Player4)
                    {
                        BuyDamageClean();
                    }
                    break;
                }
            case PlayerTurns.p5:
                {
                    if (PC.playerNumber == PlayerNumber.Player5)
                    {
                        BuyDamageClean();
                    }
                    break;
                }
            case PlayerTurns.p6:
                {
                    if (PC.playerNumber == PlayerNumber.Player6)
                    {
                        BuyDamageClean();
                    }
                    break;
                }
        }


    }
    void BuyDamageClean()
    {
        if (playerMoney >= DamageUpgradeCost)
        {
            playerMoney = playerMoney - DamageUpgradeCost;
            DamageBonus = DamageBonus + 1;
            if (Sshifter != null)
            {
                if (!Sshifter.Predator)
                {
                    Sshifter.DamageBuyUpdate();
                }

            }
        }
        else
        {
            Debug.Log("You don't have enough money for this");
        }
    }

    public void BuyArmorAction()
    {
        if (playerActionMenuMode)
        {
            Deselect();
        }
        switch (turnSys.pTurn)
        {
            case PlayerTurns.errorhandler:
                {
                    Debug.Log("Not A valid Player turn");
                    break;
                }
            case PlayerTurns.p1:
                {
                    if (PC.playerNumber == PlayerNumber.Player1)
                    {
                        BuyArmorClean();
                    }

                    break;
                }
            case PlayerTurns.p2:
                {
                    if (PC.playerNumber == PlayerNumber.Player2)
                    {
                        BuyArmorClean();
                    }
                    break;
                }
            case PlayerTurns.p3:
                {
                    if (PC.playerNumber == PlayerNumber.Player3)
                    {
                        BuyArmorClean();
                    }
                    break;
                }
            case PlayerTurns.p4:
                {
                    if (PC.playerNumber == PlayerNumber.Player4)
                    {
                        BuyArmorClean();
                    }
                    break;
                }
            case PlayerTurns.p5:
                {
                    if (PC.playerNumber == PlayerNumber.Player5)
                    {
                        BuyArmorClean();
                    }
                    break;
                }
            case PlayerTurns.p6:
                {
                    if (PC.playerNumber == PlayerNumber.Player6)
                    {
                        BuyArmorClean();
                    }
                    break;
                }
        }


    }
    void BuyArmorClean()
    {
        if (playerMoney >= ArmorUpgradeCost)
        {
            playerMoney = playerMoney - ArmorUpgradeCost;
            Armor = Armor + 1;
            if (Sshifter != null)
            {
                if (Sshifter.Predator)
                {
                    Sshifter.ArmorBuyUpdate();
                }

            }
        }
        else
        {
            Debug.Log("You don't have enough money for this");
        }
    }

    public void occupiedBy(Tile_Properties tprop)
    {
        switch (tprop.whoIsOnTile)
        {
            case isOnTile.p1:
                {
                    for (int i = 0; i < turnSys.PCs.Length; i++)
                    {
                        if (turnSys.PCs[i].playerNumber == PlayerNumber.Player1)
                        {
                            targetList.Add(turnSys.PCs[i]);
                        }
                    }
                    break;
                }
            case isOnTile.p2:
                {
                    for (int i = 0; i < turnSys.PCs.Length; i++)
                    {
                        if (turnSys.PCs[i].playerNumber == PlayerNumber.Player2)
                        {
                            targetList.Add(turnSys.PCs[i]);
                        }
                    }
                    break;
                }
            case isOnTile.p3:
                {
                    for (int i = 0; i < turnSys.PCs.Length; i++)
                    {
                        if (turnSys.PCs[i].playerNumber == PlayerNumber.Player3)
                        {
                            targetList.Add(turnSys.PCs[i]);
                        }
                    }
                    break;
                }
            case isOnTile.p4:
                {
                    for (int i = 0; i < turnSys.PCs.Length; i++)
                    {
                        if (turnSys.PCs[i].playerNumber == PlayerNumber.Player4)
                        {
                            targetList.Add(turnSys.PCs[i]);
                        }
                    }
                    break;
                }
            case isOnTile.p5:
                {
                    for (int i = 0; i < turnSys.PCs.Length; i++)
                    {
                        if (turnSys.PCs[i].playerNumber == PlayerNumber.Player5)
                        {
                            targetList.Add(turnSys.PCs[i]);
                        }
                    }
                    break;
                }
            case isOnTile.p6:
                {
                    for (int i = 0; i < turnSys.PCs.Length; i++)
                    {
                        if (turnSys.PCs[i].playerNumber == PlayerNumber.Player6)
                        {
                            targetList.Add(turnSys.PCs[i]);
                        }
                    }
                    break;
                }
            case isOnTile.empty:
                {
                    Debug.Log("woah! how did this happen???");
                    break;
                }
        }
        SecureTargets();
    }

    void SecureTargets()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            EnableTargetByPlayerSwitch(i);
        }
    }

    public void Generic_Attack_Melee(PlayableCharacter Victim)
    {
        if (ActionPoints >= AttackActionCost)
        {
            DamageBonus = DamageBonus + specialDamageBonus;
            //RNG Set
            RNGeezus.numberOfDice = 1;
            RNGeezus.diceMax = 8;
            //RNG call
            RNGeezus.Damage_Roll();
            //Set attack properties
            RNGeezus.missOn = 2;
            RNGeezus.armorPierce = false;
            //Damage calculation
            Victim.Target_ArchetypeSwitch(Victim, PC);

            DamageBonus = DamageBonus - specialDamageBonus;
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    public void Target_ArchetypeSwitch(PlayableCharacter Target, PlayableCharacter Agressor) //Keep a close eye on this one, as it may help us make all subsequent damage calculations easier
    {
        switch (Target.characterArchetype)
        {
            case (CharacterArchetype.genericCharacter):
                {
                    Target.TakeDamage(Agressor);
                    break;
                }
            case (CharacterArchetype.ShapeShifter):
                {
                    PC_ShapeShifter temp = null;
                    temp = Target.GetComponent<PC_ShapeShifter>();
                    if (temp.Predator)
                    {
                        temp.Predator_TakeDamage(Agressor);
                    }
                    else
                    {
                        temp.Goliath_TakeDamage(Agressor);
                    }
                    break;
                }
            case (CharacterArchetype.LadyLuck):
                {
                    PC_LadyLuck temp = null;
                    temp = Target.GetComponent<PC_LadyLuck>();
                    if (temp.MissFortune)
                    {
                        Agressor.RNGeezus.missOn = Agressor.RNGeezus.missOn + (Agressor.RNGeezus.diceMax / 4);
                        Target.TakeDamage(Agressor);
                    }
                    else
                    {
                        Target.TakeDamage(Agressor);
                    }
                    break;
                }
            default: //since no character Archetypes have been implemented yet
                {
                    Target.TakeDamage(Agressor);
                    break;
                }
        }
    }

    public void Agressor_ArchetypeSwitch(PlayableCharacter Agressor, PlayableCharacter Target) //Keep a close eye on this one, as it may help us make all subsequent damage calculations easier
    {
        switch (Agressor.characterArchetype)
        {
            case (CharacterArchetype.genericCharacter):
                {
                    Agressor.Generic_Attack_Melee(Target);
                    break;
                }
            case (CharacterArchetype.Chieftain):
                {
                    Agressor.chief.Chieftain_Attack(Target);
                    break;
                }
            case (CharacterArchetype.ShapeShifter):
                {
                    if (Agressor.Sshifter.Predator)
                    {
                        Agressor.Sshifter.Predator_Attack(Target);
                    }
                    else
                    {
                        Agressor.Sshifter.Goliath_Attack(Target);
                    }
                    break;
                }
            case (CharacterArchetype.LadyLuck):
                {
                    Agressor.lady.LadyLuck_Attack(Target);
                    break;
                }
            case (CharacterArchetype.Headhuntress):
                {
                    Agressor.huntress.Headhuntress_Attack(Target);
                    break;
                }
            case (CharacterArchetype.Duelist):
                {
                    Agressor.duel.duel_AgressorModeSwitch(Agressor, Target);
                    break;
                }
            default: //since no character Archetypes have been implemented yet
                {
                    Agressor.Generic_Attack_Melee(Target);
                    break;
                }
        }
        //Action point deduction
        ActionPoints = ActionPoints - AttackActionCost;
        /*if (Bleeding)
            {
                bleedTick(MineActionCost);
            }*/
    }

    //VV  Works for Base Lady Luck, Chieftain, Headhuntress, and Soulstealer
    public void TakeDamage(PlayableCharacter Agressor) //This was created to simplify damage calculations and character specific cases...
    {
        int rawDamage = Agressor.RNGeezus.Result + Agressor.DamageBonus;
        //check armor piercing
        int effectiveArmor = Armor + specialArmor;
        if (Agressor.RNGeezus.armorPierce && !trueArmor)
        {
            effectiveArmor = 0;
        } else if (Agressor.RNGeezus.trueDamage) {
            effectiveArmor = 0;
        }
        //this is used later
        int effectiveDamage = 0;

        //check to see if the enemy missed
        int hitCheck = 0;
        for (int i = 0; i < Agressor.RNGeezus.numberOfDice; i++)
        {
            if (Agressor.RNGeezus.diceResult[i] > Agressor.RNGeezus.missOn)
            {
                hitCheck++;
            }
        }
        if (hitCheck == 0)
        {
            Debug.Log("You missed!");
            Agressor.RNGeezus.isHit = false;
        }
        else
        {
            Agressor.RNGeezus.isHit = true;
            effectiveDamage = rawDamage - effectiveArmor;
            //Chip damage
            if (effectiveDamage <= 0)
            {
                Agressor.RNGeezus.isChip = true;
                effectiveDamage = 1;
                PC.Health = PC.Health - 1;
                Debug.Log("You chip for 1 damage...");
            }
            //Kill damage
            else if (effectiveDamage >= Health) //add PC. to Health? [PRONE TO EXPLODING]
            {
                Agressor.RNGeezus.isChip = false;
                Agressor.VictoryPoints = Agressor.VictoryPoints + 2;
                if (Agressor.GetComponent<PC_Headhuntress>() != null)
                {
                    Agressor.VictoryPoints = Agressor.VictoryPoints + 3;
                }
                effectiveDamage = Health;
                Health = 0;
                Debug.Log("You have killed your target");
            }
            else if (effectiveDamage < Health)
            {
                Agressor.RNGeezus.isChip = false;
                Health = Health - effectiveDamage;
                Debug.Log("You did " + effectiveDamage + " damage!");
            }            
        }
        if (Health == 0)
        {
            currentTile.whoIsOnTile = isOnTile.empty;
            //Agressor.FindValidMovement(); [replace this]
            this.gameObject.SetActive(false); //[PRONE TO EXPLODING]
        }
        else if (Health < 0)
        {
            Debug.Log("WTF, negative health?");
        }
    }

    public void curseTick()
    {
        int curseDamage = 0;
        if (Cursed)
        {
            if (Health > CurseLevel)
            {
                curseDamage = CurseLevel;
                Health = Health - CurseLevel;
                CurseStorage = CurseStorage + CurseLevel;
                CurseLevel++;
            }
            else
            {
                curseDamage = Health - 1;
                CurseStorage = CurseStorage + Health - 1;
                Health = 1;
                Debug.Log("The curse brings you to the brink of death. But it needs you to live");
            }
            if (Sshifter != null) {
                Sshifter.potential = Sshifter.potential + curseDamage;
            }
        }
    }
    public void bleedTick()
    {
        if ((Health - (BleedSeverity)) >= 1)
        {
            Health = Health - (BleedSeverity);
            Debug.Log(PC.name + " took " + (BleedSeverity) + " points of Bleed Damage");
        }
        else
        {
            Health = 0;
            Debug.Log(PC.name + " has died from thier wounds.");
            //[Oscar Note] Activate kill code
        }
        if (Sshifter != null)
        {
            Sshifter.potential = Sshifter.potential + BleedSeverity;
        }
    }
    public void bleedTaper()
    {
        int BleedTaper;
        if (BleedSeverity % 3 == 0)
        {
            BleedTaper = BleedSeverity / 3;
        }
        else
        {
            BleedTaper = (BleedSeverity / 3) + 1;
        }
        BleedSeverity = BleedSeverity - BleedTaper;
        if (BleedSeverity == 0)
        {
            Bleeding = false;
            Debug.Log("You are no longer bleeding.");
        }
        else
        {
            Debug.Log("Your bleeding slows down to: " +BleedSeverity);
        }
    }
    public void overhealCheck(int heal)
    {
        if ((Health + heal) < MAX_Health)
        {
            Health = Health + heal;
            Debug.Log("You heal for " + heal + "HP");
        }
        else if ((Health + heal) >= MAX_Health)
        {
            Health = MAX_Health;
            Debug.Log("You have been healed to full health");
        }
        else if (Health == MAX_Health)
        {
            Debug.Log("You're in no need of healng");
        }
    }
}
