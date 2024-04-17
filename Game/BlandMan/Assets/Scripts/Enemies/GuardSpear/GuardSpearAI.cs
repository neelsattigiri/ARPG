using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpearAI : MonoBehaviour
{
    // Public Animators

    public Animator enemy_torso;
    public Animator enemy_legs;
    


    // Public variables
    public Transform playerLock;
    public float moveSpeed = 0.6f;
    public Rigidbody2D ownRB;
    public float aggroRange = 5f;
    public float followRange = 7f;
    public bool isAttacking = false;
    public GameObject boundary_left;
    public GameObject boundary_right;
    public bool isFacingRight = true;
    public EnemyStats enemyStats;

    // Private variables


    private bool aggro = false;
    private int direction; // Right = 1, Stop = 0, Left = -1
    private Vector2 initialposition;
    private bool outOfBounds = false;


    // Start is called before the first frame update
    void Start()
    {
        // Set initial position of the enemy
        initialposition = new Vector2(ownRB.position.x, ownRB.position.y);

    }

    // Update is called once per frame
    void Update()
    {
        PlayerLockCheck();
        Move();
        Flip();
    }

    private void FixedUpdate()
    {
        // Allow movement only when not attacking
        if (!isAttacking)
        {
            // Movement based on direction calculated by 'Move' function
            ownRB.velocity = new Vector2(direction * moveSpeed, ownRB.velocity.y);
        }
        // Don't move along x axis when in the process of attacking
        else
        {
            ownRB.velocity = new Vector2(0, ownRB.velocity.y);
        }
        
    }

    // Check if player is within aggro range
    private void PlayerLockCheck()
    {
        // If enemy outside the bounds set by left and right boundary game objects
        if(transform.position.x <= boundary_left.transform.position.x || transform.position.x >= boundary_right.transform.position.x)
        {
            outOfBounds = true;
        }

        // If player is less than 2 units above the enemy and within the aggro distance
        if (Mathf.Abs(playerLock.transform.position.y - transform.position.y) <= 1.5 && Mathf.Abs(playerLock.transform.position.x - transform.position.x) <= aggroRange)
        {
            aggro = true;
        }
        // If player is more than 2 units above the enemy or beyond the follow range
        else if(Mathf.Abs(playerLock.transform.position.y - transform.position.y) > 1.5 || Mathf.Abs(playerLock.transform.position.x - transform.position.x) > followRange)
        {
            aggro = false;
        }
    }

    // Function to move based on aggro and direction w.r.t player
    private void Move()
    {
        // If not already in the process of attacking
        if (!isAttacking)
        {
            // If enemy has aggro locked onto player and not out of bounds
            if (aggro && !outOfBounds)
            {
                // If player is on the right of enemy more than 1.2 units away
                if (playerLock.transform.position.x - transform.position.x > 1.2)
                {
                    // Move right
                    direction = 1;
                }
                // If player is on the right of enemy more than 1.2 units away
                else if (playerLock.transform.position.x - transform.position.x < -1.2)
                {
                    // Move left
                    direction = -1;
                }
                // If player is within 1.2 units of enemy
                else
                {
                    // Don't move
                    direction = 0;

                    // Attack
                    enemyAttack1();

                }
            }
            else
            {
                // If enemy out of bounds but still aggroed to player
                if (aggro)
                {
                    // If within attack range
                    if (Mathf.Abs(playerLock.transform.position.x - transform.position.x) <= 1.2f)
                    {
                        // Don't move
                        direction = 0;

                        // Attack
                        enemyAttack1();

                        
                    }

                    // If enemy is outside the left boundary but player is to the right of enemy
                    else if (transform.position.x <= boundary_left.transform.position.x && playerLock.transform.position.x > transform.position.x)
                    {
                        direction = 1;
                        outOfBounds = false;
                    }
                    // If enemy is outside the right boundary but player is to the left of enemy
                    else if (transform.position.x >= boundary_right.transform.position.x && playerLock.transform.position.x < transform.position.x)
                    {
                        direction = -1;
                        outOfBounds = false;
                    }
                    else
                    {
                        // Don't move
                        direction = 0;
                    }
                }
                // Enemy not aggroed to player
                // If enemy to the left of initial position plus some buffer value
                else if (transform.position.x < initialposition.x - 0.5)
                {
                    // Move right
                    direction = 1;
                    // Clear out of bounds flag when inside the left boundary
                    if(transform.position.x > boundary_left.transform.position.x)
                    {
                        outOfBounds = false;
                    }
                }
                // If enemy to the right of initial position plus some buffer value
                else if (transform.position.x > initialposition.x + 0.5)
                {
                    // Move left
                    direction = -1;
                    // Clear out of bounds flag when inside the right boundary
                    if (transform.position.x < boundary_right.transform.position.x)
                    {
                        outOfBounds = false;
                    }
                }
                else
                {
                    // Don't move
                    direction = 0;
                    // Clear out of bounds flag
                    outOfBounds = false;
                }

            }
            // Animator input
            enemy_torso.SetFloat("moveSpeed", Mathf.Abs(direction));
            enemy_legs.SetFloat("moveSpeed", Mathf.Abs(direction));
        }
        else
        {
            // Don't move
            direction = 0;
        }
        
    }

    // Function to flip the direction of enemy
    private void Flip()
    {
        // If not already in the process of attacking
        if (!isAttacking)
        {
            // Flip the character if the direction of motion is is different from direction of sprite
            if (isFacingRight && direction == -1 || !isFacingRight && direction == 1)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
        
    }

    // Function to force the character to face the opposite direction
    private void ForceFlip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Function to make the enemy attack the player
    private void enemyAttack1()
    {
        // Flip enemy based on direction of player
        if ((playerLock.transform.position.x < transform.position.x) && isFacingRight)
        {
            ForceFlip();
        }
        else if ((playerLock.transform.position.x > transform.position.x) && !isFacingRight)
        {
            ForceFlip();
        }


        if (playerLock.transform.position.y > transform.position.y + 1f)
        {
            // Animator input for enemy Attack2
            enemy_torso.SetTrigger("Attack2");
        }
        else
        {
            // Animator input for enemy Attack1
            enemy_torso.SetTrigger("Attack1");
        }
        
        // Enemy is in the process of attacking
        isAttacking = true;
    }

    // Function to trigger the getting parried
    public void GetParried()
    {
        // Animator input - get parried animation
        enemy_torso.SetTrigger("getParried");
        // Reduce stamina by 1 point when parried
        this.gameObject.GetComponent<EnemyStats>().ReduceStamina(1); 
    }


}
