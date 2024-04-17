using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    // Public variables
    public Animator enemy_torso;
    public Animator enemy_legs;
    public MonoBehaviour AI;
    public int maxHP = 10;
    public SpriteRenderer enemySprite;
    public int maxStamina = 3;

    // Private variables
    private int currentHP;
    private int currentStamina;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int dmg)
    {
        // Debug.Log("Enemy Took DMG");
        currentHP -= dmg;
        // Toggle sprite colour to red and back
        StartCoroutine(SetColorBack());

        if (currentHP <= 0)
        {
            // Debug.Log("Enemy Dead");
            // Play death animations
            enemy_torso.SetTrigger("death");
            enemy_legs.SetTrigger("death");

            // Disable the rigidbody, collider and AI script of enemy when dead
            this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            AI.enabled = false;

            // Start couroutine to remove enemy from display
            StartCoroutine(Die());

        }

        
    }

    // Coroutine Toggle colour of spirte to red and back when taking damage
    IEnumerator SetColorBack()
    {
        enemySprite.color = new Color(220, 0, 0, 255);
        yield return new WaitForSeconds(0.25f);
        enemySprite.color = new Color(255, 255, 255, 255);

    }

    // Coroutine to deactivate enemy game object on death
    IEnumerator Die()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }

    // Function to reduce enemy stamina
    public void ReduceStamina(int value)
    {
        // Reduce stamina based on function input
        currentStamina -= value;
        // Check if stamina less than or equal to zero
        if (currentStamina <= 0)
        {
            // Animator input - Enemyuu stun animation
            enemy_torso.SetTrigger("stunned");
            // Reset stamina to max stamina
            currentStamina = maxStamina;
        }
    }


}
