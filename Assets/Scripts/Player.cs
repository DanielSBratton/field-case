using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D coll;

    Collider2D noiseSource;
    public Enemy enemy;
    public GameObject canvas;

    Vector2 direction;
    public int volume = 0;

    public enum playState { start, alive, dead, finish }
    public playState currentState = playState.start;
    public float stateStartTime = Time.time;

    public AudioClip LoudSound;

    // Get Components
    void Start()
    {
        stateStartTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // Analyze input, move player, and check for noise sources
    void FixedUpdate()
    {
        switch (currentState)
        {
            // Starting screen logic - waiting for input to start game
            case playState.start:
                if (Keyboard.current.anyKey.IsActuated() && Time.time - stateStartTime > 1f)
                {
                    canvas.transform.GetChild(3).gameObject.SetActive(false); // Hide start screen
                    currentState = playState.alive;
                }
                break;

            // Normal gameplay state
            case playState.alive:
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

                    volume = 0; // Reset volume

                    // Determine noise level based on tag of object overlapped
                    foreach (var hit in results)
                    {
                        noiseSource = hit;
                        switch (hit.gameObject.tag)
                        {
                            case "Alert1":
                                volume = 20;
                                if (!this.GetComponent<AudioSource>().isPlaying)
                                {
                                    this.GetComponent<AudioSource>().Play();
                                }
                                break;
                            case "Alert2":
                                volume = 60;
                                if (!this.GetComponent<AudioSource>().isPlaying)
                                {
                                    this.GetComponent<AudioSource>().PlayOneShot(LoudSound);
                                }
                                break;
                            case "Alert3":
                                volume = 100;
                                break;
                            default:
                                volume = 0;
                                break;
                        }
                        break; // Only consider the first noisy object
                    }
                    if (volume > 0)
                    {
                        enemy.Noise(rb.position, volume); // Notify enemy of sound
                    }
                    else
                    {
                        this.GetComponent<AudioSource>().Stop();
                        volume = 0;
                    }
                }
                else
                {
                    this.GetComponent<AudioSource>().Stop();
                    volume = 0;
                }
            break;

            // Player death state - waiting for input to restart
            case playState.dead:
                if (Keyboard.current.anyKey.IsActuated() && Time.time - stateStartTime > 1f)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart the scene
                }
                break;
            case playState.finish:
                // Game finished, no movement
                break;
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
            currentState = playState.dead;
            stateStartTime = Time.time;
            canvas.transform.GetChild(4).gameObject.SetActive(true); // Show death screen
            Debug.Log("Player has died.");
        }
    }
}
