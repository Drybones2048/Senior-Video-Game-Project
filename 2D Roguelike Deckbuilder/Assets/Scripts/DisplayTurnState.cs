using UnityEngine;
using TMPro;

public class DisplayTurnState : MonoBehaviour
{
    [SerializeField] private TMP_Text roundStartText;
    [SerializeField] private TMP_Text playerTurnText;
    [SerializeField] private TMP_Text enemyTurnText;

    void Awake()
    {
        TurnEndRoutine.PrintRoundStartMessage.AddListener(PrintRoundStartMessage);
        TurnEndRoutine.PrintPlayerTurnMessage.AddListener(PrintPlayerTurnMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.AddListener(PrintEnemyTurnMessage);
        TurnEndRoutine.RemovePlayerTurnMessage.AddListener(RemovePlayerTurnMessage);
    }

    void OnDestroy() {
        TurnEndRoutine.PrintRoundStartMessage.RemoveListener(PrintRoundStartMessage);
        TurnEndRoutine.PrintPlayerTurnMessage.RemoveListener(PrintPlayerTurnMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(PrintEnemyTurnMessage);
    }

    void PrintRoundStartMessage() {
        roundStartText.gameObject.SetActive(true);
    }

    void PrintPlayerTurnMessage() {
        roundStartText.gameObject.SetActive(false);
        playerTurnText.gameObject.SetActive(true);
    }

    void RemovePlayerTurnMessage() {
        playerTurnText.gameObject.SetActive(false);
    }

    void PrintEnemyTurnMessage() {
        playerTurnText.gameObject.SetActive(true);
    }

}
