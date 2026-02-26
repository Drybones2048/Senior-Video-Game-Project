using UnityEngine;
using TMPro;

public class StatusEffectUI : MonoBehaviour
{
    [Header("Status Effect Text")]
    public TextMeshProUGUI weakenedText;

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
        if (PlayerStatusEffects.Instance == null)
        {
            // No status effect system, hide everything
            if (weakenedText != null)
                weakenedText.gameObject.SetActive(false);
           
            return;
        }

        if (weakenedText != null) // Update weakened status display
        {
            if (PlayerStatusEffects.Instance.isWeakened) // If statement to check if player is weakened
            {
                // Show weakened text with turn count
                weakenedText.gameObject.SetActive(true);
                
                if (PlayerStatusEffects.Instance.weakenTurnsRemaining > 1)
                {
                    weakenedText.text = $"Weakened (20% less damage for {PlayerStatusEffects.Instance.weakenTurnsRemaining} turns)";
                }
                else
                {
                    weakenedText.text = "Weakened (20% less damage this turn)";
                }
            }
            else // Hide weakened text
            {
                weakenedText.gameObject.SetActive(false);
            }
        }
    }
}

