using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class MissionCompleted : MonoBehaviour
{
    public ObjectType requiredObjectType;
    public PlayerPickAndDrop player;
    public int missionID;
    public bool missionCompleted = false;
    public int requiredObjectCount;
    public int objectCount;
    [SerializeField] private GameObject anotherOfferPanel;
    [SerializeField] private TextMeshProUGUI anotherOfferInfoText;
    [SerializeField] private CameraController mouseLook;
   [SerializeField] private GameObject wantToSellPanel;
   [SerializeField] private GameObject npcAcceptedOfferPanel;

    public TMP_InputField offerInputField;
    public TMP_Text acceptanceText;
    [SerializeField] private int npcTradeDeclineDelay = 20;
    [SerializeField] private float npcPrice = 15.0f;

    private float minAnotherOfferPrice = 1f;
    private float maxAnotherOfferPrice = 10f;
    private bool canItSell = true;
    private int offerDeclineCount = 0;
    private int offerPrice;
    private Collider collider;
    [SerializeField] private Collider behindNpcCollider;
    
    [System.Serializable]
    public class ProbabilityRange
    {
        public float minValue;
        public float maxValue;
        public float acceptanceProbability;
    }

    public List<ProbabilityRange> acceptanceProbabilities = new List<ProbabilityRange>();


    private void Start()
    {
        collider = this.GetComponent<Collider>();
        offerInputField.onValueChanged.AddListener(ValidateInput);
    }

    private void ValidateInput(string input)
    {
        Regex regex = new Regex("^[0-9]*$");
        if (!regex.IsMatch(input))
        {
            offerInputField.text = Regex.Replace(input, "[^0-9]", "");
        }
    }
    private float CalculateAcceptanceProbability(float _offerPrice)
    {
        foreach (var range in acceptanceProbabilities)
        {
            if (_offerPrice >= range.minValue && _offerPrice <= range.maxValue)
            {
                return range.acceptanceProbability;
            }
        }
        return 0;
    }
 
    public void SubmitOffer()
    {
        float submitOfferPrice;

        if (float.TryParse(offerInputField.text, out submitOfferPrice))

        {
            if (submitOfferPrice > npcPrice * 2)
            {
                acceptanceText.text = "Degerinden fazla girdin!";
                return;
            }

            float acceptanceProbability = 0f;
            var random = Random.value;

            if (player != null && player.inHandObjType == requiredObjectType)
                acceptanceProbability = CalculateAcceptanceProbability(submitOfferPrice);

            Debug.Log($"acceptanceProbability {acceptanceProbability}");
            Debug.Log($"random value= {random}");
            if (random < acceptanceProbability)
            {          
                objectCount++;
                CompletedMission(player);
                mouseLook.enabled = true;
                mouseLook.HideCursor();
                player.GetComponent<PlayerMovement>().enabled = true;
                collider.enabled = false;
                behindNpcCollider.enabled = true;
                //acceptanceText.text = "Anlastik!";
                
                NpcAcceptedOfferPanelAnim();
                ClosePanelWithFade(offerInputField.gameObject, 1.5f);
            }
            else
            {
                acceptanceText.text = "Fiyati begenmedim!";
            }
        }
        else
        {
            acceptanceText.text = "";
        }
    }
    private void NpcAcceptedOfferPanelAnim()
    {
        npcAcceptedOfferPanel.SetActive(true);
        DOVirtual.DelayedCall(1.5f,()=> npcAcceptedOfferPanel.SetActive(false));
    }
    public void WantToSellAcceptButton()
    {
       
        ClosePanelWithFade(wantToSellPanel, 1.5f);
        OpenPanelWithFade(offerInputField.gameObject);
        mouseLook.ShowCursor();
        mouseLook.enabled = false;
       // player.GetComponent<PlayerMovement>().enabled = false;
        player.enabled = false;
    }
    public void WantToSellDeclineButton()
    {
        ClosePanelWithFade(wantToSellPanel, 1.5f);
        mouseLook.enabled = true;
        mouseLook.HideCursor();
        player.enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        SaleInteract.IsSaleNow = false;
    }
    public void AnotherOfferDecline()
    {
        StartCoroutine(AnotherOfferDelay(npcTradeDeclineDelay));
        ClosePanelWithFade(anotherOfferPanel,.5f);
        WantToSellDeclineButton();
    }


    
    public void AnotherOfferAccept()
    {
        if (player.inHandObjType != ObjectType.Null && player.currentObjectGrabbling != null)
            Destroy(player.currentObjectGrabbling.gameObject);

        player.inHandObjType = ObjectType.Null;
        player.InHand = false;
        player.currentObjectGrabbling = null;
        ClosePanelWithFade(anotherOfferPanel, 1.5f);
        mouseLook.enabled = true;
        mouseLook.HideCursor();
        player.GetComponent<PlayerMovement>().enabled = true;
        PlayerMoneyManager.Instance.AddMoney(offerPrice);
        collider.enabled = false;
        behindNpcCollider.enabled = true;
        
    }
    public IEnumerator AnotherOffer(float offerDuration = 1f)
    {
        if (!player.InHand)
        {
            acceptanceText.text = "You have no object in your hand!";
            yield break;
        }
        if(!canItSell) yield break;
        offerPrice = (int)Random.Range(minAnotherOfferPrice, maxAnotherOfferPrice);
        anotherOfferInfoText.text = $"Your object is not {requiredObjectType}! I give you {offerPrice}";

        OpenPanelWithFade(anotherOfferPanel);
        mouseLook.enabled = false;
        player.GetComponent<PlayerMovement>().enabled = false;

    }
 
    
    private IEnumerator AnotherOfferDelay(int delay)
    {
        offerDeclineCount++;
        
        if (offerDeclineCount == 3)
        {
            canItSell = false;
            yield return new WaitForSeconds(delay);
            canItSell = true;
            offerDeclineCount = 0;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //Interact();
    }

    public void Interact()
    {
        if (player != null)
        {
            player.enabled = false;
            if (requiredObjectType == player.inHandObjType)
            {
                // offerInputField.onEndEdit.AddListener(delegate { this.SubmitOffer(); });
                offerInputField.text = "";
                OpenPanelWithFade(wantToSellPanel);
                //offerInputField.gameObject.SetActive(true); satmak ister misini aciyoz
                mouseLook.ShowCursor();
                //player.GetComponent<PlayerMovement>().enabled = false;
                mouseLook.enabled = false;
            }
            else
                StartCoroutine(AnotherOffer());
        }
        else
        {
            Debug.Log("Mission already completed.");
        }
    }

    private void OpenPanelWithFade(GameObject panel)
    {
        panel.transform.localScale = Vector3.one;
        panel.SetActive(true);
    }
    private void ClosePanelWithFade(GameObject panel, float duration)
    {
        panel.transform.DOScale(Vector3.zero, duration).From(Vector3.one)
            .OnComplete(() => panel.SetActive(false));

    }


/*
    private void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            ClosePanelWithFade(wantToSellPanel, 1.5f);
            ClosePanelWithFade(offerInputField.gameObject, 1.5f);
            player.enabled = true;
            mouseLook.HideCursor();
            player.GetComponent<PlayerMovement>().enabled = true;
            mouseLook.enabled = true;
            //offerInputField.onEndEdit.RemoveAllListeners();

            acceptanceText.text = "";
        }
    }
*/
    public void CompletedMission(PlayerPickAndDrop player)
    {
        if (player != null && player.currentObjectGrabbling != null)
        {
            Destroy(player.currentObjectGrabbling.gameObject);
            player.inHandObjType = ObjectType.Null;
            player.InHand = false;
            player.currentObjectGrabbling = null;
            player.GetComponent<PlayerPickAndDrop>().enabled = true;
            if (objectCount == requiredObjectCount)
            {
                FindObjectOfType<MissionManager>().CompleteMission(missionID);
                //missionCompleted = true;
                Debug.Log("COMPLETED MISSION");
            }
        }
    }
}