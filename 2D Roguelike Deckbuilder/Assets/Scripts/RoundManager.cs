using UnityEngine;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    public HandView handView;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Will be a list storing all of the cards in the starting hand
        List<Card> startingHand = new List<Card> 
        {
            new DefendCard(),
            new DefendCard(),
            new DefendCard(),
            new DefendCard()
        };

        handView.DisplayHand(startingHand); // starts the chain to display cards on screen
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
