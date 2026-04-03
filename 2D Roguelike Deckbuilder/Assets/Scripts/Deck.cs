using UnityEngine;
using System.Collections.Generic; // needed to use List
using System;
using Random=UnityEngine.Random;
using UnityEngine.Events;
using System.Collections;

public class Deck : MonoBehaviour // Will store a list of all the cards in the deck
{
    public HandView handView;
    public CardLibrary cardLibrary;

    public AudioClip deckShuffleSound;

    public Player player;

    //This is where you define the contents of your starting deck
    /*List<string> deckIDs = new List<string> {
        "attack",
        "empowering_shield",
        "press_and_fall",
        "attack",
        "attack",
        "defend",
        "defend",
        "defend",
        "defend",
        "defend",
        "defend"
    }; */

    List<string> deckIDs;

    //*****UPDATE: This is the deck containing all card data that you'll pass by reference to other functions
    public static List<CardInstance> deck = new List<CardInstance>();

    //List of persistent cards that are exhausted during combat. Resets every combat.
    public static List<CardInstance> exhaustPile = new List<CardInstance>();

    // The pool of cards that the player can draw each turn
    public static List<CardInstance> drawPile = new List<CardInstance>();

    // The current hand that the player has 
    public static List<CardInstance> currentHand = new List<CardInstance>();

    // The cards that the player has discarded
    public static List<CardInstance> discard = new List<CardInstance>();

    void Awake() {
        //drawHand() is no longer called in start, instead it is called by the combatStart event which is invoked in TurnEndRoutine
        RoundManager.combatStart.AddListener(drawHand);
        /*Listen for when player starts their turn, then draw back up to 5 cards, should happen before game registers user input
         i.e. before the startPlayerTurn event is invoked*/
        RoundManager.dealHand.AddListener(startNewTurn);
        RoundManager.endPlayerTurn.AddListener(discardAll); // Listen for when the player clicks the end turn button, then discard all cards
        RoundManager.enemyDead.AddListener(discardAll);
        RewardCardHandler.addedCard.AddListener(AddCardToDeck);
    }

    // The entire deck the player has at the start of the game
    void Start()
    {
        //****UPDATE**** deck is now built here
        AssignDeckByClass();
        deck = BuildRuntimeDeck(deckIDs);

        // At the start of combat, add all cards in the deck to the draw pile
        drawPile.AddRange(deck);
    }

    void OnDestroy() {
        RoundManager.combatStart.RemoveListener(drawHand);
        RoundManager.dealHand.RemoveListener(startNewTurn);
        RoundManager.endPlayerTurn.RemoveListener(discardAll);
        RoundManager.enemyDead.RemoveListener(discardAll);
        RewardCardHandler.addedCard.RemoveListener(AddCardToDeck);
    }

    void AssignDeckByClass() {
        if (CombatManager.Instance.playerClass == PlayerClass.Horus)
        {
            deckIDs = new List<string> {
                "attack",
                "attack",
                "attack",
                "attack",
                "unyielding_sky",
                "unyielding_sky",
                "unyielding_sky",
                "unyielding_sky",
                "unyielding_sky",
                "unyielding_sky"
           };
        }
        else if (CombatManager.Instance.playerClass == PlayerClass.Ra) {
            deckIDs = new List<string> {
                "attack",
                "attack",
                "attack",
                "attack",
                "attack",
                "solar_wrath",
                "defend",
                "defend",
                "defend",
                "defend"
           };
        }
        else if (CombatManager.Instance.playerClass == PlayerClass.Set) {
            deckIDs = new List<string> {
                "attack",
                "attack",
                "attack",
                "attack",
                "rite_of_frailty",
                "defend",
                "defend",
                "defend",
                "defend"
           };
        }
        else { }
    }

    //****UPDATE**** new helper for building the deck
    public List<CardInstance> BuildRuntimeDeck(List<string> deckIDs)
    {
        var runtimeDeck = new List<CardInstance>(deckIDs.Count);

        foreach (string id in deckIDs)
        {
            var instance = cardLibrary.CreateInstance(id);
            if (instance != null)
                runtimeDeck.Add(instance);
        }

        return runtimeDeck;
    }

    //drawHand() is no longer called in start, instead it is called by the combatStart event which is invoked in TurnEndRoutine
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
            CardInstance cardPick = randomDraw(drawPile);
            
            if(cardPick != null)
            {
                drawPile.Remove(cardPick); // Remove the card from draw pile
                currentHand.Add(cardPick); // Add the card to hand
            }
        }

        handView.DisplayHand(currentHand);
    } 

    public void resetDeck() // Will be used to reset deck after combat ends and a new card is added to the deck
    {
        drawPile.AddRange(discard); // Add all discarded cards back to draw pile
        drawPile.AddRange(exhaustPile); //Add any exhausted cards back into the deck

        discard.Clear(); // Clear the discard pile
        exhaustPile.Clear(); //Clear the exhaust pile
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

            if (deckShuffleSound != null && didReshuffle) // Play card draw sound effect
            {
                AudioSource.PlayClipAtPoint(deckShuffleSound, Camera.main.transform.position, 0.2f);
            }
            
            // Draw a card
            CardInstance cardPick = randomDraw(drawPile);
            
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

    CardInstance randomDraw(List<CardInstance> list)
    {
        if (list == null || list.Count == 0)
        { 
            return null; 
        }

        int randomIndex = RoundManager.instance.RNG.Next(0, list.Count);

        return list[randomIndex];
    }

    // Adds a card to the discard pile
    public static void discardAdd(CardInstance card)
    {
        currentHand.Remove(card);

        //when a persistent card is played, it is not added to the discard pile, it's exhausted
        if (card.type != CardType.Persistent)
        {
            discard.Add(card);
        }
        else {
            exhaustPile.Add(card);
        }
    }

    public void discardAll() // Method to discard all cards in hand when the player clicks the end round button
    {
        List<CardInstance> cardsToDiscard = new List<CardInstance>(currentHand);
        
        foreach(CardInstance card in cardsToDiscard)
        {
            discardAdd(card);
        }
        
        handView.AnimateAllCardsToDiscard(); // Animate all cards to discard pile
    }

    public void AddCardToDeck(CardInstance card) // Method that is used when adding new cards with card rewards
    {
        deck.Add(card); // Add the new card to the player's deck

        deckIDs.Add(card.id); // Add the ID of the new card to the ID list

        drawPile.Add(card); // Also adds the new card to the player's draw pile (needed for card reset between combats)

        Debug.Log($"Added {card.name} to deck!");

        resetDeck(); // Shuffles after discarding all cards
    }
    
}