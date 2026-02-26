using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class CardHover : MonoBehaviour
{
    CanvasGroup cg; // Variable that manages visibility of magnified card

    [SerializeField] CardView previewView; // The CardView of the hovered card
    
    private Card currentlyShowingCard = null; // Track which card is currently being previewed
    
    void Awake() { // Card starts off as hidden
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    void OnEnable(){ // Since the magnified card is in the scene, this triggers the listeners to show the hidden magnified card
        CardView.HoverEnter += Show;
        CardView.HoverExit += Hide;
        
        // Subscribe to status effect changes so preview updates when player gets weakened
        PlayerStatusEffects.OnStatusEffectsChanged += UpdatePreviewIfVisible;
    }

    void OnDisable() // Unsubscribe to prevent memory leaks
    {
        CardView.HoverEnter -= Show;
        CardView.HoverExit -= Hide;
        PlayerStatusEffects.OnStatusEffectsChanged -= UpdatePreviewIfVisible;
    }

    void Show(Card card){ // Reveals the hidden magnified card
        currentlyShowingCard = card; // Remember which card we're showing
        
        previewView.cardData = card; 
        previewView.cardData.sprite = card.sprite;
        
        UpdatePreviewDescription(); // Update description with status effects
        
        cg.alpha = 1; // Reveals hidden magnified card
    }

    void Hide(){ // Re-hides card when not hovered
        currentlyShowingCard = null; // Clear the current card
        
        cg.alpha = 0;
    } 

    void UpdatePreviewIfVisible() // Called when status effects change while a card is being previewed (might need for when player buffs on their turn)
    {
        if (cg.alpha > 0 && currentlyShowingCard != null)
        {
            UpdatePreviewDescription();
        }
    }

    void UpdatePreviewDescription() // Update the preview card's description based on current status effects
    {
        if (currentlyShowingCard == null) return;

        // Update the preview card's text
        previewView.nameText.text = currentlyShowingCard.cardName;
        previewView.costText.text = currentlyShowingCard.cost.ToString();
        
        // Handle different card types
        if (currentlyShowingCard is DefendCard defend)
        {
            previewView.descriptionText.text = $"Gain {defend.defendAmount} Block";
        }
        else if (currentlyShowingCard is AttackCard attack)
        {
            int actualDamage = attack.GetActualDamage();
            
            // Check if damage is modified
            if (attack.IsDamageModified())
            {
                // Show modified damage
                previewView.descriptionText.text = $"Deal {actualDamage} Damage";
            }
            else
            {
                // Show normal damage
                previewView.descriptionText.text = $"Deal {attack.attackAmount} Damage";
            }
        }
    }
}
