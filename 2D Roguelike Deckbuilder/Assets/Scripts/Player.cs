using UnityEngine;

public class Player : MonoBehaviour
{
    //public variables
    public int maxHealth = 100; // Max health value that we want to give the player character
    public PlayerHealth healthBar;
    public int currentHealth;

    public int shieldAmount;

    //private variables
    bool myTurn;

    void Start(){ 
        // Sets the healthbar full with its max value
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        shieldAmount = 0;
    }   

    void Update(){
        
    }

    public void TakeDamage(int damage){ // function that will be used strictly to keep track of damage
        currentHealth -= damage;

        healthBar.setHealth(currentHealth);
    }

    public void GainShield(int shield)
    {
        shieldAmount += shield;
    }
} 
