using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 initialPosition;
    private float deltaY;
    private float deltaX;


    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position;
        deltaY = playerTransform.position.y - initialPosition.y;
        deltaX = playerTransform.position.x - initialPosition.x;
    }

    private void LateUpdate()
    {
        //if the player position (Y) is decreased, the camera follows the player
        if (transform.position.y + deltaY > playerTransform.position.y)
        {
            initialPosition.y = playerTransform.position.y - deltaY;
            initialPosition.x = playerTransform.position.x - deltaX;

            transform.position = initialPosition;
        }

        initialPosition.x = playerTransform.position.x - deltaX;
        transform.position = initialPosition;
        
    }

}
