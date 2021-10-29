using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///[CURRENT FOCUS]: Random Turn order and random spawn points, then Action Point refresh broadcast, and cooldown update broadcast. Implementing rouns


public enum PlayerTurns { errorhandler, p1,p2,p3,p4,p5,p6}

public class TurnSystem : MonoBehaviour {
    public PlayerTurns pTurn;
    public PlayableCharacter[] PCs;
    public PlayableCharacter currentPlayer;
    //public PlayableCharacter[] tempPCs;
    public int[] turnOrder;

    // Use this for initialization
    void Start () {
        pTurn = PlayerTurns.errorhandler;
    }

    public void FindPlayers() {
        PCs = FindObjectsOfType<PlayableCharacter>();
        turnOrder = new int[PCs.Length];
        hardcodedTurnOrder();
        for (int k = 0; k < PCs.Length; k++)
        {
           PCs[k].StartCharacters();
        }
    }


    //Hardcoded For NOW
    public void changeTurn() {
        currentPlayer = null;
        switch (pTurn) {
            case PlayerTurns.errorhandler:
                {
                    pTurn = PlayerTurns.p1;
                    StartNextTurn();
                    break;
                }
            case PlayerTurns.p1:
                {
                    EndTurn();
                    pTurn = PlayerTurns.p2;
                    StartNextTurn();
                    break;
                }
            case PlayerTurns.p2:
                {
                    EndTurn();
                    pTurn = PlayerTurns.p3;
                    StartNextTurn();
                    break;
                }
            case PlayerTurns.p3:
                {
                    EndTurn();
                    pTurn = PlayerTurns.p4;
                    StartNextTurn();
                    break;
                }
            case PlayerTurns.p4:
                {
                    EndTurn();
                    pTurn = PlayerTurns.p5;
                    StartNextTurn();
                    break;
                }
            case PlayerTurns.p5:
                {
                    EndTurn();
                    pTurn = PlayerTurns.p6;
                    StartNextTurn();
                    break;
                }
            case PlayerTurns.p6:
                {
                    EndTurn();
                    pTurn = PlayerTurns.p1;
                    StartNextTurn();
                    break;
                }
        }
    }

    void hardcodedTurnOrder() {
        for (int i = 0; i < PCs.Length; i++) {
            turnOrder[i] = i+1;
        }
    }



    void EndTurn() {
        PlayableCharacter tempPlayer;
        for (int i = 0; i < PCs.Length; i++)
        {
            switch (PCs[i].playerNumber) {
                case PlayerNumber.Player1:
                    {

                        if (pTurn == PlayerTurns.p1) {
                            tempPlayer = PCs[i];
                            tempPlayer.EndTurnConditions();
                        }
                        
                        break;
                    }
                case PlayerNumber.Player2:
                    {
                        if (pTurn == PlayerTurns.p2)
                        {
                            tempPlayer = PCs[i];
                            tempPlayer.EndTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player3:
                    {
                        if (pTurn == PlayerTurns.p3)
                        {
                            tempPlayer = PCs[i];
                            tempPlayer.EndTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player4:
                    {
                        if (pTurn == PlayerTurns.p4)
                        {
                            tempPlayer = PCs[i];
                            tempPlayer.EndTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player5:
                    {
                        if (pTurn == PlayerTurns.p5)
                        {
                            tempPlayer = PCs[i];
                            tempPlayer.EndTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player6:
                    {
                        if (pTurn == PlayerTurns.p6)
                        {
                            tempPlayer = PCs[i];
                            tempPlayer.EndTurnConditions();
                        }
                        break;
                    }
            }
            
        }
    }

    void StartNextTurn() {
        for (int i = 0; i < PCs.Length; i++)
        {
            switch (PCs[i].playerNumber)
            {
                case PlayerNumber.Player1:
                    {

                        if (pTurn == PlayerTurns.p1)
                        {
                            currentPlayer = PCs[i];
                            currentPlayer.StartTurnConditions();
                        }

                        break;
                    }
                case PlayerNumber.Player2:
                    {
                        if (pTurn == PlayerTurns.p2)
                        {
                            currentPlayer = PCs[i];
                            currentPlayer.StartTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player3:
                    {
                        if (pTurn == PlayerTurns.p3)
                        {
                            currentPlayer = PCs[i];
                            currentPlayer.StartTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player4:
                    {
                        if (pTurn == PlayerTurns.p4)
                        {
                            currentPlayer = PCs[i];
                            currentPlayer.StartTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player5:
                    {
                        if (pTurn == PlayerTurns.p5)
                        {
                            currentPlayer = PCs[i];
                            currentPlayer.StartTurnConditions();
                        }
                        break;
                    }
                case PlayerNumber.Player6:
                    {
                        if (pTurn == PlayerTurns.p6)
                        {
                            currentPlayer = PCs[i];
                            currentPlayer.StartTurnConditions();
                        }
                        break;
                    }
            }

        }
    }
}
