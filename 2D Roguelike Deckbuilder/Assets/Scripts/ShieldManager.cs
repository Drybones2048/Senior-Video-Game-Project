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
        Player.shieldBroken += Hide;
        Player.shieldDamaged += DamagedShield;
    }

    void Show(DefendCard card){ // Shows shield bar
        currentShield += card.defendAmount;
        
        defendText.text = currentShield.ToString();

        cg.alpha = 1; // Reveals shield bar
    }

    void Hide() // Hides defend bar when block is removed
    {
        currentShield = 0;

        cg.alpha = 0;
    }

    void DamagedShield(int damage) // Gets called when shield is damaged but not fully destroyed
    {
        currentShield -= damage;
        
        defendText.text = currentShield.ToString();
    }
}
