using UnityEngine;
using System;
using System.Collections;

public class Ball : MonoBehaviour
{
    public static Ball instance = null;

    [Header("Player References")]
    public GameController gameController;
    public Renderer playerRenderer;
    public AudioSource popSound;
    public GameObject splatHolder;
    public GameObject splatPrefab;

    [SerializeField] private GameObject trailEffect = null;
    [SerializeField] private GameObject fireParticleFX = null;
    [SerializeField] private ParticleSystem splashParticleFX = null;
    public GameObject smokeParticleFX = null;


    [Header("Player Variables")]
    public float jumpForce = 25;
    public float globalGravity = -9.8f;
    public float gravityScale = 1.0f;

    public bool isInAir = true;

    // scale section - to make the wiggly bounciness effect
    [SerializeField] private float minScale = 0.85f;
    [SerializeField] private float maxScale = 1.25f;
    [SerializeField] private float scalingFactor = 5;
    private Vector3 scale;

    [SerializeField] private Color colorPowerup = Color.yellow;
    private Color colorNormal;

    [Space]
    private bool isDead = false;
    private bool canStillDestroy = false;
    
    //To prevent colliding with 2 platforms at the same time
    private float collisionTimer = 0.1f;
    private float collisionCounter = 0f;
    
    private int stackDestroyed = 0;

    private Vector3 gravity;
    private Rigidbody playerRB;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        colorNormal = playerRenderer.material.color;
        scale = playerRenderer.transform.localScale;
        fireParticleFX.SetActive(false);
        smokeParticleFX.SetActive(false);
    }

    private void Update()
    {
        ScaleBall();
        collisionCounter += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        gravity = globalGravity * gravityScale * Vector3.up * 5;
        playerRB.AddForce(gravity, ForceMode.Acceleration);
    }

    //In Game Over , adding force is turned off
    public void GameOver()
    {
        isDead = true;
        playerRB.velocity = Vector3.zero;
    }

    //adding force for the player ball
    private void AddingForce()
    {
        if (!isDead)
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    


    private void OnCollisionEnter(Collision collision)
    {
        if (!canStillDestroy)
        {
            // normal collision, not a powerup

            if (collision.gameObject.CompareTag("Platform"))
            {
                //if condition to only allow only 1 collision
                if (collisionCounter > collisionTimer)
                {
                    collisionCounter = 0f;

                    AddingForce();
                    PlaySound();
                    splashParticleFX.Play();

                    gameController.ProgressBarUpdate();

                    Vector3 splatPosition = collision.contacts[0].point;
                    splatPosition.y += 0.5f;
                    Instantiate(splatPrefab, splatPosition, Quaternion.Euler(90, UnityEngine.Random.Range(0f, 360f), 0), splatHolder.transform);
                }
            }


            if (collision.gameObject.CompareTag("Ring"))
            {
                gameController.ResetStreak();

                if (collisionCounter > collisionTimer)
                {
                    collisionCounter = 0;

                    AddingForce();
                    PlaySound();

                    isInAir = true;
                }
            }

            if (collision.gameObject.CompareTag("Deadly"))
            {
                GameOver();
                splashParticleFX.Play();
                gameController.GameOverInit(false);
                Debug.Log("Died");
            }
        }
        else
        {
            // collision on the last layer of the powerup
            if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Deadly") || collision.gameObject.CompareTag("Ring"))
            {

                gameController.VibratePowerup();

                gameController.powerUpCounter = 0;
                collisionCounter = 0f;

                AddingForce();

                gameController.ProgressBarUpdate();
                gameController.SetScore();

                gameController.DestroyTopLayer();
                canStillDestroy = false;
                
            }
            if (collision.gameObject.CompareTag("Platform") && collision.gameObject.CompareTag("Deadly")
                || collision.gameObject.CompareTag("Ring") && collision.gameObject.CompareTag("Deadly"))
            {

                gameController.VibratePowerup();

                gameController.powerUpCounter = 0;
                collisionCounter = 0f;

                AddingForce();

                gameController.ProgressBarUpdate();
                gameController.SetScore();

                gameController.DestroyTopLayer();
                canStillDestroy = false;
                
            }
        }

        //colliding with the end platform
        if (collision.gameObject.CompareTag("End"))
        {
            GameOver();
            gameController.GameOverInit(true);
            Debug.Log("Ended");
        }


        //isInAir = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!gameController.gameStarted)
        {
            return;
        }

        if (!canStillDestroy)
        {
            // normal collision, not a powerup

            if (collision.gameObject.CompareTag("Platform"))
            {
                //if condition to only allow only 1 collision
                if (collisionCounter > collisionTimer)
                {
                    collisionCounter = 0f;

                    AddingForce();
                    PlaySound();
                    splashParticleFX.Play();

                    gameController.ProgressBarUpdate();

                    Vector3 splatPosition = collision.contacts[0].point;
                    splatPosition.y += 0.5f;
                    Instantiate(splatPrefab, splatPosition, Quaternion.Euler(90, UnityEngine.Random.Range(0f, 360f), 0), splatHolder.transform);
                }
            }


            if (collision.gameObject.CompareTag("Ring"))
            {
                gameController.ResetStreak();

                if (collisionCounter > collisionTimer)
                {
                    collisionCounter = 0;

                    AddingForce();
                    PlaySound();

                    isInAir = true;
                }
            }

            if (collision.gameObject.CompareTag("Deadly"))
            {
                if (collisionCounter > collisionTimer)
                { collisionCounter = 0f;

                    GameOver();
                    splashParticleFX.Play();
                    gameController.GameOverInit(false);
                    Debug.Log("Died");
                }
            }
        }
    }

    //when hitting the Disappear trigger of each platform
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ring"))
        {
            gameController.ProgressBarUpdate();
            gameController.AddStreak();
            gameController.VibrateRing();
            gameController.DestroyTopLayer();
        }


        if (gameController.isPowerup)
        {
            if (other.CompareTag("DisappearTrigger"))
            {
                gameController.SetScore();
                gameController.VibratePowerup();
                gameController.DestroyTopLayer();
                stackDestroyed++;
                if (stackDestroyed == 2)
                {       
                    stackDestroyed = 0;
                    gameController.isPowerup = false;
                    FireFX(false);
                    canStillDestroy = true;
                }
            }

        }   
    }

    //change ball color when on Power UP
    public void ChangeColor()
    {
        playerRenderer.material.color = colorPowerup;
    }

    //restoring ball color when powerup ends
    public void ResetColor()
    {
        playerRenderer.material.color = colorNormal;
    }

    //fire effect when the ball is on power up
    public void FireFX(bool enable)
    {
        if (enable)
        {
            ChangeColor();
            fireParticleFX.SetActive(true);
            trailEffect.SetActive(false);
        }
        else
        {
            fireParticleFX.SetActive(false);
            Invoke("ResetColor", 0.269f);
            trailEffect.SetActive(true);
            //ResetColor();
        }

        gameController.powerUpCounter = 0;
    }

    //scaling ball , if in air, it gets smaller, when it hits a platform it gets bigger
    private void ScaleBall()
    {
        if (isInAir)
        {
            if (scale.x > minScale)
            {
                scale.x -= scalingFactor * Time.deltaTime;
            }
            else
            {
                scale.x = minScale;
            }

            playerRenderer.transform.localScale = scale;
        }
        else
        {
            if (scale.x < maxScale)
            {
                scale.x += scalingFactor * Time.deltaTime;
            }
            else
            {
                scale.x = maxScale;
            }

            playerRenderer.transform.localScale = scale;
        }

    }

    //playing the pop sound with a random pitch
    public void PlaySound()
    {
        popSound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        popSound.Play();
    }
}