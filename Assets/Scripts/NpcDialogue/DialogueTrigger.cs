using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
    [SerializeField] private Transform npcTransform;
    private bool hasSpoken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger");
            other.gameObject.GetComponentInParent<DialogueManager>().DialogueStart(dialogueStrings,npcTransform);
            hasSpoken = true;
        }
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