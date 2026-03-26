using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyStatusEffects : MonoBehaviour
{
    public bool isStrengthened = false; // Boolean for strengthened condition (enemy deals 20% more damage per stack)

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

    // Returns the actual damage the enemy deals after applying Strengthen stacks.
    // Each stack adds a flat 20%, so 2 stacks = 40% bonus, 3 stacks = 60%, etc.
    public int GetModifiedAttackDamage(int baseDamage)
    {
        if (isStrengthened)
        {
            StatusEffect strengthen = findStatusEffect(EffectType.Strengthen);
            if (strengthen != null)
            {
                float multiplier = 1f + (strengthen.quantity * 0.2f);
                return Mathf.FloorToInt(baseDamage * multiplier);
            }
        }

        return baseDamage;
    }

    public bool HasAnyStatusEffects() // Checks and sees if the enemy has any status effects on them
    {
        return isStrengthened;
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
    }
}