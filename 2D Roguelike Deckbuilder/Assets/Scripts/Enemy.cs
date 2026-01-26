using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Max health value that we want to give the player character

    public EnemyHealth healthBar;
    public int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage){
        currentHealth -= damage;

        healthBar.setHealth(currentHealth);
    }
}
