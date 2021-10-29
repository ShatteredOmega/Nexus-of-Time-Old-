using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerStats : MonoBehaviour {

    [Header("Set Dynamically")]
    PlayableCharacter playerScript;
    public string nameStr;
    public int playerTurn; // number of the player who's turn it is
    public TurnSystem turnSys; //Stores reference to the "TurnSystem" code

    [Header("Set In Editor")]
    public GameObject[] playersArr; // should be set dynamically in next sprint
    public Slider hpSlider;
    public Slider apSlider;
    public Slider vpSlider;
    public Text hpText;
    public Text nameText;
    public Text armorText;
    public Text attackText;
    public Text resourceText;
    public Text apText;
    public Text vpText;
    //public Image charIcon;

    // None of this should be going on in Start or in Update
    void Start()
    {
        //looks for an object with a "TurnSystem" component and stores it
        turnSys = FindObjectOfType<TurnSystem>();
    }

    // 
    void Update()
    {
        WhoTurnIsIt();
        UpdateAllStats(playerTurn);
    }


    /******************** SET METHODS ********************/
    /* These methods will be called when values are being*/
    /* set to any of the UI gameobjects like the sliders */
    /* or text objects displaying numeric values         */
    /*****************************************************/

    // SLIDERS
    // SET HEALTH SLIDER MAX AND MINIMUM VALUES
    public void SetMinAndMaxHPSliderValues (int playerNum)
    {
        // set the min value (should always be 0)
        hpSlider.minValue = 0;
        // set the max value from the character's stats
        hpSlider.maxValue = GetPlayerMaxHealth(playerNum);
    }
    // SET ACTION POINT SLIDER MAX AND MINIMUM VALUES
    public void SetMinAndMaxAPSliderValues (int playerNum)
    {
        // set the min value (should always be 0)
        apSlider.minValue = 0;
        // set the max value from the character's stats
        apSlider.maxValue = GetPlayerMaxActionPoints(playerNum);
    }
    // SET VICTORY POINT SLIDER MAX AND MINIMUM VALUES
    public void SetMinAndMaxVPSliderValues (int playerNum)
    {
        // set the min value (should always be 0)
        vpSlider.minValue = 0;
        // set the max value from the character's stats
        vpSlider.maxValue = GetPlayerMaxVictoryPoints(playerNum);
    }

    // SET CURRENT HEALTH ON SLIDER
    public void SetCurrentHPOnSlider (int playerNum)
    {
        hpSlider.value = GetPlayerHealth(playerNum);
    }
    // SET CURRENT AP ON SLIDER
    public void SetCurrentAPOnSlider (int playerNum)
    {
        apSlider.value = GetPlayerActionPoints(playerNum);
    }
    // SET CURRENT VP ON SLIDER
    public void SetCurrentVPOnSlider (int playerNum)
    {
        vpSlider.value = GetPlayerVictoryPoints(playerNum);
    }
    
    // TEXT
    // SET CURRENT HEALTH ON HP TEXT
    public void SetCurrentHealthText (int playerNum)
    {
        // set the current health value
        hpText.text = FormatCurrentOverMaxText
            (GetPlayerHealthAsString(playerNum), 
            GetPlayerMaxHealthAsString(playerNum));
    }
    // SET CURRENT ARMOR
    public void SetCurrentArmor (int playerNum)
    {
        armorText.text = GetPlayerArmorAsString(playerNum);
    }
    // SET CURRENT ATTACK
    public void SetCurrentAttack (int playerNum)
    {
        attackText.text = GetPlayerAttackAsString(playerNum);
    }
    // SET CURRENT RESOURCES
    public void SetCurrentResources (int playerNum)
    {
        resourceText.text = GetPlayerResourcesAsString(playerNum);
    }
    // SET CURRENT AP ON AP TEXT
    public void SetCurrentAPText (int playerNum)
    {
        apText.text = FormatCurrentOverMaxText
            (GetPlayerActionPointsAsString(playerNum), 
            GetPlayerMaxActionPointsAsString(playerNum));
    }
    // SET CURRENT VP ON VP TEXT
    public void SetCurrentVPText (int playerNum)
    {
        vpText.text = FormatCurrentOverMaxText
            (GetPlayerVictoryPointsAsString(playerNum),
            GetPlayerMaxVictoryPointsAsString(playerNum));
    }

    //ICON AND NAME
    // SET THE CURRENT NAME
    public void SetCurrentName (int playerNum)
    {
        nameText.text = GetPlayerName(playerNum);
    }
    // SET THE CURRENT ICON

    /******************** SET METHOD  ********************/
    /* This method can be called to update all of the    */
    /* character's values at once. I will also add some  */
    /* methods that compound parts of the stats for      */
    /*****************************************************/

    // COMOUNDED METHOD THAT UPDATES ALL THE HEALTH RELATED STATS AT ONCE
    public void UpdateAllHealthStats (int playerNum)
    {
        SetMinAndMaxHPSliderValues(playerNum);
        SetCurrentHPOnSlider(playerNum);
        SetCurrentHealthText(playerNum);
    }
    // COMPOUNDED METHOD THAT UPDATES ALL THE ACTION POINT STATS AT ONCE
    public void UpdateAllAPStats (int playerNum)
    {
        SetMinAndMaxAPSliderValues(playerNum);
        SetCurrentAPOnSlider(playerNum);
        SetCurrentAPText(playerNum);
    }
    // COMPOUNDED METHOD THAT UPDATES ALL THE VICTORY POINT STATS AT ONCE
    public void UpdateAllVPStats (int playerNum)
    {
        SetMinAndMaxVPSliderValues(playerNum);
        SetCurrentVPOnSlider(playerNum);
        SetCurrentVPText(playerNum);
    }
    // COMPOUNDED METHO THAT UPDATES THE ATTACK AND ARMOR STATS AT ONCE
    public void UpdateArmorAndAttack (int playerNum)
    {
        SetCurrentArmor(playerNum);
        SetCurrentAttack(playerNum);
    }
    // COMPOUNDED METHOD THAT UPDATES THE NAME AND ICON AT ONCE
    public void UpdateNameAndIcon (int playerNum)
    {
        SetCurrentName(playerNum);
        //SetCurrentIcon(playerNum);
    }

    // COMPOUNDED METHOD THAT UPDATES ALL STATS
    public void UpdateAllStats (int playerNum)
    {
        UpdateAllHealthStats(playerNum);
        UpdateAllAPStats(playerNum);
        UpdateAllVPStats(playerNum);
        UpdateArmorAndAttack(playerNum);
        UpdateNameAndIcon(playerNum);
        SetCurrentResources(playerNum);
    }


    /******************** GET METHODS ********************/
    /* These methods will be called by the other methods */
    /* when they need to get the stat values of the      */
    /* current character from the PlayableCharacter class*/
    /*****************************************************/

    // GET NAME
    public string GetPlayerName (int playerNum)
    {
        switch (playerNum)
        {
            case 1:
                nameStr = GameObject.FindGameObjectWithTag("Player1").name;
                break;
            case 2:
                nameStr = GameObject.FindGameObjectWithTag("Player2").name;
                break;
            case 3:
                nameStr = GameObject.FindGameObjectWithTag("Player3").name;
                break;
            case 4:
                nameStr = GameObject.FindGameObjectWithTag("Player4").name;
                break;
            case 5:
                nameStr = GameObject.FindGameObjectWithTag("Player5").name;
                break;
            case 6:
                nameStr = GameObject.FindGameObjectWithTag("Player6").name;
                break;
        }
        return nameStr;
    }

    // GET HEALTH
    public int GetPlayerHealth (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.Health;
    }
    // GET HEALTH AS STRING
    public string GetPlayerHealthAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.Health.ToString();
    }
    // GET MAX HEALTH
    public int GetPlayerMaxHealth (int playerNum)
    {
        SetProperPlayerScript(playerNum);

        return playerScript.MAX_Health;
    }
    // GET MAX HEALTH AS STRING
    public string GetPlayerMaxHealthAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.MAX_Health.ToString();
    }

    // GET VICTORY POINTS
    public int GetPlayerVictoryPoints (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.VictoryPoints;
    }
    // GET VICTORY POINTS AS STRING
    public string GetPlayerVictoryPointsAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.VictoryPoints.ToString();
    }
    // GET MAX VICTORY POINTS
    public int GetPlayerMaxVictoryPoints (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.MAX_VictoryPoints;
    }
    // GET MAX VICTORY POINTS AS STRING
    public string GetPlayerMaxVictoryPointsAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.MAX_VictoryPoints.ToString();
    }

    // GET ACTION POINTS
    public int GetPlayerActionPoints (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.ActionPoints;
    }
    // GET ACTION POITNS AS STRING
    public string GetPlayerActionPointsAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.ActionPoints.ToString();
    }
    // GET MAX ACTION POINTS 
    public int GetPlayerMaxActionPoints (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.MAX_ActionPoints;
    }
    // GET MAX ACTION POINTS AS STRING
    public string GetPlayerMaxActionPointsAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.MAX_ActionPoints.ToString();
    }

    // GET RESOURCES
    public int GetPlayerResources (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.playerMoney;
    }
    // GET RESOURCES AS STRING
    public string GetPlayerResourcesAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.playerMoney.ToString();
    }

    // GET ATTACK
    public int GetPlayerAttack (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.DamageBonus;
    }
    // GET ATTACK AS STRING
    public string GetPlayerAttackAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.DamageBonus.ToString();
    }

    // GET ARMOR
    public int GetPlayerArmor (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.Armor;
    }
    // GET ARMOR AS STRING
    public string GetPlayerArmorAsString (int playerNum)
    {
        SetProperPlayerScript(playerNum);
        return playerScript.Armor.ToString();
    }

    // GET ICON
    // Still not sure how to handle this one

    /******************** MISC METHODS ********************/

    // Method for formatting Text to be CURRENT / MAX
    public string FormatCurrentOverMaxText (string currentVal, string maxVal)
    {
        return currentVal + " / " + maxVal;
    }

    // Method for setting the proper character's player script component
    public void SetProperPlayerScript (int playerNum)
    {
        // set the appropriate player script component
        playerScript = playersArr[playerNum - 1].
            GetComponent<PlayableCharacter>();
    }

    // Method that figures out who's turn it is and sets playerTurn to it
    public void WhoTurnIsIt ()
    {
        switch (turnSys.pTurn.ToString())
        {
            case "p1":
                playerTurn = 1;
                break;
            case "p2":
                playerTurn = 2;
                break;
            case "p3":
                playerTurn = 3;
                break;
            case "p4":
                playerTurn = 4;
                break;
            case "p5":
                playerTurn = 5;
                break;
            case "p6":
                playerTurn = 6;
                break;
            case "errorhandler":
                //FIXME If I don't have it set to p1 the game breaks
                playerTurn = 1;
                break;
            default:
                Debug.Log("An Unexpected Error Occurred: " +
                    "pTurn Not set to a recognizable value: " + 
                    turnSys.pTurn.ToString());
                break;
        }
    }
}
