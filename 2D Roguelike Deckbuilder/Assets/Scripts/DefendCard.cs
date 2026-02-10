using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefendCard : Card
{
    public int defendAmount = 5; // the amount that all defend cards will defend for

    public DefendCard(){ // Default constructor for base defend card
        cardName = "Defend";
        
        cost = 1;

        description = $"Gain {defendAmount} Block";

        sprite = "Basic Defense";
    }
    
    // Overloaded constructor to be used on every other defend card other than base one
    public DefendCard(int defendVal, string cardName, int cost, string description, string sprite){ 
        defendAmount = defendVal;
        this.cardName = cardName;
        this.cost = cost;
        this.description = description;
        this.sprite = sprite;
    }

    public override void Play() // Will write code here for player shield
    {
        throw new System.NotImplementedException();
    }

}
