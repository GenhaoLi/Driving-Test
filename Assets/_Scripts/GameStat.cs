using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameStatus
{
    NeverStarted,
    Active,
    Paused,
    EndedForGood,
}

public class GameStat : MonoBehaviour
{
    float playerSpeed, collidingCarSpeed, notColldingCarSpeed;

    string infoTextContent;

    public static GameStatus GameStatus;

    static Button gameStatusButton, restartButton;

    static Text buttonText;

    public static Vector3 CRASH_SITE = new Vector3(92.5f, 0.1f, -43f);

    // BIAS is caused by the actual shape of the car
    public static float TTC_BIAS;

    public static float TTC_AT_START;

    public static float TTC_AT_PAUSE;

    static GameStat()
    {
        TTC_BIAS = 0.26f;
        TTC_AT_START = 5;
        TTC_AT_PAUSE = 0.5f;
    }

    Text infoText;
    private IEnumerator GameControl()
    {
        yield return new WaitForSeconds(TTC_AT_START - TTC_AT_PAUSE);
        PauseGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonText = GameObject.Find("ButtonText").GetComponent<Text>();
        gameStatusButton = GameObject.Find("GameStatusButton").GetComponent<Button>();
        restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        restartButton.gameObject.SetActive(false);

        infoText = GameObject.Find("InfoText").GetComponent<Text>();
        playerSpeed = collidingCarSpeed = notColldingCarSpeed = -1;

        // game status setting
        Time.timeScale = 0;
        GameStatus = GameStatus.NeverStarted;
        gameStatusButton.onClick.AddListener(StartGame);
        buttonText.text = "Start";

        StartCoroutine(GameControl());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update button listener
        //gameStatusButton.onClick.RemoveAllListeners();
        //switch (GameStatus)
        //{
        //    case GameStatus.NeverStarted:
        //        gameStatusButton.onClick.AddListener(StartGame);
        //        break;
        //    case GameStatus.Active:
        //        gameStatusButton.onClick.AddListener(PauseGame);
        //        break;
        //    case GameStatus.Paused:
        //        gameStatusButton.onClick.AddListener(StartGame);
        //        break;
        //    case GameStatus.EndedForGood:
        //        gameStatusButton.onClick.AddListener(RestartGame);
        //        break;
        //    default:
        //        break;
        //}

        // update info
        playerSpeed = GameObject.Find("Player").GetComponent<Rigidbody>().velocity.magnitude;
        collidingCarSpeed = GameObject.Find("Car1").GetComponent<Rigidbody>().velocity.magnitude;
        notColldingCarSpeed = GameObject.Find("Car2").GetComponent<Rigidbody>().velocity.magnitude;
        infoText.text = GetUpdatedInfoText(playerSpeed, collidingCarSpeed, notColldingCarSpeed);
    }

    void Update()
    {
        //print(Time.time);
        if (Time.time + TTC_AT_PAUSE == TTC_AT_START)
        {
            PauseGame();
        }
    }

    // default speed unit: m/s, unless stated in var name
    string GetUpdatedInfoText(float playerSpeed, float collidingCarSpeed, float notColldingCarSpeed = -1)
    {
        var infoText = $"Your speed: {(playerSpeed * 3.6).ToString("0.00")} km/h \n"
            + $"Colliding car speed: {(collidingCarSpeed * 3.6).ToString("0.00")} km/h \n"
            + $"Not colliding car speed: {(notColldingCarSpeed * 3.6).ToString("0.00")} km/h \n"
            + $"Time elapsed: {Time.time.ToString("0.00")} s";
        return infoText;
    }

    public static void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public static void StartGame()
    {
        Time.timeScale = 1;
        GameStatus = GameStatus.Active;
        restartButton.gameObject.SetActive(false);
        gameStatusButton.onClick.RemoveAllListeners();
        buttonText.text = "Pause";
        gameStatusButton.onClick.AddListener(PauseGame);
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
        GameStatus = GameStatus.Paused;
        restartButton.gameObject.SetActive(true);
        gameStatusButton.onClick.RemoveAllListeners();
        buttonText.text = "Resume";
        gameStatusButton.onClick.AddListener(StartGame);
    }



}
