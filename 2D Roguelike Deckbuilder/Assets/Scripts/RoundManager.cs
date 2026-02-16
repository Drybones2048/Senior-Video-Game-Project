using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public enum gameState { 
    playerTurn,
    enemyTurn,
    interim
}

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;    //singleton

    public System.Random RNG { get; private set; }  //One RNG for the whole program
    public int currentSeed { get; private set; }
    public static UnityEvent endPlayerTurn = new UnityEvent();
    public static UnityEvent<int> energyChanged = new UnityEvent<int>();
    public gameState currentState;
    public TurnEndRoutine routine;

    [SerializeField] private int defaultMaxEnergy = 3; //reference point, would only ever be different from max energy if an effect altered your starting energy amount
    public int maxEnergy { get; private set; }
    public int currentEnergy { get; private set; }

    void Awake() {
        instance = this;    //initialize an instance of RoundManager before game starts
        startNewRun();

        Enemy.endEnemyTurn.AddListener(EndEnemyTurn);
        TurnEndRoutine.StartPlayerTurn.AddListener(startPlayerTurn);
    }

    void OnDestroy() {
        Enemy.endEnemyTurn.RemoveListener(EndEnemyTurn);
        TurnEndRoutine.StartPlayerTurn.RemoveListener(startPlayerTurn);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPlayerTurn();
    }

    void startNewRun(int? seed = null) {
        //initialize energy and state variables in awake because other scripts depend on them
        maxEnergy = defaultMaxEnergy;
        currentEnergy = maxEnergy;
        currentState = gameState.playerTurn;

        //TODO: Make sure through play-testing that it's not possible to get some super unlucky seed draws
        //i.e. make sure you can't get seeds that are so unfavorable that it makes the run unfairly difficult
        currentSeed = seed ?? Environment.TickCount;
        RNG = new System.Random(currentSeed);

        Debug.Log("Run Seed: " + currentSeed);
    }

    private void startPlayerTurn()
    {
        currentState = gameState.playerTurn;
        currentEnergy = maxEnergy;
        energyChanged.Invoke(currentEnergy);
    }

    public void endTurnButton() {
        if (currentState == gameState.playerTurn)
        {
            currentState = gameState.interim;
            Debug.Log("Player turn ended, starting enemy turn");
            routine.EndPlayerTurn();    //start coroutine
        }
        else {
            Debug.Log("Not the player's turn!");
        }
    }

    private void EndEnemyTurn() {
        currentState = gameState.interim;
        routine.EndEnemyTurn(); //start coroutine
    }

    public void decrementEnergy(int amount) {
        if (currentEnergy - amount < 0) {
            throw new System.Exception("Card cost exceeded remaining energy.");
        }

        currentEnergy -= amount;
        energyChanged.Invoke(currentEnergy);
    }
}
