using UnityEngine;

public class SpeedParticleController : MonoBehaviour
{
    public ParticleSystem ps;
    public Rigidbody playerRb;
    public float speedMultiplier = 2f;

    void Update()
    {
        if (ps == null || playerRb == null) return;

        var main = ps.main;
        float speed = playerRb.linearVelocity.magnitude;

        main.startSpeed = speed * speedMultiplier;
    }
}
