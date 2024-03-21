using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSorting : MonoBehaviour
{
    public static ObjectSorting Instance { get; private set; }
    public List<GameObject> placedObjList = new List<GameObject>();
    private Vector3 standardVector;
    public int areaLimit = 4;
    [SerializeField] private int row = 1;
    [SerializeField] private float distance = 1.2f;

    private void Awake() => Instance = this;

    private void Start()
    {
        //this is pick area script ?? or 1 genel manager?
        standardVector = this.transform.position;
    }
    public void AddItem(GameObject item)
    {
        if (!placedObjList.Contains(item))
        {
            placedObjList.Add(item);
            UpdateSort();
        }
    }
    public void RemoveItem(GameObject item)
    {
        if (placedObjList.Contains(item))
        {
            placedObjList.Remove(item);
            UpdateSort();
        }
    }
    public void UpdateSort()
    {
        float xOffset = standardVector.x;
        float zOffset = standardVector.z;

        for (int i = 0; i < placedObjList.Count; i++)
        {
            if (0 == i % row && i != 0)
            {
                xOffset = standardVector.x;
                zOffset += distance;
            }

            placedObjList[i].transform.position =
                new Vector3(xOffset + distance * (i % row), standardVector.y, zOffset);
        }
    }
    
    public void RemoveAll()
    {
        placedObjList.Clear();
    }
}
