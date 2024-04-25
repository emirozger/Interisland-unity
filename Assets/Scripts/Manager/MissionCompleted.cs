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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public TMP_InputField offerInputField;
    public TMP_Text acceptanceText;



    [SerializeField] private float npcPrice = 15.0f;

    private readonly float[] minOfferPrices = { 1f, 2f, 5f, 1f }; // Minimum offer price for each object type
    private readonly float[] maxOfferPrices = { 5f, 7f, 10f, 6f }; // Maximum offer price for each object type

    [System.Serializable]
    public class ProbabilityRange
    {
        public float minValue;
        public float maxValue;
        public float acceptanceProbability;
    }

    public List<ProbabilityRange> acceptanceProbabilities = new List<ProbabilityRange>();

    private float CalculateAcceptanceProbability(float offerPrice)
    {
        foreach (var range in acceptanceProbabilities)
        {
            if (offerPrice >= range.minValue && offerPrice <= range.maxValue)
            {
                return range.acceptanceProbability;
            }
        }
        return 0;
    }

    public void SubmitOffer()
    {
        float offerPrice;

        if (float.TryParse(offerInputField.text, out offerPrice))

        {
            if (offerPrice > npcPrice * 2)
            {
                acceptanceText.text = "Invalid offer price!";
                return;
            }

            float acceptanceProbability = 0f;
            var random = Random.value;

            if (player != null && player.inHandObjType == requiredObjectType)
                acceptanceProbability = CalculateAcceptanceProbability(offerPrice);

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

    public void AnotherOfferAccept()
    {
        if (player.inHandObjType != ObjectType.Null && player.currentObjectGrabbling != null)
            Destroy(player.currentObjectGrabbling.gameObject);

        player.inHandObjType = ObjectType.Null;
        player.InHand = false;
        player.currentObjectGrabbling = null;
        anotherOfferPanel.SetActive(false);
        mouseLook.enabled = true;
        
    }
    public void AnotherOfferDecline()
    {
        anotherOfferPanel.SetActive(false);
        mouseLook.enabled = true;
    }
    public IEnumerator AnotherOffer(float offerDuration = 1f)
    {
        if (!player.InHand)
        {
            acceptanceText.text = "You have no object in your hand!";
            yield break;
        }
        int offerPrice = (int)Random.Range(minOfferPrices[(int)requiredObjectType], maxOfferPrices[(int)requiredObjectType]);
        anotherOfferInfoText.text = $"Your object is not {requiredObjectType}! I give you {offerPrice}";
        anotherOfferPanel.SetActive(true);
        mouseLook.enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null)
        {
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
            if (objectCount == requiredObjectCount)
            {
                FindObjectOfType<MissionManager>().CompleteMission(missionID);
                //missionCompleted = true;
                Debug.Log("COMPLETED MISSION");
            }
        }
    }
}