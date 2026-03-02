using UnityEngine;
using System.Collections.Generic;

public enum CardType { Attack, Defend, Special, Persistent }
public enum UniqueBehavior { None, PressAndFall, PiercingStrike, EmpoweringShield, Juggernaut } //going to add more here once I decide what needs unique behavior
public enum CardClass { All, BlockStance, StatusEffectStance, AttackStance }
public enum IsStartingCard { Yes, No }    //Is this card in the class's deck at the start of each run?

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("Card Attributes")]
    public string id;
    public string displayName;
    public CardType type;
    public int cost;

    public int damage;
    public int shield;

    public List<StatusEffect> statusEffects;
    public UniqueBehavior uniqueBehavior;   //hardcoding behavior for certain cards, logic resolved in another script
    public CardClass cardClass;
    public IsStartingCard isStartingCard;
}
