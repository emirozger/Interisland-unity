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

    public void OnInteract()
    {
        if (PlayerPickAndDrop.Instance.InHand)
            return;
        switch (objectType)
        {
            case ObjectType.Null:
                break;
            case ObjectType.EmptyCase:
                break;
            case ObjectType.OrangeFish:
                playerMoneyManager.BuyObject(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                Debug.Log("Buy Orange Fish");
                break;
            case ObjectType.BlueFish:
                Debug.Log("Buy Blue Fish");
                playerMoneyManager.BuyObject(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Pineapple:
                Debug.Log("Buy Pineapple");
                playerMoneyManager.BuyObject(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Peach:
                Debug.Log("Buy Peach");
                playerMoneyManager.BuyObject(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            case ObjectType.Apple:
                Debug.Log("Buy Aplle");
                playerMoneyManager.BuyObject(this.objectPrice);
                Instantiate(instantiatePrefab, spawnPos.position, Quaternion.Euler(-90, 0, 0));
                break;
            default:
                Debug.Log("default");
                break;
        }
    }
}