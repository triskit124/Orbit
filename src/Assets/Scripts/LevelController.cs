using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // needed to switch scenes

public class LevelController : MonoBehaviour {

    public int levelNumber;
    public int numPlanetsToPlace;
    public int livesAlloted;
    public float timeNeededToPass;
    public static bool levelPaused;
    public Text livesRemainingText;
    public Text timeRemainingText;
    public Text numPlanetsToPlaceText;
    public GameObject GUIHolder;
    public GameObject gameLossGUIHolder;
    public GameObject gameWinGUIHolder;
    public GameObject gamePausedGUIHolder;
    public GameObject[] levelSelectionBodies;

    int planetsLeft;
    int livesLeft;
    float timeLeft;
    bool gameLoss = false;
    bool gameWin = false;
    bool startingCountdown = false;
    static AudioController audioController;
    static float timeSinceClick;
    static float clickTime = 0.2f;


    void Awake () {

        //PlayerPrefs.SetInt("level", 0);

        audioController = FindObjectOfType<AudioController>();

        planetsLeft = numPlanetsToPlace;
        livesLeft = livesAlloted;
        timeLeft = timeNeededToPass;
        
        UpdateGUIFields();

        if (SceneManager.GetActiveScene().name == "LevelSelection") {
            LevelUnlockManager();
        }
    }

    void Update() {

        timeSinceClick += Time.deltaTime;

        // if there are enough planets placed, start counting dowm
        if (planetsLeft <= 0 && timeLeft > 0) {
            if (!levelPaused && !gameLoss && !gameWin) {
                if (!startingCountdown) {
                    audioController.Play("start_timer_sound");
                    startingCountdown = true;
                }
                timeLeft -= Time.deltaTime;
                UpdateGUIFields();
            }
        // else, reset time left if needed
        } else if (timeLeft != timeNeededToPass) {
            timeLeft = timeNeededToPass; // reset time left 
            UpdateGUIFields();
            startingCountdown = false;
        }

        // LEVEL WIN CONDITION, time left is zero
        if (timeLeft <= 0f && !gameLoss && !gameWin) {
            audioController.Play("level_win_sound");
            gameWin = true;
            gameWinGUIHolder.SetActive(true);
            GUIHolder.SetActive(false);

            int currentLevelsUnlocked = PlayerPrefs.GetInt("level");

            if (currentLevelsUnlocked < levelNumber + 1) {
                PlayerPrefs.SetInt("level", levelNumber + 1); // set a Global variable to keep track of which levels have been unlocked
            }
        }

        // LEVEL LOSS CONDITION, lives left is zero
        if (livesLeft <= 0 && !gameWin && !gameLoss) {
            audioController.Play("level_loss_sound");
            gameLoss = true;
            gameLossGUIHolder.SetActive(true);
            GUIHolder.SetActive(false);
        }

    }

    void OnValidate() {
        UpdateGUIFields();
    }

    // handles loosing a planet
    public void PlanetLost () {
        audioController.Play("planet_lost");
        if (livesLeft > 0) {
            livesLeft--;
        }
        planetsLeft++;
        timeLeft = timeNeededToPass;

        if (planetsLeft > 0) { 
            startingCountdown = false;
        }

        UpdateGUIFields();
    }

    // handles loosing a sun
    public void SunLost () {
        if (livesLeft > 0) {
            livesLeft = 0;
        }
        //timeLeft = timeNeededToPass;
        //UpdateGUIFields();
    }

    // handles when a planet is placed
    public void PlanetPlaced () {
        audioController.Play("planet_placed");
        planetsLeft--;
        timeLeft = timeNeededToPass; // reset timer when a new planet is placed
        UpdateGUIFields();
    }

    // updates the text fields in the gui
    void UpdateGUIFields () {
        livesRemainingText.text = livesLeft.ToString();
        timeRemainingText.text = timeLeft.ToString();
        numPlanetsToPlaceText.text = Mathf.Max(0, planetsLeft).ToString();
    }

    public void ResetLevel() {
        buttonClick();

        /*
        if (gameLoss) {
            gameLoss = false;
            gameLossGUIHolder.SetActive(false);
        }
        */

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reload the level
    }

    public void NextLevel() {
        buttonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // next level
    }

    public void PauseLevel() {
        buttonClick();
        levelPaused = true;
        gamePausedGUIHolder.SetActive(true);
    }

    public void UnPauseLevel() {
        buttonClick();
        levelPaused = false;
        gamePausedGUIHolder.SetActive(false);
    }

    public static void GoToMainMenu() {
        buttonClick();
        SceneManager.LoadScene("MainMenu");
        levelPaused = false; // since you can get to main menu from pause screen, need to unpause
    }

    public static void GoToLevelSelection() {
        buttonClick();
        SceneManager.LoadScene("LevelSelection");
    }

    public static void GoToAboutPage() {
        buttonClick();
        SceneManager.LoadScene("About");
    }

    public static void GoToLevel(string levelString) {
        buttonClick();
        SceneManager.LoadScene("Level" + levelString);
    }

    private void LevelUnlockManager() {
        int levelsUnlocked = PlayerPrefs.GetInt("level");

        // if this is the first time playing the game, unlock level 1
        if (levelsUnlocked == 0) {
            PlayerPrefs.SetInt("level", 1);
            levelsUnlocked = 1;
        }

        if (levelsUnlocked > levelSelectionBodies.Length) {
            PlayerPrefs.SetInt("level", levelSelectionBodies.Length);
            levelsUnlocked = levelSelectionBodies.Length;
        }

        for (int i = 0; i < levelsUnlocked; i++) {
            levelSelectionBodies[i].SetActive(true);
        }
    }

    private static void buttonClick() {
        if (timeSinceClick > clickTime) {
            audioController.Play("button_click");
        }
        timeSinceClick = 0f;
    }

}
