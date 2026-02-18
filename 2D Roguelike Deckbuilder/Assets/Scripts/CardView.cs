using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
     [Header("UI")]
     public TextMeshProUGUI nameText;
     public TextMeshProUGUI costText;
     public TextMeshProUGUI descriptionText;

     public Card cardData;

     public static event Action<Card> HoverEnter; 
     public static event Action HoverExit;

     public static event Action<List<Card>> cardClicked;

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
        if(RoundManager.instance.currentEnergy - cardData.cost < 0)
        {
            Debug.Log("Cannot play card, not enough energy!");
        }
        else //change to if-else currentState = playerTurn
        {
            RoundManager.instance.decrementEnergy(cardData.cost);    //replaced old line that directly modified energy field

            cardData.Play(); // Plays the card's effect

            Deck.discardAdd(cardData); //Add to discard's list

            HandView handView = FindFirstObjectByType<HandView>();

            if(handView != null)
            {
                handView.AnimateCardToDiscard(this, () =>
                {
                    // After discard animation completes, refresh the hand
                    cardClicked?.Invoke(Deck.currentHand);
                });
            }
            else
            {
                // Fallback: if no HandView found, just refresh immediately
                cardClicked?.Invoke(Deck.currentHand);
            }
        }
    }

    public void Refresh() { // Refreshes any card's data with any updated values
        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();
        descriptionText.text = cardData.description;

    }
}
