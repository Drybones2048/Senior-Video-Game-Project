using UnityEngine;
using TMPro;

public class DisplayTurnState : MonoBehaviour
{
    [SerializeField] private TMP_Text roundStartText;
    [SerializeField] private TMP_Text playerTurnText;
    [SerializeField] private TMP_Text enemyTurnText;

    [SerializeField] private RoundManager round;

    void Awake()
    {
        TurnEndRoutine.PrintRoundStartMessage.AddListener(PrintRoundStartMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.AddListener(PrintEnemyTurnMessage);
        TurnEndRoutine.RemovePlayerTurnMessage.AddListener(RemovePlayerTurnMessage);
        TurnEndRoutine.RemoveEnemyTurnMessage.AddListener(RemoveEnemyTurnMessage);
    }

    void OnDestroy() {
        TurnEndRoutine.PrintRoundStartMessage.RemoveListener(PrintRoundStartMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(PrintEnemyTurnMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(RemovePlayerTurnMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(RemoveEnemyTurnMessage);
    }

    void PrintRoundStartMessage() {
        roundStartText.text = $"Round " + RoundManager.roundNumber;
        roundStartText.gameObject.SetActive(true);
        playerTurnText.gameObject.SetActive(true);
    }

    void RemovePlayerTurnMessage() {
        playerTurnText.gameObject.SetActive(false);
        roundStartText.gameObject.SetActive(false);
    }

    void PrintEnemyTurnMessage() {
        enemyTurnText.gameObject.SetActive(true);
    }

    void RemoveEnemyTurnMessage() {
        enemyTurnText.gameObject.SetActive(false);
    }

}
