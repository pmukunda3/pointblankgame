using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterHealth : MonoBehaviour
{
    public float CurrHealth { get; set; }
    public float MaxHealth {
        get { return maxHealth; }
        set {
            maxHealth = value;
            Init();
        }
    }

    public Slider healthBar;

    private float maxHealth = 20f;

    void Init()
    {
        //full health at start of game
        CurrHealth = MaxHealth;
        healthBar.value = CalculateHealth();  
    }

    public void DealDamage(float damageValue)
    {
        CurrHealth -= damageValue;
        healthBar.value = CalculateHealth(); 
        if(CurrHealth <= 0)
        {
            EventManager.TriggerEvent<PlayerControl.PlayerDeathEvent>();
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
