using UnityEngine;
using System;
using System.Collections.Generic;

public class CardInstance
{
    public CardData card;
    public bool upgraded;

    public CardInstance(CardData card, bool upgraded = false)
    {
        this.card = card;
        this.upgraded = upgraded;
    }

    public string GetDescription()
    {
        string modifiedDescription = description;

        modifiedDescription = modifiedDescription.Replace("{damage}", actualDamage.ToString());
        modifiedDescription = modifiedDescription.Replace("{block}", block.ToString());

        return modifiedDescription;
    }

    //GETTERS
    public int cost => card.cost;
    public int damage
        => card.damage + (upgraded ? 3 : 0);   // example upgrade logic where damage is increased by 3
    public int actualDamage;

    public int block
        => card.block + (upgraded ? 3 : 0);    // example upgrade logic where block is increaesd by 3

    public string id => card.id;
    public string name => card.displayName;
    public string description => card.description;
    public string sprite => card.sprite;

    public CardType type => card.type;
    public List<StatusEffect> statusEffects => card.statusEffects;
    public UniqueBehavior uniqueBehavior => card.uniqueBehavior;
    public CardClass cardClass => card.cardClass;
    public IsStartingCard isStartingCard => card.isStartingCard;
}
