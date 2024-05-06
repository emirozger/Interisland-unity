using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
    [SerializeField] private Transform npcTransform;
    [SerializeField] private Text toggleText;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask npcLayerMask;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private DialogueManager dialogueManager;
    private Highlight npcHighlight;

    [Min(1)]public int missionID;

    private RaycastHit hit;
    private bool hasSpoken = false;

    private void Start()
    {
        npcHighlight = GetComponent<Highlight>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }
    private void Update()
    {
        if (hit.collider !=null)
            npcHighlight.ToggleHighlight(false);
        
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,out hit, 5f, npcLayerMask))
        {
            if (hit.collider == null) return;
            npcHighlight.ToggleHighlight(true);
            if (!Input.GetKeyDown(KeyCode.E)) return;
            npcHighlight.ToggleHighlight(false);
            dialogueManager?.DialogueStart(dialogueStrings,npcTransform);
            hasSpoken = true;
            animator.SetTrigger("Talking");
            dialogueManager.instantiateTransform = this.transform;

        }
    }
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