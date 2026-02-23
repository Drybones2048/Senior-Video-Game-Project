using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EnemyAttack : MonoBehaviour
{
    //public static event Action playerWeakened;

    //Uses RNG to determine attack damage
    public void attackPlayer(Player player) {
        int damage = RoundManager.instance.RNG.Next(10, 51);
        player.TakeDamage(damage);
        Debug.Log("Enemy dealt " + damage + " damage to player!");
    }

    /*public void weakenPlayer(Player player) // Will be used to decrease player's damage by 20% and shows recuded damage on text of all attack cards
    {
        playerWeakened.Invoke();

        player.weakened = true;
    }*/
}
