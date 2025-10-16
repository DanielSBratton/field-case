using UnityEngine;

public class Evidence : MonoBehaviour
{
    // Waits for player to collide with evidence object
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Notify EvidenceManager to collect evidence
            this.GetComponentInParent<EvidenceManager>().CollectEvidence();

            // Disable the evidence object to avoid redundant triggers
            this.gameObject.SetActive(false);
        }
    }
}
