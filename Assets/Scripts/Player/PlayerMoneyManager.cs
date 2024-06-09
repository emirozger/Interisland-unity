using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoneyManager : MonoBehaviour
{
    public static PlayerMoneyManager Instance;
    private int money = 0;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI areaText;

    private int totalArea = 4;
    private int placedArea = 0;

    public int GetPlacedArea
    {
        get { return placedArea; }
        set
        {
            placedArea = value;
            areaText.text = placedArea + "/" + totalArea;
        }
    }


    public int AddMoney(int amount)
    {
        GetMoney += amount;
        return GetMoney;
    }

    public int SubtractMoney(int amount)
    {
        if (GetMoney < amount) return 0;
        GetMoney -= amount;
        return GetMoney;
    }

    public int GetMoney
    {
        get { return money; }
        set
        {
            money = value;
            playerMoneyText.text = money.ToString();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GetMoney = 15;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddMoney(10);
        }
    }

    public void BuyObject(int amount)
    {
        if (GetMoney < amount)
            return;
        GetMoney -= amount;
    }
}