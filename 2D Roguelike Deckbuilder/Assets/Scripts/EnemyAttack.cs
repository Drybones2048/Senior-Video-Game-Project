using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class EnemyAttack : MonoBehaviour
{
    public EnemyAttackPattern attackPattern; // Will store the scriptable object of the enemy in question
    public TextMeshProUGUI enemyIntentText;

    [SerializeField] Vector2 centipedeTextPos;
    [SerializeField] Vector2 skeletonTextPos;
    [SerializeField] Vector2 pharaohTextPos;

    void Awake()
    {
        RoundManager.enemyDead.AddListener(moveText);
        Player.playerDead.AddListener(moveTextDown);
        DeathScreen.gameWon.AddListener(moveTextDown);
    }
    
    void Update(){
        if (attackPattern == null) return;

        EnemyMove currentMove = attackPattern.GetMoveForRound(RoundManager.instance.roundNumber);
        if (currentMove != null) // Get the intent text displayed correctly every round
        {
            enemyIntentText.text = currentMove.intentLabel;
        }

        EnemyMove move = attackPattern.GetMoveForRound(RoundManager.instance.roundNumber);
        if(move != null && move.moveType == EnemyMoveType.Damage && EnemyStatusEffects.Instance.isStrengthened)
        {
            enemyIntentText.text = $"{GetActualDamage(move.value)} Damage";
        }

        if(move != null && move.moveType == EnemyMoveType.Damage && EnemyStatusEffects.Instance.isWeakened) // If the enemy is weakened mid-turn, update the intent text with new damage values
        {
            enemyIntentText.text = $"{GetActualDamage(move.value)} Damage";
        }
    }

    void OnDestroy()
    {
        RoundManager.enemyDead.RemoveListener(moveText);
        Player.playerDead.RemoveListener(moveTextDown);
        DeathScreen.gameWon.RemoveListener(moveTextDown);
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
                int actualDamage = GetActualDamage(move.value);
                player.TakeDamage(actualDamage);
                Debug.Log($"Enemy dealt {actualDamage} damage to player!");
                break;
 
            case EnemyMoveType.Weaken:
                PlayerStatusEffects.Instance.ApplyWeaken(move.duration, move.value);
                Debug.Log($"Enemy applied Weaken ({move.value} stacks, {move.duration} duration) to player!");
                break;
 
            case EnemyMoveType.Poison:
                PlayerStatusEffects.Instance.ApplyPoison(move.duration, move.value);
                Debug.Log($"Enemy applied Poison ({move.value} stacks, {move.duration} duration) to player!");
                break;
            
            case EnemyMoveType.Strengthen:
                if (EnemyStatusEffects.Instance != null)
                    EnemyStatusEffects.Instance.ApplyStrengthen(move.value);
                else
                    Debug.LogWarning("EnemyAttack: EnemyStatusEffects instance not found. Make sure it is in the scene.");
                break;

            // ADD MORE TO THIS SWITCH CASE FOR MORE TYPES OF ENEMY ACTIONS LIKE CONFUSE
        }
    }

    int GetActualDamage(int baseDamage) // Makes all damage values go through this method and is altered based on if the enemy has strengthen
    {
        if (EnemyStatusEffects.Instance != null)
            return EnemyStatusEffects.Instance.GetModifiedAttackDamage(baseDamage);
 
        return baseDamage;
    }

    void moveText() // Function that moves the intent text of the enemies since their sprites are different heights
    {
        if(RoundManager.instance.encounterNumber == 0) // If the centipede is killed, move the text higher for the skeleton
        {
            enemyIntentText.rectTransform.anchoredPosition = centipedeTextPos;

        } if(RoundManager.instance.encounterNumber == 1) // If the centipede is killed, move the text higher for the skeleton
        {
            enemyIntentText.rectTransform.anchoredPosition = skeletonTextPos;

        } else if(RoundManager.instance.encounterNumber == 2){ // If the skeleton is killed, move the text highest for the pharaoh

            enemyIntentText.rectTransform.anchoredPosition = pharaohTextPos;
        } 
    }
    void moveTextDown() // When the player dies, move the intent text to centipede height again
    {
        enemyIntentText.rectTransform.anchoredPosition = centipedeTextPos;
    }
}
