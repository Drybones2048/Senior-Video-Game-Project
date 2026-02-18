using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TurnEndRoutine : MonoBehaviour
{
    private Coroutine flow;
    public static UnityEvent PrintRoundStartMessage = new UnityEvent();
    public static UnityEvent PrintPlayerTurnMessage = new UnityEvent();
    public static UnityEvent PrintEnemyTurnMessage = new UnityEvent();
    public static UnityEvent RemovePlayerTurnMessage = new UnityEvent();
    [SerializeField] private float roundStartDelay = 3f;
    [SerializeField] private float enemyTurnStartDelay = 2f;
    [SerializeField] private float playerTurnStartDelay = 2f;
    [SerializeField] private float playerTurnMessageDisplayLength = 2f;


    public void StartRound() {
        //if it's not null, something is probably wrong
        if (flow != null)
        {
            return;
        }
        flow = StartCoroutine(ResolveRoundStart());
    }

    public void EndPlayerTurn() {
        //don't start another turn if the turn is in progress
        if (flow != null)
        {
            return;
        }
        flow = StartCoroutine(ResolvePlayerTurn());
    }

    public void EndEnemyTurn()
    {
        //prevent overlapping coroutines
        if (flow != null)
        {
            StopCoroutine(flow);
        }
        flow = StartCoroutine(ResolveEnemyTurn());
    }

    private IEnumerator ResolveRoundStart() {
        PrintRoundStartMessage.Invoke();
        yield return new WaitForSeconds(roundStartDelay);
        RoundManager.instance.currentState = gameState.playerTurn;
        PrintPlayerTurnMessage.Invoke();
        /*at this point we've given the user freedom to select cards again, so it could cause a bug if they start a new coroutine by clicking the endTurn button
          before "yield return new WaitForSeconds playerTurnMessageDisplayLength" has finished. It could cause "RemovePlayerTurnMessage.Invoke();" to never run*/
        yield return new WaitForSeconds(playerTurnMessageDisplayLength);
        RemovePlayerTurnMessage.Invoke();


        flow = null;
    }

    private IEnumerator ResolvePlayerTurn() {
        yield return new WaitForSeconds(enemyTurnStartDelay);
        RoundManager.instance.currentState = gameState.enemyTurn;
        RoundManager.startEnemyTurn.Invoke();    //I could move this event from being instantiated in RoundManager, to being instantiated in this file. I probably should
    }

    private IEnumerator ResolveEnemyTurn()
    {
        yield return new WaitForSeconds(playerTurnStartDelay);
        RoundManager.instance.currentState = gameState.playerTurn;
        RoundManager.startPlayerTurn.Invoke();
        flow = null;    //end of the turn
    }
}
