using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MissionCompleted : MonoBehaviour
{
    public ObjectType requiredObjectType;
    public PlayerPickAndDrop pickAndDrop;
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
                acceptanceText.DOFade(1, .2f);
                acceptanceText.text = "Degerinden fazla girdin!";
                acceptanceText.DOFade(0, 1f).SetDelay(.5f);
                return;
            }

            float acceptanceProbability = 0f;
            var random = Random.value;

            if (pickAndDrop != null && pickAndDrop.inHandObjType == requiredObjectType)
                acceptanceProbability = CalculateAcceptanceProbability(submitOfferPrice);

            Debug.Log($"acceptanceProbability {acceptanceProbability}");
            Debug.Log($"random value= {random}");
            if (random < acceptanceProbability)
            {
                objectCount++;
                CompletedMission(pickAndDrop);
                mouseLook.enabled = true;
                mouseLook.HideCursor();
                pickAndDrop.GetComponent<PlayerMovement>().enabled = true;
                // TODO : collider.enabled = false;
                behindNpcCollider.enabled = true;
                //acceptanceText.text = "Anlastik!";
                AudioManager.Instance.PlayOneShot("Offer Yes");
                PlayerMoneyManager.Instance.AddMoney((int)submitOfferPrice);
                SaleInteract.Instance.SetSaleInteractPanelActive(false);
                SaleInteract.Instance.enabled = false;
                NpcAcceptedOfferPanelAnim();
                pickAndDrop.CloseAllInteractionPanels();
                ClosePanelWithFade(offerInputField.transform.parent.gameObject);
            }
            else
            {
                acceptanceText.text = string.Empty;
                Camera.main.DOShakeRotation(.2f,10,10,90);
                AudioManager.Instance.PlayOneShot("Offer No");
                acceptanceText.DOFade(1f, .2f);
                acceptanceText.DOText("Anlaşamadık!", 1.5f).OnComplete(() => acceptanceText.DOFade(0f, .2f));
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
        DOVirtual.DelayedCall(1.5f, () => npcAcceptedOfferPanel.SetActive(false));
        SaleInteract.Instance.SetSaleInteractPanelActive(false);
        SaleInteract.Instance.enabled = true;
    }

    public void WantToSellAcceptButton()
    {
        ClosePanelWithFade(wantToSellPanel);
        OpenPanelWithFade(offerInputField.transform.parent.gameObject);
        mouseLook.ShowCursor();
        mouseLook.enabled = false;
        // player.GetComponent<PlayerMovement>().enabled = false;
        pickAndDrop.enabled = false;
        SaleInteract.Instance.enabled = false;
    }

    public void WantToSellDeclineButton()
    {
        ClosePanelWithFade(wantToSellPanel);
        mouseLook.enabled = true;
        mouseLook.HideCursor();
        pickAndDrop.enabled = true;
        pickAndDrop.GetComponent<PlayerMovement>().enabled = true;
        SaleInteract.IsSaleNow = false;
        DOVirtual.DelayedCall(1.5f, () => SaleInteract.Instance.enabled = true);
    }

    public void AnotherOfferDecline()
    {
        StartCoroutine(AnotherOfferDelay(npcTradeDeclineDelay));
        ClosePanelWithFade(anotherOfferPanel);
        WantToSellDeclineButton();
        //DOVirtual.DelayedCall(1.5f, () => SaleInteract.Instance.enabled = true);
    }


    public void AnotherOfferAccept()
    {
        if (pickAndDrop.inHandObjType != ObjectType.Hicbirsey && pickAndDrop.currentObjectGrabbling != null)
        {
            var obj = pickAndDrop.currentObjectGrabbling.gameObject;
            obj.transform.DOShakeScale(.45f, 10f, 10, 90, true)
                .OnComplete((() => Destroy(obj)));
            AudioManager.Instance.PlayOneShot("Another Offer");
            pickAndDrop.inHandObjType = ObjectType.Hicbirsey;
            pickAndDrop.InHand = false;
            pickAndDrop.currentObjectGrabbling = null;
            ClosePanelWithFade(anotherOfferPanel);
            mouseLook.enabled = true;
            mouseLook.HideCursor();
            pickAndDrop.GetComponent<PlayerMovement>().enabled = true;
            pickAndDrop.enabled = true;
            PlayerMoneyManager.Instance.AddMoney(offerPrice);
            //TODO: collider.enabled = false;
            behindNpcCollider.enabled = true;
            pickAndDrop.CloseAllInteractionPanels();
            SaleInteract.Instance.SetSaleInteractPanelActive(false);
            SaleInteract.Instance.enabled = true;
        }
    }

    public IEnumerator AnotherOffer(float offerDuration = 1f)
    {
        if (!pickAndDrop.InHand)
        {
            acceptanceText.text = "You have no object in your hand!";
            yield break;
        }

        if (!canItSell) yield break;
        offerPrice = (int)Random.Range(minAnotherOfferPrice, maxAnotherOfferPrice);
        anotherOfferInfoText.text =
            $"Senin urunun {requiredObjectType} degil! Sana {offerPrice} $ kadar teklifim var, kabul eder misin?" +
            $"(3 defa red cevabi verirsen 20 saniye sonra tekrar teklif yaparim)";

        OpenPanelWithFade(anotherOfferPanel);
        mouseLook.enabled = false;
        pickAndDrop.GetComponent<PlayerMovement>().enabled = false;
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
        if (pickAndDrop != null)
        {
            SaleInteract.Instance.enabled = false;
            SaleInteract.Instance.SetSaleInteractPanelActive(false);
            pickAndDrop.enabled = false;
            AudioManager.Instance.Stop("Walk");
            if (requiredObjectType == pickAndDrop.inHandObjType)
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

    private void ClosePanelWithFade(GameObject panel)
    {
        panel.SetActive(false);
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
            player.inHandObjType = ObjectType.Hicbirsey;
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