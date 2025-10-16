using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject enemy;
    public GameObject[] evidenceItems;

    [SerializeField] int evidenceCollected;
    [SerializeField] bool allEvidenceCollected;

    // Update is called once per frame
    void Update()
    {
        
    }

    // Triggered when player collects an evidence item
    public void CollectEvidence()
    {
        // Increment the count of collected evidence and update ui
        evidenceCollected++;
        canvas.transform.GetChild(evidenceCollected - 1).gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;

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
        Debug.Log("Game finished!");
    }
}
