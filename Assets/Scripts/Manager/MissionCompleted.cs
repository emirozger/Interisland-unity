using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
   

    public TMP_InputField offerInputField;
    public TMP_Text acceptanceText;
    [SerializeField] private int npcTradeDeclineDelay = 20;
    [SerializeField] private float npcPrice = 15.0f;

    private float minAnotherOfferPrice = 1f;
    private float maxAnotherOfferPrice = 10f;
    private bool canItSell = true;
    private int offerDeclineCount = 0;
    
    [System.Serializable]
    public class ProbabilityRange
    {
        public float minValue;
        public float maxValue;
        public float acceptanceProbability;
    }

    public List<ProbabilityRange> acceptanceProbabilities = new List<ProbabilityRange>();

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

    public void AnotherOfferDecline()
    {
        StartCoroutine(AnotherOfferDelay(npcTradeDeclineDelay));
        anotherOfferPanel.SetActive(false);
        mouseLook.enabled = true;
    }

    private int offerPrice;
    
    public void AnotherOfferAccept()
    {
        if (player.inHandObjType != ObjectType.Null && player.currentObjectGrabbling != null)
            Destroy(player.currentObjectGrabbling.gameObject);

        player.inHandObjType = ObjectType.Null;
        player.InHand = false;
        player.currentObjectGrabbling = null;
        anotherOfferPanel.SetActive(false);
        mouseLook.enabled = true;
        PlayerMoneyManager.Instance.AddMoney(offerPrice);
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
            player.GetComponent<PlayerPickAndDrop>().enabled = false;
            if (requiredObjectType == player.inHandObjType)
            {
                // offerInputField.onEndEdit.AddListener(delegate { this.SubmitOffer(); });
                offerInputField.text = "";
                offerInputField.gameObject.SetActive(true);
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
            player.GetComponent<PlayerPickAndDrop>().enabled = true;
            offerInputField.gameObject.SetActive(false);

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