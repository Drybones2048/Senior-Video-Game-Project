using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance {get; private set;}
    public static UnityEvent<CardInstance> shieldGained = new UnityEvent<CardInstance>();
    public static UnityEvent combatOver = new UnityEvent();

    public Enemy currentEnemy;

    public Player player;
    public PlayerClass playerClass;

    public AudioClip attackSound;

    public GameObject slash;

    public Animator animator;

    public float blockDamageDelay = 0.5f;
    public float perfectBlockPercent = 0.3f;
    float initialPerfectBlockPercent;
    bool empoweringShield = false;
    bool juggernaut = false;

    void Awake()
    {
        Instance = this;
        CardView.playerCardPlayed.AddListener(PlayPlayerCard);
        slash.SetActive(false);
        combatOver.AddListener(Reset);
        initialPerfectBlockPercent = perfectBlockPercent;   //saving the block damage at the start of the run as a return point for resetting the combat.
    }

    void OnDestroy() {
        CardView.playerCardPlayed.RemoveListener(PlayPlayerCard);
        combatOver.RemoveListener(Reset);
    }

    void Reset() {
        empoweringShield = false;
        juggernaut = false;
        perfectBlockPercent = initialPerfectBlockPercent;
    }

    void PlayPlayerCard(CardInstance card) {
        if (card.type == CardType.Attack)
        {
            int actualDamage = GetActualDamage(card.damage);
            currentEnemy.TakeDamage(actualDamage);
            if (card.uniqueBehavior == UniqueBehavior.PressAndFall)
            {
                PressAndFall(card, actualDamage);
            }

            PlayAttackEffectsAndSounds();

            if (currentEnemy.CurrentHealth <= 0) // When the enemy's health reaches 0 or lower, will trigger event that after card rewards should resent the combat with a new enemy
            {
                RoundManager.enemyDead.Invoke();
            }

        }

        else if (card.type == CardType.Defend)
        {
            player.GainShield(card.block);
            shieldGained.Invoke(card);
        }

        else if (card.type == CardType.Persistent)
        {
            if (card.uniqueBehavior == UniqueBehavior.Juggernaut)
            {
                juggernaut = true;
            }
            if (card.uniqueBehavior == UniqueBehavior.EmpoweringShield)
            {
                empoweringShield = true;
            }
        }

        else if (card.type == CardType.Special) {
            if (card.uniqueBehavior == UniqueBehavior.RiteOfFrailty)
            {
                player.TakeDamage(card.damage);
                EnemyStatusEffects.Instance.ApplyWeaken(2, 1);
            }
            else if (card.uniqueBehavior == UniqueBehavior.LifeDrain)
            {
                player.Heal(3 * EnemyStatusEffects.Instance.allStatusEffects.Count(e => e.effectType == EffectType.Weaken || e.effectType == EffectType.Poison || e.effectType == EffectType.Confuse));
            }
            else if (card.uniqueBehavior == UniqueBehavior.StaggeringShield)
            {
                EnemyStatusEffects.Instance.ApplyWeaken(2, 1);
                player.GainShield(card.block);
                shieldGained.Invoke(card);
            }
            else if (card.uniqueBehavior == UniqueBehavior.SerpentsGift)
            {
                //apply poison, this line is temporary until poisoning enemy is implemented
                EnemyStatusEffects.Instance.ApplyWeaken(2, 1);
            }
            else if (card.uniqueBehavior == UniqueBehavior.EyeOfEternity)
            {
                //apply poison, this line is temporary until confusing enemy is implemented
                EnemyStatusEffects.Instance.ApplyWeaken(2, 1);
            }
            else if (card.uniqueBehavior == UniqueBehavior.HexStorm) {
                currentEnemy.TakeDamage(card.damage * EnemyStatusEffects.Instance.allStatusEffects.Count(e => e.effectType == EffectType.Weaken || e.effectType == EffectType.Poison || e.effectType == EffectType.Confuse));
            }
            else { }
        }


        else { }
    }

    public int GetActualDamage(int attackAmount) // Get the actual damage that the card will do considering status effects like weaken (or strengthen in the future)
    {
        if (PlayerStatusEffects.Instance != null)
        {
            return PlayerStatusEffects.Instance.GetModifiedAttackDamage(attackAmount);
        }

        return attackAmount; // No status effects system, return base damage
    }

    // Check if damage is modified by status effects
    public bool IsDamageModified()
    {
        if (PlayerStatusEffects.Instance != null)
        {
            return PlayerStatusEffects.Instance.isWeakened || PlayerStatusEffects.Instance.isStrengthened;
        }

        return false;
    }

    IEnumerator SlashReveal(float delayTime) // Coroutine will wait for the sprite animation to play before hiding it again
    {
        yield return new WaitForSeconds(delayTime); 
        slash.SetActive(false); // hide the slash sprite
    }

    //a coroutine that delays the slash effect when block damage is dealt
    IEnumerator BlockDamageRoutine(int blockedDamage)
    {
        yield return new WaitForSeconds(blockDamageDelay);
        currentEnemy.TakeDamage((int)(blockedDamage * perfectBlockPercent));
        PlayAttackEffectsAndSounds();
        if (empoweringShield)
        {
            ApplyEmpoweringShield();
        }
        if (juggernaut)
        {
            ApplyJuggernaut();
        }
    }

    public void DealBlockDamage(int blockedDamage) {
        StartCoroutine(BlockDamageRoutine(blockedDamage));
    }

    void ApplyEmpoweringShield() {
        Debug.Log("Empowering shield effect triggered");

        PlayerStatusEffects.Instance.ApplyStrengthen(2, 2); // Calls the singleton of the status player status effect class and applies 2 strengthen for the 2 turn default
    }

    void ApplyJuggernaut() {
        Debug.Log("Juggernaut effect triggered");
        perfectBlockPercent *= 2f;
    }

    void PressAndFall(CardInstance card, int actualDamage) {
        player.GainShield(actualDamage);
        shieldGained.Invoke(card);
    }

    void PlayAttackEffectsAndSounds() {
        // Show the slash sprite and play the animation
        slash.SetActive(true);
        animator.Play("Sword Slash Animation", 0, 0);


        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position, 0.2f);
        }

        StartCoroutine(SlashReveal(0.5f)); // wait for animation before hiding the sprite
    }
}
