using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class PlayerStatusEffects : MonoBehaviour {
    public bool isWeakened = false; // Boolean for weakened condition (player deals 20% less damage)

    public int weakenTurnsRemaining = 0; // Status effect durations (turns remaining)

    public static event Action OnStatusEffectsChanged; // Events that other systems can subscribe to

    public static PlayerStatusEffects Instance { get; private set; } // Singleton pattern (may not be needed)

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
    }

    public void ApplyWeaken(int turns = 1) // Apply weaken status effect
    {
        weakenTurnsRemaining = Mathf.Max(weakenTurnsRemaining, turns); // Take the higher value
        isWeakened = true;
        
        Debug.Log($"Player weakened for {weakenTurnsRemaining} turn(s)!");
        
        OnStatusEffectsChanged?.Invoke();  // Notify listeners that status effects changed
    }

    public void RemoveWeaken() // Remove weaken status effect
    {
        isWeakened = false;
        weakenTurnsRemaining = 0;
        
        Debug.Log("Weaken effect removed!");
        
        OnStatusEffectsChanged?.Invoke();
    }

    public void DecrementStatusEffects() // Call this at the end of the player's turn to decrement status effects and remove any text or symbols
    {
        if (weakenTurnsRemaining > 0)
        {
            weakenTurnsRemaining--;
            
            if (weakenTurnsRemaining <= 0)
            {
                RemoveWeaken();
            }
            else
            {
                Debug.Log($"Weaken: {weakenTurnsRemaining} turn(s) remaining");
                OnStatusEffectsChanged?.Invoke();
            }
        }
    }

    public int GetModifiedAttackDamage(int baseDamage) // Calculate modified attack damage (apply weaken if active)
    {
        if (isWeakened)
        {
            return Mathf.FloorToInt(baseDamage * 0.8f); // Reduce damage by 20% when weakened
        }
        
        return baseDamage;
    }

    public bool HasAnyStatusEffects() // Check if player has any active status effects (will expand this as other statuses get added)
    {
        return isWeakened;
    }
}
    
