using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Player : MonoBehaviour
{
    //public variables
    public int maxHealth = 100; // Max health value that we want to give the player character
    public PlayerHealth healthBar;
    public int currentHealth;

    public int shieldAmount;
    
    public static event Action shieldBroken;

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

        if(shieldAmount > 0 && damage > shieldAmount) // Player has shield but not enough to block all the damage
        {
            currentHealth = currentHealth - (damage - shieldAmount);

            shieldBroken?.Invoke();

        } else if (shieldAmount > 0 && shieldAmount > damage) // Shield is able to block all of the damage
        {
            shieldAmount -= damage;
        }
        else // Player takes regular damage because they played no block
        {
            currentHealth -= damage;
        }

        healthBar.setHealth(currentHealth);
    }

    public void GainShield(int shield)
    {
        shieldAmount += shield;
    }
} 
