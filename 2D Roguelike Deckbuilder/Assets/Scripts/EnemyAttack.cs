using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //Uses RNG to determine attack damage
    public void attackPlayer(Player player) {
        int damage = RoundManager.instance.RNG.Next(10, 51);
        player.TakeDamage(damage);
        Debug.Log("Enemy dealt " + damage + " damage to player!");
    }
}
