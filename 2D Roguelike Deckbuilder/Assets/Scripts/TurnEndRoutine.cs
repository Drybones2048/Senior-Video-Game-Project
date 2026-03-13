using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TurnEndRoutine : MonoBehaviour
{
    private Coroutine flow;
    public static UnityEvent PrintCombatStartMessage = new UnityEvent();
    public static UnityEvent RemoveCombatStartMessage = new UnityEvent();
    public static UnityEvent PrintRoundStartMessage = new UnityEvent();
    public static UnityEvent PrintEnemyTurnMessage = new UnityEvent();
    public static UnityEvent RemovePlayerTurnMessage = new UnityEvent();
    public static UnityEvent RemoveEnemyTurnMessage = new UnityEvent();
    //[SerializeField] private float roundStartDelay = 3f;
    [SerializeField] private float enemyTurnActionDelay = 2f;   //how long it takes for the enemy to act. Later the enemy attacks will have animations which will take time on-screen
    [SerializeField] private float playerTurnStartDelay = 2f;   //how long after the enemy acts before the player's turn starts
    [SerializeField] private float playerTurnMessageDisplayLength = 1.4f;   //how long the player turn message stays on the screen
    [SerializeField] private float enemyTurnMessageDisplayLength = 2f;  //how long the enemy turn message stays on the screen
    [SerializeField] private float combatStartDelay = 2f;   //how long after the combat starts before the player's turn starts
    


    public void StartCombat() {
        flow = StartCoroutine(ResolveCombatStart());
    }   

    public void EndPlayerTurn() {
        flow = StartCoroutine(ResolvePlayerTurn());
    }

    public void EndEnemyTurn()
    {
        flow = StartCoroutine(ResolveEnemyTurn());
    }

    private IEnumerator ResolveCombatStart() {
        UI_Manager.instance.PrintCombatStartMessage();
        yield return new WaitForSeconds(combatStartDelay);
        UI_Manager.instance.RemoveCombatStartMessage();

        //start player's first turn
        RoundManager.combatStart.Invoke();  //This will deal the player's first hand
        UI_Manager.instance.PrintRoundStartMessage();
        yield return new WaitForSeconds(playerTurnMessageDisplayLength);
        UI_Manager.instance.RemovePlayerTurnMessage();
        RoundManager.startPlayerTurn.Invoke();  //now the game registers user input
    }

    private IEnumerator ResolvePlayerTurn() { // Print enemy turn messages and switch to enemy turn
        UI_Manager.instance.PrintEnemyTurnMessage();
        yield return new WaitForSeconds(enemyTurnMessageDisplayLength);
        UI_Manager.instance.RemoveEnemyTurnMessage();

        yield return new WaitForSeconds(enemyTurnActionDelay);  //this is just supposed to simulate the time it would take for the enemy animations to happen on screen
        RoundManager.startEnemyTurn.Invoke();
    }

    private IEnumerator ResolveEnemyTurn()
    {
        yield return new WaitForSeconds(playerTurnStartDelay);
        RoundManager.dealHand.Invoke();  //Deal the player's hand before accepting user input
        UI_Manager.instance.PrintRoundStartMessage();
        yield return new WaitForSeconds(playerTurnMessageDisplayLength);
        UI_Manager.instance.RemovePlayerTurnMessage();
        RoundManager.startPlayerTurn.Invoke();  //now the game registers user input

        flow = null; // Added to have the round text print every time with player turn start text
    }
}
