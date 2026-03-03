using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance {get; private set;}

    public Enemy currentEnemy;

    public Player player;

    void Awake()
    {
        Instance = this;
        CardView.playerCardPlayed.AddListener(PlayPlayerCard);
    }

    void OnDestroy() {
        CardView.playerCardPlayed.RemoveListener(PlayPlayerCard);
    }

    void PlayPlayerCard(CardInstance card) {
        if (card.type == CardType.Attack) {
            //****UPDATE***** getting the actual damage here rather than in the AttackCard script
            int actualDamage = GetActualDamage(card.damage);
            currentEnemy.TakeDamage(actualDamage);
        }

        else if (card.type == CardType.Defend) {
            player.GainShield(card.block);
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
}
