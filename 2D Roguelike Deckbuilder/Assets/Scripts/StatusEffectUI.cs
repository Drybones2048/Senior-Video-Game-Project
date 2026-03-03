using UnityEngine;
using TMPro;

public class StatusEffectUI : MonoBehaviour
{
    [Header("Status Effect Text")]
    public TextMeshProUGUI weakenedText;
    public GameObject weakenedIcon;
    public TextMeshProUGUI poisonedQuantityText;
    public TextMeshProUGUI poisonedTurnsText;
    public GameObject poisonedIcon;

    void OnEnable()
    {
        PlayerStatusEffects.OnStatusEffectsChanged += UpdateStatusDisplay;
    }

    void OnDisable()
    {
        PlayerStatusEffects.OnStatusEffectsChanged -= UpdateStatusDisplay;
    }

    void Start() // Initialize display
    {
        UpdateStatusDisplay();
    }

    void UpdateStatusDisplay()
    {
        if (PlayerStatusEffects.Instance == null) // No status effect system, hide everything
        {
            if (weakenedText != null && weakenedIcon != null)
            {
                weakenedText.gameObject.SetActive(false);
                weakenedIcon.gameObject.SetActive(false);
            }
            if(poisonedQuantityText != null && poisonedIcon != null){
                poisonedQuantityText.gameObject.SetActive(false);
                poisonedIcon.gameObject.SetActive(false);
                poisonedTurnsText.gameObject.SetActive(false);
            }
            return;
        }

        if (weakenedText != null && weakenedIcon != null) // Update weakened status display
        {
            if (PlayerStatusEffects.Instance.isWeakened) // If statement to check if player is weakened
            {
                // Show weakened text with turn count
                weakenedText.gameObject.SetActive(true);
                weakenedIcon.gameObject.SetActive(true);
                
                if (PlayerStatusEffects.Instance.findStatusEffect(EffectType.Weaken).turnDuration > 0)
                {
                    weakenedText.text = PlayerStatusEffects.Instance.findStatusEffect(EffectType.Weaken).turnDuration.ToString();
                }
            }
            else // Hide weakened text
            {
                weakenedText.gameObject.SetActive(false);
                weakenedIcon.gameObject.SetActive(false);
            }
        }
        if(poisonedQuantityText != null && poisonedIcon != null && poisonedTurnsText != null)
        {
            if (PlayerStatusEffects.Instance.isPoisoned) // If player is poisoned, show poisoned UI
            {
                poisonedQuantityText.gameObject.SetActive(true);
                poisonedIcon.gameObject.SetActive(true);
                poisonedTurnsText.gameObject.SetActive(true);

                StatusEffect poison = PlayerStatusEffects.Instance.findStatusEffect(EffectType.Poison); // Grab the poison status information

                if (poison.turnDuration > 0) // If there is more than one turn left on poison, set text
                {
                    poisonedQuantityText.text = poison.quantity.ToString();
                    poisonedTurnsText.text = $"{poison.turnDuration.ToString()} Turns";
                }
            }
            else // If player is not poisoned, stay hiddden
            {
                poisonedQuantityText.gameObject.SetActive(false);
                poisonedIcon.gameObject.SetActive(false);
                poisonedTurnsText.gameObject.SetActive(false);
            }
        }
    }
}

