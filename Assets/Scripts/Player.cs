using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D coll;

    Collider2D noiseSource;
    public Enemy enemy;

    Vector2 direction;
    public int noise = 0;
    bool alive = true;

    // Get Components
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // Analyze input, move player, and check for noise sources
    void FixedUpdate()
    {
        // Processes only if player is alive
        if (alive)
        {
            // Movement handling
            InputRead();
            rb.MovePosition(rb.position + (direction * 0.5f * Time.deltaTime));

            // Noise handling
            if (direction != Vector2.zero)
            {
                // Check for overlap with noisy objects
                List<Collider2D> results = new List<Collider2D>();
                int hits = coll.Overlap(new ContactFilter2D
                {
                    layerMask = LayerMask.GetMask("Noisy"),
                    useLayerMask = true,
                    useTriggers = true
                }, results);

                noise = 0; // Reset noise

                // Determine noise level based on tag of object overlapped
                foreach (var hit in results)
                {
                    noiseSource = hit;
                    switch (hit.gameObject.tag)
                    {
                        case "Alert1":
                            noise = 5;
                            break;
                        case "Alert2":
                            noise = 6;
                            break;
                        case "Alert3":
                            noise = 7;
                            break;
                        default:
                            noise = 0;
                            break;
                    }
                    break; // Only consider the first noisy object
                }
                enemy.Noise(rb.position, noise); // Notify enemy of noise
            }
        }
        else
        {
            direction = Vector2.zero; // Stop movement if dead
        }
    }

    // Read player input and adjust direction
    void InputRead()
    {
        if (Keyboard.current[Key.W].IsActuated() || Keyboard.current[Key.UpArrow].IsActuated())
        {
            if (direction.y < 5)
            {
                direction.y += 0.5f;
            }
        }
        if (Keyboard.current[Key.S].IsActuated() || Keyboard.current[Key.DownArrow].IsActuated())
        {
            if (direction.y > -5)
            {
                direction.y -= 0.5f;
            }
        }

        if (Keyboard.current[Key.A].IsActuated() || Keyboard.current[Key.LeftArrow].IsActuated())
        {
            if (direction.x > -5)
            {
                direction.x -= 0.5f;
            }
        }
        if (Keyboard.current[Key.D].IsActuated() || Keyboard.current[Key.RightArrow].IsActuated())
        {
            if (direction.x < 5)
            {
                direction.x += 0.5f;
            }
        }

        if (Mathf.Abs(direction.x) > 0)
        {
            direction.x -= 0.25f * Mathf.Sign(direction.x);
        }
        if (Mathf.Abs(direction.y) > 0)
        {
            direction.y -= 0.25f * Mathf.Sign(direction.y);
        } 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Trigger death on collision with enemy
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            direction = Vector2.zero;
            alive = false;
            Debug.Log("Player has died.");
        }
    }
}
