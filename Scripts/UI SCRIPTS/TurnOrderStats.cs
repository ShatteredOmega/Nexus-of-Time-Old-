using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderStats : MonoBehaviour {

    [Header("Set Dynamically")]
    public TurnOrder playerStats;
    public int thisObjectNum; // gonna be used to pass the number to methods
                              // this number is the same as the player this obj
                              // belongs too. example if thisObjectNum = 1
                              // then this object is for Player 1
    public TurnSystem turnSys; //Stores reference to the "TurnSystem" code
    public Image isTurnImage; // image that highlights who's turn it is

    [Header("Set in Editor")]
    public Slider healthSlider;
    public Slider victorySlider;
    public Text nameText;
    

    // Use this for initialization
    void Start() {
        // looks for an object with a "TurnSystem" component and stores it 
        turnSys = FindObjectOfType<TurnSystem>();
        // sets the image for turn highlighting
        isTurnImage = this.GetComponent<Image>();

        // Set thisObjectNum
        GetThisGONum();
        

        // get the component from parents so we can use the get methods in
        // TurnOrder
        playerStats = GetComponentInParent<TurnOrder>();

        // set the name of the character, this will be based on the name of the
        // character's GameObject
        nameText.text = playerStats.GetPlayerName(thisObjectNum);
        

        // set the min and max values for the sliders
        // set the health slider Max values
        healthSlider.maxValue = playerStats.GetPlayerMaxHealth(thisObjectNum);
        // set the health slider Min value, which is always 0
        healthSlider.minValue = 0;
        // set health to max at start
        healthSlider.value = healthSlider.maxValue;

        // set the victory slider max value
        victorySlider.maxValue = playerStats.GetPlayerMaxVictoryPoints
            (thisObjectNum);
        // set the victory slider min value, which is always 0
        victorySlider.minValue = 0;
        // set the victory value to 0 at the start
        victorySlider.value = 0;

	}
	
	// Update is called once per frame
	void Update () {
        // Update health and Victory points
        // This should be handled whenever the values are changed but for now
        // This is the easiest way to do this without intruding on back end
        SetTurnHighlight();
        UpdateHealth();
        UpdateVictoryPoints();
	}

    // GET THIS GAMEOBJECTS NUMBER AND CORRESPONDING PLAYER
    public void GetThisGONum()
    {
        switch (this.name)
        {
            case "P1":
                thisObjectNum = 1;
                break;
            case "P2":
                thisObjectNum = 2;
                break;
            case "P3":
                thisObjectNum = 3;
                break;
            case "P4":
                thisObjectNum = 4;
                break;
            case "P5":
                thisObjectNum = 5;
                break;
            case "P6":
                thisObjectNum = 6;
                break;
        }
    }

    // METHOD UPDATES HEALTH VALUE
    public void UpdateHealth ()
    {
        healthSlider.value = playerStats.GetPlayerHealth(thisObjectNum);
    }

    // METHOD UPDATES THE VICTORY POINTS
    public void UpdateVictoryPoints ()
    {
        victorySlider.value = playerStats.GetPlayerVictoryPoints(thisObjectNum);
    }
/*
    // METHOD RETURNS TRUE IF IT'S THIS PLAYERS TURN
    public bool IsItMyTurn ()
    {
        bool myTurn;
        switch (turnSys.pTurn.ToString())
        {
            case "p1":
                if (thisObjectNum == 1)
                    myTurn = true;
                else
                    myTurn = false;
                    break;
            case "p2":
                if (thisObjectNum == 2)
                    myTurn = true;
                else
                    myTurn = false;
                    break;
            case "p3":
                if (thisObjectNum == 3)
                    myTurn = true;
                else
                    myTurn = false;
                    break;
            case "p4":
                if (thisObjectNum == 4)
                    myTurn = true;
                else
                    myTurn = false;
                    break;
            case "p5":
                if (thisObjectNum == 5)
                    myTurn = true;
                else
                    myTurn = false;
                    break;
            case "p6":
                if (thisObjectNum == 6)
                    myTurn = true;
                else
                    myTurn = false;
                    break;
            case "Errorhandler":
                myTurn = false;
                break;
            default:
                myTurn = false;
                break;
        }
        return myTurn;
    }
*/
    // METHOD CHANGES HIGHLIGHT MARKER IF ITS THEIR TURN
    public void SetTurnHighlight ()
    {
        switch (turnSys.pTurn.ToString())
        {
            case "p1":
                if (thisObjectNum == 1)
                    isTurnImage.enabled = true;
                else
                    isTurnImage.enabled = false;
                break;
            case "p2":
                if (thisObjectNum == 2)
                    isTurnImage.enabled = true;
                else
                    isTurnImage.enabled = false;
                break;
            case "p3":
                if (thisObjectNum == 3)
                    isTurnImage.enabled = true;
                else
                    isTurnImage.enabled = false;
                break;
            case "p4":
                if (thisObjectNum == 4)
                    isTurnImage.enabled = true;
                else
                    isTurnImage.enabled = false;
                break;
            case "p5":
                if (thisObjectNum == 5)
                    isTurnImage.enabled = true;
                else
                    isTurnImage.enabled = false;
                break;
            case "p6":
                if (thisObjectNum == 6)
                    isTurnImage.enabled = true;
                else
                    isTurnImage.enabled = false;
                break;
            case "Errorhandler":
                isTurnImage.enabled = false;
                break;
            default:
                isTurnImage.enabled = false;
                break;
        }
    }
}