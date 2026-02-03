using System.Collection.Generic;
using UnityEngine;

public abstract class GameAction
{
    //SUMMARY
    //3 lists of reactions, store all reactions while this action is performed

    //Actions that happen before your game action
    public List<GameAction> PreReactions { get; private set; } = new();

    //Actions that happen during your game action
    public List<GameAction> PerformReactions { get; private set; } = new();

    //Actions that happen after your game action
    public List<GameAction> postReactions { get; private set; } = new();
}
