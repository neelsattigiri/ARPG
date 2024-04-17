using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpearHeavyAttack1 : MonoBehaviour
{
    // Public variables
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public float attackRange = 1f;
    public GuardSpearHeavyAI guardSpearHeavy;
    public int attackDmg = 1;

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
        guardSpearHeavy.isAttacking = true;
    }

    // Function to end the attack
    public void Attack1_End()
    {
        guardSpearHeavy.isAttacking = false;
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
                guardSpearHeavy.GetParried();

            }
            else
            {
                // Call damage function on the collider object
                enemy.GetComponent<PlayerStats>().Damage(attackDmg);

                // Trigger knockback on enemy based on direction
                if (guardSpearHeavy.isFacingRight)
                {
                    enemy.GetComponent<PlayerMovement>().ForcedMovement(1, 0,  0.0625f);
                }
                else if (!guardSpearHeavy.isFacingRight)
                {
                    enemy.GetComponent<PlayerMovement>().ForcedMovement(-1, 0,  0.0625f);
                }
            }


        }
    }

    // Function to set Charge windup flag to true
    public void ChargeWindupEnable()
    {
        guardSpearHeavy.chargeWindup = true;
    }

    // Function to set Charge windup flag to false
    public void ChargeWindupDisable()
    {
        guardSpearHeavy.chargeWindup = false;
    }

    // Function to set charging  flag to false
    public void IsChargingDisable()
    {
        guardSpearHeavy.isCharging = false;
        guardSpearHeavy.chargeDirection = 0;
    }

}
