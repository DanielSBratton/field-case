using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject enemy;
    public GameObject[] evidenceItems;
    public Player player;

    [SerializeField] int evidenceCollected;
    [SerializeField] bool allEvidenceCollected;

    public AudioClip collectSound;

    // Triggered when player collects an evidence item
    public void CollectEvidence()
    {
        // Increment the count of collected evidence, update ui, and play sound
        evidenceCollected++;
        canvas.transform.GetChild(evidenceCollected - 1).gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        canvas.GetComponent<AudioSource>().PlayOneShot(collectSound);

        // Spawn enemy after collecting 2 pieces of evidence
        if (evidenceCollected == 2)
        {
            enemy.SetActive(true);
        }

        Debug.Log($"Evidence collected: {evidenceCollected}/{evidenceItems.Length}");

        if (evidenceCollected == evidenceItems.Length)
        {
            allEvidenceCollected = true;
            this.GetComponent<Collider2D>().isTrigger = true; // Disable further triggers
            Debug.Log("All evidence collected!");
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        player.currentState = Player.playState.finish;
        canvas.transform.GetChild(5).gameObject.SetActive(true); // Show finish screen
        Debug.Log("Game finished!");
    }
}
