using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TurnEndRoutine : MonoBehaviour
{
    private Coroutine flow;
    public static UnityEvent StartPlayerTurn = new UnityEvent();
    [SerializeField] private float enemyTurnStartDelay = 2f;
    [SerializeField] private float playerTurnStartDelay = 2f;


    public void EndPlayerTurn() {
        //don't start another turn if the turn is in progress
        if (flow != null)
        {
            return;
        }
        flow = StartCoroutine(WaitForEnemyTurn());
    }

    public void EndEnemyTurn()
    {
        //prevent overlapping coroutines
        if (flow != null)
        {
            StopCoroutine(flow);
        }
        flow = StartCoroutine(WaitForPlayerTurn());
    }

    private IEnumerator WaitForEnemyTurn() {
        yield return new WaitForSeconds(enemyTurnStartDelay);
        RoundManager.instance.currentState = gameState.enemyTurn;
        RoundManager.endPlayerTurn.Invoke();    //I could move this event from being instantiated in RoundManager, to being instantiated in this file. I probably should
    }

    private IEnumerator WaitForPlayerTurn()
    {
        yield return new WaitForSeconds(playerTurnStartDelay);
        RoundManager.instance.currentState = gameState.playerTurn;
        StartPlayerTurn.Invoke();
        flow = null;    //end of the turn
    }
}
