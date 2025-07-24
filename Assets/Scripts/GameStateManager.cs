using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameStateManager : MonoBehaviour
{

    /// <summary>
    /// Description of the GameState.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Pause,
        GameOver,
        Clear
    }

    [Header("Reference section for each UI")]
    public GameObject MainMenuUI;
    public GameObject InGameMenuUI;
    public GameObject PauseMenuUI;
    public GameObject GameOverMenuUI;
    public GameObject ClearUI;

    [Header("Platform resources")]
    [Tooltip("The position where platform is destroyed")]
    public GameObject platformEndPosition;
    [Tooltip("Container to store different kinds of platform prefabs")]
    public List<GameObject> platformReferenceList;
    [Tooltip("Reference to the obstacleSpawner")]
    public ObstacleSpawner obstacleSpawner;
    [Tooltip("Queue to manage the runtime platform instances")]
    private Queue<GameObject> platformInstanceList;
    [Tooltip("Interval for platform generation")]
    private float spawnInterval = 2f;

    [Header("UI reference")]
    public TextMeshProUGUI elapsedTimeUGUI;

    [Header("GamePlay references")]
    public GameObject player;
    public GameObject bottomDeadlineIndicator;
    [Tooltip("Delay between scene transition")]
    [Range(0.1f,0.5f)]
    public float SceneTransitionDelay = 0.3f;

    [Header("Time section")]
    [Tooltip("Total duration of the game")]
    public float gametime;

    private float time_start;
    private float time_current;
 

    [Tooltip("0: basic platform, 1:deformed platform")]
    private int indexBasicPlatform=0;
    private int indexDeformedPlatform = 1;

    public GameState GetGameState() { return CurrentState; }
    public void ChangeState(GameState state) 
    {
        HandleStateChange((int)state);
    }
    private GameState CurrentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeState(GameState.MainMenu);
        
        platformInstanceList = new Queue<GameObject>();

        time_start = Time.time;
        time_current = 0f;

        InvokeRepeating("SpawnPlatform", 0f, spawnInterval);
        StartCoroutine(Timer(gametime));

    }

    IEnumerator Timer(float gametime)
    {
        yield return new WaitForSeconds(gametime);
        HandleStateChange((int)GameState.Clear);
    }
    // Update is called once per frame
    void Update()
    {
        time_current = Time.time - time_start;
        elapsedTimeUGUI.text = time_current.ToString();
       
        if (platformInstanceList.Count > 0)
        {
            GameObject frontPlatform = platformInstanceList.Peek();
            if (frontPlatform.transform.position.z < platformEndPosition.transform.position.z)
            {
       
                platformInstanceList.Dequeue();
                Destroy(frontPlatform);
       
            }
        }

        

        if(player.transform.position.y<bottomDeadlineIndicator.transform.position.y )
        {
            ChangeState(GameState.GameOver);
            SceneManager.LoadScene(0);
        }
    }

    public void HandleStateChange(int state)
    {
        CurrentState = (GameState)state;
        switch (CurrentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0;
                HideAllUI();
                MainMenuUI.SetActive(true);
              
                break;
            case GameState.Playing:
                Time.timeScale = 1;
                HideAllUI();
                InGameMenuUI.SetActive(true);
                break;
            case GameState.Pause:
                Time.timeScale = 0;
                HideAllUI();
                PauseMenuUI.SetActive(true);
                break;
            case GameState.GameOver:
                Time.timeScale = 0;
                HideAllUI();
                GameOverMenuUI.SetActive(true);
                break;
            case GameState.Clear:
                Time.timeScale = 0;
                HideAllUI();
                ClearUI.SetActive(true);
                break;
            default:
                break;
        }

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void HideAllUI()
    {
        MainMenuUI.SetActive(false);
        InGameMenuUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        GameOverMenuUI.SetActive(false);
        ClearUI.SetActive(false);
    }
    private IEnumerator TransitionToState(GameState newState)
    {
        if(newState != GameState.MainMenu)
        yield return new WaitForSeconds(SceneTransitionDelay);

        CurrentState = newState;
        HandleStateChange((int)CurrentState);
    }

    private void SpawnPlatform()
    {
        Vector3 spawnPos = this.transform.position;
        GameObject platform = Instantiate(platformReferenceList[indexBasicPlatform], spawnPos, Quaternion.identity);
        platformInstanceList.Enqueue(platform);

        
    }


}
