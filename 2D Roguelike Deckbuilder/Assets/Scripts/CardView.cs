using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour
{
     [Header("UI")]
     public TextMeshProUGUI nameText;
     public TextMeshProUGUI costText;
     public TextMeshProUGUI descriptionText;

     private Card cardData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(Card card){
        cardData = card;

        nameText.text = card.cardName; // Sets text
        costText.text = card.cost.ToString(); // Sets text

        if(card is DefendCard defend){ // If the card is a defend card, print this text
            descriptionText.text = $"Gain {defend.defendAmount} Block";
        }
    }

    /*public void OnPlay(){
        cardData.Play(Player.Instance, Enemy.Instance);
        //would destroy card after using
    }*/
}
