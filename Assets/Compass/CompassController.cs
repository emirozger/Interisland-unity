using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    [SerializeField] private RawImage CompassImage;
    [SerializeField] private Transform orientation;
    [SerializeField] private TextMeshProUGUI CompassDirectionText;

    public void Update()
    {
        CompassImage.uvRect = new Rect(orientation.localEulerAngles.y / 360, 0, 1, 1);
        Vector3 forward = orientation.transform.forward;
        forward.y = 0;

        float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
        headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5));
        int displayangle;
        displayangle = Mathf.RoundToInt(headingAngle);

        switch (displayangle)
        {
            case 0:
                CompassDirectionText.text = "N";
                break;
            case 360:
                CompassDirectionText.text = "N";
                break;
            case 45:
                CompassDirectionText.text = "NE";
                break;
            case 90:
                CompassDirectionText.text = "E";
                break;
            case 130:
                CompassDirectionText.text = "SE";
                break;
            case 180:
                CompassDirectionText.text = "S";
                break;
            case 225:
                CompassDirectionText.text = "SW";
                break;
            case 270:
                CompassDirectionText.text = "W";
                break;
            default:
                CompassDirectionText.text = $"{headingAngle}°";
                break;
        }
    }
}