using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
                acceptanceText.text = "Invalid offer price!";
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
                acceptanceText.text = "NPC accepted the offer!";
                objectCount++;
                CompletedMission(player);
                offerInputField.gameObject.SetActive(false);
                mouseLook.enabled = true;
                mouseLook.HideCursor();
                player.GetComponent<PlayerMovement>().enabled = true;
                collider.enabled = false;
                behindNpcCollider.enabled = true;
                acceptanceText.text = "";
            }
            else
            {
                acceptanceText.text = "NPC rejected the offer.";
            }
        }
        else
        {
            acceptanceText.text = "Invalid offer price!";
        }
    }

    public void WantToSellAcceptButton()
    {
        wantToSellPanel.SetActive(false);
        offerInputField.gameObject.SetActive(true);
        mouseLook.ShowCursor();
        mouseLook.enabled = false;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.enabled = false;
    }
    public void WantToSellDeclineButton()
    {
        wantToSellPanel.SetActive(false);
        mouseLook.enabled = true;
        mouseLook.HideCursor();
        player.enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
    }
    public void AnotherOfferDecline()
    {
        StartCoroutine(AnotherOfferDelay(npcTradeDeclineDelay));
        anotherOfferPanel.SetActive(false);
        WantToSellDeclineButton();
    }


    
    public void AnotherOfferAccept()
    {
        if (player.inHandObjType != ObjectType.Null && player.currentObjectGrabbling != null)
            Destroy(player.currentObjectGrabbling.gameObject);

        player.inHandObjType = ObjectType.Null;
        player.InHand = false;
        player.currentObjectGrabbling = null;
        anotherOfferPanel.SetActive(false);
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
        anotherOfferPanel.SetActive(true);
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
        if (player != null)
        {
            player.enabled = false;
            if (requiredObjectType == player.inHandObjType)
            {
                // offerInputField.onEndEdit.AddListener(delegate { this.SubmitOffer(); });
                offerInputField.text = "";
                wantToSellPanel.SetActive(true);
                //offerInputField.gameObject.SetActive(true); satmak ister misini aciyoz
                mouseLook.ShowCursor();
                player.GetComponent<PlayerMovement>().enabled = false;
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

    private void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            wantToSellPanel.SetActive(false);
            offerInputField.gameObject.SetActive(false);
            player.enabled = true;
            mouseLook.HideCursor();
            player.GetComponent<PlayerMovement>().enabled = true;
            mouseLook.enabled = true;
            //offerInputField.onEndEdit.RemoveAllListeners();

            acceptanceText.text = "";
        }
    }

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