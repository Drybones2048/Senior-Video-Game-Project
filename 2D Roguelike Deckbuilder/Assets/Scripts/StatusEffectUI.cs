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
    public TextMeshProUGUI strengthenQuantityText;
    public TextMeshProUGUI strengthenTurnsText;
    public GameObject strengthenIcon;

    public GameObject enemyStrengthIcon;
    public TextMeshProUGUI enemyStrengthenQuantityText;
    public TextMeshProUGUI enemyStrengthenTurnsText;
    public GameObject infinitySymbol;


    void OnEnable()
    {
        PlayerStatusEffects.OnStatusEffectsChanged += UpdateStatusDisplay;
        EnemyStatusEffects.OnEnemyStatusEffectsChanged += UpdateEnemyStatusDisplay;
    }

    void OnDisable()
    {
        PlayerStatusEffects.OnStatusEffectsChanged -= UpdateStatusDisplay;
        EnemyStatusEffects.OnEnemyStatusEffectsChanged -= UpdateEnemyStatusDisplay;
    }

    void Start() // Initialize display
    {
        UpdateStatusDisplay();
        UpdateEnemyStatusDisplay();
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
            if(poisonedQuantityText != null && poisonedIcon != null && poisonedTurnsText != null){
                poisonedQuantityText.gameObject.SetActive(false);
                poisonedIcon.gameObject.SetActive(false);
                poisonedTurnsText.gameObject.SetActive(false);
            }
            if(strengthenQuantityText != null && strengthenIcon != null && strengthenTurnsText != null)
            {
                strengthenQuantityText.gameObject.SetActive(false);
                strengthenIcon.gameObject.SetActive(false);
                strengthenTurnsText.gameObject.SetActive(false);
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
                    poisonedTurnsText.text = $"{poison.turnDuration.ToString()} T";
                }
            }
            else // If player is not poisoned, stay hiddden
            {
                poisonedQuantityText.gameObject.SetActive(false);
                poisonedIcon.gameObject.SetActive(false);
                poisonedTurnsText.gameObject.SetActive(false);
            }
        }
        if(strengthenQuantityText != null && strengthenIcon != null && strengthenTurnsText != null)
        {
            if (PlayerStatusEffects.Instance.isStrengthened) // If the player is strengthened, show the icons and update the text
            {
                strengthenQuantityText.gameObject.SetActive(true);
                strengthenIcon.gameObject.SetActive(true);
                strengthenTurnsText.gameObject.SetActive(true);

                StatusEffect strengthen = PlayerStatusEffects.Instance.findStatusEffect(EffectType.Strengthen);

                if(strengthen.turnDuration > 0)
                {
                    strengthenQuantityText.text = strengthen.quantity.ToString();
                    strengthenTurnsText.text = $"{strengthen.turnDuration.ToString()} T";
                }
            }
            else // If the player is not strengthened, hide all of the assets
            {
                strengthenQuantityText.gameObject.SetActive(false);
                strengthenIcon.gameObject.SetActive(false);
                strengthenTurnsText.gameObject.SetActive(false);
            }
        }
    }

    void UpdateEnemyStatusDisplay()
    {
        if (EnemyStatusEffects.Instance == null) // No status effect system, hide everything
        {
            if (enemyStrengthenQuantityText != null && enemyStrengthIcon != null)
            {
                enemyStrengthenQuantityText.gameObject.SetActive(false);
                enemyStrengthIcon.gameObject.SetActive(false);
                infinitySymbol.gameObject.SetActive(false);
                enemyStrengthenTurnsText.gameObject.SetActive(false);
            } 
            return;       
        }

        if(enemyStrengthenQuantityText != null && enemyStrengthIcon != null)
        {
            if (EnemyStatusEffects.Instance.isStrengthened) // Checks for strengthen for display
            {
                enemyStrengthIcon.gameObject.SetActive(true);

                // If the turn duration is -1 that means that the strengthen never goes away
                if(EnemyStatusEffects.Instance.findStatusEffect(EffectType.Strengthen).turnDuration == -1)
                {
                    enemyStrengthenQuantityText.text = EnemyStatusEffects.Instance.findStatusEffect(EffectType.Strengthen).quantity.ToString();
                    enemyStrengthenQuantityText.gameObject.SetActive(true);
                    infinitySymbol.gameObject.SetActive(true);
                }
                else // If the strengthen lasts for a limited number of turns
                {
                    enemyStrengthenTurnsText.text = EnemyStatusEffects.Instance.findStatusEffect(EffectType.Strengthen).turnDuration.ToString();
                    enemyStrengthenQuantityText.text = EnemyStatusEffects.Instance.findStatusEffect(EffectType.Strengthen).quantity.ToString();
                    enemyStrengthenQuantityText.gameObject.SetActive(true);
                    enemyStrengthenTurnsText.gameObject.SetActive(true);
                }
            }
            else // If the enemy isn't strengthened, hide everything
            {
                enemyStrengthenQuantityText.gameObject.SetActive(false);
                enemyStrengthIcon.gameObject.SetActive(false);
                infinitySymbol.gameObject.SetActive(false);
                enemyStrengthenTurnsText.gameObject.SetActive(false);
            }
        }
    }
}

