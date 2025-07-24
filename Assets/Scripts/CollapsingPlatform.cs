using UnityEngine;
using System.Collections;
public class CollapsingPlatform : MonoBehaviour
{
    public float collapseDelay = 2f;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //destroy this platform object after 2 seconds.
            StartCoroutine(CollapseAfterDelay());
        }
    }

    IEnumerator CollapseAfterDelay()
    {
        yield return new WaitForSeconds(collapseDelay);
        Destroy(gameObject);
    }
}
