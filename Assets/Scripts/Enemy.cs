using Unity.Mathematics;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Enemy : MonoBehaviour
{
    System.Random rand = new System.Random();
    Rigidbody2D rb;

    public Vector2 startPos;
    public Vector2 direction;

    public Vector2 target;
    public Vector2 targetDist;
    public Vector2 targetDir;

    public int moveTimer = 0;
    public float noise = 0;

    public AudioClip passiveSound;

    // Get Component
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // Analyze noise and move enemy
    void FixedUpdate()
    {
        
        if (moveTimer <= 0)
        {
            direction = new Vector2(Mathf.Clamp(rand.Next(-1, 1) + direction.x, -4f, 4f), Mathf.Clamp(rand.Next(-1, 1) + direction.y, -2f, 2f));
            moveTimer = rand.Next(10, 40);
        }
        
        if (noise > 0)
        {
            targetDist = target - (Vector2)transform.position;
            targetDir = targetDist.normalized * (noise / (targetDist.magnitude*0.1f) );
            direction = Vector2.Lerp( direction, targetDir, noise / (targetDist.magnitude*0.1f) );
            direction = new Vector2(Mathf.Clamp(direction.x, -8f, 8f), Mathf.Clamp(direction.y, -3f, 3f));
            noise = Mathf.Max( 0, noise - Time.deltaTime );
        }
        else
        {
            noise = 0;
        }

        rb.MovePosition( rb.position + (direction * Time.deltaTime) );

        moveTimer--;
    }

    // Called when player makes noise
    public void Noise(Vector2 source, int volume)
    {
        target = source;
        targetDir = (target - (Vector2)transform.position);
        noise = Mathf.Max(0, volume / targetDir.magnitude);
    }

    // Bounce off borders
    void OnCollisionEnter2D(Collision2D collision)
    {
        direction = -direction;
    }
}
