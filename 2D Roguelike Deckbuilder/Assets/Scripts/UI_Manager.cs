using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;    //singleton

    [SerializeField] private TMP_Text combatStartText;
    [SerializeField] private TMP_Text roundStartText;
    [SerializeField] private TMP_Text playerTurnText;
    [SerializeField] private TMP_Text enemyTurnText;

    void Awake()
    {
        instance = this;

        TurnEndRoutine.PrintRoundStartMessage.AddListener(PrintRoundStartMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.AddListener(PrintEnemyTurnMessage);
        TurnEndRoutine.RemovePlayerTurnMessage.AddListener(RemovePlayerTurnMessage);
        TurnEndRoutine.RemoveEnemyTurnMessage.AddListener(RemoveEnemyTurnMessage);
        TurnEndRoutine.PrintCombatStartMessage.AddListener(PrintCombatStartMessage);
        TurnEndRoutine.RemoveCombatStartMessage.AddListener(RemoveCombatStartMessage);
    }

    void OnDestroy()
    {
        TurnEndRoutine.PrintRoundStartMessage.RemoveListener(PrintRoundStartMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(PrintEnemyTurnMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(RemovePlayerTurnMessage);
        TurnEndRoutine.PrintEnemyTurnMessage.RemoveListener(RemoveEnemyTurnMessage);
        TurnEndRoutine.PrintCombatStartMessage.RemoveListener(PrintCombatStartMessage);
        TurnEndRoutine.RemoveCombatStartMessage.RemoveListener(RemoveCombatStartMessage);
    }

    public void PrintCombatStartMessage()
    {
        combatStartText.gameObject.SetActive(true);
    }

    public void RemoveCombatStartMessage()
    {
        combatStartText.gameObject.SetActive(false);
    }

    public void PrintRoundStartMessage()
    {
        roundStartText.text = $"Round " + RoundManager.instance.roundNumber;
        roundStartText.gameObject.SetActive(true);
        playerTurnText.gameObject.SetActive(true);
    }

    public void RemovePlayerTurnMessage()
    {
        playerTurnText.gameObject.SetActive(false);
        roundStartText.gameObject.SetActive(false);
    }

    public void PrintEnemyTurnMessage()
    {
        enemyTurnText.gameObject.SetActive(true);
    }

    public void RemoveEnemyTurnMessage()
    {
        enemyTurnText.gameObject.SetActive(false);
    }
}
