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

    public override void Play()
    {
        CombatManager.Instance.currentEnemy.TakeDamage(attackAmount);
    }
}
