using UnityEngine;

public class MinimapPlayerIcon : MonoBehaviour
{
    public RectTransform mapRect;   // Minimap image rect
    public RectTransform iconRect;  // Player icon rect
    public Transform player;        // Player object

    [Header("World bounds shown on the minimap")]
    public Vector2 worldMin;        // Bottom-left world coordinate
    public Vector2 worldMax;        // Top-right world coordinate

    void Update()
    {
        // Convert world → 0..1 normalized
        float normX = Mathf.InverseLerp(worldMin.x, worldMax.x, player.position.x);
        float normY = Mathf.InverseLerp(worldMin.y, worldMax.y, player.position.z);

        // Convert 0..1 → map UI pixel coordinates
        Vector2 mapSize = mapRect.rect.size;

        float uiX = Mathf.Lerp(-mapSize.x / 2f, mapSize.x / 2f, normX);
        float uiY = Mathf.Lerp(-mapSize.y / 2f, mapSize.y / 2f, normY);

        iconRect.anchoredPosition = new Vector2(uiX, uiY);

        // Optional icon rotation (arrow points same direction as player)
        iconRect.localEulerAngles = new Vector3(0, 0, -player.eulerAngles.y);
    }
}
