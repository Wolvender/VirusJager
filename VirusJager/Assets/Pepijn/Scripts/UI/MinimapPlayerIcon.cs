using UnityEngine;

public class MinimapPlayerIcon : MonoBehaviour
{
    public RectTransform mapRect;   // Minimap background
    public RectTransform iconRect;  // Player icon
    public Transform player;        // Player object

    public Vector2 worldMin;
    public Vector2 worldMax;

    void Update()
    {
        // Normalize world position (0 to 1)
        float normX = Mathf.InverseLerp(worldMin.x, worldMax.x, player.position.x);
        float normY = Mathf.InverseLerp(worldMin.y, worldMax.y, player.position.z);

        // Convert normalized values to UI map coordinates
        Vector2 mapSize = mapRect.sizeDelta;
        float uiX = (normX - 0.5f) * mapSize.x;
        float uiY = (normY - 0.5f) * mapSize.y;

        iconRect.anchoredPosition = new Vector2(uiX, uiY);

        // Optional: rotate with player
        iconRect.localEulerAngles = new Vector3(0, 0, -player.eulerAngles.y);
    }
}
