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
        energyChanged.Invoke(currentEnergy);
    }

    public void StartNewCombat() {
        roundNumber = 0;
        encounterNumber++; // Keeps track of which encounter we are on for purposes of enemy spawning
        currentEnemy.SpawnNewEnemy(encounterNumber); // Spawns the new enemy on screen

        roundNumber++; 
        routine.StartCombat();

        /*if (CombatManager.Instance.playerClass == PlayerClass.Set) {
            int random = RNG.Next(0, 3);
            if (random == 0)
            {
                //apply confuse
                EnemyStatusEffects.Instance.ApplyWeaken(3, 1); //temporary, until enemy confuse is implemented
            }
            else if (random == 1) {
                //apply poison
                EnemyStatusEffects.Instance.ApplyWeaken(3, 1); //temporary, until enemy poison is implemented
            }
            else if (random == 2) {
                //apply weaken
                EnemyStatusEffects.Instance.ApplyWeaken(3, 1);
            }
            else { }
        }*/
    }

    private void StartPlayerTurn()
    {
        Debug.Log("Starting player turn");

        ShieldManager.removeShield.Invoke(); // Removes all shield from the player at the start of a new round
        player.ResetShield();
        currentState = gameState.playerTurn;
        currentEnergy = maxEnergy;
        energyChanged.Invoke(currentEnergy);

        if (CombatManager.Instance.getPlayerClass() == PlayerClass.Set) {
            CombatManager.Instance.ApplySetPassive();
        }
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

    public void EndCombat() // Created an event for when the combat ends because the enemy dies (or player does but that is not programmed currently)
    {
        currentState = gameState.interim;
    }

    public void decrementEnergy(int amount) { // Method that decreases the current energy the player has for the turn
        if (currentEnergy - amount < 0) {
            throw new System.Exception("Card cost exceeded remaining energy.");
        }

        currentEnergy -= amount;
        energyChanged.Invoke(currentEnergy);
    }
    public void ReinitialiseForNewRun() // Is used by the start screen after the player chooses a class, resets all of the run variables
    {
        currentState = gameState.interim;

        roundNumber = 0;
        encounterNumber = 0;
        maxEnergy = defaultMaxEnergy;
        currentEnergy = maxEnergy;

        // Heal the player to full HP and remove all status effects
        player.currentHealth = player.maxHealth;
        player.healthBar.setMaxHealth(player.maxHealth);
        PlayerStatusEffects.Instance.clearAllStatusEffects();

        // Heal the enemy to full HP and remove all status effects
        currentEnemy.currentHealth = currentEnemy.maxHealth;
        currentEnemy.healthBar.setMaxHealth(currentEnemy.maxHealth);
        EnemyStatusEffects.Instance.ClearAllStatusEffects();

        currentSeed = Environment.TickCount;
        RNG = new System.Random(currentSeed);
        Debug.Log("New run started. Seed: " + currentSeed);

        energyChanged.Invoke(currentEnergy);
        StartNewCombat();
    }
}