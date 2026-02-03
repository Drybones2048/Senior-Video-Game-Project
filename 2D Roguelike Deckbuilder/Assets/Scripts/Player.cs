using UnityEngine;

public class Player : MonoBehaviour
{
    //public variables
    public int maxHealth = 100; // Max health value that we want to give the player character
    public PlayerHealth healthBar;
    public int currentHealth;

    //private variables
    bool myTurn;

    void Start(){ 
        // Sets the healthbar full with its max value
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);

        //Indicate that it is the player's turn and ready for input
        myTurn = true;
    }   

    void Update(){
        //only accept input if it's the player's turn
        if (myTurn) {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                TakeDamage(20);
            }
        }
    }

    void TakeDamage(int damage){ // function that will be used strictly to keep track of damage
        currentHealth -= damage;

        healthBar.setHealth(currentHealth);
    }
} 
