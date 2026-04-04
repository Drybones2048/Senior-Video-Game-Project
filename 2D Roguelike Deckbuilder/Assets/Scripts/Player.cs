using UnityEngine;
using UnityEngine.EventSystems;
using System;

public enum PlayerClass { All, Ra, Set, Horus }

public class Player : MonoBehaviour
{
    //public variables
    public int maxHealth = 100; // Max health value that we want to give the player character
    public PlayerHealth healthBar;
    public int currentHealth;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite horusCharacter;
    [SerializeField] private Sprite setCharacter;
    [SerializeField] private Sprite raCharacter;

    public bool weakened; // status effect to indicate the player will do 20% (balance pending) less damage on each attack card's listed value

    public int shieldAmount;
    
    public static event Action shieldBroken;

    public static event Action<int> shieldDamaged;

    //private variables
    bool myTurn;

    void Start(){ 
        // Sets the healthbar full with its max value
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        shieldAmount = 0;
        ShieldManager.removeShield.AddListener(removeAllShield);

        // Rest of this method is used to set the character's sprite based on the Egyptian god picked
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(CombatManager.Instance.playerClass == PlayerClass.Horus)
        {
            spriteRenderer.sprite = horusCharacter;
            
        } else if(CombatManager.Instance.playerClass == PlayerClass.Ra)
        {
            spriteRenderer.sprite = raCharacter;
            
        } else if(CombatManager.Instance.playerClass == PlayerClass.Set)
        {
            spriteRenderer.sprite = setCharacter;

        }
    }   

    public void GainShield(int shield)
    {
        shieldAmount += shield;
    }
    public void ResetShield() 
    {
        shieldAmount = 0;
    }

    public void TakeDamage(int damage){ // function that will be used strictly to keep track of damage
        if (shieldAmount > 0 && damage > shieldAmount) // Player has shield but not enough to block all the damage
        {
            currentHealth -= (damage - shieldAmount);

            shieldAmount = 0;

            shieldBroken?.Invoke();

        } else if (shieldAmount > 0 && shieldAmount >= damage) // Shield is able to block all of the damage
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

            //***UPDATE*** Now deal block damage for Horus class after a perfect block 
            if (CombatManager.Instance.playerClass == PlayerClass.Horus)
            {
                CombatManager.Instance.DealBlockDamage(damage);
            }
            
        }
        else // Player takes regular damage because they played no block
        {
            currentHealth -= damage;
        }

        healthBar.setHealth(currentHealth);
    }

    void removeAllShield() // Method that is called to update the player's shield value to 0 at the end of a round
    {
        shieldAmount = 0;
    }

    public void TakeDirectDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
    }

    public void Heal(int heal) {
        currentHealth += heal;
        healthBar.setHealth(currentHealth);
    }
} 
