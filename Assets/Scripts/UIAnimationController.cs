using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;



public class UIAnimationController : MonoBehaviour
{

    [Header("Settings")] public LoopType loopRidderFlipper;
    public LoopType GoldFrameLoop;
    public float pauseBGFillEnd, pauseBG2FillEnd;
    public float pauseBGFillEndTime, pauseBG2FillEndTime;
    public float fillSpeed = 1f;
    public float holdDurationForMaxFill = 3f;

    [Header("References")] [SerializeField]
    private Transform shipTeleportTransform;

    public Image ridder;
    public Image[] PauseBG;
    public GameObject PausePanel;
    public GameObject[] PauseButtons;
    public Transform InventoryImage;
    public Button InventoryButton;
    public Transform QuestImage;
    public Button QuestButton;
    public Image backToShipImage;
    public Image AnimRidder;
    public Image AnimPaper;
    public Image AnimLeft;
    public Image AnimRight;
    public Image[] AnimFireworks;
    public TextMeshProUGUI NewIslandName;
    public TextMeshProUGUI AnimText;
    public GameObject newIslandAnimPanel;
    public Image soundSlider;
    public Image SoundSlider0;
    public Image SoundSlider1;

    [Header("Ease Settings")] public Ease RidderAnimEase;
    public Ease ridderEase;
    public Ease FireworksEase;
    public Ease NewIslandEase;
    public Ease ScreenUIEase;
    public Ease AnimPanelEase;
    public Ease PauseButtonsEase;

    private bool isFilling;
    private float holdStartTime;
    private bool isGamePaused = false;

    private Tween inventoryTween;
    private Tween questTween;


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            NewIslandAnim();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {


            if (isGamePaused)
                ClosePauseMenu();
            else
                OpenPauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.I))
            Inventory();

        if (Input.GetKeyDown(KeyCode.T))
            Quest();

        BackToShip();
    }

    public void ShowSlider()
    {
        soundSlider.DOFade(1f, 1.2f).From(0).SetEase(PauseButtonsEase);
        SoundSlider0.DOFade(1f, 1.3f).From(0).SetEase(PauseButtonsEase);
        SoundSlider1.DOFade(1f, 1.2f).From(0).SetEase(PauseButtonsEase);
    }

    [SerializeField] private GameObject gameplayUI, interactionUI;
    [SerializeField] private Image blackImage;
    [SerializeField] private CameraController mouseLook;
    public void OpenPauseMenu()
    {
        mouseLook.enabled = false;
        mouseLook.ShowCursor();
        blackImage.DOFade(.85f, 0.01f);
        gameplayUI.SetActive(false);
        interactionUI.SetActive(false);
        PausePanel.transform.DOScale(new Vector3(1, 1, 0), 0f);
        PausePanel.SetActive(true);
        ridder.transform.DORotate(new Vector3(0, 0, -180), 3).SetLoops(-1, loopRidderFlipper);
        ridder.transform.DOScale(1f, .4f).From(1.75f).SetEase(ridderEase);
        ridder.DOFade(1f, .6f).From(0).SetEase(ridderEase);

        foreach (var image in PauseBG)
        {
            image.DOFillAmount(pauseBGFillEnd, pauseBGFillEndTime).From(0);
        }

        PauseBG.ElementAt(2).transform.DOScaleX(1f, 0.4f).From(2f);

        foreach (var button in PauseButtons)
        {
            button.GetComponent<Button>().image.DOFade(1f, 1f).From(0).SetEase(PauseButtonsEase);
            button.GetComponentInChildren<TextMeshProUGUI>().DOFade(1f, 1f).From(0).SetEase(PauseButtonsEase);
        }

        ShowSlider();
        isGamePaused = true;
        
    }


    public void ClosePauseMenu()
    {
        mouseLook.enabled = true;
        mouseLook.HideCursor();
        blackImage.DOFade(0f, .01f);
        PausePanel.transform.DOScale(new Vector3(0, 0, 1), 0.5f)
            .OnComplete(() => PausePanel.SetActive(false));
        isGamePaused = false;  
        gameplayUI.SetActive(true);
        interactionUI.SetActive(true);
    }

    public void NewIslandAnim()
    {
        newIslandAnimPanel.SetActive(true);
        AnimRidder.transform.DORotate(new Vector3(0, 0, -180), 3);
        AnimRidder.rectTransform.DOScale(Vector3.one, 1f).From(Vector3.zero);
        AnimPaper.rectTransform.DOScaleX(1f, 1f).From(0f);
        AnimLeft.rectTransform.DOLocalMoveX(-287, 1f).From(27);
        AnimRight.rectTransform.DOLocalMoveX(358f, 1.1f).From(68);
       

        for (int i = 0; i < AnimFireworks.Length; i++)
        {
            float random = Random.Range(1.25f, 2f);
            AnimFireworks[i].rectTransform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetDelay(random).SetLoops(10);
            AnimFireworks[i].DOFade(0, 1).From(1).SetDelay(random).SetLoops(10)
                .OnComplete(() => AnimFireworks[i].DOKill());
        }

        AnimText.DOFade(1, 1).From(0);
        NewIslandName.DOFade(1, 2).From(0);
        NewIslandName.rectTransform.DOScale(Vector3.one, 1f).From(2.2f).SetEase(NewIslandEase);
        newIslandAnimPanel.transform.DOScale(0f, 1f).SetDelay(3f);

        DOVirtual.DelayedCall(3f, () => 
        {
            //newIslandAnimPanel.transform.DOScale(0f, 1f);
           // .OnComplete(() => newIslandAnimPanel.SetActive(false));
            //.OnComplete(() => newIslandAnimPanel.transform.DOScale(1f, 1f));
      
        });
    }

    public void Inventory()
    {

        if (inventoryTween != null && inventoryTween.IsActive())
        {
            return;
        }

        if (InventoryImage.localScale == Vector3.one)
        {
            inventoryTween = InventoryImage.DOScale(Vector3.zero, 0.5f).From(Vector3.one)
                .OnComplete(() => inventoryTween = null);
        }
        else
        {
            inventoryTween = InventoryImage.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(ScreenUIEase)
                .OnComplete(() => inventoryTween = null);
        }
    }

    public void Quest()
    {

        if (questTween != null && questTween.IsActive())
            return;

        if (QuestImage.localScale == Vector3.one)
            questTween = QuestImage.DOScale(Vector3.zero, 0.5f).From(Vector3.one).OnComplete(() => questTween = null);

        else
            questTween = QuestImage.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(ScreenUIEase)
                .OnComplete(() => questTween = null);
    }

    public float requiredHoldTime = 3f;
    private float fillStartTime;


    private void BackToShip()
    {
        backToShipImage.fillAmount = 0f;

        // Holding 'P' key
        if (Input.GetKey(KeyCode.P))
        {
            isFilling = true;
        }
        // Releasing 'P' key
        else if (Input.GetKeyUp(KeyCode.P))
        {
            isFilling = false;
        }

        if (isFilling)
        {
            fillStartTime += Time.deltaTime;
            print(fillStartTime);

            backToShipImage.DOFillAmount(1f, 1f);
            if (fillStartTime >= 1f)
            {
                PlayerMovement.Instance.TeleportToShip(shipTeleportTransform);
                backToShipImage.fillAmount = 0f;
                fillStartTime = 0f;
            }
        }
        else
        {
            fillStartTime = 0f;
            backToShipImage.DOFillAmount(0f, fillSpeed);
        }
    }

}
