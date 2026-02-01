using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : Singleton<CardHover>
{
    [SerializeField] private CardView cardHover;

    public void Show(Card card, Vector3 position){
        cardHover.gameObject.SetActive(true);
        cardHover.Setup(card);
        cardHover.transform.position = position;
    }

    public void Hide(){
        cardHover.gameObject.SetActive(false);
    }
}
