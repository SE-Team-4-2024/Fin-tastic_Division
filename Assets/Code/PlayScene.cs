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
    public GameObject fishPrefab; 
    public GameObject denominatorBarPanelPrefab; // Prefab for denominator bar panel
    private int correctAnswerIndex;
    private int currentQuestionIndex = 0;
    private const int totalQuestions = 5;
    private int correctlyAnswered = 0;
    public RectTransform numeratorBarPanel; // Reference to the numerator bar panel
    private List<RectTransform> denominatorBarPanels = new List<RectTransform>(); // List to store references to denominator bar panels

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
        GenerateFishes(problem.numerator, () =>
        {
            // Callback to trigger after all fishes have finished animating
            // Generate denominator bar panels and animate fishes to move to the correct denominator bar
            GenerateDenominatorBarPanels(problem.denominator);
            AnimateFishesToDenominator(problem.correct_option_index, problem.denominator);
        });

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
       // Generate denominator bar panels
        GenerateDenominatorBarPanels(problem.denominator);
        
    }

   void GenerateFishes(int numerator, System.Action onComplete)
{
    Debug.Log("Generating fishes. Numerator: " + numerator);

    // Clear existing fish objects in the numeratorBarPanel
    foreach (Transform child in numeratorBarPanel.transform)
    {
        GameObject.Destroy(child.gameObject);
    }

    // Calculate spacing between each fish image
    float panelWidth = numeratorBarPanel.rect.width;
    float spacing = panelWidth / numerator;

    Debug.Log("Panel width: " + panelWidth);
    Debug.Log("Spacing between fishes: " + spacing);

    // Keep track of the number of fishes instantiated
    int fishesInstantiated = 0;

    // Define the animation duration for each fish
    float animationDuration = 0.5f; // Adjust as needed

    // Define the delay before moving fishes to denominator bar panels
    float delayBeforeMoving = 1.0f; // 1 second delay

    // Instantiate and animate fish images
    for (int i = 0; i < numerator; i++)
    {
        // Instantiate fish image from the prefab
        GameObject fish = Instantiate(fishPrefab, numeratorBarPanel);

        // Calculate position of the fish image outside the panel
        float startXPos = -panelWidth / 2f - spacing; // Start from outside the panel
        float targetXPos = -panelWidth / 2f + spacing * (i + 0.5f); // Move the fish to the left edge of the panel

        // Set initial position of the fish outside the panel
        fish.transform.localPosition = new Vector3(startXPos, 0f, 0f);

        // Animate the fish to move into its position within the panel
        StartCoroutine(AnimateFish(fish, new Vector3(targetXPos, 0f, 0f), animationDuration, () =>
        {
            // Increment the count of instantiated fishes
            fishesInstantiated++;

            // Check if all fishes have been instantiated
            if (fishesInstantiated == numerator)
            {
                // Invoke the onComplete callback when all fishes are instantiated and animated
                StartCoroutine(DelayBeforeMovingToDenominator(delayBeforeMoving, onComplete));
            }
        }));
    }
}

IEnumerator AnimateFish(GameObject fish, Vector3 targetPosition, float duration, System.Action onComplete)
{
    float elapsedTime = 0f;
    Vector3 startingPosition = fish.transform.localPosition;

    while (elapsedTime < duration)
    {
        // Calculate the interpolation factor (0 to 1)
        float t = elapsedTime / duration;

        // Interpolate the position between starting and target positions
        fish.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, t);

        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Wait for the next frame
        yield return null;
    }

    // Ensure the fish reaches the target position exactly
    fish.transform.localPosition = targetPosition;

    // Invoke the onComplete callback when the animation is completed
    onComplete?.Invoke();
}

IEnumerator DelayBeforeMovingToDenominator(float delay, System.Action onComplete)
{
    // Wait for the specified delay before moving fishes to denominator bar panels
    yield return new WaitForSeconds(delay);

    // Once the delay is over, invoke the onComplete callback
    onComplete?.Invoke();
}


void GenerateDenominatorBarPanels(int denominator)
{
    // Clear existing denominator bar panels
    foreach (RectTransform panel in denominatorBarPanels)
    {
        Destroy(panel.gameObject);
    }
    denominatorBarPanels.Clear();

    // Calculate the width of each panel
    float panelWidth = numeratorBarPanel.rect.width / 2; // Two panels per row

       // Calculate the spacing between panels horizontally
    float horizontalSpacing = panelWidth;

    // Calculate the spacing between panels vertically
    float verticalSpacing = 200f; // Adjust this value as needed

    // Define the offset from the top of the screen
    float yOffset = 700f; // Adjust this value as needed

    // Check if there's only one denominator panel
    if (denominator == 1)
    {
        // Instantiate the denominator bar panel from the prefab
        GameObject panelObject = Instantiate(denominatorBarPanelPrefab, transform);

        // Add the RectTransform component to the list
        RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
        denominatorBarPanels.Add(panelRectTransform);

        // Set the position of the panel to the center of the screen
        panelRectTransform.localPosition = new Vector3(0f, yOffset, 0f);
    }
    else
    {
        // Track the current row
        int row = 0;

        // Instantiate and position denominator bar panels
        for (int i = 0; i < denominator; i++)
        {
            // Instantiate the denominator bar panel from the prefab
            GameObject panelObject = Instantiate(denominatorBarPanelPrefab, transform);

            // Add the RectTransform component to the list
            RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
            denominatorBarPanels.Add(panelRectTransform);

            // Calculate the position of the panel
            float panelXPosition = i % 2 == 0 ? -panelWidth / 2 : panelWidth / 2;
            float panelYPosition = yOffset - row * verticalSpacing;

            // Set the position of the panel
            panelRectTransform.localPosition = new Vector3(panelXPosition, panelYPosition, 0f);

            // Check if two panels have been placed in the current row
            if (i % 2 == 1)
            {
                // Move to the next row
                row++;
            }
        }
    }
}

void AnimateFishesToDenominator(int correctIndex, int denominator)
{
    // Get the total number of fishes
    int totalFishes = numeratorBarPanel.transform.childCount;

    // Calculate the horizontal spacing between fishes
    float spacing = numeratorBarPanel.rect.width / denominator;

    // Initialize the current panel index
    int currentPanelIndex = 0;

    // Loop through each fish
    StartCoroutine(AnimateFishesSequentially(totalFishes, denominator, spacing, currentPanelIndex));
}

IEnumerator AnimateFishesSequentially(int totalFishes, int denominator, float spacing, int currentPanelIndex)
{
    // Check total number of fishes
    Debug.Log("Total fishes: " + totalFishes);

    // Loop through each fish
    for (int i = 0; i < totalFishes; i++)
    {
        // Get the fish transform
        Transform fishTransform = numeratorBarPanel.transform.GetChild(i);

        // Check if fishTransform exists
        if (fishTransform == null)
        {
            Debug.LogError("Fish transform is null at index: " + i);
            yield break; // Exit the coroutine if the fish transform is null
        }

        // Calculate the target position for the fish in the current denominator bar panel
        float targetXPos = Mathf.Clamp(denominatorBarPanels[currentPanelIndex].localPosition.x + (i % denominator) * spacing, 0, 720);
        float targetYPos = Mathf.Clamp(denominatorBarPanels[currentPanelIndex].localPosition.y, 0, 1560); 
        Vector3 targetPosition = new Vector3(targetXPos, targetYPos, 0f);

        // Animate the fish to the target position within the denominator bar panel
        yield return StartCoroutine(AnimateFishToDenominator(fishTransform.gameObject, denominatorBarPanels[currentPanelIndex], targetPosition));

        // Move to the next panel for the next fish
        currentPanelIndex = (currentPanelIndex + 1) % denominator;

        // Add a short delay before animating the next fish
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed
    }
}



IEnumerator AnimateFishToDenominator(GameObject fish, RectTransform targetPanel, Vector3 targetPosition)
{
    float animationDuration = 1.0f; // Duration of the animation in seconds
    float elapsedTime = 0f;
    Vector3 startingPosition = fish.transform.localPosition;

    // Calculate the distance between starting and target positions
    float distance = Vector3.Distance(startingPosition, targetPosition);

    while (elapsedTime < animationDuration)
    {
        // Calculate the interpolation factor (0 to 1)
        float t = elapsedTime / animationDuration;

        // Ease in-out interpolation function for smoother animation
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        // Interpolate the position between starting and target positions using smooth step
        Vector3 newPosition = Vector3.Lerp(startingPosition, targetPosition, smoothT);

        // Ensure the fish stays within the bounds of the target panel
        float panelWidth = targetPanel.rect.width;
        float halfPanelWidth = panelWidth / 2f;
        float minX = targetPanel.localPosition.x - halfPanelWidth;
        float maxX = targetPanel.localPosition.x + halfPanelWidth;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        fish.transform.localPosition = newPosition;

        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Wait for the next frame
        yield return null;
    }

    // Ensure the fish reaches the target position exactly
    fish.transform.SetParent(targetPanel); // Set the fish's parent to the target panel
    fish.transform.localPosition = targetPosition;
}




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
    pauseButton.interactable = true;
    EnableGameInputs();
    LoadNextProblem();
}
}

