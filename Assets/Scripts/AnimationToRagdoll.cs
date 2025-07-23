using System.Collections;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    [SerializeField]Collider col;
    [SerializeField]Rigidbody rb;
    [SerializeField]GameObject hip;
    [SerializeField]Camera camera;
    [SerializeField] float respawnTime=30f;
    Rigidbody[] rigidbodies;
    Collider[] colliders;


    bool bIsRagdoll=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbodies=GetComponentsInChildren<Rigidbody>();
        colliders=GetComponentsInChildren<Collider>();
        ToggleRagdoll(true);
    }

    void Update()
    {
        CameraHandling();
    }

    public float getRespawnTime(){return respawnTime;}

    private void CameraHandling()
    {
        if(!bIsRagdoll)
        {
            //camera positioning
            Vector3 campos = this.transform.position + new Vector3(0f, 5f, -10f);
            camera.transform.position = campos;
            camera.transform.LookAt(this.transform, new Vector3(0f, 1f, 0f));
        }

        else{
            //camera positioning
            Vector3 campos = hip.transform.position + new Vector3(0f, 5f, -10f);
            camera.transform.position = campos;
            camera.transform.LookAt(hip.transform, new Vector3(0f, 1f, 0f));
            
        }
    } 
    private void ToggleRagdoll(bool bisAnimating)
    {
        bIsRagdoll=!bisAnimating;
        //myCollider.enabled=bisAnimating;

        foreach(Rigidbody ragdollBone in rigidbodies)
        {
           
            ragdollBone.isKinematic=bisAnimating;

            
        }

        foreach(Collider collider in colliders)
        {
            Physics.IgnoreCollision(collider,col);
        }

        rb.isKinematic=false;

        GetComponent<Animator>().enabled=bisAnimating;
        if(bisAnimating)
        {
            RandomAnimation();
        }

  
    }

    void RandomAnimation()
    {
   
        Animator animator =GetComponent<Animator>();
      
        animator.CrossFade("Base Layer.Walk",0.2f);
        
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision" + collision.gameObject.name);
        if(!bIsRagdoll && collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("ragdoll!");
            ToggleRagdoll(false); //->enable ragdoll.
            StartCoroutine(GetBackUp());
        }
    }

    private IEnumerator GetBackUp()
    {

        yield return new WaitForSeconds(respawnTime);
        ToggleRagdoll(true);
        this.transform.position=hip.transform.position;
    }


}
