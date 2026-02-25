using UnityEngine;
using System.Collections.Generic; // needed to use List
using System;
using Random=UnityEngine.Random;
using UnityEngine.Events;
using System.Collections;

public class Deck : MonoBehaviour // Will store a list of all the cards in the deck
{
    public HandView handView;

    public AudioClip deckShuffleSound;

    // The deck of cards that the player has, starting amounts defined here.
    List<Card> deck = new List<Card> 
        {
            new DefendCard(),
            new DefendCard(),
            new DefendCard(),
            new DefendCard(),
            new AttackCard(),
            new AttackCard(),
            new AttackCard(),
            new AttackCard(),
            new AttackCard()
        };
    
    // The pool of cards that the player can draw each turn
    public static List<Card> drawPile = new List<Card>();

    // The current hand that the player has 
    public static List<Card> currentHand = new List<Card>();

    // The cards that the player has discarded
    public static List<Card> discard = new List<Card>();

    void Awake() {
        //drawHand() is no longer called in start, instead it is called by the battleStart event which is invoked in TurnEndRoutine
        RoundManager.battleStart.AddListener(drawHand);
        /*Listen for when player starts their turn, then draw back up to 5 cards, should happen before game registers user input
         i.e. before the startPlayerTurn event is invoked*/
        RoundManager.dealHand.AddListener(startNewTurn);
        RoundManager.endPlayerTurn.AddListener(discardAll); // Listen for when the player clicks the end turn button, then discard all cards
    }

    // The entire deck the player has at the start of the game
    void Start()
    {
        // At the start of combat, add all cards in the deck to the draw pile
        drawPile.AddRange(deck);
    }

    void OnDestroy() {
        RoundManager.battleStart.RemoveListener(drawHand);
        RoundManager.dealHand.RemoveListener(startNewTurn);
        RoundManager.endPlayerTurn.RemoveListener(discardAll);
    }

    //drawHand() is no longer called in start, instead it is called by the battleStart event which is invoked in TurnEndRoutine
    public void drawHand()
    {
        int cardsToDraw = 5;
        
        for(int i = 0; i < cardsToDraw; i++)
        {
            // Check if draw pile is empty
            if(drawPile.Count == 0)
            {
                // Reshuffle discard into draw pile
                if(discard.Count > 0)
                {
                    Debug.Log("Draw pile empty! Reshuffling discard pile into draw pile.");
                    drawPile.AddRange(discard); // Add all discarded cards back to draw pile
                    discard.Clear(); // Clear the discard pile
                }
                else
                {
                    // No cards left to draw at all
                    Debug.Log("No more cards to draw! Draw pile and discard pile are both empty.");
                    break; // Exit the loop, can't draw any more cards
                }
            }
            
            // Draw a card
            Card cardPick = randomDraw(drawPile);
            
            if(cardPick != null)
            {
                drawPile.Remove(cardPick); // Remove the card from draw pile
                currentHand.Add(cardPick); // Add the card to hand
            }
        }

        handView.DisplayHand(currentHand);
    } 

    // NEW: Method to start a new turn - resets animation flag and draws new hand
    public void startNewTurn()
    {
        StartCoroutine(StartNewTurnCoroutine());
    }

    IEnumerator StartNewTurnCoroutine()
    {
        bool didReshuffle = false;
        int cardsToDraw = 5;
        
        // Draw cards into hand (this may trigger reshuffle with sound)
        for(int i = 0; i < cardsToDraw; i++)
        {
            if(drawPile.Count == 0) // Check if draw pile is empty
            {
                if(discard.Count > 0) // Reshuffle discard into draw pile
                {
                    Debug.Log("Draw pile empty! Reshuffling discard pile into draw pile.");
                    drawPile.AddRange(discard); // Add all discarded cards back to draw pile
                    discard.Clear(); // Clear the discard pile
                    didReshuffle = true; // Flag that we reshuffled
                }
                else // No cards left to draw at all
                {
                    Debug.Log("No more cards to draw! Draw pile and discard pile are both empty.");
                    break; // Exit the loop, can't draw any more cards
                }
            }

            if (deckShuffleSound != null) // Play card draw sound effect
            {
                AudioSource.PlayClipAtPoint(deckShuffleSound, Camera.main.transform.position, 0.2f);
            }
            
            // Draw a card
            Card cardPick = randomDraw(drawPile);
            
            if(cardPick != null)
            {
                drawPile.Remove(cardPick); // Remove the card from draw pile
                currentHand.Add(cardPick); // Add the card to hand
            }
        }

        if(didReshuffle) // Implement a pause between reshuffle and draw sound effects
        {
            Debug.Log("Reshuffle pause");
            yield return new WaitForSeconds(1f);
        }
        
        handView.ResetDrawFlag(); // Reset so draw animation plays again
        handView.DisplayHand(currentHand); // Show cards with animation (plays draw sounds)
    }

    T randomDraw<T>(List<T> list) // method to draw cards randomly
    {
        if(list == null || list.Count == 0)
        {
            return default(T);
        }

        int randomIndex = Random.Range(0, list.Count);

        return list[randomIndex];
    }

    // Adds a card to the discard pile
    public static void discardAdd(Card card)
    {
        currentHand.Remove(card);

        discard.Add(card);
    }

    public void discardAll() // Method to discard all cards in hand when the player clicks the end round button
    {
        List<Card> cardsToDiscard = new List<Card>(currentHand);
        
        foreach(Card card in cardsToDiscard)
        {
            discardAdd(card);
        }
        
        handView.AnimateAllCardsToDiscard(); // Animate all cards to discard pile
    }
    
}