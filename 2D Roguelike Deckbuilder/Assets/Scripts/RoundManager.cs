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

    public Enemy currentEnemy;
    public Player player;

    public System.Random RNG { get; private set; }  //One RNG for the whole program
    public int currentSeed { get; private set; }
    public static UnityEvent endPlayerTurn = new UnityEvent();
    public static UnityEvent endEnemyTurn = new UnityEvent();
    public static UnityEvent startPlayerTurn = new UnityEvent();
    public static UnityEvent startEnemyTurn = new UnityEvent();
    public static UnityEvent combatStart = new UnityEvent();
    public static UnityEvent enemyDead = new UnityEvent();
    public static UnityEvent dealHand = new UnityEvent();

    public int encounterNumber = 0;

    public static UnityEvent<int> energyChanged = new UnityEvent<int>();
    public gameState currentState { get; private set; }
    public TurnEndRoutine routine;

    [SerializeField] private int defaultMaxEnergy = 3; //reference point, would only ever be different from max energy if an effect altered your starting energy amount
    public int maxEnergy { get; private set; }
    public int currentEnergy { get; private set; }
    public int roundNumber { get; private set; }

    void Awake() {
        instance = this;    //initialize an instance of RoundManager before game starts
        startNewRun();
        //initialize energy and state variables in awake because other scripts depend on them
        maxEnergy = defaultMaxEnergy;
        currentEnergy = maxEnergy;
        currentState = gameState.interim;

        endEnemyTurn.AddListener(EndEnemyTurn);
        startEnemyTurn.AddListener(SetEnemyState);
        startPlayerTurn.AddListener(StartPlayerTurn);
        enemyDead.AddListener(EndCombat);
    }

    void OnDestroy() {
        endEnemyTurn.RemoveListener(EndEnemyTurn);
        startEnemyTurn.RemoveListener(SetEnemyState);
        startPlayerTurn.RemoveListener(StartPlayerTurn);
        enemyDead.RemoveListener(EndCombat);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNewCombat(); //we can put this here for now, but when we create multiple rounds, we'll need this to be called multiple times
        energyChanged.Invoke(currentEnergy);
    }

    void startNewRun(int? seed = null) {
        //TODO: Make sure through play-testing that it's not possible to get some super unlucky seed draws
        //i.e. make sure you can't get seeds that are so unfavorable that it makes the run unfairly difficult
        roundNumber = 0;
        currentSeed = seed ?? Environment.TickCount;
        RNG = new System.Random(currentSeed);
        Debug.Log("Run Seed: " + currentSeed);
    }

    public void StartNewCombat() {
        roundNumber = 0;
        encounterNumber++; // Keeps track of which encounter we are on for purposes of enemy spawning
        currentEnemy.SpawnNewEnemy(encounterNumber); // Spawns the new enemy on screen

        roundNumber++; 
        routine.StartCombat();
    }

    private void StartPlayerTurn()
    {
        Debug.Log("Starting player turn");

        ShieldManager.removeShield.Invoke(); // Removes all shield from the player at the start of a new round
        player.ResetShield();
        currentState = gameState.playerTurn;
        currentEnergy = maxEnergy;
        energyChanged.Invoke(currentEnergy);
    }

    public void endTurnButton() {
        if (currentState == gameState.playerTurn)
        {
            currentState = gameState.interim;
            Debug.Log("Player turn ended, resolving");
            endPlayerTurn.Invoke();
            PlayerStatusEffects.Instance.DecrementStatusEffects(); // Decrement all status effects that the player has on them at the end of a round
            routine.EndPlayerTurn();    //start coroutine
        }
        else {
            Debug.Log("Not the player's turn!");
        }
    }

    //Just sets the currentState to enemyTurn
    private void SetEnemyState() {
        currentState = gameState.enemyTurn;
    }

    private void EndEnemyTurn() {
        currentState = gameState.interim;
        EnemyStatusEffects.Instance.DecrementStatusEffects();
        roundNumber++;  //this is the only place other than startCombat(called once per combat) where roundNumber is incremented
        routine.EndEnemyTurn(); //start coroutine
    }

    private void EndCombat() // Created an event for when the combat ends because the enemy dies (or player does but that is not programmed currently)
    {
        currentState = gameState.interim;
    }

    public void decrementEnergy(int amount) {
        if (currentEnergy - amount < 0) {
            throw new System.Exception("Card cost exceeded remaining energy.");
        }

        currentEnergy -= amount;
        energyChanged.Invoke(currentEnergy);
    }
}
