using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HandView : MonoBehaviour
{
    public CardView attackPrefab;
    public CardView defendPrefab;

    public Transform handArea;

    public float fanRadius = 300f;
    public float fanAngle = 30f;

    private List<CardView> cardViews = new();

    void OnEnable() // Whenever a card is played, the card hand will update
    {
        CardView.cardClicked += DisplayHand;
    }

    public void DisplayHand(List<Card> cards){
        ClearHand(); // Delete all the cards in hand for re-display

        for(int i = 0; i < cards.Count; i++){ // Adds all of the cards on screen
            CardView view;

            if(cards[i].cardName == "Attack")
            {
                view = Instantiate(attackPrefab, handArea);
            } else //if(cards[i].cardName == "Defend")
            { 
                view = Instantiate(defendPrefab, handArea);
            }

            
            view.Setup(cards[i]);
            cardViews.Add(view);
        }

        FanCards();
    }

    void FanCards(){ // function that does the math on how to display the cards based on HandArea's location
        int count = cardViews.Count;
        float radius = 400f;
        float maxAngle = 20f;

        if (count == 1){ // If there is only one card in hand, display it vertically

            cardViews[0].transform.localPosition = Vector3.zero;
            cardViews[0].transform.localRotation = Quaternion.identity;
            return;

        } else if(count == 2) { // if there is only two cards in hand, make sure they are close together

            float smallAngle = 6f; // tighter spread

            for (int i = 0; i < 2; i++)
            {
                float angle = (i == 0) ? -smallAngle : smallAngle;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 pos = new Vector3(
                    Mathf.Sin(rad) * radius,
                    Mathf.Cos(rad) * radius - radius,
                    0
                );

                cardViews[i].transform.localPosition = pos;
                cardViews[i].transform.localRotation = Quaternion.Euler(0, 0, -angle);
            }

            return;
        }

        float angleStep = (maxAngle * 2) / (count - 1);
        float startAngle = -maxAngle;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(
                Mathf.Sin(rad) * radius,
                Mathf.Cos(rad) * radius - radius,
                0
            );
            cardViews[i].transform.localPosition = pos;

            cardViews[i].transform.localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    // Every time the player plays a card, this function will be called to delete the assets
    void ClearHand()
    {
        foreach (CardView view in cardViews)
        {
            Destroy(view.gameObject);
        }

        cardViews.Clear();
    }

}
