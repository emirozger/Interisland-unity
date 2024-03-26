using System.Collections;
using TMPro;
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

    private float CalculateAcceptanceProbability(float offerPrice)
    {
        float acceptanceProbability = 0f;

        if (offerPrice <= npcPrice)
        {
            if (offerPrice <= 5f)
                acceptanceProbability = 1f;
            else if (offerPrice <= 10f)
                acceptanceProbability = 0.99f - (offerPrice - 5f) * 0.01f;
            else if (offerPrice <= 15f)
                acceptanceProbability = 0.95f - (offerPrice - 10f) * 0.05f;
            else
                acceptanceProbability = 0.7f - (offerPrice - 15f) * 0.1f;
        }
        else
        {
            if (offerPrice <= 20f)
                acceptanceProbability = 0.6f - (offerPrice - 16f) * 0.05f;
            else if (offerPrice <= 25f)
                acceptanceProbability = 0.35f - (offerPrice - 21f) * 0.05f;
            else if (offerPrice <= 30f)
                acceptanceProbability = 0.15f - (offerPrice - 26f) * 0.05f;
        }

        return Mathf.Clamp01(acceptanceProbability);
    }

    public void SubmitOffer()
    {
        float offerPrice;
        if (float.TryParse(offerInputField.text, out offerPrice))
        {
            if (offerPrice > 30f)
            {
                acceptanceText.text = "Invalid offer price!";
                return;
            }

            float acceptanceProbability = 0f;

            if (player != null && player.inHandObjType == requiredObjectType)
                acceptanceProbability = CalculateAcceptanceProbability(offerPrice);

            if (Random.value < acceptanceProbability)
            {
                acceptanceText.text = "NPC accepted the offer!";
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

    public IEnumerator AnotherOffer(float offerDuration = 1f)
    {
        if (!player.InHand)
        {
            acceptanceText.text = "You have no object in your hand!";
            yield break;
        }

        int offerPrice = (int)Random.Range(minOfferPrices[(int)requiredObjectType], maxOfferPrices[(int)requiredObjectType]);
        acceptanceText.text = $"Your object is not {requiredObjectType}! I give you {offerPrice}";
        yield return new WaitForSeconds(offerDuration);

        if (player.inHandObjType != ObjectType.Null && player.currentObjectGrabbling != null)
            Destroy(player.currentObjectGrabbling.gameObject);

        player.inHandObjType = ObjectType.Null;
        player.InHand = false;
        player.currentObjectGrabbling = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null && !missionCompleted)
        {
            if (requiredObjectType == player.inHandObjType)
                offerInputField.gameObject.SetActive(true);
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
        if (player != null && !missionCompleted)
        {
            offerInputField.gameObject.SetActive(false);
            acceptanceText.text = "";
        }
    }

    public void CompletedMission(PlayerPickAndDrop player)
    {
        if (objectCount == requiredObjectCount)
        {
            FindObjectOfType<MissionManager>().CompleteMission(missionID);
            missionCompleted = true;
            Debug.Log("COMPLETED MISSION");
        }
        else if (player != null && player.currentObjectGrabbling != null)
        {
            Destroy(player.currentObjectGrabbling.gameObject);
            player.inHandObjType = ObjectType.Null;
            player.InHand = false;
            player.currentObjectGrabbling = null;
            objectCount++;
        }
    }
}