using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public Player player;
    public EnemyHealth healthBar;
    [SerializeField] private EnemyAttack attack;
    [SerializeField] private int maxHealth; // The max health for the enemy will be set in the inspector
    [SerializeField] private int currentHealth; //don't attempt to modify from within inspector, that's only for debugging
    public int CurrentHealth => currentHealth;  //CurrentHealth is publicly readable, currentHealth is private. Also don't try and set it in the inspector

    void Awake() {
        RoundManager.startEnemyTurn.AddListener(StartEnemyTurn);
    }

    void OnDestroy() {
        RoundManager.startEnemyTurn.RemoveListener(StartEnemyTurn);
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    void StartEnemyTurn() {
        attack.attackPlayer(player);
        Debug.Log("Enemy turn ended, resolving");
        RoundManager.endEnemyTurn.Invoke();
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;

        healthBar.setHealth(currentHealth);
    }
}
