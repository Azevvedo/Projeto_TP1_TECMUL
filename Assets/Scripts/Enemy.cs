using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 50;

    // Call this to damage the enemy
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject); // remove from scene
    }
}
