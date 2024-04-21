using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // Public Animators

    public Animator playerTorso;
    public Animator playerLegs;

    // Public variables
    public float movementSpeed = 8f;
    public float jumpPower = 16f;
    public float doublejumpPower = 8f;
    public int max_jump_count = 2;
    public Rigidbody2D playerRB;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public bool dashing = false;
    public bool can_dash = true;
    public float dashtime = 1f;
    public float dashcooldown = 1f;
    public float dashspeed = 4f;
    public int dash_count = 1;

    // Private variables
    private float horizontal;
    private bool isFacingRight = true;
    public int current_jump_count;

    private bool forcedMovement = false;
    private float forcedMovementDirectionX = 0;
    private float forcedMovementDirectionY = 0;

    public bool walljump = false;
    public bool wallimpact = false;
    public bool wallhang = false;
    public float wallhangtime = 0.2f;
    public bool iswalljumping = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set the default jump count
        current_jump_count = max_jump_count;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is grounded
        IsGrounded();
        // Check if the player is walled
        IsWalled();

        // Check if player is not affected by forced movement
        if(!forcedMovement)
        {
            // Move when move keys are pressed
            Move();

            // Jump when jump key is pressed
            Jump();

            Dash();

            // Flip the character to face the direction he is movnig in
            Flip();

            // Check for attack input
            Attack1();
        }

        
        


    }

    private void FixedUpdate()
    {
        // Check if player is not affection by forced movemement
        if (!forcedMovement)
        {
            // If player is hanging on a wall
            if (wallhang)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
            }
            // If player is jumping from a wall
            if (walljump)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y);
            }
            // When not dashing
            else if (!dashing)
            {
                playerRB.velocity = new Vector2(horizontal * movementSpeed, playerRB.velocity.y);
            }
            // When dashing
            else
            {
                // If facing right, dash right
                if (isFacingRight)
                {
                    playerRB.velocity = new Vector2(dashspeed * 1 * movementSpeed, 0);
                }
                // If facing left, dash left
                else
                {
                    playerRB.velocity = new Vector2(dashspeed * -1 * movementSpeed, 0);
                }

                
            }

        }
        // If affected by forced movement
        else if(forcedMovement)
        {
            // Move the player in the direction of forced movement
            playerRB.velocity = new Vector2(10 * forcedMovementDirectionX , 6 * forcedMovementDirectionY);
        }
        // When not affected by any explicit input or force, don't move in x direction
        else
        {
            playerRB.velocity = new Vector2(0, playerRB.velocity.y);
        }

        // Gravity implementation
        // If not dashing
        if (!dashing && !wallhang)
        {
            // When player is jumping or pushed upwards, gravity is 30
            if (playerRB.velocity.y > 0)
            {
                playerRB.AddForce(new Vector2(0, -40));
            }
            // When moving down , gravity is 15. If falling at velocity > 5, stop applying gravity
            else if (playerRB.velocity.y >= -8f)
            {
                playerRB.AddForce(new Vector2(0, -30));
            }
        }

        


    }

    // Function to check if player is grounded
    public bool IsGrounded()
    {   
        // Check if player is grounded - use overlap circle to detect collision below the player
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (grounded)
        {
            if(playerRB.velocity.y == 0)
            {
                current_jump_count = max_jump_count;
            }
            
            
            dash_count = 1;
        }
        // Animator input
        playerLegs.SetBool("grounded", grounded);
        playerTorso.SetBool("grounded", grounded);
        playerLegs.SetFloat("verticalSpeed", playerRB.velocity.y);
        playerTorso.SetFloat("verticalSpeed", playerRB.velocity.y);
        return grounded;
    }

    private bool IsWalled()
    {
        // Check if player is walled - use overlap circle to detect collision infront of the player
        bool walled = Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);

        // If walled and grounded
        if (walled && !IsGrounded())
        {

            // When not moving horizontally, reset the jump counter
            if (playerRB.velocity.x == 0)
            {
                current_jump_count = max_jump_count;
            }
            
            // If not already hitting the wall, if walled
            if (!wallimpact)
            {
                // Start the process of hitting the wall
                WallImpact();
                StartCoroutine(WallHang());
                iswalljumping = false;
            }
            // If not jumping from the wall
            else if (!iswalljumping)
            {
                // Slide down the wall
                playerRB.velocity = new Vector2(playerRB.velocity.x, -0.6f);

            }
            else
            {
                // Fall at regular velocity
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y);
            }

            // Reset dash count
            dash_count = 1;
        }
        else
        {
            // IF not walled and grounded
            wallhang = false;
            wallimpact = false;
        }

        // Animator input
        playerTorso.SetBool("walled", walled);
        playerLegs.SetBool("walled", walled);
        return walled;
    }

    // Function for checking input for movement
    private void Move()
    {

        if (IsWalled() && !IsGrounded())
        {
            horizontal = 0;
        }
        else
        {
            // Set the horizonal velocity of player
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        

        // Animator input
        playerLegs.SetFloat("moveSpeed", Mathf.Abs(playerRB.velocity.x));
        playerTorso.SetFloat("moveSpeed", Mathf.Abs(playerRB.velocity.x));
    }

    // Function for player to Flip based on direction of movement
    private void Flip()
    {
        // Flip the character
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // Function to force the player to flip
    private void ForceFlip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Function for player to Jump
    private void Jump()
    {

        // Jump when Jump key is pressed and jump counter not zero
        if (Input.GetButtonDown("Jump") && current_jump_count > 0)
        {
            // When ready for double jump, set animator trigger again
            if (current_jump_count == 1)
            {
                playerTorso.SetTrigger("Jump");
            }
            // When jumping, cancel dash
            dashing = false;

            // If walled not grounded
            if(IsWalled() && !IsGrounded())
            {
                // Start a walljump
                iswalljumping = true;

                // Jump away from the direction of the wall
                if (isFacingRight)
                {
                    playerRB.velocity = new Vector2(-jumpPower/1.5f, jumpPower/1.5f);
                }
                else
                {
                    playerRB.velocity = new Vector2(jumpPower/1.5f, jumpPower/1.5f);
                }
                // Reset wall hanging
                wallhang = false;
                // Walljump
                StartCoroutine(Walljumping());
            }
            // If not walled or grounded, jump normally if jump counter is not zero
            else
            {
                if (current_jump_count == 2)
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, jumpPower);
                }
                else if (current_jump_count == 1)
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, doublejumpPower);
                }

                
            }
            // Reduce the jump counter by one.
            current_jump_count--;
        }
        // Reduce jump velocity if Jump key is released early
        if (Input.GetButtonUp("Jump") && playerRB.velocity.y > 0f)
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y * 0.5f);
        }

    }

    // Function to implement dashing
    private void Dash()
    {
        // When jump button is pressed
        if (Input.GetButtonDown("Fire2"))
        {
            
            // If nt dashing, allowed to dash and dash count not zero
            if(!dashing && can_dash && dash_count > 0)
            {
                // If walled but not grounded
                if (IsWalled() && !IsGrounded())
                {
                    // Force the player to flip
                    Debug.Log("flip");
                    
                    ForceFlip();
                    current_jump_count--;
                }

                // Start dashing
                StartCoroutine(Dashing());
            }
        }
        // If jump button is released
        if (Input.GetButtonUp("Fire2"))
        {
            // Cancel  dash
            dashing = false;
        }

        // Animator input - dashing
        playerTorso.SetBool("dashing", dashing);
        playerLegs.SetBool("dashing", dashing);
    }

    // Function to freeze player velocity when hitting a wall
    private void WallImpact()
    {
        wallimpact = true;
        playerRB.velocity = new Vector2(0, 0);
        
    }

    // Coroutine to process wall hang
    IEnumerator WallHang()
    {
        wallhang = true;
        yield return new WaitForSeconds(wallhangtime);
        wallhang = false;
    }

    // Function to check input for basic attack 
    private void Attack1()
    {
        // Attack when Fire1 key is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            // If S key is held
            if (Input.GetKey(KeyCode.S))
            {
                // Animator input - attack animation to attak down
                playerTorso.SetTrigger("attack3");
            }
            // If W key is held
            else if (Input.GetKey(KeyCode.W))
            {
                // Animator input - attack animation to attack up
                playerTorso.SetTrigger("attack2");
            }
            else
            {
                // Animator input - attack animation to attack
                playerTorso.SetTrigger("attack1");
            }
            
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            // Animator input - parry animation
            playerTorso.SetTrigger("parry");
        }
        
    }

    // Function to enable forced movement of player
    public void ForcedMovement(float directionX, float directionY, float knockbacktime)
    {
        // DirectionX - Magnitude of knockback in X directon
        forcedMovementDirectionX = directionX;
        // DirectionY - Magnitude of knockback in Y directon
        forcedMovementDirectionY = directionY;
        // Cancel dash during forced movement
        dashing = false;
        // Start knocking the player
        StartCoroutine(Knockback(knockbacktime));

    }

    // Coroutine to set knockback for player
    IEnumerator Knockback(float knockbacktime)
    {
        // Set forced movement flag
        forcedMovement = true;
        // Wait for some time - knockbacktime
        yield return new WaitForSeconds(knockbacktime);
        // Reset forced movement flag
        forcedMovement = false;
    }

    // Coroutine for dashing
    IEnumerator Dashing()
    {
        // If dash counter not zero
        if (dash_count > 0)
        {
            // Decrement dash counter
            dash_count--;
            // Start dashing
            dashing = true;
            // Disable player dash
            can_dash = false;
            // Wait for dashtime
            yield return new WaitForSeconds(dashtime);
            // End the dash
            dashing = false;
            // Wait for dash cooldown            
            yield return new WaitForSeconds(dashcooldown);
            // Enable player dash
            can_dash = true;
        }
        
    }

    // Coroutine for wall jump
    IEnumerator Walljumping()
    {
        // Start walljumping
        walljump = true;
        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);
        // End wall jump
        walljump = false;
    }

    // Function to check collision with other objects
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check tag of the collider - if enemy
        if (collision.gameObject.tag == "Enemy")
        {
            // Get the values of touch damange and knockback parameters from colliding object
            int touchDmg = collision.gameObject.GetComponent<OnTouch>().touchDmg;
            int moveX = collision.gameObject.GetComponent<OnTouch>().forcedMovementX;
            int moveY = collision.gameObject.GetComponent<OnTouch>().forcedMovementY;
            // Default knockback multiplider
            int knockbackmultiplier = 1;

            // If dashing
            if (dashing)
            {
                // Set knockback multiplier as 3
                knockbackmultiplier = 3;
            }
            else
            {
                // Set knockback multiplier as 1
                knockbackmultiplier = 1;
            }

            // Check if player is to the right or left of collider

            // If left of collider by 0.2 units
            if (collision.gameObject.GetComponent<Rigidbody2D>().position.x - playerRB.position.x > 0.2)
            {
                // Reverse knockback direction for x axis
                moveX = -moveX;
            }
            // If right of collider by 0.2 units
            else if (collision.gameObject.GetComponent<Rigidbody2D>().position.x - playerRB.position.x < -0.2)
            {
                // Do nothing - don't change direction (default is right)
            }
            else
            {
                // Don't knockback in x direction
                moveX = 0;
            }
            // Check if player is to the above or below of collider
            // If above the collider by 0.3 units
            if (collision.gameObject.GetComponent<Rigidbody2D>().position.y - playerRB.position.y > 0.3)
            {
                // Reverse the direction for y axis
                moveY = -moveY;
            }
            // If below the collider by 0.3 units
            else if (collision.gameObject.GetComponent<Rigidbody2D>().position.y - playerRB.position.y < -0.3)
            {
                // Do nothing - don't chnge direction (default is up)
            }
            else
            {
                // Don't knockback in y direction
                moveY = 0;
            }
            
            // Initiate forced movement with directions calculated above with time 0.0625 seconds
            ForcedMovement(moveX * knockbackmultiplier, moveY , 0.0625f);

            // Knockback also causes damage to the player based on dmg property of collider
            this.gameObject.GetComponent<PlayerStats>().Damage(touchDmg);
        }


    }

}
