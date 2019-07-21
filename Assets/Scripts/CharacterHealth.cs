using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CharacterHealth : MonoBehaviour
{
    public float CurrHealth {
        get { return currHealth; }
        set {
            currHealth = value;
            healthBar.value = CalculateHealth();
        }
    }
    public float MaxHealth {
        get { return maxHealth; }
        set {
            maxHealth = value;
            Init();
        }
    }

    public Slider healthBar;

    private float maxHealth = 20f;
    private float currHealth;

    private bool playerAlive = true;

    void Init()
    {
        //full health at start of game
        currHealth = MaxHealth;
        healthBar.value = CalculateHealth();  
    }

    public void DealDamage(float damageValue)
    {
        CurrHealth -= damageValue;
        healthBar.value = CalculateHealth(); 
        if (currHealth <= 0 && playerAlive) {
            playerAlive = false;
            EventManager.TriggerEvent<PlayerControl.PlayerDeathEvent>();
        }
    }

    float CalculateHealth()
    {
        return currHealth / maxHealth; 
    }
    void Die()
    {
        CurrHealth = 0;
        Debug.Log("dead");
    }
}
