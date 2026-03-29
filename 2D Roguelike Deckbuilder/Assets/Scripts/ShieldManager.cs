using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class ShieldManager : MonoBehaviour
{
    CanvasGroup cg;

    public TextMeshProUGUI defendText;

    public static UnityEvent removeShield = new UnityEvent(); // Unity event that will be used to remove player shield at the start of a new round

    int currentShield;

    public AudioClip shieldGain;

    void Awake() {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    void OnEnable()
    {
        CombatManager.shieldGained.AddListener(Show);
        removeShield.AddListener(removeAllShield);
        Player.shieldBroken += Hide;
        Player.shieldDamaged += DamagedShield;
    }

    void OnDisable() {
        removeShield.RemoveListener(removeAllShield);
        CombatManager.shieldGained.RemoveListener(Show);
    }

    void Show(CardInstance card){ // Shows shield bar
        if (card.uniqueBehavior == UniqueBehavior.PressAndFall)
        {
            currentShield += CombatManager.Instance.GetActualDamage(card.damage);
        }
        else 
        {
            currentShield += card.block;
        }
        
        defendText.text = currentShield.ToString();

        if(shieldGain != null)
        {
            AudioSource.PlayClipAtPoint(shieldGain, Camera.main.transform.position, 1f);
        }

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

    void removeAllShield() // Method that will trigger to remove all player shield at the start of a new round
    {
        Hide();

        defendText.text = "0";
    }
}
