using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayScene : MonoBehaviour
{
    public TextMeshProUGUI questionsText, stageText, accuracyText, wrongText;
    public Button[] answerButtons;
    public GameObject pauseMenuPanel, completeGamePanel;   // backgroundOverlayPanel;
    public Button closeButton, restartButton, backToMainMenuButton, pauseButton, resumeButton, playAgainButton, pauseExitToMainMenuButton;

    public RectTransform horizontalBar; // Reference to the horizontal bar panel
    public GameObject fishPrefab;
    //private GameObject backgroundOverlayPanel;
    private int correctAnswerIndex;
    private int currentQuestionIndex = 0;
    private const int totalQuestions = 5;
    private int correctlyAnswered = 0;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        completeGamePanel.SetActive(false);
        pauseButton.onClick.AddListener(PauseGame);
        closeButton.onClick.AddListener(ResumeGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        playAgainButton.onClick.AddListener(PlayAgainButton);
        pauseExitToMainMenuButton.onClick.AddListener(BackToMainMenu);

        LoadNextProblem(); // Start loading the first problem
    }

    void LoadNextProblem()
    {
        StartCoroutine(DivisionProblemLoader.LoadDivisionProblem(DisplayProblem, OnError));
    }

    void OnError(string error)
    {
        Debug.LogError(error);
    }

    void DisplayProblem(DivisionProblem problem)
    {
        // Reset colors of option buttons
        ResetButtonColors();
        stageText.text = $" {currentQuestionIndex + 1}/{totalQuestions}";
        questionsText.text = $"{problem.numerator} / {problem.denominator} ?";
         // Instantiate fishes for the numerator
        // GenerateFishes(problem.numerator);

        // Ensure that the correct index is within the range of options
        correctAnswerIndex = Mathf.Clamp(problem.correct_option_index, 0, problem.options.Count - 1);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int optionIndex = i; // Store the current index

            // Check if the option index is within the range of available options
            if (optionIndex < problem.options.Count)
            {
                int option = problem.options[optionIndex];
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = option.ToString();
            }
            else
            {
                // If the option index exceeds the number of available options, hide the button
                answerButtons[i].gameObject.SetActive(false);
            }

            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => AnswerSelected(optionIndex));
        }
    }

    // void GenerateFishes(int numerator)
    // {
    //     // Calculate spacing between each fish image
    //     float barWidth = horizontalBar.rect.width;
    //     float spacing = barWidth / numerator;

    //     // Instantiate fish images
    //     for (int i = 0; i < numerator; i++)
    //     {
    //         // Instantiate fish image
    //         GameObject fish = Instantiate(fishPrefab, horizontalBar);

    //         // Calculate position of the fish image within the horizontal bar
    //         float xPos = -barWidth / 2f + spacing * (i + 0.5f);
    //         fish.transform.localPosition = new Vector3(xPos, 0f, 0f);
    //     }
    // }



    void ResetButtonColors()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white; // Set the color back to default
        }
    }

    void AnswerSelected(int index)
    {
        // Reset button colors before processing the selected answer
        ResetButtonColors();

        bool isCorrect = index == correctAnswerIndex;
        answerButtons[index].GetComponent<Image>().color = isCorrect ? Color.green : Color.red;
        correctlyAnswered += isCorrect ? 1 : 0;

        StartCoroutine(ContinueAfterFeedback(isCorrect, index));
    }


    IEnumerator ContinueAfterFeedback(bool isCorrect, int index)
    {
        yield return new WaitForSeconds(1);

        currentQuestionIndex++;
        if (currentQuestionIndex < totalQuestions)
        {
            LoadNextProblem();
        }
        else
        {
            CompleteGame();
        }
    }

    void CompleteGame()
    {
        completeGamePanel.SetActive(true);
        double accuracy = ((double)correctlyAnswered / totalQuestions) * 100;
        accuracyText.text = "Accuracy:" + $"{accuracy}%";
        wrongText.text = "Wrong:" + (totalQuestions - correctlyAnswered);
        DisableGameInputs();
    }

    void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
       // backgroundOverlayPanel.SetActive(true);
        DisableGameInputs();
    }

    void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
      //  backgroundOverlayPanel.SetActive(false);
        EnableGameInputs();
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("HomeViewController");
    }

    void RestartGame()
    {
        currentQuestionIndex = 0;
        correctlyAnswered = 0;
        ResumeGame();
        LoadNextProblem();
    }

    void DisableGameInputs()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }
        pauseButton.interactable = false;
    }

    void EnableGameInputs()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = true;
        }
        pauseButton.interactable = true;
    }

    void PlayAgainButton()
    {
        currentQuestionIndex = 0;
        correctlyAnswered = 0;
        completeGamePanel.SetActive(false);
        pauseButton.interactable = true;
        EnableGameInputs();
        LoadNextProblem();

    }
}
