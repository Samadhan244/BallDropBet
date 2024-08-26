using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] AudioSource audioSource; [SerializeField] AudioClip ballFinishSound, buttonSound, newHighScoreSound;
    [SerializeField] Image[] buttonSeleced;
    [SerializeField] Ball[] balls;
    [SerializeField] Rotator[] rotators;
    [SerializeField] Mover[] movers;
    [SerializeField] Hideable[] hideables;
    [SerializeField] Spinner[] spinners;
    [SerializeField] GameObject currentWinningBall, main, gameOver, musOn, musOff;
    [SerializeField] TextMeshProUGUI timerText, backgroundChosenText, livesText, placesText, scoreText, eventText, newHighScoreText;
    [SerializeField] int lives = 3, finishersCount, score, highScore, gainedScore, backgorundChosen = 1, backgroundColor;
    Ball chosenBall;
    float timer; bool timerActive, highestScoreReached;

    void Start()
    {
        livesText.text = lives.ToString();
        backgroundChosenText.text = backgorundChosen + "/12";
        highScore = PlayerPrefs.GetInt("HighScore", 50);  // Load our highest score
        BallChoosing(0);  // Default chosen Ball will be Red at the game start
    }

    void Update()
    {
        timerText.text = (timerActive ? timer += Time.deltaTime : timer).ToString("F1");

        if (!currentWinningBall) currentWinningBall = balls[0].gameObject;  // To avoid NullReference bug
        foreach (Ball x in balls) if (currentWinningBall && x.transform.position.y < currentWinningBall.transform.position.y) currentWinningBall = x.gameObject;  // Check which Ball is winning(whoever is lowest)
        if (currentWinningBall && mainCamera) mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, currentWinningBall.transform.position.y, mainCamera.transform.position.z);  // Camera follows winner ball
    }

    public int FinishersCount() { return finishersCount; }

    public void PressToStart()  // Fresh start, the Ball is chosen
    {
        audioSource.PlayOneShot(buttonSound);
        placesText.text = "      Places:";
        eventText.gameObject.SetActive(false);
        newHighScoreText.gameObject.SetActive(false);
        finishersCount = 0;
        timer = 0;
        timerActive = true;

        // Teleport the Balls to starting position's random radius. All the Rotator/Mover/Hideable/Spinner obstacles will have a random stats(rotation speed, rotation direction, moving speed, fade speed) every time
        foreach (var x in balls) x.Teleport();
        foreach (var x in rotators) x.Randomizer();
        foreach (var x in movers) x.Randomizer();
        foreach (var x in hideables) x.Randomizer();
        foreach (var x in spinners) x.Randomizer();
        Invoke(nameof(DisableMain), 0.1f);  // Disable Balls choosing menu. We need a delay to avoid bug
    }

    public void NewGame()
    {
        lives = 3;
        score = 0;
        livesText.text = lives.ToString();
        scoreText.text = $"Chosen: <color={ReturnBallColor(chosenBall.name)}>{chosenBall.name}</color>\nScore: {score}\nH Score: {highScore}";
        placesText.text = "      Places:";
        eventText.gameObject.SetActive(false);
        main.SetActive(true);
        Invoke(nameof(DisableGameOver), 0.1f);  // Disable GameOver menu. We need a delay to avoid bug
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    // Menus to disable
    void DisableMain() { main.SetActive(false); }
    void DisableGameOver() { gameOver.SetActive(false); }

    public void Finished(string ballName)
    {
        audioSource.PlayOneShot(ballFinishSound);
        finishersCount += 1;
        placesText.text += $"\n{finishersCount}. <color={ReturnBallColor(ballName)}>{ballName}</color> ({timer.ToString("F1")})";

        if (finishersCount == 5)  // Final result(When every Ball finishes)
        {
            eventText.gameObject.SetActive(true);
            int place = chosenBall.place;
            gainedScore = place == 1 ? 100 : place == 2 ? 60 : place == 3 ? 40 : place == 4 ? 20 : 10;
            score += gainedScore;
            lives += place == 1 ? 1 : (place == 2 || place == 3) ? 0 : -1;
            eventText.text = lives <= 0 ? $"Game over! Your score is: {score}" : place == 1 ? $"1st Place! Life + 1" : place == 2 ? $"2nd Place!" : place == 3 ? $"3rd Place!" : place == 4 ? $"4th Place! Life - 1" : $"5th Place! Life - 1";

            timerActive = false;
            livesText.text = lives.ToString();
            scoreText.text = $"Chosen: <color={ReturnBallColor(chosenBall.name)}>{chosenBall.name}</color>\nScore: {score} (<color=green>+{gainedScore}</color>)\nH Score: {highScore}";
            if (!highestScoreReached && score > highScore)  // New high score
            {
                audioSource.PlayOneShot(newHighScoreSound);
                newHighScoreText.gameObject.SetActive(true);
                highestScoreReached = true;
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }

            // If we have enought lives, we continue the game or esle we game over
            if (lives > 0) main.SetActive(true);
            else gameOver.SetActive(true);
        }
    }

    string ReturnBallColor(string ballName)
    {
        return ballName.ToLowerInvariant() switch
        {
            "red" => "#FF0000",
            "cyan" => "#00FFFF",
            "green" => "#00FF00",
            "purple" => "#FF00FF",
            "yellow" => "#FFFF00",
            _ => "#000000" // Default color if the color name is not recognized
        };
    }

    public void MusicOnOff()  // Button to toggle music on or off
    {
        musOff.SetActive(!musOff.activeSelf);
        musOn.SetActive(!musOff.activeSelf);
        AudioListener.volume = musOn.activeSelf ? 1 : 0;
        audioSource.PlayOneShot(buttonSound);
    }

    public void BackgroundColorChanger()  // Button to change the background color
    {
        Color[] colors = new Color[]
        {
            new Color(0f, 0f, 0.8f),      // Blue
            new Color(0f, 0.5f, 1f),      // Light Blue
            new Color(0f, 0.5f, 0.6f),    // Teal
            new Color(0f, 0.7f, 0f),      // Green
            new Color(0.5f, 0.9f, 0f),    // Lime Green
            new Color(0.8f, 0.8f, 0f),    // Yellow
            new Color(0.9f, 0.5f, 0f),    // Orange
            new Color(0.7f, 0f, 0f),      // Red
            new Color(0.8f, 0f, 0.8f),    // Magenta
            new Color(0.5f, 0f, 1f),      // Purple
            new Color(0.5f, 0.5f, 0.5f),  // Gray
            new Color(0.1f, 0.1f, 0.1f),  // Black
        };
        backgorundChosen = backgorundChosen == 12 ? 1 : backgorundChosen + 1;
        backgroundChosenText.text = backgorundChosen + "/12";
        backgroundColor = (backgroundColor + 1) % colors.Length;
        mainCamera.backgroundColor = colors[backgroundColor];
        audioSource.PlayOneShot(buttonSound);
    }

    public void BallChoosing(int index)  // Button to select a ball in the "main" menu
    {
        chosenBall = balls[index];
        foreach (Image x in buttonSeleced) x.color = new Color(x.color.r, x.color.g, x.color.b, 128f / 255f);  // Dim all non-selected button images
        buttonSeleced[index].color = new Color(buttonSeleced[index].color.r, buttonSeleced[index].color.g, buttonSeleced[index].color.b, 255f / 255f);  // Highlight the selected button image
        scoreText.text = $"Chosen: <color={ReturnBallColor(chosenBall.name)}>{chosenBall.name}</color>\nScore: {score}\nH Score: {highScore}";  // Display the chosen ball's name with its color and the current and highest score
    }
}