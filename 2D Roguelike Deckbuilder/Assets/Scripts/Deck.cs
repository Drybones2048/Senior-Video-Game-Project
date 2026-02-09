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
    
    List<Card> currentHand = new List<Card>();

    // The entire deck the player has at the start of the game
    void Start()
    {
        for(int i = 0; i < 5; i++) // Player draws 5 random cards to hand at the start of the round
        {
            currentHand.Add(randomDraw(deck));
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
    
}
