using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class EnemyAttack : MonoBehaviour
{
    public EnemyAttackPattern attackPattern; // Will store the scriptable object of the enemy in question
    public TextMeshProUGUI enemyIntentText;

    void Update(){
        if (attackPattern == null) return;
 
        EnemyMove currentMove = attackPattern.GetMoveForRound(RoundManager.instance.roundNumber);
        if (currentMove != null) // Get the intent text displayed correctly every round
        {
            enemyIntentText.text = currentMove.intentLabel;
        }
    }
 
    public void attackPlayer(Player player) // Method that is called at the start of the enemy's turn every round, will do attack actions based on the enemy's scriptable object
    {
        if (attackPattern == null)
        {
            Debug.LogWarning("EnemyAttack: No attack pattern assigned!");
            return;
        }
 
        EnemyMove move = attackPattern.GetMoveForRound(RoundManager.instance.roundNumber);
        if (move == null)
        {
            Debug.LogWarning("EnemyAttack: No move found for round " + RoundManager.instance.roundNumber);
            return;
        }
 
        switch (move.moveType) // Switch case that determines the values for actions based on the attack type
        {
            case EnemyMoveType.Damage:
                player.TakeDamage(move.value);
                Debug.Log($"Enemy dealt {move.value} damage to player!");
                break;
 
            case EnemyMoveType.Weaken:
                PlayerStatusEffects.Instance.ApplyWeaken(move.value, move.duration);
                Debug.Log($"Enemy applied Weaken ({move.value} stacks, {move.duration} duration) to player!");
                break;
 
            case EnemyMoveType.Poison:
                PlayerStatusEffects.Instance.ApplyPoison(move.value, move.duration);
                Debug.Log($"Enemy applied Poison ({move.value} stacks, {move.duration} duration) to player!");
                break;

            // ADD MORE TO THIS SWITCH CASE FOR MORE TYPES OF ENEMY ACTIONS LIKE STRENGTHEN AND CONFUSE
        }
    }
}
