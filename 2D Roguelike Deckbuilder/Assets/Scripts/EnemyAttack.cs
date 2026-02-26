using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EnemyAttack : MonoBehaviour
{
    //Uses RNG to determine attack damage
    public void attackPlayer(Player player) {
        int enemyChoice = RoundManager.instance.RNG.Next(1, 3);

        switch (enemyChoice) // Enemy will either do damage to the player or apply status randomly
        {
            case 1:
                int damage = RoundManager.instance.RNG.Next(10, 51);
                player.TakeDamage(damage);
                Debug.Log("Enemy dealt " + damage + " damage to player!");
                
                break;
            case 2:
                PlayerStatusEffects.Instance.ApplyWeaken(1);
                
                break;
        }
    }
}
