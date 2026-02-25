using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TurnEndRoutine : MonoBehaviour
{
    private Coroutine flow;
    public static UnityEvent PrintBattleStartMessage = new UnityEvent();
    public static UnityEvent RemoveBattleStartMessage = new UnityEvent();
    public static UnityEvent PrintRoundStartMessage = new UnityEvent();
    public static UnityEvent PrintEnemyTurnMessage = new UnityEvent();
    public static UnityEvent RemovePlayerTurnMessage = new UnityEvent();
    public static UnityEvent RemoveEnemyTurnMessage = new UnityEvent();
    //[SerializeField] private float roundStartDelay = 3f;
    [SerializeField] private float enemyTurnActionDelay = 2f;   //how long it takes for the enemy to act. Later the enemy attacks will have animations which will take time on-screen
    [SerializeField] private float playerTurnStartDelay = 2f;   //how long after the enemy acts before the player's turn starts
    [SerializeField] private float playerTurnMessageDisplayLength = 1.4f;   //how long the player turn message stays on the screen
    [SerializeField] private float enemyTurnMessageDisplayLength = 2f;  //how long the enemy turn message stays on the screen
    [SerializeField] private float battleStartDelay = 2f;   //how long after the battle starts before the player's turn starts
    


    public void StartBattle() {
        flow = StartCoroutine(ResolveBattleStart());
    }   

    public void EndPlayerTurn() {
        flow = StartCoroutine(ResolvePlayerTurn());
    }

    public void EndEnemyTurn()
    {
        flow = StartCoroutine(ResolveEnemyTurn());
    }

    private IEnumerator ResolveBattleStart() {
        UI_Manager.instance.PrintBattleStartMessage();
        yield return new WaitForSeconds(battleStartDelay);
        UI_Manager.instance.RemoveBattleStartMessage();

        //start player's first turn
        RoundManager.battleStart.Invoke();  //This will deal the player's first hand
        UI_Manager.instance.PrintRoundStartMessage();
        yield return new WaitForSeconds(playerTurnMessageDisplayLength);
        UI_Manager.instance.RemovePlayerTurnMessage();
        RoundManager.startPlayerTurn.Invoke();  //now the game registers user input
    }

    /*private IEnumerator ResolveRoundStart() {
        PrintRoundStartMessage.Invoke();
        yield return new WaitForSeconds(roundStartDelay);
        RoundManager.startPlayerTurn.Invoke();
        //RoundManager.instance.currentState = gameState.playerTurn;
        at this point we've given the user freedom to select cards again, so it could cause a bug if they start a new coroutine by clicking the endTurn button
          before "yield return new WaitForSeconds playerTurnMessageDisplayLength" has finished. It could cause "RemovePlayerTurnMessage.Invoke();" to never run
        yield return new WaitForSeconds(playerTurnMessageDisplayLength);
        RemovePlayerTurnMessage.Invoke();


        flow = null;
    }*/

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

    /*public void StartRound() {
        //if it's not null, something is probably wrong
        if (flow != null)
        {
            return;
        }
        flow = StartCoroutine(ResolveRoundStart());
    }*/
}
