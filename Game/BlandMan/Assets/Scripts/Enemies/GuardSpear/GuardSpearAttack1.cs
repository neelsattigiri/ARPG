using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpearAttack1 : MonoBehaviour
{
    // Public variables
    public Transform attackPoint;
    public Transform attackPointUp;
    public LayerMask enemyLayer;
    public float attackRange = 1f;
    public float attackRangeUp = 1f;
    public GuardSpearAI guardSpear;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to start the attack
    public void Attack1_Start()
    {
        guardSpear.isAttacking = true;
    }

    // Function to end the attack
    public void Attack1_End()
    {
        guardSpear.isAttacking = false;
    }

    // Function to trgger damage and knockback on attack
    public void Attack()
    {
        // Get a list of all colliders of a layer that have been hit
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // Iterate through all the colliders hit
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<PlayerStats>().parry)
            {
                // Get parried
                // Debug.Log("enemy got parried");
                guardSpear.GetParried();

            }
            else
            {
                // Call damage function on the collider object
                enemy.GetComponent<PlayerStats>().Damage(2);

                // Trigger knockback on enemy based on direction
                if (guardSpear.isFacingRight)
                {
                    enemy.GetComponent<PlayerMovement>().ForcedMovement(1, 0 , 0.0625f);
                }
                else if (!guardSpear.isFacingRight)
                {
                    enemy.GetComponent<PlayerMovement>().ForcedMovement(-1, 0 , 0.0625f);
                }
            }                
        }
    }

    // Function to trgger damage and knockback on attack
    public void Attack2()
    {
        // Get a list of all colliders of a layer that have been hit
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointUp.position, 2f, enemyLayer);

        // Iterate through all the colliders hit
        foreach (Collider2D enemy in hitEnemies)
        {
            // Call damage function on the collider object
            enemy.GetComponent<PlayerStats>().Damage(2);

            // Trigger knockback on enemy based on direction
            if (guardSpear.isFacingRight)
            {
                enemy.GetComponent<PlayerMovement>().ForcedMovement(1, 1f, 0.0625f);
            }
            else if (!guardSpear.isFacingRight)
            {
                enemy.GetComponent<PlayerMovement>().ForcedMovement(-1, 1f, 0.0625f);
            }

        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPointUp.position, attackRangeUp);


    }



}
