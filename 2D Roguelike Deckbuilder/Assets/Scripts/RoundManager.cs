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
    public HandView handView;

    //numeric variables
    public int maxEnergy = 3;
    public int energy;

    void Awake() {
        instance = this;    //initialize an instance of RoundManager before game starts
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Will be a list storing all of the cards in the starting hand
        List<Card> startingHand = new List<Card> 
        {
            new DefendCard(),
            new DefendCard(),
            new DefendCard(),
            new DefendCard()
        };

        handView.DisplayHand(startingHand); // starts the chain to display cards on screen

        energy = maxEnergy; //initialize energy to the max at turn's start

        //should be player's turn at start of the round
        currentState = gameState.playerTurn; 
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
