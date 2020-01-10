using UnityEngine;

public class RingScript : MonoBehaviour
{
    private GameController gameController;
    private ExplosionScript explosionScript;

    [Header("Ring Attributes")]
    public Renderer ringMesh;
    public BoxCollider ringBoxColliderTrigger;
    public BoxCollider ringBoxColliderRight;
    public BoxCollider ringBoxColliderLeft;
    public Rigidbody ringRigidBody;

    
    private void Start()
    {
        gameController = GameController.instance;
        explosionScript = transform.gameObject.GetComponent<ExplosionScript>();
    }
    

    /// <summary>
    /// This function turns off the ring box colliders, all of them
    /// it enables use of gravity
    /// it disables isKinematic
    /// it adds explosion force to the ring
    /// it destroys after some time afterwards
    /// </summary>
    public void DestroyRing()
    {
        ringBoxColliderTrigger.enabled = false;
        ringBoxColliderRight.enabled = false;
        ringBoxColliderLeft.enabled = false;
        ringRigidBody.isKinematic = false;

        ringRigidBody.useGravity = true;
        ringRigidBody.AddExplosionForce(gameController.power, gameController.explosionPosition, gameController.radius, gameController.upForce, ForceMode.Impulse);


        Destroy(transform.gameObject, 1.0f);
    }
    
}
