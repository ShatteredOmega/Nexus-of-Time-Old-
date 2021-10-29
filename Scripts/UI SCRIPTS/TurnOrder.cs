using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnOrder : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string nameStr;

    [Header("Set In Editor")]
    public GameObject[] playersArr; // should be set dynamically in next sprint


    /******************** GET METHODS ********************/
    /* These methods will be called by the individual    */
    /* turn order panel objects that display each of the */
    /* individual palyer's stats in the panel            */
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
        // playerNum - 1 = player index in array
        PlayableCharacter playerScript =
            playersArr[playerNum-1].GetComponent<PlayableCharacter>();

        return playerScript.Health;
    }

    // GET MAX HEALTH
    public int GetPlayerMaxHealth (int playerNum)
    {
        PlayableCharacter playerScript =
            playersArr[playerNum-1].GetComponent<PlayableCharacter>();

        return playerScript.MAX_Health;
    }

    // GET VICTORY POINTS
    public int GetPlayerVictoryPoints (int playerNum)
    {
        PlayableCharacter playerScript =
            playersArr[playerNum-1].GetComponent<PlayableCharacter>();

        return playerScript.VictoryPoints;
    }

    // GET MAX VICTORY POINTS
    public int GetPlayerMaxVictoryPoints (int playerNum)
    {
        PlayableCharacter playerScript =
            playersArr[playerNum-1].GetComponent<PlayableCharacter>();

        return playerScript.MAX_VictoryPoints;
    }
}
