using System.Collections;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//used system.serializable so that the class objects can be seen in the editor
[System.Serializable]
public class ColorPalette
{

    public Color playerColor;
    public Color ringColor;
    public Color safeColor;
    public Color deadlyColor;
    public Gradient backgroundGradient;
}

public class GameController : MonoBehaviour
{
    public static GameController instance = null;


    public GameObject playerBall;

    private SpawnManager spawnManager;
    private Ball ballScript;

    [Header("Gameplay UI")]
    public GameObject gameplayUI;
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI scoreText;
    public Image progressLevelBar;
    private Animator streakTextAnimator;


    [Header("Main UI")]
    public GameObject mainUI;
    public TextMeshProUGUI mainCurrentLevelText;

    [Header("GameOver UI")]
    public GameObject gameOverUI;
    public GameObject tapToContinueText;
    public TextMeshProUGUI gameOverCompletedLevelText;
    public TextMeshProUGUI gameOverCurrentLevelText;
    public TextMeshProUGUI gameOverNextLevelText;
    public Image gameOverProgressLevelBar;
    public GameObject gameOverLevelSection;
    public GameObject gameOverVictoryText;
    public GameObject gameOverLoseText;
    public Animator tapToContinueAnimator;


    [Header("Movements & Control Attributes")]
    public GameObject levelController;

    [Header("Explosion Attributes")]
    public Vector3 explosionPosition;
    public float power = 10.0f;
    public float radius = 5.0f;
    public float upForce = 1.0f;
    

    [Header("Values and References")]
    //Values of Movement
    public float desktopSensitivityX = -15f;
    public float mobileSensitivityX = -15f;
    public float minimumX = -20f;
    public float maximumX = 40f;
    public float positionX = 0f;
    public float initialPositionX = 0f;
    public float secondPositionX = 0f;
    public float draggingSpeed = 20f;

    [Header("Level Colors")]

    public Material playerMaterial;
    public Material ringMaterial;
    public Material safeMaterial;
    public Material deadlyMaterial;
    public SpriteRenderer splatRenderer;
    public MF.GradientBackground cameraBackgroundScript;

    public ColorPalette[] colorPalettes;
    
    [Space]
    public bool gameOver = false;
    public bool gameStarted = false;

    //To indicate if has the powerup or not
    public bool isPowerup = false;
    
    //Level Score
    private int score = 0;

    //Current Level
    private int currentLevel = 0;

    //streak value
    private int streak = 0;

    //temp streak value
    public int powerUpCounter = 0;
    
    //Slow Motion Factor when losing / winning level
    private float slowMotionFactor = 0.1f;

    //progress Bar Fill Rate
    private float fillRate = 0f;
    private float playerYPos = 0f;
    private float endLayerPos = 0f;

    //vibration long patterns
    private long[] gameoverVibratePattern = new long[] { 0, 150, 150, 250 };

    
    private void Awake()
    {
        instance = this;
        GetLevelVariables();

        AssigningColors();
        GameAnalytics.Initialize();
    }

    private void Start()
    {
        
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SetVariables();

        mainUI.SetActive(true);
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameStarted)
        {
            MoveController();
        }

        if (Input.GetKeyDown("space"))
        {
            RestartGame();
        }
    }

    //starting game
    public void StartGame()
    {
        streak = 0;
        score = 0;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        isPowerup = false;
        gameStarted = true;
        
        streakText.enabled = false;

        gameplayUI.SetActive(true);
        gameOverUI.SetActive(false);
        mainUI.SetActive(false);

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, Application.version, currentLevel.ToString("00000"));

    }

    //restarting the game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void GameOverInit(bool win)
    {
        gameStarted = false;
        EndGame(win);
    }

    //Game ended, whether by winning or losing
    public void EndGame(bool win)
    {
        Debug.Log("Ended game");
        VibrateGameover();
        mainUI.SetActive(false);
        gameplayUI.SetActive(false);

        if (!win)
        {
            // Lost the round
            Time.timeScale = slowMotionFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;


            gameOverLoseText.SetActive(true);
            gameOverVictoryText.SetActive(false);
            gameOverCurrentLevelText.gameObject.SetActive(true);
            gameOverNextLevelText.gameObject.SetActive(true);
            gameOverProgressLevelBar.gameObject.SetActive(true);
            gameOverCompletedLevelText.gameObject.SetActive(false);

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, Application.version, currentLevel.ToString("00000"), score);
        }
        else
        {
            gameOverLevelSection.SetActive(false);
            gameOverCompletedLevelText.gameObject.SetActive(true);
            gameOverLoseText.SetActive(false);
            gameOverVictoryText.SetActive(true);

            LevelUp();

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, Application.version, currentLevel.ToString("00000"), score);
        }

        gameOverUI.SetActive(true);
    }

    //setting score and appointing it to the text
    public void SetScore()
    {
        if (powerUpCounter > 0)
        {
            score += currentLevel * powerUpCounter;
        }
        else
        {
            score += currentLevel;
        }

        scoreText.text = score.ToString();
    }

    //adding the level value when finishing the level
    public void LevelUp()
    {
        currentLevel++;
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "CurrentLevel", currentLevel);
    }

    /// <summary>
    /// Set game variable values from the start
    /// </summary>
    private void SetVariables()
    {
        streakTextAnimator = streakText.GetComponent<Animator>();
        spawnManager = GetComponent<SpawnManager>();
        ballScript = playerBall.GetComponent<Ball>();
    }

    //getting level value and assigning it to the Text elements
    private void GetLevelVariables()
    {
        currentLevel = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "CurrentLevel", 1);
        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();

        // game over ui to adjust
        gameOverCurrentLevelText.text = currentLevel.ToString();
        gameOverNextLevelText.text = (currentLevel + 1).ToString();

        mainCurrentLevelText.text = "LEVEL " + currentLevel;
        gameOverCompletedLevelText.text = "LEVEL " + currentLevel;

    }

    //Ball Movement on phone or in editor
    private void MoveController()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (initialPositionX == 0)
                {
                    positionX = levelController.transform.position.x;
                    initialPositionX = positionX;
                }
                else
                {
                    positionX = initialPositionX;
                }
            }

            if (Input.GetMouseButton(0))
            {
                positionX -= Input.GetAxis("Mouse X") * desktopSensitivityX * Time.deltaTime;
                positionX = Mathf.Clamp(positionX, minimumX, maximumX);
                levelController.transform.position = new Vector3(positionX, 0, 0);
                initialPositionX = positionX;
            }
            if (Input.GetMouseButtonUp(0))
            {
                initialPositionX = levelController.transform.position.x;
            }
        }
        else
        {
            if (Input.touches.Length > 0)
            {
                Touch t = Input.GetTouch(0);

                if(t.phase == TouchPhase.Began)
                {
                    if (initialPositionX == 0)
                    {
                        secondPositionX = playerBall.transform.position.x;
                        initialPositionX = t.position.x;
                    }
                    else
                    {
                        initialPositionX = t.position.x;
                    }
                }
                if(t.phase == TouchPhase.Moved)
                {
                    secondPositionX = t.position.x;
                    positionX -= (secondPositionX - initialPositionX) * mobileSensitivityX * Time.deltaTime;
                    positionX = Mathf.Clamp(positionX, minimumX, maximumX);
                    levelController.transform.position = new Vector3(positionX, 0, 0);
                    initialPositionX = secondPositionX;
                }
            }
        }
    }

    //Reseting the streak to -1
    public void ResetStreak()
    {
        if (streak > 2)
        {
            //ballScript.ResetColor();
            ballScript.FireFX(false);
        }
        
        if (ballScript.smokeParticleFX.activeInHierarchy)
        {
            ballScript.smokeParticleFX.SetActive(false);
        }

        streak = -1;
        powerUpCounter = -1;

        Debug.Log("Streak: " + streak);

    }
    
    /// <summary>
    /// Adding streak
    /// showing streak animation depending on the streak value
    /// showing effects depending on the streak value
    /// </summary>
    public void AddStreak()
    {
        streak++;
        if (!isPowerup)
        {
            powerUpCounter++;
        }
        Debug.Log("Streak: " + streak);


        SetScore();

        if (powerUpCounter > 1)
        {
            StartCoroutine(ShowPerfectStreak());
        }

        if (powerUpCounter == 2)
        {
            ballScript.smokeParticleFX.SetActive(true);
        }

        if (powerUpCounter == 4)
        {
            ballScript.smokeParticleFX.SetActive(false);
            isPowerup = true;
            powerUpCounter = 0;
            ballScript.FireFX(true);

        }
    }

    //updating progress bar
    public void ProgressBarUpdate()
    {
        playerYPos = playerBall.transform.position.y;

        endLayerPos = spawnManager.finalPosition;
        fillRate = 1.0f * playerYPos / endLayerPos;
        
        progressLevelBar.fillAmount = Mathf.Lerp(0, 1, fillRate);
        gameOverProgressLevelBar.fillAmount = fillRate;
    }

    //Showing Streak animation with different colors
    public IEnumerator ShowPerfectStreak()
    {
        switch (powerUpCounter)
        {
            case 2:
                streakText.color = new Color(0f, 255f, 255f);
                break;

            case 3:
                streakText.color = new Color(255f, 0f, 170f);
                break;

            case 4:
                streakText.color = new Color(255f, 215f, 0f);
                break;

        }

        streakText.text = "Perfect x" + powerUpCounter;
        streakTextAnimator.Play("FloatingStreakTextAnim");


        Debug.Log("Played Anim");
        streakTextAnimator.SetTrigger("ShowText");
        yield break;

    }


    //explode every element of the top layer
    public void DestroyTopLayer()
    {
        explosionPosition = playerBall.transform.position;
        if (isPowerup)
        {
            explosionPosition.z = 2;
        }
        else
        {
            explosionPosition.z = -1;
        }

        Transform currentLayer = spawnManager.currentLayers[0].transform;

        for (int i = 0; i < currentLayer.childCount; i++)
        {
            if (currentLayer.GetChild(i).gameObject.CompareTag("Platform") || currentLayer.GetChild(i).gameObject.CompareTag("Deadly"))
            {
                currentLayer.GetChild(i).gameObject.GetComponent<PlaneScript>().DestroyPlane();
            }
            else if (currentLayer.GetChild(i).gameObject.gameObject.CompareTag("Ring"))
            {
                currentLayer.GetChild(i).gameObject.GetComponent<RingScript>().DestroyRing();
            }
        }

        spawnManager.currentLayers.RemoveAt(0);
    }

    
    /// <summary>
    /// Assign colors to the Player
    /// Assign colors to the Splat effect
    /// Assign colors to the Safe platform
    /// Assign colors to the Deadly platform
    /// Assign colors to the Ring
    /// </summary>
    private void AssigningColors()
    {
       
        //int selectedColor = Random.Range(0, colorPalettes.Length);
        switch (currentLevel % 8)
        {
            case 0:
                playerMaterial.color = colorPalettes[0].playerColor;
                splatRenderer.color = colorPalettes[0].playerColor;
                safeMaterial.color = colorPalettes[0].safeColor;
                ringMaterial.color = colorPalettes[0].ringColor;
                deadlyMaterial.color = colorPalettes[0].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[0].backgroundGradient;
                break;
            case 1:
                playerMaterial.color = colorPalettes[1].playerColor;
                splatRenderer.color = colorPalettes[1].playerColor;
                safeMaterial.color = colorPalettes[1].safeColor;
                ringMaterial.color = colorPalettes[1].ringColor;
                deadlyMaterial.color = colorPalettes[1].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[1].backgroundGradient;
                break;
            case 2:
                playerMaterial.color = colorPalettes[2].playerColor;
                splatRenderer.color = colorPalettes[2].playerColor;
                safeMaterial.color = colorPalettes[2].safeColor;
                ringMaterial.color = colorPalettes[2].ringColor;
                deadlyMaterial.color = colorPalettes[2].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[2].backgroundGradient;
                break;
            case 3:
                playerMaterial.color = colorPalettes[3].playerColor;
                splatRenderer.color = colorPalettes[3].playerColor;
                safeMaterial.color = colorPalettes[3].safeColor;
                ringMaterial.color = colorPalettes[3].ringColor;
                deadlyMaterial.color = colorPalettes[3].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[3].backgroundGradient;
                break;
            case 4:
                playerMaterial.color = colorPalettes[4].playerColor;
                splatRenderer.color = colorPalettes[4].playerColor;
                safeMaterial.color = colorPalettes[4].safeColor;
                ringMaterial.color = colorPalettes[4].ringColor;
                deadlyMaterial.color = colorPalettes[4].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[4].backgroundGradient;
                break;
            case 5:
                playerMaterial.color = colorPalettes[5].playerColor;
                splatRenderer.color = colorPalettes[5].playerColor;
                safeMaterial.color = colorPalettes[5].safeColor;
                ringMaterial.color = colorPalettes[5].ringColor;
                deadlyMaterial.color = colorPalettes[5].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[5].backgroundGradient;
                break;
            case 6:
                playerMaterial.color = colorPalettes[6].playerColor;
                splatRenderer.color = colorPalettes[6].playerColor;
                safeMaterial.color = colorPalettes[6].safeColor;
                ringMaterial.color = colorPalettes[6].ringColor;
                deadlyMaterial.color = colorPalettes[6].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[6].backgroundGradient;
                break;
            case 7:
                playerMaterial.color = colorPalettes[7].playerColor;
                splatRenderer.color = colorPalettes[7].playerColor;
                safeMaterial.color = colorPalettes[7].safeColor;
                ringMaterial.color = colorPalettes[7].ringColor;
                deadlyMaterial.color = colorPalettes[7].deadlyColor;
                cameraBackgroundScript.Gradient = colorPalettes[7].backgroundGradient;
                break;
        }

        //playerMaterial.color = colorPalettes[selectedColor].playerColor;
        //splatRenderer.color = colorPalettes[selectedColor].playerColor;
        //safeMaterial.color = colorPalettes[selectedColor].safeColor;
        //ringMaterial.color = colorPalettes[selectedColor].ringColor;
        //deadlyMaterial.color = colorPalettes[selectedColor].deadlyColor;
        //cameraBackgroundScript.Gradient = colorPalettes[selectedColor].backgroundGradient;
        //Debug.Log("Assigned colors");
    }

/// <summary>
/// The section below is for the vibrations and Haptic feedback 
/// Each case is written independently
/// </summary>
    public void VibrateRing()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            Vibration.Vibrate(50);
#elif UNITY_IOS
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactLight);
#endif
#endif
    }

    public void VibratePowerup()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            Vibration.Vibrate(150);
#elif UNITY_IOS
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
#endif
#endif
    }

    public void VibrateGameover()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            Vibration.Vibrate(gameoverVibratePattern , -1);
#elif UNITY_IOS
            iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactHeavy);
#endif
#endif
    }

}


