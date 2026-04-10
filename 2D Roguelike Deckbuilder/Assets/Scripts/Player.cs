using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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

    public static UnityEvent playerDead = new UnityEvent();

    //private variables
    bool myTurn;

    void Start(){ 
        // Sets the healthbar full with its max value
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        shieldAmount = 0;
        ShieldManager.removeShield.AddListener(removeAllShield);

        StartScreen.afterChosenClass.AddListener(setSprite);
    }   

    void OnDestroy()
    {
        StartScreen.afterChosenClass.RemoveListener(setSprite);
    }

    void Update()
    {
        if (currentHealth <= 0 && CombatManager.Instance.radiantRebirth == true)
        {
            Heal((int)(maxHealth * 0.3));
            CombatManager.Instance.radiantRebirth = false;
        }
        else if (currentHealth <= 0 && CombatManager.Instance.radiantRebirth == false)
        {
            Debug.Log("Player dead!");
            playerDead.Invoke();
            CombatManager.combatOver.Invoke();
        }
        else { }
    }

    void setSprite() // Once the player has chosen their class on the select class screen, the sprite is set correctly here
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(CombatManager.Instance.getPlayerClass() == PlayerClass.Horus)
        {
            spriteRenderer.sprite = horusCharacter;
            
        } else if(CombatManager.Instance.getPlayerClass() == PlayerClass.Ra)
        {
            spriteRenderer.sprite = raCharacter;
            
        } else if(CombatManager.Instance.getPlayerClass() == PlayerClass.Set)
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
            if (CombatManager.Instance.getPlayerClass() == PlayerClass.Horus)
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

        if(currentHealth + heal <= maxHealth) // If the heal value would not put the player at or above max, do the full heal
        {
            currentHealth += heal;

        } else{ // If the heal would make the player go above the max health value, just set the player's health to max
            currentHealth = maxHealth;
        }
        healthBar.setHealth(currentHealth);
    }
} 
