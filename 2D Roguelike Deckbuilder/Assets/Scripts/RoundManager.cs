using UnityEngine;
using System.Collections.Generic;

public enum gameState { 
    playerTurn,
    enemyTurn,
    interim
}

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;    //instance of RoundManager
    public gameState currentState { get; private set; }    //initialize game state

    //numeric variables
    public int maxEnergy = 3;
    public static int energy;

    void Awake() {
        instance = this;    //initialize an instance of RoundManager before game starts
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energy = maxEnergy; //initialize energy to the max at turn's start

        //should be player's turn at start of the round
        currentState = gameState.playerTurn; 
    }
}
