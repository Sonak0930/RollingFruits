using System.Collections;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    public Collider playerCollider;
    public Rigidbody playerRigidbody;
    public GameObject playerHip;
    public Camera playerMainCamera;
    public float respawnTime = 30f;
    Rigidbody[] rigidbodies;
    Collider[] colliders;


    bool IsRagdoll = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        ToggleRagdoll(true);
    }

    void Update()
    {
        SwitchCameraTransformWhenRagdoll();
    }

    public float getRespawnTime() { return respawnTime; }

    private void SwitchCameraTransformWhenRagdoll()
    {
        if (IsRagdoll) {
            Vector3 campos = playerHip.transform.position + new Vector3(0f, 5f, -10f);
            playerMainCamera.transform.position = campos;
            playerMainCamera.transform.LookAt(playerHip.transform, new Vector3(0f, 1f, 0f));

        } else {
            Vector3 campos = this.transform.position + new Vector3(0f, 5f, -10f);
            playerMainCamera.transform.position = campos;
            playerMainCamera.transform.LookAt(this.transform, new Vector3(0f, 1f, 0f));

        }
    }
    private void ToggleRagdoll(bool bisAnimating)
    {
        IsRagdoll = !bisAnimating;
        //myCollider.enabled=bisAnimating;

        foreach (Rigidbody ragdollBone in rigidbodies) {

            ragdollBone.isKinematic = bisAnimating;
        }

        foreach (Collider collider in colliders) {
            Physics.IgnoreCollision(collider, playerCollider);
        }

        playerRigidbody.isKinematic = false;

        GetComponent<Animator>().enabled = bisAnimating;
        if (bisAnimating) {
            PlayWalkAnimation();
        }


    }

    void PlayWalkAnimation()
    {

        Animator animator = GetComponent<Animator>();

        animator.CrossFade("Base Layer.Walk", 0.2f);


    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision" + collision.gameObject.name);
        if (!IsRagdoll && collision.gameObject.CompareTag("Obstacle")) {
            Debug.Log("ragdoll!");
            ToggleRagdoll(false); //->enable ragdoll.
            StartCoroutine(GetBackUp());
        }
    }

    private IEnumerator GetBackUp()
    {

        yield return new WaitForSeconds(respawnTime);
        ToggleRagdoll(true);
        this.transform.position = playerHip.transform.position;
    }


}
