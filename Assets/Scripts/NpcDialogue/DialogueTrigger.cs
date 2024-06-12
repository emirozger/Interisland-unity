using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
    [SerializeField] private TMP_Text npcNameText;

    [Min(1)] public int missionID;

    public bool hasSpoken = false;
    public float blinkDuration = 0.5f;
    public float blinkInterval = .5f;

    private void Start()
    {
        npcHighlight = GetComponent<Highlight>();
        npcNameText = GetComponentInChildren<TMP_Text>();
        
    }

    private void Update()
    {
        if (UIAnimationController.Instance.IsGamePaused)
            return;
        if (npcNameText != null)
        {
            npcNameText.transform.rotation = Quaternion.Slerp(npcNameText.transform.rotation,Camera.main.transform.rotation,Time.deltaTime * 10);
        }
    }

    public void StartBlinking(TMP_Text text)
    {
        text.color = Color.yellow;
        DOTween.Sequence()
            .Append(text.DOFade(0, blinkDuration))
            .AppendInterval(blinkInterval)
            .Append(text.DOFade(1, blinkDuration))
            .AppendInterval(blinkInterval)
            .SetLoops(-1);
    }

    public void StopBlinking(TMP_Text text)
    {
            text.DOKill();
            text.color = Color.white;
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
        MissionManager.Instance.ToggleMissionPanel(true, description);
    }
}

[Serializable]
public class DialogueString
{
    public string text; //npc says
    public bool isEnd;

    [Header("Branch")] public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("Events")] public UnityEvent OnStartDialogue;
    public UnityEvent OnEndDialogue;
}