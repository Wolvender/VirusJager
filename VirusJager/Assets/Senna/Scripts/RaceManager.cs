using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public Transform finishWaypoint;
    [Header("10 Position UI Images (stack them in same position!)")]
    public Image[] positionImages = new Image[10];  // ← Drag your 10 UI Images here!

    [Header("Player")]
    public GameObject playerObject;

    private Transform playerTransform;
    private List<RBCNavRacer> enemies = new List<RBCNavRacer>();
    private int lastPosition = -1;
    private float logTimer = 0f;

    void Update()
    {
        // ───── PLAYER ─────
        if (playerTransform == null && playerObject != null)
        {
            playerTransform = playerObject.transform;
            Debug.Log("<color=cyan>PLAYER LOCKED IN → " + playerObject.name + "</color>");
        }

        if (playerTransform == null || finishWaypoint == null) return;

        // ───── ENEMIES ─────
        var allRacers = FindObjectsOfType<RBCNavRacer>();
        enemies = allRacers.Where(s => s.gameObject != playerObject).ToList();

        if (enemies.Count == 0) return;

        // ───── CALCULATE POSITION ─────
        float playerDist = Vector3.Distance(playerTransform.position, finishWaypoint.position);
        int playerPosition = 1;  // 1-based (1st place)
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, finishWaypoint.position) < playerDist)
                playerPosition++;
        }

        // ───── TURN SPRITES ON/OFF (MAGIC HAPPENS HERE) ─────
        UpdatePositionSprites(playerPosition);

        // ───── LOG EVERY SECOND ─────
        logTimer += Time.deltaTime;
        if (logTimer >= 1f)
        {
            Debug.Log($"<color=magenta>🥇 {playerPosition}th PLACE   |   {enemies.Count} enemies</color>");
            logTimer = 0f;
        }

        if (playerPosition < lastPosition && lastPosition != -1)
            Debug.Log("<color=green>🚀 OVERTAKE SUCCESS! YOU'RE {playerPosition}th!</color>");

        lastPosition = playerPosition;
    }

    void UpdatePositionSprites(int position)
    {
        // Turn ALL images OFF first
        foreach (var img in positionImages)
            if (img != null) img.gameObject.SetActive(false);

        // Turn the correct position ON
        if (position >= 1 && position <= 10 && positionImages[position - 1] != null)
        {
            positionImages[position - 1].gameObject.SetActive(true);

            // Bonus: Color tint based on position
            Image activeImg = positionImages[position - 1];
            activeImg.color = GetColor(position);
        }
    }

    Color GetColor(int pos) => pos switch
    {
        1 => new Color(1f, 0.8f, 0f),      // Gold glow
        2 => new Color(0.9f, 0.9f, 0.9f),  // Silver shine
        3 => new Color(0.8f, 0.5f, 0.3f),  // Bronze warm
        _ => Color.white                    // Normal for 4th+
    };
}