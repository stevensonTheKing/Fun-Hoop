using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    private GameController gameController;


    public GameObject level;
    private GameObject createdLayer = null;
    
    [Header("Different Layer Difficulties")]
    public GameObject[] lowEasyLayers;
    public GameObject[] lowMediumLayers;
    public GameObject[] lowHardLayers;
    public GameObject[] highEasyLayers;
    public GameObject[] highMediumLayers;
    public GameObject[] highHardLayers;

    [Space]
    //final layer
    public GameObject endLayer;

    public List<GameObject> currentLayers = new List<GameObject>();
    public int numberOfLayers = 20;
    public int position = -30;
    public int differenceBetweenLayers = 40;
    public int finalPosition = 0;
    private int currentLevel;

    // Start is called before the first frame update
    private void Start()
    {
        gameController = GetComponent<GameController>();
        CreateLevel();
    }

    /// <summary>
    /// This function creates the level
    /// depending on the number of layers needed
    /// depending on the player level to set its random generated layers
    /// the first layer is always the same
    /// </summary>
    private void CreateLevel()
    {
        currentLevel = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "CurrentLevel", 1);
        if (currentLevel < 201)
        {
            createdLayer = Instantiate(lowEasyLayers[0], new Vector3(0, position, 0), Quaternion.identity, level.transform);
        }
        else
        {
            createdLayer = Instantiate(highEasyLayers[0], new Vector3(0, position, 0), Quaternion.identity, level.transform);
        }
        currentLayers.Add(createdLayer);
        position -= differenceBetweenLayers;

        switch (currentLevel)
        {
            //This is used to check if the current level is less than the specific number
            //It can be used if the cases should be checked in ranges 
            case int n when (n <= 10):
                LowLevelPartition(numberOfLayers,0,0);
               
                break;

            case int n when (n > 10 && n <= 25):
                LowLevelPartition(Mathf.Floor(numberOfLayers * 0.8f), Mathf.Floor(numberOfLayers * 0.2f), 0);

                break;

            case int n when (n > 25 && n <= 50):
                LowLevelPartition(Mathf.Floor(numberOfLayers * 0.6f), Mathf.Floor(numberOfLayers * 0.4f), 0);
                
                break;

            case int n when (n > 50 && n <= 75):
                LowLevelPartition(Mathf.Floor(numberOfLayers * 0.2f), Mathf.Floor(numberOfLayers * 0.8f), 0);
                
                break;

            case int n when (n > 75 && n <= 100):
                LowLevelPartition(Mathf.Floor(numberOfLayers * 0.1f), Mathf.Floor(numberOfLayers * 0.7f), Mathf.Floor(numberOfLayers * 0.2f));

                break;

            case int n when (n > 100 && n <= 150):
                LowLevelPartition(0, Mathf.Floor(numberOfLayers * 0.5f), Mathf.Floor(numberOfLayers * 0.5f));

                break;

            case int n when (n > 150 && n <= 200):
                LowLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
                //old layers spawn start from here
            case int n when (n > 200 && n <= 210):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
            case int n when (n > 210 && n <= 225):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
            case int n when (n > 225 && n <= 250):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
            case int n when (n > 250 && n <= 300):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
            case int n when (n > 300 && n <= 350):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
            case int n when (n > 350 && n <= 400):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
            case int n when (n > 400):
                HigherLevelPartition(0, Mathf.Floor(numberOfLayers * 0.3f), Mathf.Floor(numberOfLayers * 0.7f));

                break;
        }
        Instantiate(endLayer, new Vector3(10, position, 0), Quaternion.identity, level.transform);
        finalPosition = position;     
    }

    private void LowLevelPartition(float easy, float medium , float hard) 
    {
        for (int i = 1; i < easy; i++)
        {
            LevelVariation(lowEasyLayers);

        }
        for (int i = 1; i < medium; i++)
        {
            LevelVariation(lowMediumLayers);
        }
        for (int i = 1; i < hard; i++)
        {
            LevelVariation(lowHardLayers);
        }
    }

    //Level creation for higher level
    private void HigherLevelPartition(float easy, float medium , float hard) 
    {
        for (int i = 1; i < easy; i++)
        {
            LevelVariation(highEasyLayers);

        }
        for (int i = 1; i < medium; i++)
        {
            LevelVariation(highMediumLayers);
        }
        for (int i = 1; i < hard; i++)
        {
            LevelVariation(highHardLayers);
        }
    }

    private void LevelVariation(GameObject[] List)
    {
        createdLayer = Instantiate(List[Random.Range(0, List.Length - 1)], new Vector3(0, position, 0), Quaternion.identity, level.transform);
        currentLayers.Add(createdLayer);
        position -= differenceBetweenLayers;

    }
}
