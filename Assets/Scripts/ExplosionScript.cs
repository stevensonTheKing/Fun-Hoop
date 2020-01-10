using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    //public GameObject explodingElement;
    //public float power = 10.0f;
    //public float radius = 5.0f;
    //public float upForce = 1.0f;
    //public float timeToDisappear = 0.3f;
    //Vector3 explosionPosition;

    public RingScript ringScript;
    public PlaneScript planeScript;

    // Start is called before the first frame update
    private void Start()
    {
        //explodingElement = GameObject.FindGameObjectWithTag("Player");

        if (transform.gameObject.CompareTag("Ring"))
        {

            ringScript = GetComponent<RingScript>();
            planeScript = null;
        }
        else
        {
            ringScript = null;
            planeScript = GetComponent<PlaneScript>();

        }
    }

    // Update is called once per frame
    private void Update()
    {

    }
    
    //The layer is set not active aka turned off
    private void DestroyLayer()
    {
        Debug.Log("Layer Disappered");
        transform.parent.gameObject.SetActive(false);
    }
}
