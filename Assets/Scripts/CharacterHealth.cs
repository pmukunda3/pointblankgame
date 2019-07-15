using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterHealth : MonoBehaviour
{
    public float CurrHealth { get; set; }
    public float MaxHealth { get; set; }

    public Slider healthBar;  


    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = 20f;
        //full health at start of game
        CurrHealth = MaxHealth;
        healthBar.value = CalculateHealth();  
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            DealDamage(6);
        }

    }

    void DealDamage(float damageValue)
    {
        CurrHealth -= damageValue;
        healthBar.value = CalculateHealth(); 
        if(CurrHealth <= 0)
        {
            Die();
        }
    }

    float CalculateHealth()
    {
        return CurrHealth / MaxHealth; 
    }
    void Die()
    {
        CurrHealth = 0;
        Debug.Log("dead");
    }
}
