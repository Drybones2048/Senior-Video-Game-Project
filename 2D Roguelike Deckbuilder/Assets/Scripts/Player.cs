using UnityEngine;
using UnityEngine.EventSystems;
using System;

public enum PlayerClass { Ra, Set, Horus }

public class Player : MonoBehaviour
{
    //public variables
    public int maxHealth = 100; // Max health value that we want to give the player character
    public PlayerHealth healthBar;
    public int currentHealth;

    public bool weakened; // status effect to indicate the player will do 20% (balance pending) less damage on each attack card's listed value

    public int shieldAmount;
    
    public static event Action shieldBroken;

    public static event Action<int> shieldDamaged;

    public PlayerClass playerClass = PlayerClass.Horus;

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
    public void GainShield(int shield)
    {
        shieldAmount += shield;
    }

    public void TakeDamage(int damage){ // function that will be used strictly to keep track of damage

        if(shieldAmount > 0 && damage >= shieldAmount) // Player has shield but not enough to block all the damage
        {
            currentHealth -= (damage - shieldAmount);

            shieldAmount = 0;

            shieldBroken?.Invoke();

            //***UPDATE*** perfect block did not occur so set bool=false
            CombatManager.Instance.perfectBlock = false;

        } else if (shieldAmount > 0 && shieldAmount > damage) // Shield is able to block all of the damage
        {
            shieldAmount -= damage;

            if(shieldAmount == 0) // If shield is able to block all damage but it uses all shield, destroy shield
            {
                shieldBroken?.Invoke();
            }
            else // else just decrement shield val
            {
                shieldDamaged?.Invoke(damage);
            }

            //****UPDATE**** set bool in CombatManager after enemy attack if perfect block occured.
            CombatManager.Instance.perfectBlock = true;
            CombatManager.Instance.blockedDamage = damage;
            
        }
        else // Player takes regular damage because they played no block
        {
            currentHealth -= damage;

            //***UPDATE*** perfect block did not occur so set bool=false
            CombatManager.Instance.perfectBlock = false;
        }

        healthBar.setHealth(currentHealth);
    }

    public void TakeDirectDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
    }
} 
