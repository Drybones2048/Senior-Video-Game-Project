using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerStatusEffects : MonoBehaviour {
    public bool isWeakened = false; // Boolean for weakened condition (player deals 20% less damage)

    public bool isPoisoned = false; // Boolean for poisoned condition (player takes 5-x damage every turn, with x being number of turns)

    public bool isStrengthened = false; // Boolean for strengthened condition (player does 20% more damage per attack card for 2 turns)

    public Player player;

    public static event Action OnStatusEffectsChanged; // Events that other systems can subscribe to

    public static PlayerStatusEffects Instance { get; private set; } // Singleton pattern (may not be needed)


    public List<StatusEffect> allStatusEffects = new List<StatusEffect>(); // List to store the different status effects the player has

    void Awake()
    {
        if (Instance == null) // Set up singleton
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RoundManager.endPlayerTurn.AddListener(PoisonDamage);
        RoundManager.enemyDead.AddListener(clearAllStatusEffects);
    }

    void OnDestroy()
    {
        RoundManager.endPlayerTurn.RemoveListener(PoisonDamage);
        RoundManager.enemyDead.RemoveListener(clearAllStatusEffects);
    }

    public void ApplyWeaken(int turns = 1, int quantity = 1) // Apply weaken status effect
    {
        if (isWeakened && findStatusEffect(EffectType.Weaken) != null) // If the player is already weakened, extend the duration of the weaken
        {
            findStatusEffect(EffectType.Weaken).turnDuration += turns;
            OnStatusEffectsChanged?.Invoke();
        }
        else
        {
            StatusEffect weaken = new StatusEffect(); // Create a new weakened status effect object

            // Fill in weakened's information based on the enemy attack
            weaken.effectType = EffectType.Weaken;
            weaken.effectStartOffset = 0;
            weaken.turnDuration = turns;
            weaken.quantity = quantity;
            weaken.effectTarget = EffectTarget.Player;
            
            allStatusEffects.Add(weaken); // Add status effect to list of status effects that the player is inflicted with
            isWeakened = true;

            Debug.Log($"Player weakened for {weaken.turnDuration} turn(s)!");
        
            OnStatusEffectsChanged?.Invoke();  // Notify listeners that status effects changed
        }
    }

    public void RemoveWeaken() // Remove weaken status effect
    {
        StatusEffect weaken = findStatusEffect(EffectType.Weaken);
        weaken.turnDuration = 0;
        weaken.quantity = 0;

        allStatusEffects.Remove(weaken); // Remove weakened from list of status effects
        isWeakened = false;
        
        Debug.Log("Weaken effect removed!");
        OnStatusEffectsChanged?.Invoke();
    }

    public void ApplyPoison(int turns = 5, int quantity = 5)
    {
        if(isPoisoned && findStatusEffect(EffectType.Poison) != null)
        {
            findStatusEffect(EffectType.Poison).quantity += quantity;
            Debug.Log($"More poison has been added! {findStatusEffect(EffectType.Poison).quantity} poison remaining for {findStatusEffect(EffectType.Poison).turnDuration} turns!");
            OnStatusEffectsChanged?.Invoke();
        }
        else
        {
            StatusEffect poisoned = new StatusEffect();
            poisoned.effectType = EffectType.Poison;
            poisoned.effectStartOffset = 0;
            poisoned.turnDuration = turns;
            poisoned.quantity = quantity;
            poisoned.effectTarget = EffectTarget.Player;

            allStatusEffects.Add(poisoned);
            isPoisoned = true;

            Debug.Log($"Player poisoned for {poisoned.turnDuration} turn(s)!");
            OnStatusEffectsChanged?.Invoke();
        }
    }

    public void PoisonDamage()
    {
        if (isPoisoned)
        {
           int poisonDamage = findStatusEffect(EffectType.Poison).quantity;
            Debug.Log($"Player takes {poisonDamage} damage from poison!");
            player.TakeDirectDamage(poisonDamage); 
        } 
    }

    public void RemovePoison()
    {
        StatusEffect poison = findStatusEffect(EffectType.Poison);
        poison.turnDuration = 0;
        poison.quantity = 0;

        allStatusEffects.Remove(poison);
        isPoisoned = false;

        Debug.Log("Poison effect removed!");
        OnStatusEffectsChanged?.Invoke();
    }

    public void ApplyStrengthen(int turns = 2, int quantity = 1) // Method to apply strengthen to the character
    {
        if(isStrengthened && findStatusEffect(EffectType.Strengthen) != null)
        {
            findStatusEffect(EffectType.Strengthen).quantity += quantity;
            findStatusEffect(EffectType.Strengthen).turnDuration += turns;
            OnStatusEffectsChanged?.Invoke();
        }
        else
        {
            StatusEffect strengthened = new StatusEffect();
            strengthened.effectType = EffectType.Strengthen;
            strengthened.effectStartOffset = 0;
            strengthened.turnDuration = turns;
            strengthened.quantity = quantity;
            strengthened.effectTarget = EffectTarget.Player;

            allStatusEffects.Add(strengthened);
            isStrengthened = true;

            Debug.Log($"Player strengthened for {strengthened.turnDuration} turn(s)!");
            OnStatusEffectsChanged?.Invoke();
        }
    }

    public void RemoveStrengthen() // Remove strengthen status effect
    {
        StatusEffect strengthen = findStatusEffect(EffectType.Strengthen);
        strengthen.turnDuration = 0;
        strengthen.quantity = 0;

        allStatusEffects.Remove(strengthen); // Remove strengthen from list of status effects
        isStrengthened = false;
        
        Debug.Log("Strengthened effect removed!");
        OnStatusEffectsChanged?.Invoke();
    }

    public void DecrementStatusEffects() // Call this at the end of the player's turn to decrement status effects and remove any text or symbols
    {
        if(HasAnyStatusEffects()) // Checks to see if there are any status effects currently on the player
        {
            for(int i = 0; i < allStatusEffects.Count; i++) // If there are any status effects, go through them one by one and decrement their turn duration
            {
                if(allStatusEffects[i].effectType is EffectType.Weaken && allStatusEffects[i].turnDuration > 0) // Check to see if player is weakened
                {
                    allStatusEffects[i].turnDuration--; // Decrements turn duration

                    if(allStatusEffects[i].turnDuration <= 0) // If the turn duration is 0 or lower, remove the weakened status
                    {
                        RemoveWeaken();
                    }
                    else // Player still has weaken
                    {
                        Debug.Log($"Weaken: {allStatusEffects[i].turnDuration} turn(s) remaining");
                        OnStatusEffectsChanged?.Invoke();
                    }
                }
                else if(allStatusEffects[i].effectType is EffectType.Poison && allStatusEffects[i].turnDuration > 0)
                {
                    allStatusEffects[i].turnDuration--; // Decrements turn duration
                    allStatusEffects[i].quantity--;

                    if(allStatusEffects[i].turnDuration <= 0)
                    {
                        RemovePoison();
                    }
                    else
                    {
                        Debug.Log($"Poison: {allStatusEffects[i].turnDuration} turn(s) remaining");
                        OnStatusEffectsChanged?.Invoke();
                    }
                }
                else if(allStatusEffects[i].effectType is EffectType.Strengthen && allStatusEffects[i].turnDuration > 0)
                {
                    allStatusEffects[i].turnDuration--; // Decrements turn duration

                    if(allStatusEffects[i].turnDuration <= 0)
                    {
                        RemoveStrengthen();
                    }
                    else
                    {
                        Debug.Log($"Strengthen: {allStatusEffects[i].turnDuration} turn(s) remaining");
                        OnStatusEffectsChanged?.Invoke();
                    }
                }
            }
        }
    }

    public int GetModifiedAttackDamage(int baseDamage) // Calculate modified attack damage (apply weaken if active)
    {
        int damageValue = baseDamage;

        if (isStrengthened)
        {
            StatusEffect strengthen = findStatusEffect(EffectType.Strengthen);
            if(strengthen != null)
            {
                float multiplier = 1f + (strengthen.quantity * 0.2f);
                damageValue = Mathf.FloorToInt(damageValue * multiplier); // Increase damage by 20% when strengthened
            }
            
        }

        if (isWeakened)
        {
            damageValue = Mathf.FloorToInt(damageValue * 0.8f); // Reduce damage by 20% when weakened
        }
        
        return damageValue;
    }

    public bool HasAnyStatusEffects() // Check if player has any active status effects (will expand this as other statuses get added)
    {
        return isWeakened || isPoisoned || isStrengthened;
    }

    public StatusEffect findStatusEffect(EffectType requestedType) // Goes through the status effect list and returns the first reference to the requested status effect in the list
    {
        foreach(StatusEffect statusEffect in allStatusEffects)
        {
            if(statusEffect.effectType == requestedType) // If we found the requested status effect, return it
            {
                return statusEffect;
            }
        }

        return null; // Did not find status effect requested
    }

    public void clearAllStatusEffects() // Called when the enemy dies to remove all status effects on the player for the next combat
    {
        if (isPoisoned)
        {
            RemovePoison();
        }
        if (isWeakened)
        {
            RemoveWeaken();
        }
        if (isStrengthened)
        {
            RemoveStrengthen();
        }
    }
}
    
