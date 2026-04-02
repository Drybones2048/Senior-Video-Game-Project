using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyStatusEffects : MonoBehaviour
{
    public bool isStrengthened = false; // Boolean for strengthened condition (enemy deals 20% more damage per stack)

    public bool isWeakened = false; // Boolean for weakened status effect (20% decrease in incoming damage)
    public static event Action OnEnemyStatusEffectsChanged; // Event that other systems (e.g. UI) can subscribe to

    public static EnemyStatusEffects Instance { get; private set; } // Singleton pattern

    public List<StatusEffect> allStatusEffects = new List<StatusEffect>(); // List of all active status effects on the enemy

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RoundManager.enemyDead.AddListener(ClearAllStatusEffects);
    }

    void OnDestroy()
    {
        RoundManager.enemyDead.RemoveListener(ClearAllStatusEffects);
    }

    // Apply Strengthen to the enemy. Each stack adds a flat 20% damage bonus and lasts the whole fight.
    public void ApplyStrengthen(int stacks = 1)
    {
        if (isStrengthened && findStatusEffect(EffectType.Strengthen) != null) // If already strengthened, add more stacks
        {
            findStatusEffect(EffectType.Strengthen).quantity += stacks;
            Debug.Log($"Enemy Strengthen stacked! Now at {findStatusEffect(EffectType.Strengthen).quantity} stack(s) ({findStatusEffect(EffectType.Strengthen).quantity * 20}% bonus damage).");
            OnEnemyStatusEffectsChanged?.Invoke();
        }
        else // Otherwise add the strengthened condition to the list
        {
            StatusEffect strengthen = new StatusEffect();
            strengthen.effectType = EffectType.Strengthen;
            strengthen.effectStartOffset = 0;
            strengthen.turnDuration = -1;   // -1 signals this effect lasts the entire fight
            strengthen.quantity = stacks;
            strengthen.effectTarget = EffectTarget.Enemy;

            allStatusEffects.Add(strengthen);
            isStrengthened = true;

            Debug.Log($"Enemy is Strengthened! {stacks} stack(s) applied - {stacks * 20}% bonus damage for the rest of the fight.");
            OnEnemyStatusEffectsChanged?.Invoke();
        }
    }

    public void RemoveStrengthen() // Method to remove all strengthen from the enemy
    {
        StatusEffect strengthen = findStatusEffect(EffectType.Strengthen);
        if (strengthen == null) return;

        strengthen.turnDuration = 0;
        strengthen.quantity = 0;

        allStatusEffects.Remove(strengthen);
        isStrengthened = false;

        Debug.Log("Strengthen effect removed!");
        OnEnemyStatusEffectsChanged?.Invoke();
    }

    public void ApplyWeaken(int turns = 1, int quantity = 1) // Method to apply weaken to the enemy
    {
        if(isWeakened && findStatusEffect(EffectType.Weaken) != null) // If the enemy is already weakened, increase the duration
        {
            findStatusEffect(EffectType.Weaken).turnDuration += turns;
            OnEnemyStatusEffectsChanged.Invoke();
        }
        else // The enemy is not already weakened
        {
            StatusEffect weaken = new StatusEffect(); // Create weakened status effect

            weaken.effectType = EffectType.Weaken;
            weaken.effectStartOffset = 0;
            weaken.turnDuration = turns;
            weaken.quantity = quantity;
            weaken.effectTarget = EffectTarget.Enemy;

            allStatusEffects.Add(weaken);
            isWeakened = true;

            Debug.Log($"Enemy weakened for {weaken.turnDuration} turn(s)!");

            OnEnemyStatusEffectsChanged.Invoke();
        }
    }

    public void RemoveWeaken() // Method that will remove weaken off the enemy if they die or the duration runs out
    {
        StatusEffect weaken = findStatusEffect(EffectType.Weaken);
        weaken.turnDuration = 0;
        weaken.quantity = 0;

        allStatusEffects.Remove(weaken);
        isWeakened = false;
        Debug.Log("Weakened on the enemy removed!");
        OnEnemyStatusEffectsChanged.Invoke();
    }

    public void DecrementStatusEffects() // Enemy decrement the turn duration of status effects like weaken
    {
        if (HasAnyStatusEffects()) // Checks if the enemy has any status effects at all
        {
            for(int i = 0; i < allStatusEffects.Count; i++) // Goes through all status effects
            {
                if(allStatusEffects[i].effectType is EffectType.Weaken && allStatusEffects[i].turnDuration > 0) // Checks if enemy is weakened and has a positive turn duration
                {
                    allStatusEffects[i].turnDuration--; // Decreases number of turns weakened lasts for

                    if(allStatusEffects[i].turnDuration <= 0) // If the turn duration is zero or below, remove it as no longer active
                    {
                        RemoveWeaken();
                    }
                    else // If there are a positive amount of turns left with weaken on the enemy, update UI and keep weaken
                    {
                        Debug.Log($"Enemy weaken: {allStatusEffects[i].turnDuration} turn(s) remaining");
                        OnEnemyStatusEffectsChanged.Invoke();
                    }
                }
            }
        }
    }

    public int GetModifiedAttackDamage(int baseDamage) // Calculates how much damage an attack will do considering weaken and strengthen
    {
        int damageValue = baseDamage;

        if (isStrengthened)
        {
            StatusEffect strengthen = findStatusEffect(EffectType.Strengthen);
            if (strengthen != null)
            {
                float multiplier = 1f + (strengthen.quantity * 0.2f);
                damageValue = Mathf.FloorToInt(damageValue * multiplier);
            }
        }

        if (isWeakened)
        {
            damageValue = Mathf.FloorToInt(damageValue * 0.8f);
        }

        return damageValue;
    }

    public bool HasAnyStatusEffects() // Checks and sees if the enemy has any status effects on them
    {
        return isStrengthened || isWeakened;
    }

    public StatusEffect findStatusEffect(EffectType requestedType) // Interate to find requested status effect
    {
        foreach (StatusEffect statusEffect in allStatusEffects)
        {
            if (statusEffect.effectType == requestedType)
                return statusEffect;
        }

        return null;
    }

    void ClearAllStatusEffects() // Called when the enemy dies to reset for the next combat
    {
        if (isStrengthened)
        {
            RemoveStrengthen();
        }

        if (isWeakened)
        {
            RemoveWeaken();
        }
    }
}