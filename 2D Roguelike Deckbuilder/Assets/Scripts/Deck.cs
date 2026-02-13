using UnityEngine;
using System.Collections.Generic; // needed to use List
using System;
using Random=UnityEngine.Random;

public class Deck : MonoBehaviour // Will store a list of all the cards in the deck
{
    public HandView handView;

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
    static List<Card> drawPile = new List<Card>();

    // The current hand that the player has 
    public static List<Card> currentHand = new List<Card>();

    // The cards that the player has discarded
    static List<Card> discard = new List<Card>();

    // The entire deck the player has at the start of the game
    void Start()
    {
        // At the start of combat, add all cards in the deck to the draw pile
        for(int i = 0; i < deck.Count - 1; i++)
        {
            drawPile.Add(deck[i]);
        }

        drawHand();
    }

    public void drawHand()
    {
        for(int i = 0; i < 5; i++) // Player draws 5 random cards to hand at the start of the round
        {
            Card cardPick = randomDraw(deck); // Draws a random card

            drawPile.Remove(cardPick); // Removes the card from draw pile

            currentHand.Add(cardPick); // Adds the card to hand
        }

        handView.DisplayHand(currentHand);
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
    
}
