using UnityEngine;
using System.Collections.Generic;

public enum CardType { Attack, Defend, Special, Persistent }
public enum UniqueBehavior { None, PressAndFall, PiercingStrike } //going to add more here once I decide what needs unique behavior

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("Card Attributes")]
    public string name;
    public CardType type;
    public int cost;

    public int damage;
    public int shield;

    public List<StatusEffect> statusEffects;
    public UniqueBehavior uniqueBehavior;   //hardcoding behavior for certain cards, logic resolved in another script
}
