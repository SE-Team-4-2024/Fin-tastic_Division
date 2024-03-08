using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure to add this for TextMeshPro support
using UnityEngine.SceneManagement;


public class GameLogic : MonoBehaviour
{
    public TextMeshProUGUI questionText, stageText, accuracyText, wrongText;
    public Button[] answerButtons;
    //public GameObject barPrefab; // Prefab for the bars
    //public GameObject fishPrefab; // Prefab for the fish images
    //private List<GameObject> bars = new List<GameObject>(); // To keep track of bars
    private int correctAnswerIndex;
    private int currentQuestionIndex = 0;
    private const int totalQuestions = 5;
    private int correctlyAnswered = 0;
    public GameObject pauseMenuPanel, completeGamePanel;
    public Button closeButton, restartButton, backButton, pauseButton, resumeButton, completeRestartButton, completeMenuButton;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        completeGamePanel.SetActive(false);
        pauseButton.onClick.AddListener(PauseGame);
        closeButton.onClick.AddListener(ResumeGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        backButton.onClick.AddListener(BackToMainMenu);
        completeRestartButton.onClick.AddListener(CompleteRestartGame);
        completeMenuButton.onClick.AddListener(BackToMainMenu);
        GenerateQuestion();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateQuestion()
    {
        if (currentQuestionIndex >= totalQuestions)
        {
            // Optionally, handle the end of the quiz here
            foreach (var button in answerButtons)
            {
                button.onClick.RemoveAllListeners();
                button.GetComponent<Image>().color = Color.white; // Reset to default or another color
            }
            CompleteGame();
        }
        else
        {
            stageText.text = $"Stage: {currentQuestionIndex + 1}/{totalQuestions}";

            // Clear previous listeners and reset button colors
            foreach (var button in answerButtons)
            {
                button.onClick.RemoveAllListeners();
                button.GetComponent<Image>().color = Color.white; // Reset to default or another color
            }

            int a = 1;
            int b = 1;
            int answer = 1;
            int flag = 0;
            while (flag == 0)
            {
                a = Random.Range(1, 100);
                b = Random.Range(2, 10);
                if (a % b == 0 && a != b)
                {
                    answer = a / b;
                    flag = 1;
                }
            }
            //SetupBarsAndFish(a, b);
            // Ensure the division has an integer result
            questionText.text = $"{a} ÷ {b} = ?"; // Display problem

            correctAnswerIndex = Random.Range(0, answerButtons.Length);
            HashSet<int> usedAnswers = new HashSet<int> { answer }; // To track unique answers

            for (int i = 0; i < answerButtons.Length; i++)
            {
                int option;
                if (i == correctAnswerIndex)
                {
                    option = answer;
                }
                else
                {
                    do
                    {
                        option = Random.Range(1, 20); // Adjust range as needed to avoid generating the correct answer
                    } while (usedAnswers.Contains(option)); // Ensure uniqueness

                    usedAnswers.Add(option); // Add the newly generated option to the set
                }

                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = option.ToString();

                int index = i; // Local copy for the closure below
                answerButtons[i].onClick.AddListener(() => AnswerSelected(index));
            }
        }
    }

    void AnswerSelected(int index)
    {
        bool isCorrect = index == correctAnswerIndex;
        answerButtons[index].GetComponent<Image>().color = isCorrect ? Color.green : Color.red;
        correctlyAnswered += isCorrect ? 1 : 0;

        // Wait a little before continuing to the next question
        StartCoroutine(ContinueAfterFeedback(isCorrect, index));
    }

    IEnumerator ContinueAfterFeedback(bool isCorrect, int index)
    {
        // Provide some feedback time for the user to see the color change
        yield return new WaitForSeconds(1); // 1 second wait

        // Optionally reset color or leave it changed as feedback
        // answerButtons[index].GetComponent<Image>().color = Color.white;

        if (isCorrect)
        {
            currentQuestionIndex++;
            GenerateQuestion();
        }
        else
        {
            // Handle incorrect answer if needed
            // This could involve showing a message, ending the game, etc.
            // For simplicity, we'll just proceed to the next question.
            currentQuestionIndex++;
            GenerateQuestion();
        }
    }
    // void SetupBarsAndFish(int numerator, int denominator)
    // {
    //     ClearBars(); // Clear existing bars and fish

    //     int fishPerBar = numerator / denominator;
    //     // Create the answer bars
    //     for (int i = 0; i <= denominator; i++) // Include an extra bar for the numerator
    //     {
    //         GameObject bar = Instantiate(barPrefab, parentTransform); // Specify the parent transform as needed
    //         bars.Add(bar);

    //         if (i == 0) // If it's the numerator bar
    //         {
    //             for (int j = 0; j < numerator; j++)
    //             {
    //                 Instantiate(fishPrefab, bar.transform); // Add all fish to the numerator bar
    //             }
    //         }
    //         else // Answer bars
    //         {
    //             for (int j = 0; j < fishPerBar; j++)
    //             {
    //                 Instantiate(fishPrefab, bar.transform); // Distribute fish according to the division answer
    //             }
    //         }
    //     }
    // }

    // void ClearBars()
    // {
    //     foreach (GameObject bar in bars)
    //     {
    //         Destroy(bar); // Clear previous bars and fish
    //     }
    //     bars.Clear();
    // }
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Show pause menu
        DisableGameInputs();
        // Optionally disable game inputs here
    }
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Hide pause menu
        EnableGameInputs();
        // Optionally re-enable game inputs here
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("HomeScreen");
    }
    public void RestartGame()
    {
        currentQuestionIndex = 0;
        correctlyAnswered = 0;
        ResumeGame();
        GenerateQuestion();
    }
    void DisableGameInputs()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = false; // Make other buttons non-interactable
        }
        pauseButton.interactable = false;
        // Disable other input methods as needed
    }

    void EnableGameInputs()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = true; // Make buttons interactable again
        }
        pauseButton.interactable = true;
        // Re-enable other input methods as needed
    }
    void CompleteGame()
    {
        completeGamePanel.SetActive(true);
        double accuracy = ((double)correctlyAnswered / totalQuestions) * 100;
        accuracyText.text = (accuracy) + "%";
        wrongText.text = "" + (totalQuestions - correctlyAnswered);
        DisableGameInputs();
    }
    public void CompleteRestartGame()
    {
        currentQuestionIndex = 0;
        correctlyAnswered = 0;
        completeGamePanel.SetActive(false); // Hide pause menu
        EnableGameInputs();
        GenerateQuestion();
    }
}
