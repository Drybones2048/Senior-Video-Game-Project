using UnityEngine;

public class AttackCard : Card
{
    public int attackAmount = 5;

    private static Sprite artwork;

    public AttackCard()
    {
        cardName = "Attack";

        cost = 1;

        description = $"Deal {attackAmount} Damage";

        
    }

    public AttackCard(int attackVal, string cardName, int cost, string description){ 
        attackAmount = attackVal;
        this.cardName = cardName;
        this.cost = cost;
        this.description = description;
        
    }
}
