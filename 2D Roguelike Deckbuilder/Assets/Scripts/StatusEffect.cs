using UnityEngine;
using System;

public enum EffectType { Weaken, Strengthen, Poison, Confuse }
public enum EffectTarget { Player, Enemy }

[Serializable]
public class StatusEffect
{
    public EffectType effectType;
    [Min(0)] public int effectStartOffset;
    public int turnDuration;
    [Min(1)] public int quantity;
    public EffectTarget effectTarget;
}
