using UnityEngine;

public class MinimapPlayerIcon : MonoBehaviour
{
    public RectTransform mapRect;       // Your map background image
    public RectTransform iconRect;      // This icon
    public Transform player;            // Your actual car object

    public Vector2 worldMin;            // lowest X/Z in world coords
    public Vector2 worldMax;            // highest X/Z in world coords

    void Update()
    {
        // Normalize world position between 0 and 1
        float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, player.position.x);
        float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, player.position.z);

        // Convert to map UI position
        float mapX = (normalizedX * mapRect.sizeDelta.x) - (mapRect.sizeDelta.x / 2f);
        float mapY = (normalizedY * mapRect.sizeDelta.y) - (mapRect.sizeDelta.y / 2f);

        iconRect.anchoredPosition = new Vector2(mapX, mapY);
    }
}
