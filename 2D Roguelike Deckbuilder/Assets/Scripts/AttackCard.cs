using UnityEngine;
using UnityEngine.UI;

public class AttackCard : Card
{
    public int attackAmount = 5;

    public AttackCard() // Default constructor for the base attack card in the game
    {
        cardName = "Attack";

        cost = 1;

        description = $"Deal {attackAmount} Damage";

        sprite = "Base Attack";
    }

    public AttackCard(int attackVal, string cardName, int cost, string description, string sprite){ 
        attackAmount = attackVal;
        this.cardName = cardName;
        this.cost = cost;
        this.description = description;
        this.sprite = sprite;
    }

    public int GetActualDamage() // Get the actual damage that the card will do considering status effects like weaken (or strengthen in the future)
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

    public override void Play()
    {
        int actualDamage = GetActualDamage();

        CombatManager.Instance.currentEnemy.TakeDamage(actualDamage);
    }
}
