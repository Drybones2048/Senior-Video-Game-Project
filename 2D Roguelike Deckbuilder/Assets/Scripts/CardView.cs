using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
     [Header("UI")]
     public TextMeshProUGUI nameText;
     public TextMeshProUGUI costText;
     public TextMeshProUGUI descriptionText;

     public Image cardArtImage;

     public CardInstance cardData;

     public static event Action<CardInstance> HoverEnter; 
     public static event Action HoverExit;

     public static event Action<List<CardInstance>> cardClicked;
     public static UnityEvent<CardInstance> playerCardPlayed = new UnityEvent<CardInstance>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(CardInstance card){
        cardData = card;

        nameText.text = card.name; // Sets text
        costText.text = card.cost.ToString(); // Sets text

        if (cardArtImage != null && !string.IsNullOrEmpty(card.sprite)) // Loads the card's sprite
        {
            cardArtImage.sprite = Resources.Load<Sprite>(card.sprite);
        }

        UpdateCardDescription();

        PlayerStatusEffects.OnStatusEffectsChanged += UpdateCardDescription;
    }

    void OnDestroy()
    {
        // Unsubscribe when card is destroyed to prevent memory leaks
        PlayerStatusEffects.OnStatusEffectsChanged -= UpdateCardDescription;
    }

    void UpdateCardDescription()
    {
        //***UPDATE*** Removed if-else block chain, now you can call "GetDescription()" to get the description with modified damage values
        int damage = cardData.damage;
        cardData.actualDamage = CombatManager.Instance.GetActualDamage(cardData.damage);
        descriptionText.text = cardData.GetDescription();
        cardData.actualDamage = damage;

        /*
        if (cardData.id == "defend")
        {
            descriptionText.text = $"Gain {cardData.block} Block";
        }
        else if (cardData.id == "attack")
        {
            //calling GetActualDamage from CombatManager instead of AttackCard
            int actualDamage = CombatManager.Instance.GetActualDamage(cardData.damage);
            descriptionText.text = $"Deal {actualDamage} Damage";


            // Check if damage is modified
            if (CombatManager.Instance.IsDamageModified())
            {
                // Show modified damage (we'll add red color in a later step)
                descriptionText.text = $"Deal {actualDamage} Damage";
            }
            else
            {
                // Show normal damage
                descriptionText.text = $"Deal {cardData.damage} Damage";
            }
        }
        else {
            //****UPDATE***** this else statement now sets the text using the description field in CardInstance.
            //Later we will get rid of the if-else statement entirely and it will be the same action for all cards.
            //TODO: Need to figure out how to use in-line variables in description to show effect-adjusted damage values.
            descriptionText.text = cardData.description;
        } */
    }

    public void OnPointerEnter(PointerEventData eventData){ // Event that when pointer enters a card
        if(RoundManager.instance.currentState == gameState.playerTurn)
        {
            HoverEnter?.Invoke(cardData);
        }
    }

    public void OnPointerExit(PointerEventData eventData){ // Event for when pointer leaves card
        if(RoundManager.instance.currentState == gameState.playerTurn)
        {
            HoverExit?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //only allows you to select cards if it's the player's turn
        if (RoundManager.instance.currentState == gameState.playerTurn)
        {
            if (RoundManager.instance.currentEnergy - cardData.cost < 0)
            {
                Debug.Log("Cannot play card, not enough energy!");
            }
            else
            {
                RoundManager.instance.decrementEnergy(cardData.cost);    //replaced old line that directly modified energy field

                //*****UPDATE****** no longer call Play() from the Card class. Instead the cardPlayed event(new) is invoked which the CombatManager will listen for
                //cardData.Play(); //Old method of playing the card, deprecated
                playerCardPlayed.Invoke(cardData);

                if(RoundManager.instance.currentState != gameState.playerTurn) // Need to add this so card doesn't get discarded twice and duplicated if it is the card that kills the enemy
                {
                    return;
                }

                Deck.discardAdd(cardData); //Add to discard's list

                HandView handView = FindFirstObjectByType<HandView>();

                if (handView != null)
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
        else {
            Debug.Log("Can't play cards. It's not the player's turn.");
        }
    }

    public void Refresh() { // Refreshes any card's data with any updated values
        nameText.text = cardData.name;
        costText.text = cardData.cost.ToString();
        descriptionText.text = cardData.description;

    }
}
