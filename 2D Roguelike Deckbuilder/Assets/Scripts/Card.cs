using UnityEngine;

public abstract class Card // Will store a card's name, cost, description, and the Sprite it uses
{
    public string cardName;
    
    public int cost;

    public string description;

    public string sprite;

    public string type;

    public abstract void Play();
    
}
