using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public Player player;
    public EnemyHealth healthBar;
    public GameObject centipede;

    public GameObject skeleton;

    public GameObject pharaohPhase1;
    public GameObject pharaohPhase2;

    private EnemyAttack attack; // This is now assigned dynamically depending on which enemy is alive
    [SerializeField] public int maxHealth; // The max health for the enemy will be set in the inspector
    [SerializeField] public int currentHealth; //don't attempt to modify from within inspector, that's only for debugging
    public int CurrentHealth => currentHealth;  //CurrentHealth is publicly readable, currentHealth is private. Also don't try and set it in the inspector

    public static UnityEvent<int> spawnNewEnemy = new UnityEvent<int>(); // An event that spawns a new enemy on screen based on the encounter number we are currently at

    void Awake() {
        RoundManager.startEnemyTurn.AddListener(StartEnemyTurn);
        RoundManager.enemyDead.AddListener(Die);
        spawnNewEnemy.AddListener(SpawnNewEnemy);
        skeleton.SetActive(false);
        pharaohPhase1.SetActive(false);
        pharaohPhase2.SetActive(false);
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

        if(player.currentHealth <= 0 && !CombatManager.Instance.radiantRebirth) // If the player dies after the enemy's attack, do not continue with combat practices
        {
            return;
        }

        RoundManager.endEnemyTurn.Invoke();
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;

        if(currentHealth <= 0) // Every time the enemy takes damage, check to see if it is fatal
        {
            RoundManager.instance.EndCombat();
            RoundManager.enemyDead.Invoke();
        }

        healthBar.setHealth(currentHealth);
    }

    public void SpawnNewEnemy(int encounterNumber) // Will spawn a new enemy based on the current encounter number
    {
        switch (encounterNumber)
        {
            case 1:
                centipede.SetActive(true);
                skeleton.SetActive(false);
                pharaohPhase1.SetActive(false);
                pharaohPhase2.SetActive(false);
                SetupEnemy(centipede, 55);  //55
                break;
            case 2:
                centipede.SetActive(false);
                skeleton.SetActive(true);
                pharaohPhase1.SetActive(false);
                pharaohPhase2.SetActive(false);
                SetupEnemy(skeleton, 75); //75
                break;
            case 3:
                centipede.SetActive(false);
                skeleton.SetActive(false);
                pharaohPhase1.SetActive(true);
                pharaohPhase2.SetActive(false);
                SetupEnemy(pharaohPhase1, 80); //80
                break;
            case 4:
                centipede.SetActive(false);
                skeleton.SetActive(false);
                pharaohPhase1.SetActive(false);
                pharaohPhase2.SetActive(true);
                SetupEnemy(pharaohPhase2, 120); //120
                break;
        }
    }

    private void SetupEnemy(GameObject enemyObject, int health) // Helper function that serves to set health values for a given enemy and grab its' attack script
    {
        currentHealth = health;
        healthBar.setMaxHealth(health);
        healthBar.setHealth(health);
        EnemyStatusEffects.Instance.ClearAllStatusEffects();

        // Grab the EnemyAttack component from the specific child that just spawned, which will have its own ScriptableObject attack pattern assigned
        attack = enemyObject.GetComponent<EnemyAttack>();
        if (attack == null)
        {
            Debug.LogWarning($"Enemy.cs: No EnemyAttack component found on {enemyObject.name}. Make sure each enemy GameObject has one attached with its attack pattern assigned.");
        } 
    }

    void Die() // Hides the character model of the enemy that just died.
    {
        CombatManager.combatOver.Invoke();  //Unity event to indicate the combat has ended

        if (centipede.activeSelf == true)
        {
            centipede.SetActive(false);
        }
        else if (skeleton.activeSelf == true)
        {
            skeleton.SetActive(false);
        }
        else if (pharaohPhase1.activeSelf == true)
        {
            pharaohPhase1.SetActive(false);
            RoundManager.instance.StartNewCombat();
        }
        else if (pharaohPhase2.activeSelf == true)
        {
            pharaohPhase2.SetActive(false);
            DeathScreen.gameWon.Invoke();
        }
        else { }
    }
}
