using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefendCard : Card
{
    public int defendAmount = 5; // the amount that all defend cards will defend for

    public DefendCard(){
        cardName = "Defend";
        
        cost = 1;
    }

    public DefendCard(int defendVal, string cardName, int cost){
        defendAmount = defendVal;
        this.cardName = cardName;
        this.cost = cost;
    }
    /* public override void Play(Player player, Enemy enemy){
        player.GainBlock(defenseAmount);
    }*/
}
