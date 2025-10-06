using Unity.Mathematics;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Enemy : MonoBehaviour
{
    System.Random rand = new System.Random();
    Rigidbody2D rb;

    public Vector2 direction;
    public Vector2 target;
    public Vector2 targetDir;

    public int moveTimer = 0;
    public float noise = 0;

    // Get Component
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Analyze noise and move enemy
    void FixedUpdate()
    {
        
        if (moveTimer <= 0)
        {
            direction = new Vector2(Mathf.Clamp(rand.Next(-1, 1) + direction.x, -2f, 2f), Mathf.Clamp(rand.Next(-1, 1) + direction.y, -1f, 1f));
            moveTimer = rand.Next(10, 40);
        }
        
        if (noise > 0)
        {
            targetDir = ( target - (Vector2)transform.position ).normalized * noise;
            direction = Vector2.Lerp( direction, targetDir, noise / 10 );
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
    public void Noise(Vector2 source, int noise)
    {
        target = source;
        targetDir = (target - (Vector2)transform.position);
        this.noise = Mathf.Max(0, noise / targetDir.magnitude);
    }

    // Bounce off borders
    void OnCollisionEnter2D(Collision2D collision)
    {
        direction = -direction;
    }
}
