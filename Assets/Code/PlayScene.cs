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
    public GameObject pauseMenuPanel, completeGamePanel;
    public Button closeButton, restartButton, backToMainMenuButton, pauseButton, resumeButton, playAgainButton, pauseExitToMainMenuButton;

    public Transform denominatorBarsParent;
    public GameObject denominatorBarPrefab;
    public Transform[] denominatorBars; // Assign in inspector
    public GameObject fishPrefab; // Assign in inspector
    public Transform numeratorBar; // Assign in inspector

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
        stageText.text = $"Stage: {currentQuestionIndex + 1}/{totalQuestions}";
        questionsText.text = $"{problem.numerator} ÷ {problem.denominator} = ?";
        setupBarsAndFish(problem.numerator, problem.denominator);

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

    void setupBarsAndFish(int a, int b){
        GenerateAndArrangeDenominatorBars(b);
        ArrangeFishInNumerator(a);
        StartCoroutine(SplitAndMoveFish(b));
    }

    void ArrangeFishInNumerator(int count)
    {
        foreach(Transform child in numeratorBar)
        {
            Destroy(child.gameObject); // Clear existing fish
        }

        for (int i = 0; i < count; i++)
        {
            GameObject fish = Instantiate(fishPrefab, numeratorBar);
        }
        
    }

    IEnumerator SplitAndMoveFish(int denominator)
    {
        yield return new WaitForSeconds(2); // Delay for a few seconds

        int childrenCount = numeratorBar.childCount;
        int perGroup = childrenCount / denominator;

        for (int i = 0; i < denominator; i++)
        {
            for (int j = 0; j < perGroup; j++)
            {
                if (numeratorBar.childCount > 0)
                {
                    Transform fish = numeratorBar.GetChild(0);
                    fish.SetParent(denominatorBars[i]);
                }
            }
        }
    }

    void GenerateAndArrangeDenominatorBars(int denominator)
    {
        // Clear existing bars
        foreach (Transform child in denominatorBarsParent)
        {
            Destroy(child.gameObject);
        }

        // Calculate spacing and size based on the parent's width and the number of bars
        float parentWidth = denominatorBarsParent.GetComponent<RectTransform>().sizeDelta.x;


        denominatorBars = new Transform[denominator]; // Reinitialize array based on denominator

        for (int i = 0; i < denominator; i++)
        {
            GameObject bar = Instantiate(denominatorBarPrefab, denominatorBarsParent);

            denominatorBars[i] = bar.transform; // Store the reference
        }
    }

    void ResetButtonColors()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Image>().color = new Color32(255, 255, 255, 255); // Set the color back to default
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
        AudioController.instance.PlayPanelSoundAndResumeBackgroundMusic();
        completeGamePanel.SetActive(true);
        double accuracy = ((double)correctlyAnswered / totalQuestions) * 100;
        accuracyText.text = "Accuracy:" + $"{accuracy}%";
        wrongText.text = "Wrong:" + (totalQuestions - correctlyAnswered);
        foreach(Transform child in numeratorBar)
        {
            Destroy(child.gameObject); // Clear existing fish
        }
        foreach (Transform child in denominatorBarsParent)
        {
            Destroy(child.gameObject);
        }
        DisableGameInputs();
    }

    void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        DisableGameInputs();
    }

    void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
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
        EnableGameInputs();
        LoadNextProblem();
    }
}
