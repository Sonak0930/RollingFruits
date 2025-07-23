using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Pause,
        GameOver,
        Clear
    }

   
    public GameObject MainMenuUI;
    public GameObject InGameMenuUI;
    public GameObject PauseMenuUI;
    public GameObject GameOverMenuUI;
    public GameObject ClearUI;

    public GameObject endPos;
    public List<GameObject> platformSource;
    public ObstacleSpawner obsSpawner;
    private Queue<GameObject> platforms;
    private float spawnInterval = 2f;

    public TextMeshProUGUI elapsedTime;

    public GameObject player;
    public GameObject deadPlane;
    public float SceneTransitionDelay = 0.3f;
    public float gametime;

    private float time_start;
    private float time_current;
    private float time_end = 60f;
    private float gameTimer = 10f;
    private int platformIndex=0;
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
        
        platforms = new Queue<GameObject>();

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
        elapsedTime.text = time_current.ToString();
       
        if (platforms.Count > 0)
        {
            GameObject frontPlatform = platforms.Peek();
            if (frontPlatform.transform.position.z < endPos.transform.position.z)
            {
       
                platforms.Dequeue();
                Destroy(frontPlatform);
       
            }
        }

        

        if(player.transform.position.y<deadPlane.transform.position.y )
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
        GameObject platform = Instantiate(platformSource[platformIndex], spawnPos, Quaternion.identity);
        platforms.Enqueue(platform);

        
    }


}
