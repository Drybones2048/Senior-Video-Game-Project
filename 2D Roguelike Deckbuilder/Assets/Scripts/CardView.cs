using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
     [Header("UI")]
     public TextMeshProUGUI nameText;
     public TextMeshProUGUI costText;
     public TextMeshProUGUI descriptionText;

     public Card cardData;

     public static event Action<Card> HoverEnter; 
     public static event Action HoverExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(Card card){
        cardData = card;

        nameText.text = card.cardName; // Sets text
        costText.text = card.cost.ToString(); // Sets text

        if(card is DefendCard defend){ // If the card is a defend card, print this text
            descriptionText.text = $"Gain {defend.defendAmount} Block";
        }
        else if(card is AttackCard attack)
        {
            descriptionText.text = $"Deal {attack.attackAmount} Damage";
        }
    }

    public void OnPointerEnter(PointerEventData eventData){ // Event that when pointer enters a card
        HoverEnter?.Invoke(cardData);
    }

    public void OnPointerExit(PointerEventData eventData){ // Event for when pointer leaves card
        HoverExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(RoundManager.energy - cardData.cost < 0)
        {
            Debug.Log("Cannot play card, not enough energy!");
        }
        else
        {
            RoundManager.energy -= cardData.cost;
            cardData.Play();
            //Add to discard's list
            //Destroy(gameObject);
            //redisplay hand view since there are less than 5 cards in hand
        }
    }

    public void Refresh() { // Refreshes any card's data with any updated values
        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();
        descriptionText.text = cardData.description;

    }
}
