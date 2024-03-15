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
    private int correctAnswerIndex;
    private int currentQuestionIndex = 0;
    private const int totalQuestions = 5;
    private List<int[]> prevQuestions = new List<int[]>();
    private int correctlyAnswered = 0;
    public GameObject pauseMenuPanel, completeGamePanel;
    public Button closeButton, restartButton, backButton, pauseButton, resumeButton, completeRestartButton, completeMenuButton;

    public Transform denominatorBarsParent;
    public GameObject denominatorBarPrefab;
    public Transform[] denominatorBars; // Assign in inspector
    public GameObject fishPrefab; // Assign in inspector
    public Transform numeratorBar; // Assign in inspector

    


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
        prevQuestions = new List<int[]>();
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
            bool used = false;
            while (flag == 0)
            {
                a = Random.Range(1, 10);
                b = Random.Range(2, 10);
                foreach (int[] question in prevQuestions)
                {
                    if (question[0] == a && question[1] == b)
                    {
                        used = true;
                        break; // Exit the loop if a repeat question is found
                    }
                }
                if (a % b == 0 && a != b && a/b!=1 && !used)
                {
                    answer = a / b;
                    flag = 1;
                }
            }
            prevQuestions.Add(new int[] {a, b});
            setupBarsAndFish(a,b);
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
