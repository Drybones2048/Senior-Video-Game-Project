using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public Player player;
    public EnemyHealth healthBar;

    public GameObject centipede;

    public GameObject skeleton;

    [SerializeField] private EnemyAttack attack;
    [SerializeField] private int maxHealth; // The max health for the enemy will be set in the inspector
    [SerializeField] private int currentHealth; //don't attempt to modify from within inspector, that's only for debugging
    public int CurrentHealth => currentHealth;  //CurrentHealth is publicly readable, currentHealth is private. Also don't try and set it in the inspector

    public static UnityEvent<int> spawnNewEnemy = new UnityEvent<int>(); // An event that spawns a new enemy on screen based on the encounter number we are currently at

    void Awake() {
        RoundManager.startEnemyTurn.AddListener(StartEnemyTurn);
        RoundManager.enemyDead.AddListener(Die);
        spawnNewEnemy.AddListener(SpawnNewEnemy);
        skeleton.SetActive(false);
    }

    void OnDestroy() {
        RoundManager.startEnemyTurn.RemoveListener(StartEnemyTurn);
        RoundManager.enemyDead.RemoveListener(Die);
        spawnNewEnemy.RemoveListener(SpawnNewEnemy);
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

    public void SpawnNewEnemy(int encounterNumber) // Will spawn a new enemy based on the current encounter number
    {
        switch (encounterNumber)
        {
            case 1:
                centipede.SetActive(true);
                currentHealth = 55;
                healthBar.setMaxHealth(55);
                break;
            case 2:
                skeleton.SetActive(true);
                currentHealth = 75;
                healthBar.setMaxHealth(75);
                break;
        }
    }

    void Die() // Hides the character model of the enemy that just died.
    {
        if(centipede.activeSelf == true)
        {
            centipede.SetActive(false);
        } else if(skeleton.activeSelf == true)
        {
            skeleton.SetActive(false);
        }
    }
}
