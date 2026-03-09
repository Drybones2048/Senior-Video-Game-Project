using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class RewardCardHandler : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CardInstance cardData;

    public static UnityEvent<CardInstance> addedCard = new UnityEvent<CardInstance>();

    private Vector3 originalScale;
    
    void Awake()
    {
        originalScale = transform.localScale;
    }
    
    public void SetCard(CardInstance card)
    {
        cardData = card;
    }
    
    public void OnPointerDown(PointerEventData eventData) // When a card reward is clicked on, it will add the card to the player's deck
    {
        if (cardData == null) return;
        
        Debug.Log($"Player selected: {cardData.name}");
        
        // Add card to player's deck
        addedCard.Invoke(cardData);
        
        // Hide the reward screen
        CardRewardScreen.Instance.HideRewardScreen();
        
        // TODO: Continue to next encounter
    }
    
    public void OnPointerEnter(PointerEventData eventData) // Card will pop whenever player hovers over it
    {
        transform.localScale = originalScale * 1.1f;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}