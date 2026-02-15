using UnityEngine;
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
    public static UnityEvent endPlayerTurn = new UnityEvent();
    public static UnityEvent<int> energyChanged = new UnityEvent<int>();
    public gameState currentState { get; private set; }

    //numeric variables
    [SerializeField] private int defaultMaxEnergy = 3; //reference point, would only ever be different from max energy if an effect altered your starting energy amount
    public int maxEnergy { get; private set; }
    public int currentEnergy { get; private set; }

    void Awake() {
        instance = this;    //initialize an instance of RoundManager before game starts
        //initialize energy and state variables in awake because other scripts depend on them
        maxEnergy = defaultMaxEnergy;
        currentEnergy = maxEnergy;
        currentState = gameState.playerTurn;

        //TODO: Add listener to startPlayerTurn function for endEnemyTurn event from whichever function it is created
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPlayerTurn();
    }

    private void startPlayerTurn()
    {
        currentEnergy = maxEnergy;
        currentState = gameState.playerTurn;
        energyChanged.Invoke(currentEnergy);
    }

    public void endTurnButton() {
        currentState = gameState.enemyTurn;
        endPlayerTurn.Invoke();
        Debug.Log("Player turn ended, starting enemey turn");
    }

    public void decrementEnergy(int amount) {
        if (currentEnergy - amount < 0) {
            throw new System.Exception("Card cost exceeded remaining energy.");
        }

        currentEnergy -= amount;
        energyChanged.Invoke(currentEnergy);
    }
}
