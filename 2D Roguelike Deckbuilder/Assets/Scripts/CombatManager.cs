using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance {get; private set;}

    public Enemy currentEnemy;

    public Player player;

    void Awake()
    {
        Instance = this;
    }
}
