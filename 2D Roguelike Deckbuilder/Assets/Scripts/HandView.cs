using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class HandView : MonoBehaviour
{
    public CardView cardPrefab;
    public Transform handArea;

    public float fanRadius = 300f;
    public float fanAngle = 30f;

    private List<CardView> cardViews = new();

    public void DisplayHand(List<Card> cards){
        //ClearHand();

        for(int i = 0; i < cards.Count;i++ ){ // Adds all of the cards on screen
            CardView view = Instantiate(cardPrefab, handArea);
            view.Setup(cards[i]);
            cardViews.Add(view);
        }

        FanCards();
    }

    void FanCards(){ // function that does the math on how to display the cards based on HandArea's location
        int count = cardViews.Count;
        float radius = 400f;
        float maxAngle = 20f;

        if (count == 1)
        {
            cardViews[0].transform.localPosition = Vector3.zero;
            cardViews[0].transform.localRotation = Quaternion.identity;
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

}
