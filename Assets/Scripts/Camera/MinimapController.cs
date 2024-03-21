using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField]private Transform player;

    private void LateUpdate()
    {
        var newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        
        transform.rotation=Quaternion.Euler(90f,player.eulerAngles.y,0f);
    }
}
