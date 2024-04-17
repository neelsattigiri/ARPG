using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject player;
    public Transform attackPoint;
    public Transform attackPoint_walled;
    public Transform attackPoint_down;
    

    public LayerMask enemyLayer;
    public LayerMask wallLayer;
    public LayerMask trapLayer;

    public float attackRange = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to initiate an attack by player
    public void Attack()
    {
        // Get all the colliders that hit the created circle
        // Collider array for enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        // Collider array for walls
        Collider2D[] hitWalls = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, wallLayer);
        // For all enemies hit
        foreach(Collider2D enemy in hitEnemies)
        {
            // Damage the enemies
            enemy.GetComponent<EnemyStats>().Damage(2);
        }
        // For all walls hit
        foreach(Collider2D wall in hitWalls)
        {
            // Knock the player back based on direction of wall w.r.t player
            if(player.GetComponent<Rigidbody2D>().transform.position.x < wall.GetComponent<Rigidbody2D>().transform.position.x)
            {
                player.GetComponent<PlayerMovement>().ForcedMovement(-1, 0, 0.0625f);
            }
            else
            {
                player.GetComponent<PlayerMovement>().ForcedMovement(1, 0, 0.0625f);
            }
            
        }
    }

    public void Attack_Walled()
    {
        // Get all the colliders that hit the created circle and call Damage function on the collider object
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint_walled.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyStats>().Damage(2);
        }
    }

    public void Attack_Down()
    {
        // Get all the colliders that hit the created circle and call Damage function on the collider object
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint_down.position, attackRange, enemyLayer);
        Collider2D[] trap = Physics2D.OverlapCircleAll(attackPoint_down.position, attackRange, trapLayer);

        if (trap.Length != 0 || hitEnemies.Length != 0)
        {
            player.GetComponent<PlayerMovement>().ForcedMovement(0, 1.5f, 0.0625f);
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyStats>().Damage(2);
        }
    }

    public void Parry_Start()
    {
        player.GetComponent<PlayerStats>().parry = true;
    }

    public void Parry_End()
    {
        player.GetComponent<PlayerStats>().parry = false;
    }
}
