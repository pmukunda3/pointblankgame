﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterOverheat : MonoBehaviour
{
    public float CurrHeat { get; set; }
    public float MaxHeat { get; set; }

    public Slider overheatBar;


    // Start is called before the first frame update
    void Start()
    {
        MaxHeat = 20f;
        CurrHeat = 0;
        overheatBar.value = CalculateOverheat();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            DealDamage(6);
        }

    }

    void DealDamage(float damageValue)
    {
        CurrHeat += damageValue;
        overheatBar.value = CalculateOverheat();
        if (CurrHeat >= 20f)
        {
            Die();
        }
    }

    float CalculateOverheat()
    {
        return CurrHeat / MaxHeat;
    }
    void Die()
    {
        CurrHeat = 0f;
        Debug.Log("overheated");
    }
}