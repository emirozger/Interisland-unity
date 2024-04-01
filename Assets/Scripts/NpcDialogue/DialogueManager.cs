using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
  [SerializeField] private GameObject dialogueParent;
  [SerializeField] private TMP_Text dialogueText;
  [SerializeField] private Button option1Button;
  [SerializeField] private Button option2Button;
  [SerializeField] private float typingSpeed = 0.05f;
  [SerializeField] private float turnSpeed = 2f;

  private List<DialogueString> dialogueList;
  private bool optionSelected = false;
  [Header("Player")]
  [SerializeField] private PlayerMovement fpsController;
  [SerializeField] private Transform playerCameraTransform;
  [SerializeField] private CameraController mouseLook;

  private int currentDialogueIndex = 0;
  [SerializeField] private Toggle newMission;

  public GameObject instantiatePrefab;
  public Transform instantiatePos;
  private void Start()
  {
    dialogueParent.SetActive(false);
    playerCameraTransform = Camera.main.transform;
    fpsController = this.GetComponent<PlayerMovement>();
    
  }
  
  public void InstantiateCube()
  {
    Instantiate(instantiatePrefab,instantiatePos.position,Quaternion.identity);
  }

  public void CameraShake()
  {
    playerCameraTransform.DOShakeRotation(.5f, new Vector3(5, 5, 5),100,50);
  }
  
  public void DialogueStart(List<DialogueString> dialogueStrings,Transform npcTransform)
  {
    dialogueParent.SetActive(true);
    fpsController.enabled = false;
    mouseLook.enabled = false;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    StartCoroutine(TurnCameraTowardsNPC(npcTransform));
    dialogueList = dialogueStrings;
    currentDialogueIndex = 0;
    DisableButtons();
    StartCoroutine(PrintDialogue());
  }

 
  private void DisableButtons()
  {
    option1Button.gameObject.SetActive(false);
    option2Button.gameObject.SetActive(false);
    
    option1Button.interactable = false;
    option2Button.interactable = false;

    option1Button.GetComponentInChildren<TMP_Text?>().text = "No Option";
    option2Button.GetComponentInChildren<TMP_Text?>().text = "No Option";
  }
  private void EnableButtons(string text1,string text2)
  {
    option1Button.gameObject.SetActive(true);
    option2Button.gameObject.SetActive(true);
    
    option1Button.interactable = true;
    option2Button.interactable = true;

    option1Button.GetComponentInChildren<TMP_Text?>().text = text1;
    option2Button.GetComponentInChildren<TMP_Text?>().text = text2;
  }
  private IEnumerator PrintDialogue ()
  {
    while (currentDialogueIndex<dialogueList.Count)
    {
      DialogueString line = dialogueList[currentDialogueIndex];
      
      line.OnStartDialogue?.Invoke();
      
      if (line.isQuestion)
      {
        yield return StartCoroutine(TypeText(line.text));
        EnableButtons(line.answerOption1, line.answerOption2);
        option1Button.onClick.AddListener((() => HandleOptionSelected(line.option1IndexJump)));
        option2Button.onClick.AddListener((() => HandleOptionSelected(line.option2IndexJump)));

        yield return new WaitUntil((() => optionSelected));
      }
      else
      {
        yield return StartCoroutine(TypeText(line.text));
      }
      line.OnEndDialogue?.Invoke();
      optionSelected = false;
    }

    DialogueStop();
  }

  private void HandleOptionSelected(int indexJump)
  {
    optionSelected = true;
    DisableButtons();

    currentDialogueIndex = indexJump;
  }

  private IEnumerator TypeText(string text)
  {
    dialogueText.text = "";
    foreach (char letter in text.ToCharArray())
    {
      dialogueText.text += letter;
      yield return new WaitForSeconds(typingSpeed);
      
    }
    if (!dialogueList[currentDialogueIndex].isQuestion)
    {
      //bir parentez sildim.
      yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    
  
    }
    if (dialogueList[currentDialogueIndex].isEnd)
    {
      //text is end ise direkt stop oldugu icin onenddialogu invokelanamıyor.
      DialogueString line = dialogueList[currentDialogueIndex];
      line.OnEndDialogue?.Invoke();
      //
      DialogueStop();
    }

    currentDialogueIndex++;
  }

  private void DialogueStop()
  {
    StopAllCoroutines();
    dialogueText.text = "";
    dialogueParent.SetActive(false);

    mouseLook.enabled = true;
    playerCameraTransform.localRotation = Quaternion.Euler(0,0,0);
    fpsController.enabled = true;
    //Cursor.lockState = CursorLockMode.Locked;
    //Cursor.visible = false;
  }
  private IEnumerator TurnCameraTowardsNPC(Transform npcTransform)
  {
    Quaternion startRotation = playerCameraTransform.rotation;
    Quaternion targetRotation = Quaternion.LookRotation(npcTransform.position - playerCameraTransform.position);

    float elapsedTime = 0f;

    while (elapsedTime<1f)
    {
      playerCameraTransform.rotation=Quaternion.Slerp(startRotation,targetRotation,elapsedTime);
      elapsedTime += Time.deltaTime * turnSpeed;
      yield return null;
    }

    playerCameraTransform.rotation = targetRotation;
  }
}
