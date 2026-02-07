using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class CardHover : MonoBehaviour
{
    CanvasGroup cg; // Variable that manages visibility of magnified card

    [SerializeField] CardView previewView; // The CardView of the hovered card
    
    void Awake() { // Card starts off as hidden
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    void OnEnable(){ // Since the magnified card is in the scene, this triggers the listeners to show the hidden magnified card
        CardView.HoverEnter += Show;
        CardView.HoverExit += Hide;
    }

    void Show(Card card){ // Reveals the hidden magnified card
        previewView.cardData = card; 
        previewView.Refresh(); // Copies details from hovered card

        cg.alpha = 1; // Reveals hidden magnified card
    }

    void Hide(){ // Re-hides card when not hovered
        cg.alpha = 0;
    }
}
