using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class EnemyAttack : MonoBehaviour
{
    public TextMeshProUGUI enemyIntentText;

    // Clunky way of displaying enemy intents for now, will think of a better solution later
    // This specific intent pattern is meant only for the centipede monster
    void Update() 
    {
        switch (RoundManager.instance.roundNumber)
        {
            case 1:
                enemyIntentText.text = $"8 Damage";
                break;
            case 2:
                enemyIntentText.text = $"Weaken Player";
                break;
            case 3:
                enemyIntentText.text = $"Apply 5 Poison";
                break;
            case 4:
                enemyIntentText.text = $"8 Damage";
                break;
            case 5:
                enemyIntentText.text = $"10 Damage";
                break;
            case 6:
                enemyIntentText.text = $"Apply 5 Poison";
                break;
        }

    }

    //Uses RNG to determine attack damage
    public void attackPlayer(Player player) {
        int enemyChoice = RoundManager.instance.roundNumber; // Instead of what moves the enemy does being based on RNG, it will have a fixed attack pattern

        switch (enemyChoice) // Enemy will either do damage to the player or apply status randomly
        {
            case 1:
                player.TakeDamage(8);
                Debug.Log("Enemy dealt " + 8 + " damage to player!");
                
                break;
            case 2:
                PlayerStatusEffects.Instance.ApplyWeaken(1, 1);
                
                break;
            case 3:
                PlayerStatusEffects.Instance.ApplyPoison(5, 5);
                break; 
            case 4:
                player.TakeDamage(8);
                Debug.Log("Enemy dealt " + 8 + " damage to player!");
                
                break;
            case 5:
                player.TakeDamage(10);
                Debug.Log("Enemy dealt " + 10 + " damage to player!");
                
                break;
            case 6:
                PlayerStatusEffects.Instance.ApplyPoison(5, 5);
                break; 
        }
    }
}
