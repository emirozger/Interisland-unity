using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private GameObject npcInteractPanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float turnSpeed = 2f;

    private List<DialogueString> dialogueList;
    private bool optionSelected = false;
    [Header("Player")] [SerializeField] private PlayerMovement fpsController;
    [SerializeField] private CameraController mouseLook;

    private int currentDialogueIndex = 0;
    [SerializeField] private Toggle newMission;

    public GameObject instantiatePrefab;
    public Transform instantiateTransform;

    public bool isSpokeNow;

    private void Start()
    {
        dialogueParent.SetActive(false);
        playerCamera = Camera.main;
        fpsController = this.GetComponent<PlayerMovement>();
    }

    [SerializeField] private LayerMask npcLayerMask;
    [SerializeField] private Camera playerCamera;
    private RaycastHit hit;

    private void Update()
    {
        if (UIAnimationController.Instance.IsGamePaused)
            return;
        
        if (hit.collider != null)
        {
            CloseInteractVisuals();
        }

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 5f))
        {
            if (hit.collider == null) return;
            if (hit.collider.CompareTag("NPC") == false) return;
           
            var dialogueTrigger = hit.collider.GetComponent<DialogueTrigger>();
            if (Input.GetKeyDown(KeyCode.Space) && dialogueTrigger != null && dialogueTrigger.hasSpoken)
                isSpacePressed = true;
            if (dialogueTrigger.hasSpoken) return;
            OpenInteractVisuals();
            if (!Input.GetKeyDown(KeyCode.E)) return;
            CloseInteractVisuals();
            DialogueStart(dialogueTrigger.dialogueStrings, dialogueTrigger.npcTransform); //++
            dialogueTrigger.hasSpoken = true; //++
            dialogueTrigger.animator.SetTrigger("Talking"); //++
            instantiateTransform = dialogueTrigger.transform; //++
        }
    }

    private void CloseInteractVisuals()
    {
        hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
        npcInteractPanel.SetActive(false);
    }

    private void OpenInteractVisuals()
    {
        hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
        npcInteractPanel.SetActive(true);
    }

    public void InstantiateNPCPrefab(GameObject npcPrefab)
    {
        Instantiate(npcPrefab, instantiateTransform.position, instantiateTransform.rotation);
    }

    public void InstantiateObj(GameObject obj)
    {
        var newPos = new Vector3(instantiateTransform.position.x, instantiateTransform.position.y + .5f,
            instantiateTransform.position.z - 4f);
        Instantiate(obj, newPos, Quaternion.identity);
    }

    public void CameraShake()
    {
        playerCamera.transform.DOShakeRotation(.5f, new Vector3(5, 5, 5), 100, 50);
    }

    public void DialogueStart(List<DialogueString> dialogueStrings, Transform npcTransform)
    {
        isSpokeNow = true;
        AudioManager.Instance.Stop("Walk");
        dialogueParent.SetActive(true);
        fpsController.enabled = false;
        mouseLook.enabled = false;
        mouseLook.ShowCursor();

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

    private void EnableButtons(string text1, string text2)
    {
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);

        option1Button.interactable = true;
        option2Button.interactable = true;

        option1Button.GetComponentInChildren<TMP_Text?>().text = text1;
        option2Button.GetComponentInChildren<TMP_Text?>().text = text2;
    }

    private IEnumerator PrintDialogue()
    {
        while (currentDialogueIndex < dialogueList.Count)
        {
            
            AudioManager.Instance.PlayOneShot("NPC Talk");
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
            isSpacePressed = false;
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

    private bool isSpacePressed = false;

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            if (isSpacePressed)
            {
                yield return new WaitForSeconds(-1);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }
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
        playerCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        fpsController.enabled = true;
        mouseLook.HideCursor();
        isSpokeNow = false;
    }

    private IEnumerator TurnCameraTowardsNPC(Transform npcTransform)
    {
        Quaternion startRotation = playerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(npcTransform.position - playerCamera.transform.position);

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            playerCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * turnSpeed;
            yield return null;
        }

        playerCamera.transform.rotation = targetRotation;
    }
}