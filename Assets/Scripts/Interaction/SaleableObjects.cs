using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SaleableObjects : MonoBehaviour, IInteractable
{
    public ObjectType objectType;
    public int objectPrice;
    public Highlight highlight;
    private PlayerMoneyManager playerMoneyManager;

    public GameObject instantiatePrefab;
    public Transform spawnPos;


    private void Awake()
    {
        highlight = GetComponent<Highlight>();
        playerMoneyManager = FindObjectOfType<PlayerMoneyManager>();
    }

    public int Price => objectPrice;

    public void OnInteract()
    {
        if (PlayerPickAndDrop.Instance.InHand)
            return;
        
        if (playerMoneyManager.GetMoney < objectPrice)
        {
           playerMoneyManager.InitialEnoughCoinText(playerMoneyManager.enoughCoinText);
           AudioManager.Instance.PlayOneShot("Offer No");
           return;
        }
        
        switch (objectType)
        {
            case ObjectType.Hicbirsey:
                break;
            case ObjectType.BosKutu:
                break;
            case ObjectType.TuruncuBalik:
                playerMoneyManager.SubtractMoney(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                Debug.Log("Buy Orange Fish");
                break;
            case ObjectType.MaviBalik:
                Debug.Log("Buy Blue Fish");
                playerMoneyManager.SubtractMoney(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Ananas:
                Debug.Log("Buy Pineapple");
                playerMoneyManager.SubtractMoney(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Seftali:
                Debug.Log("Buy Peach");
                playerMoneyManager.SubtractMoney(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Elma:
                Debug.Log("Buy Aplle");
                playerMoneyManager.SubtractMoney(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Mektup:
                Debug.Log("Buy Post");
                playerMoneyManager.SubtractMoney(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            default:
                Debug.Log("default");
                break;
        }
    }
}