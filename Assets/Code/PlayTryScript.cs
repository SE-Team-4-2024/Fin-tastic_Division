using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayTryScript : MonoBehaviour
{
    public TextMeshProUGUI questionsText, stageText, accuracyText, wrongText;
    public Button[] answerButtons;
    public GameObject pauseMenuPanel, completeGamePanel;
    public Button closeButton, restartButton, backToMainMenuButton, pauseButton, resumeButton, playAgainButton, pauseExitToMainMenuButton;

    public GameObject fishPrefab; // Assign in inspector
    public GameObject denoBarPrefab;
    public GameObject numeratorPanel; // Assign in inspector
    public GameObject numeratorBarPanel;
    public GameObject FakeDenoPanel;
    private List<GameObject> denominatorPanels = new List<GameObject>();
    
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

        CreateNewGame();
        LoadNextProblem(); // Start loading the first problem
    }

    void CreateNewGame()
    {
        string userID = "johnWick_12"; // To change in later phase, once profile page is built up.
        Debug.Log("Creating New Game for the userID" + userID);

        // Call the asynchronous method and pass onSuccess and onError callbacks
        StartCoroutine(GameManager.CreateNewGame(userID, onSuccess, OnError));
    }

    void onSuccess(string gameID){
        Debug.Log("Game Succesfully created with gameId"+ gameID);
        PlayerPrefs.SetString("gameID", gameID);
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
        stageText.text = $"{currentQuestionIndex + 1}/{totalQuestions}";
        questionsText.text = $"{problem.numerator} ÷ {problem.denominator} = ?";

        ClearForNewQuestion(); // Clear existing fish and panels
        GenerateFish(problem.numerator);
        StartCoroutine(WaitAndHold());
        denominatorPanels = CreateDenominatorPanels(problem.denominator);
        StartCoroutine(MoveFishToDenominatorPanels(denominatorPanels));
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
    IEnumerator WaitAndHold()
    {
        Debug.Log("Running Func");
        yield return new WaitForSeconds(5);
    }


    void GenerateFish(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject fish = Instantiate(fishPrefab, numeratorBarPanel.transform);

            // Ensure it's being instantiated as a UI element with RectTransform
            RectTransform rectTransform = fish.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Correct the RectTransform settings to match the GridLayout
                rectTransform.localScale = new Vector3(1.2f,1.2f,1f);
                rectTransform.anchoredPosition3D = Vector3.zero; // Anchored position for UI
                rectTransform.sizeDelta = new Vector2(100, 100); // Or the original size of the fish prefab
            }
            else
            {
                // If there's no RectTransform, then it might not be a UI element
                Debug.LogError("The fish prefab does not have a RectTransform component, necessary for UI elements.");
            }
        }
        // If you're dynamically enabling/disabling GridLayoutGroup, do it here
        LayoutRebuilder.ForceRebuildLayoutImmediate(numeratorBarPanel.GetComponent<RectTransform>());
    }

    List<GameObject> CreateDenominatorPanels(int denominator)
    {
        List<GameObject> panels = new List<GameObject>();
        for (int i = 0; i < denominator; i++)
        {
            GameObject denoBar = Instantiate(denoBarPrefab, numeratorPanel.transform);
            GameObject fakeBar = Instantiate(denoBarPrefab, FakeDenoPanel.transform);
            panels.Add(denoBar);
        }
        return panels;
    }

    IEnumerator MoveFishToDenominatorPanels(List<GameObject> denominatorPanels)
    {
        int fishIndex = 0;
        List<RectTransform> fish = new List<RectTransform>();

        // Assume fish are direct children of numeratorPanel initially
        foreach (RectTransform child in numeratorBarPanel.GetComponent<RectTransform>())
        {
            if (child.gameObject.CompareTag("Fish")) // Make sure to tag your fish prefab as "Fish"
            {
                fish.Add(child);
            }
        }

        // This assumes an even distribution of fish across panels
        int fishPerPanel = fish.Count / denominatorPanels.Count;

        foreach (var panel in denominatorPanels)
        {
            RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
            for (int i = 0; i < fishPerPanel && fishIndex < fish.Count; i++, fishIndex++)
            {
                RectTransform fishRectTransform = fish[fishIndex];
                StartCoroutine(MoveFish(fishRectTransform, panelRectTransform));
                yield return new WaitForSeconds(0.1f); // Stagger the movement for visual effect
            }
        }
    }


    IEnumerator MoveFish(RectTransform fish, RectTransform targetPanel)
    {
        // Ensure we are working with the RectTransform's anchored position for UI movement
        Vector2 startAnchoredPosition = fish.anchoredPosition;
        Vector2 endAnchoredPosition = targetPanel.anchoredPosition; // This should be the anchored position within the panel

        float duration = 1.0f; // Duration of the move in seconds
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Interpolating the anchoredPosition to move the fish
            //fish.anchoredPosition = Vector2.Lerp(startAnchoredPosition, endAnchoredPosition, t);
            yield return null;
        }

        // Set the fish as a child of the target panel
        fish.SetParent(targetPanel, false);
        //fish.anchoredPosition3D = Vector3.zero; // Reset local position for the GridLayoutGroup to take over
        fish.localScale = new Vector3(1.2f,1.2f,1f); // Reset local scale

        // Now let the GridLayoutGroup arrange it
        GridLayoutGroup gridLayout = targetPanel.GetComponentInParent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetPanel);
        }
    }



    void ClearForNewQuestion()
    {
        // Clear fish
        foreach (RectTransform child in numeratorPanel.transform)
        {
            if (child.gameObject.CompareTag("Fish"))
            {
                Destroy(child.gameObject);
            }
        }

        // Clear denominator panels
        foreach (RectTransform child in numeratorPanel.transform)
        {
            if (child.gameObject.CompareTag("DenominatorBar"))
            {
                Destroy(child.gameObject);
            }
        }
        // Clear fake denominator panels
        foreach (RectTransform child in FakeDenoPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }


    void ResetButtonColors()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            //button.GetComponent<Image>().color = Color.white;
            //button.GetComponent<Image>().color = new Color32(0, 0, 0, 200); // Set the color back to default
        }
    }

    void AnswerSelected(int index)
    {
        // Reset button colors before processing the selected answer
        ResetButtonColors();

        bool isCorrect = index == correctAnswerIndex;
        answerButtons[index].GetComponent<Image>().color = isCorrect ? Color.green : Color.red;
        correctlyAnswered += isCorrect ? 1 : 0;

        // To update the user response in database
        StartCoroutine(UpdateUserResponseCoroutine(isCorrect)); 
        StartCoroutine(ContinueAfterFeedback(isCorrect, index));
    }

    IEnumerator UpdateUserResponseCoroutine(bool isCorrect)
    {
        string gameID = PlayerPrefs.GetString("gameID");                                        
        yield return GameManager.UpdateUserResponse(gameID, isCorrect, onSuccessfulUpdate, OnError);
    }

    void onSuccessfulUpdate(bool success){
        Debug.Log("User response updation"+ success);
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
        ClearForNewQuestion();
        //AudioController.instance.PlayPanelSoundAndResumeBackgroundMusic();
        completeGamePanel.SetActive(true);
        double accuracy = ((double)correctlyAnswered / totalQuestions) * 100;
        accuracyText.text = "Accuracy:" + $"{accuracy}%";
        wrongText.text = "Wrong:" + (totalQuestions - correctlyAnswered);
        StartCoroutine(UpdateGameCompletionStats(accuracy, 90)); // to update the accuracy and completion rate, once completion rate is done, need to remove hardcoded once..
        DisableGameInputs();
    }

    IEnumerator UpdateGameCompletionStats(double accuracy, double completionRate)
    {
        string gameID = PlayerPrefs.GetString("gameID");
        // Define the success and error callbacks
        System.Action<bool> onSuccess = (bool success) =>
        {
            // Handle onSuccess callback if needed
            Debug.Log("Updated in database as game completed");
        };
        yield return GameManager.UpdateGameCompletedStats(gameID, accuracy, completionRate, onSuccess, OnError);
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
        CreateNewGame();
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
        CreateNewGame();
        LoadNextProblem();
    }
}
