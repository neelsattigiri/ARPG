using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Public variables
    public List<SpriteRenderer> playerSprite;
    public int maxHP = 10;
    private int currentHP;
    public bool parry = false;
    public int maxStamina = 3;
    public float damage_cooldown_duration = 0.5f;
    

    // Private variables
    private int currentStamina;
    private float damage_cooldown_duration_time;
    

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        damage_cooldown_duration_time = damage_cooldown_duration / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int dmg)
    {
        Debug.Log("Player took dmg :" + dmg);
        currentHP -= dmg;
        // Toggle sprite colour to red and back
        StartCoroutine(SetColorBack());

        if (currentHP <= 0)
        {
            Debug.Log("Player Dead");
        }
        StartCoroutine(DmgCooldown());

    }

    // Coroutine to toggle sprite to red and back when taking damage
    IEnumerator SetColorBack()
    {
        foreach (SpriteRenderer sprite_r in playerSprite)
        {
            sprite_r.color = new Color(220, 0, 0, 255);
        }

        yield return new WaitForSeconds(0.25f);
        foreach (SpriteRenderer sprite_r in playerSprite)
        {
            sprite_r.color = new Color(255, 255, 255, 255);
        }
        
    }

    IEnumerator DmgCooldown()
    {
        this.gameObject.layer = 10;
        yield return new WaitForSeconds(0.25f);
        for (int i =0; i < 3; i++)
        {
            foreach (SpriteRenderer sprite_r in playerSprite)
            {
                sprite_r.color = new Color(255, 255, 255, 0.5f);
            }
            
            yield return new WaitForSeconds(damage_cooldown_duration_time);
            foreach (SpriteRenderer sprite_r in playerSprite)
            {
                sprite_r.color = new Color(255, 255, 255, 255);
            }
            
            yield return new WaitForSeconds(damage_cooldown_duration_time);
        }
        this.gameObject.layer = 8;
    }
}
