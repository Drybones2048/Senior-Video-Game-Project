using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance {get; private set;}
    public static UnityEvent<CardInstance> shieldGained = new UnityEvent<CardInstance>();

    public Enemy currentEnemy;

    public Player player;

    public AudioClip attackSound;

    public GameObject slash;

    public Animator animator;

    public bool perfectBlock = false;   //based on whether you perfectly blocked all damage on the enemy's last attack
    public float blockedDamage = 0f;
    public float perfectBlockPercent = 0.3f;
    bool empoweringShield = false;
    bool juggernaut = false;

    void Awake()
    {
        Instance = this;
        CardView.playerCardPlayed.AddListener(PlayPlayerCard);
        slash.SetActive(false);
    }

    void OnDestroy() {
        CardView.playerCardPlayed.RemoveListener(PlayPlayerCard);
    }

    void PlayPlayerCard(CardInstance card) {
        if (card.type == CardType.Attack) {
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

        else if (card.type == CardType.Defend) {
            player.GainShield(card.block);
            shieldGained.Invoke(card);
        }
        
        else {}
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
            return PlayerStatusEffects.Instance.isWeakened;
        }

        return false;
    }

    IEnumerator SlashReveal(float delayTime) // Coroutine will wait for the sprite animation to play before hiding it again
    {
        yield return new WaitForSeconds(delayTime); 
        slash.SetActive(false); // hide the slash sprite
    }

    public void DealBlockDamage() {
        if (perfectBlock)
        {
            currentEnemy.TakeDamage((int)(blockedDamage * perfectBlockPercent));
            if (empoweringShield)
            {
                ApplyEmpoweringShield();
            }
            if (juggernaut)
            {
                ApplyJuggernaut();
            }
        }
    }

    void ApplyEmpoweringShield() {
        //Create a StatusEffect and add it to the list in PlayerStatus Effects
        StatusEffect strengthenEffect = new StatusEffect();
        strengthenEffect.effectType = EffectType.Strengthen;
        strengthenEffect.effectStartOffset = 0;
        strengthenEffect.turnDuration = 1;
        strengthenEffect.quantity = 2;
        strengthenEffect.effectTarget = EffectTarget.Player;

        PlayerStatusEffects.Instance.allStatusEffects.Add(strengthenEffect);
    }

    void ApplyJuggernaut() {
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
