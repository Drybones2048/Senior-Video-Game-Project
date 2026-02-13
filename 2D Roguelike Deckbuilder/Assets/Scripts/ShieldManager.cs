using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ShieldManager : MonoBehaviour
{
    CanvasGroup cg;

    public TextMeshProUGUI defendText;

    int currentShield;

    void Awake() {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    void OnEnable()
    {
        DefendCard.shieldGained += Show;
    }

    void Show(DefendCard card){ // Shows shield bar
        currentShield += card.defendAmount;
        
        defendText.text = currentShield.ToString();

        cg.alpha = 1; // Reveals shield bar
    }
}
