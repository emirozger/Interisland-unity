using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoneyManager : MonoBehaviour
{
    public static PlayerMoneyManager Instance;
    
    public int money = 100;


    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
      
        money += amount;
    }
    public void BuyObject(int amount)
    {
        if (money<amount)
            return;
        money -= amount;
    }
}