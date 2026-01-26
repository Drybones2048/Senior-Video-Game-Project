using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100; // Max health value that we want to give the player character

    public PlayerHealth healthBar;
    public int currentHealth;

    void Start(){ 
        // Sets the healthbar full with its max value
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }   

    void Update(){
        if(Input.GetKeyDown(KeyCode.Mouse1)){
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage){ // function that will be used strictly to keep track of damage
        currentHealth -= damage;

        healthBar.setHealth(currentHealth);
    }
} 
