using UnityEngine;
using System.Collections.Generic;

public enum EnemyMoveType
{
    Damage,
    Weaken,
    Poison,
    Strengthen,
    Confuse
}
 
[System.Serializable]
public class EnemyMove
{
    public EnemyMoveType moveType;
    public int value;        // Damage amount, or stacks for status effects
    public int duration;     // Duration for status effects (ignored for Damage)
    public string intentLabel; // e.g. "8 Damage", "Apply 5 Poison"
 
    // Scaled versions of the same move used once the pattern loops
    public int scaledValue;
    public int scaledDuration;
    public string scaledIntentLabel;
}

[CreateAssetMenu(fileName = "EnemyAttackPattern", menuName = "Scriptable Objects/EnemyAttackPattern")]
public class EnemyAttackPattern : ScriptableObject
{
    public string enemyName;
    public List<EnemyMove> moves = new List<EnemyMove>();
    
    // Returns the correct move for the given round, looping with scaled values after the pattern ends
    public EnemyMove GetMoveForRound(int roundNumber)
    {
        if (moves == null || moves.Count == 0) return null;
 
        int patternLength = moves.Count;
        int zeroIndexed = roundNumber - 1;
 
        if (zeroIndexed < patternLength)
        {
            // Still within the base pattern
            return moves[zeroIndexed];
        }
        else
        {
            // Loop using scaled values and fetch the equivalent move in the repeated cycle
            int loopIndex = zeroIndexed % patternLength;
            EnemyMove baseMoveRef = moves[loopIndex];
 
            // Return a temporary move using the scaled values
            EnemyMove scaledMove = new EnemyMove
            {
                moveType        = baseMoveRef.moveType,
                value           = baseMoveRef.scaledValue,
                duration        = baseMoveRef.scaledDuration,
                intentLabel     = baseMoveRef.scaledIntentLabel
            };
 
            return scaledMove;
        }
    }
}
