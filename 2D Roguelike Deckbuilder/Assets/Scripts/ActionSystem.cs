using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;

    public bool IsPerforming { get; private set; } = false;

    //all reactions that subscribe to the pre-phase of our game action 
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();

    //all reactions that subscribe to the post-phase of our game action
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    //
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    public void Perform(GameAction action, System.Action OnPerformFinished = null) {
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    public void AddReaction(GameAction gameAction) {
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null) {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }
}
