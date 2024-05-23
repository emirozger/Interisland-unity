using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public List<DialogueString> dialogueStrings = new List<DialogueString>();
    public Transform npcTransform;
    [SerializeField] private Text toggleText;
    public Animator animator;
    public Highlight npcHighlight;

    [Min(1)]public int missionID;
    
    public bool hasSpoken = false;  

    private void Start() => npcHighlight = GetComponent<Highlight>();
    
    
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger");
            if (Input.GetKeyDown(KeyCode.E)) {

                other.gameObject.GetComponentInParent<DialogueManager>().DialogueStart(dialogueStrings, npcTransform);
                hasSpoken = true;
                animator.SetTrigger("Talking");
                other.gameObject.GetComponentInParent<DialogueManager>().instantiateTransform = this.transform; //new npc spawned pos
            }
          
        }
    }
    */
    public void AssignMission(string description)
    {
        MissionManager.Instance.missions[missionID - 1].missionDescription = description;
        toggleText.text = description;
    }
}

[Serializable]
public class DialogueString
{
    public string text; //npc says
    public bool isEnd;

    [Header("Branch")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("Events")]
    public UnityEvent OnStartDialogue;
    public UnityEvent OnEndDialogue;
}