using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public List<Mission> missions = new List<Mission>();
    public Toggle mission1;
    public static MissionManager Instance;

    private void Awake(){
        Instance=this;
    }

    public void StartMissionDescription(int id,string description)
    {
        Mission newMission = new Mission(id,description);
        missions.Add(newMission);
        ToggleMissionPanel(true, description);
    }
    public void CompleteMission(int id)
    {
        Mission mission = missions.Find(m => m.missionID == id);
        if (mission != null)
        {
            mission.CompleteMission();
            ToggleMissionPanel(false, mission.missionDescription);
        }
    }

    private void ToggleMissionPanel(bool activate, string description)
    {
        mission1.transform.DOMoveX(activate ? 150 : -150, 1f).SetDelay(.35f).SetEase(Ease.InOutBack)
            .OnComplete(() => mission1.gameObject.SetActive(activate));
        mission1.isOn = activate;
        mission1.transform.GetChild(1).GetComponent<Text>().text = description;
    }
}

[Serializable]
public class Mission
{
    public int missionID;
    public string missionDescription;
    public bool isCompleted;

    public Mission(int id,string description)
    {
        missionID = id;
        missionDescription=description;
        isCompleted = false;
    }
  

    public void CompleteMission()
    {
        isCompleted = true;
    }
}