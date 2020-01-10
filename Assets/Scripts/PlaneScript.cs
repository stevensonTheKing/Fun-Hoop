using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    [Header("Plane Attributes")]
    public BoxCollider planeBoxCollider;
    public Rigidbody planeRigidBody;
    private GameController gameController;
    public GameObject player;


    private void Start()
    {
        player = Ball.instance.gameObject;
        gameController = GameController.instance;
    }

    /// <summary>
    /// this function is for turning of the box collider of the plane
    /// enabling gravity
    /// disabling isKinematic
    /// adding explosion for
    /// destroying the plane after some time
    /// </summary>
    public void DestroyPlane()
    {
        planeBoxCollider.enabled = false;
        planeRigidBody.useGravity = true;
        planeRigidBody.isKinematic = false;
        planeRigidBody.AddExplosionForce(gameController.power, gameController.explosionPosition, gameController.radius, gameController.upForce, ForceMode.Impulse);
        Destroy(transform.gameObject, 1.0f);
    }
}
