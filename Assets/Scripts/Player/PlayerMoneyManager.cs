using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoneyManager : MonoBehaviour
{
    public static PlayerMoneyManager Instance;
    private int money = 0;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI areaText;
    public TextMeshProUGUI enoughCoinText;

    private int totalArea = 4;
    private int placedArea = 0;

    public void InitialEnoughCoinText(TextMeshProUGUI text)
    {
        text.DOFade(1f, .5f)
            .OnComplete((() => text.DOFade(0f, .5f)
                .SetDelay(.7f)));
    }

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
        AudioManager.Instance.PlayOneShot("Add Coin");
        return GetMoney;
    }

    public int SubtractMoney(int amount)
    {
        if (GetMoney < amount)
        {
            return 0;
        }

        GetMoney -= amount;
        AudioManager.Instance.PlayOneShot("Substract Coin");
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

        if (Input.GetKeyDown(KeyCode.H))
        {
            SubtractMoney(10);
        }
    }

    public void BuyObject(int amount)
    {
        if (GetMoney < amount)
            return;
        GetMoney -= amount;
    }
}