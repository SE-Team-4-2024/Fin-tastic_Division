using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayScene : MonoBehaviour
{
    private float startTime;
    private float endTime;
    public TextMeshProUGUI questionsText, stageText, accuracyText, wrongText, rateText;
    public Button[] answerButtons;
    public GameObject pauseMenuPanel, completeGamePanel;   // backgroundOverlayPanel;
    public Button closeButton, restartButton, backToMainMenuButton, pauseButton, resumeButton, playAgainButton, pauseExitToMainMenuButton;
    public GameObject fishPrefab;

    public GameObject fishPrefab2;
    public GameObject fishPrefab3;
    public GameObject fishPrefab4;

    public GameObject happyFishAnim;
    public GameObject sadFishAnim;
    public GameObject denominatorBarPanelPrefab; // Prefab for denominator bar panel
    private int correctAnswerIndex;
    private int currentQuestionIndex = 0;
    private const int totalQuestions = 5;
    private int correctlyAnswered = 0;
    public RectTransform numeratorBarPanel; // Reference to the numerator bar panel
    private List<RectTransform> denominatorBarPanels = new List<RectTransform>(); // List to store references to denominator bar panels
    // Declare a list to keep track of instantiated fish objects
    private List<GameObject> instantiatedFishes = new List<GameObject>();
    void Start()
    {
          // Ensure that both fish animations are initially inactive
        happyFishAnim.SetActive(false);
        sadFishAnim.SetActive(false);
        pauseMenuPanel.SetActive(false);
        completeGamePanel.SetActive(false);
        pauseButton.onClick.AddListener(PauseGame);
        closeButton.onClick.AddListener(ResumeGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        playAgainButton.onClick.AddListener(PlayAgainButton);
        pauseExitToMainMenuButton.onClick.AddListener(BackToMainMenu);
        
        CreateNewGame(); // To create a new game to store in database
        startTime = Time.time; // Track the start time when the game starts
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
    stageText.text = $" {currentQuestionIndex + 1}/{totalQuestions}";
    questionsText.text = $"{problem.numerator} % {problem.denominator} =?";

    // Instantiate fishes for the numerator
    GenerateFishes(problem.numerator, () =>
    {
        // Callback to trigger after all fishes have finished animating
        // Generate denominator bar panels and animate fishes to move to the correct denominator bar
        GenerateDenominatorBarPanels(problem.denominator);
        AnimateFishesToDenominator(problem.correct_option_index, problem.denominator);
    });

    // Reset the state of fish animations
    happyFishAnim.SetActive(false);
    sadFishAnim.SetActive(false);

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
void GenerateFishes(int numerator, System.Action onComplete)
{
    Debug.Log("Generating fishes. Numerator: " + numerator);

    // Clear existing fish objects in the numeratorBarPanel
    foreach (Transform child in numeratorBarPanel.transform)
    {
        GameObject.Destroy(child.gameObject);
    }

    instantiatedFishes.Clear(); // Clear the list before generating new fishes

    // Calculate spacing between each fish image
    float panelWidth = numeratorBarPanel.rect.width;
    float fishWidth = fishPrefab.GetComponent<RectTransform>().rect.width;
    float totalFishWidth = numerator * fishWidth;
    float spacing = (panelWidth - totalFishWidth) / (numerator + 1); // Divide remaining space evenly

    Debug.Log("Panel width: " + panelWidth);
    Debug.Log("Total fish width: " + totalFishWidth);
    Debug.Log("Spacing between fishes: " + spacing);

    // Keep track of the number of fishes instantiated
    int fishesInstantiated = 0;

    // Get the height of the fish prefab
    float fishHeight = fishPrefab.GetComponent<RectTransform>().rect.height;

    // Define the animation duration for each fish
    float animationDuration = 0.5f; // Adjust as needed

    // Define the delay before moving fishes to denominator bar panels
    float delayBeforeMoving = 1.0f; // 1 second delay

    // Define the scale factor to increase the size of the fish
    float scaleFactor = 1.5f; // Adjust as needed

    // Calculate the starting position to center the fishes horizontally
    float startXPos = -panelWidth / 2f + spacing + fishWidth / 2f;

    // Instantiate and animate fish images
    for (int i = 0; i < numerator; i++)
    {
        // Instantiate fish image from the prefab
        GameObject fish = Instantiate(fishPrefab, numeratorBarPanel);

        // Calculate position of the fish image within the panel
        float xPos = startXPos + (fishWidth + spacing) * i; // Add spacing between fishes
        float yPos = 0f; // Keep fishes aligned vertically
        Vector3 targetPosition = new Vector3(xPos, yPos, 0f);

        // Set initial position of the fish within the panel
        fish.transform.localPosition = new Vector3(-panelWidth / 2f - fishWidth / 2f, yPos, 0f); // Start from outside the panel

        // Set the scale of the fish based on the size of the numerator bar
        fish.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f); // Increase the size of the fish

        // Animate the fish to move into its position within the panel
        StartCoroutine(AnimateFish(fish, targetPosition, animationDuration, () =>
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
         // Add the instantiated fish object to the list
        instantiatedFishes.Add(fish);
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

    // Calculate the width of each panel
    float panelWidth = numeratorBarPanel.rect.width / 2; // Two panels per row

    // Calculate the spacing between panels horizontally
    float horizontalSpacing = panelWidth - 20f; // Increased spacing

    // Calculate the spacing between panels vertically
    float verticalSpacing = 200f; // Increased spacing

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
    float spacing = (numeratorBarPanel.rect.width / (denominator + 1)); // Add spacing between each fish

    // Ensure the denominator bar panels list contains the required number of panels
    if (denominatorBarPanels.Count < denominator)
    {
        Debug.LogError("Not enough denominator bar panels available.");
        return;
    }

    // Initialize the current panel index
    int currentPanelIndex = 0;

    // List to keep track of fishes that have reached the denominator bars
    List<GameObject> fishesMoved = new List<GameObject>();

    // Loop through each fish
    for (int i = 0; i < totalFishes; i++)
    {
        // Check if the currentPanelIndex is valid
        if (currentPanelIndex >= denominatorBarPanels.Count)
        {
            Debug.LogError("Current panel index is out of bounds: " + currentPanelIndex);
            return; // Exit the method if the index is out of bounds
        }

        // Calculate the target position for the fish in the current denominator bar panel
        float targetXPos = denominatorBarPanels[currentPanelIndex].localPosition.x + (i % denominator + 1) * spacing; // Add spacing to the left edge of the panel
        float targetYPos = denominatorBarPanels[currentPanelIndex].localPosition.y;
        Vector3 targetPosition = new Vector3(targetXPos, targetYPos, 0f);

        // Get the fish transform
        Transform fishTransform = numeratorBarPanel.transform.GetChild(i);

        // Ensure the fish transform exists
        if (fishTransform != null)
        {
            // Animate the fish to the target position within the denominator bar panel
            StartCoroutine(AnimateFishToDenominator(fishTransform.gameObject, denominatorBarPanels[currentPanelIndex], targetPosition, () =>
            {
                // Add the fish to the list of moved fishes
                fishesMoved.Add(fishTransform.gameObject);

                // If all fishes have moved, clear the numerator bar
                if (fishesMoved.Count == totalFishes)
                {
                    ClearNumeratorBar();
                }
            }));
        }
        else
        {
            Debug.LogError("Fish transform is null at index: " + i);
        }

        // Move to the next panel for the next fish
        currentPanelIndex = (currentPanelIndex + 1) % denominator;
    }
}

IEnumerator AnimateFishToDenominator(GameObject fish, RectTransform targetPanel, Vector3 targetPosition, System.Action onComplete)
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

    // Invoke the onComplete callback when the animation is completed
    onComplete?.Invoke();
}

void ClearNumeratorBar()
{
    // Clear existing fish objects in the numeratorBarPanel
    foreach (Transform child in numeratorBarPanel.transform)
    {
        GameObject.Destroy(child.gameObject);
    }
}

void ClearDenominatorBarPanels()
{
// Clear existing denominator bar panels
    foreach (RectTransform panel in denominatorBarPanels)
    {
        Destroy(panel.gameObject);
    }
    denominatorBarPanels.Clear();
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
    // Stop fish animation coroutine if it's currently running
    StopCoroutine("AnimateFishToDenominator");

     // Destroy instantiated fish objects
    foreach (GameObject fish in instantiatedFishes)
    {
        Destroy(fish);
    }
    instantiatedFishes.Clear(); // Clear the list after destroying fish objects
    bool isCorrect = index == correctAnswerIndex;
    answerButtons[index].GetComponent<Image>().color = isCorrect ? Color.green : Color.red;
    answerButtons[correctAnswerIndex].GetComponent<Image>().color = Color.green;
    correctlyAnswered += isCorrect ? 1 : 0;

    // Activate the corresponding fish animation based on the user's answer
    if (isCorrect)
    {
        AnimateFish(true); // Happy fish animation
    }
    else
    {
        AnimateFish(false); // Sad fish animation
    }
    ClearDenominatorBarPanels();

    // To update the user response in the database
    StartCoroutine(UpdateUserResponseCoroutine(isCorrect)); 
    StartCoroutine(ContinueAfterFeedback(isCorrect, index));
}

// Call this function when you want to animate a fish
void AnimateFish(bool isHappy)
{
    //numeratorBarPanel.gameObject.SetActive(false);

    // Determine the GameObject for the fish animation based on the isHappy parameter
    GameObject fishToAnimate = isHappy ? happyFishAnim : sadFishAnim;

    // Ensure the fish GameObject is active before animating
    fishToAnimate.SetActive(true);

    // Start the MoveFishAnimation coroutine for the selected fish
    StartCoroutine(MoveFishAnimation(fishToAnimate));
}

// IEnumerator coroutine to move the fish from right to left
IEnumerator MoveFishAnimation(GameObject fish)
{
    // Get the width and height of the screen
    float screenWidth = Screen.width;
    float screenHeight = Screen.height;

    // Define the starting and target positions
    Vector3 screenRight = new Vector3(screenWidth, screenHeight / 1.5f, 0f);
    Vector3 screenLeft = new Vector3(0f, screenHeight / 1.5f, 0f); // Adjusted target position
    Vector3 startPosition = Camera.main.ScreenToWorldPoint(screenRight); // Adjusted start position
    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(screenLeft);

    // Set the duration of the animation
    float duration = 5f;

    // Initialize elapsed time
    float elapsedTime = 0f;

    // Loop until the animation duration is reached
    while (elapsedTime < duration)
    {
        // Calculate the interpolation factor
        float t = elapsedTime / duration;

        // Interpolate the position between starting and target positions
        fish.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Wait for the next frame
        yield return null;
    }

    // Ensure the fish reaches the target position exactly
    fish.transform.position = targetPosition;

    // Disable the fish animation
    fish.SetActive(false);
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
    yield return new WaitForSeconds(5f);

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
    endTime = Time.time; // Track the end time when the game completes
    // Calculate the total time taken in minutes
    float totalTimeTakenInMinutes = (endTime - startTime) / 60f;
    // Calculate the rate (questions answered per minute)
    float rate = totalQuestions / totalTimeTakenInMinutes;
    // Display the rate in the rateText
    rateText.text = "Rate: " + rate.ToString("F2") + "/min";
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
    LoadNextProblem();
}

void DisableGameInputs()
{
    // Move game inputs behind the pause menu panel
    foreach (var button in answerButtons)
    {
        button.gameObject.transform.SetParent(pauseMenuPanel.transform);
        button.interactable = false;
    }
    foreach (RectTransform panel in denominatorBarPanels)
    {
        panel.gameObject.transform.SetParent(pauseMenuPanel.transform);
    }
    numeratorBarPanel.gameObject.transform.SetParent(pauseMenuPanel.transform);
    
    // Disable other game elements
    pauseButton.interactable = false;
    fishPrefab.SetActive(false);
    denominatorBarPanelPrefab.SetActive(false);
    numeratorBarPanel.gameObject.SetActive(false);
}

void EnableGameInputs()
{
    // Move game inputs back to their original parent
    foreach (var button in answerButtons)
    {
        button.gameObject.transform.SetParent(transform);
        button.interactable = true;
    }
    foreach (RectTransform panel in denominatorBarPanels)
    {
        panel.gameObject.transform.SetParent(transform);
    }
    numeratorBarPanel.gameObject.transform.SetParent(transform);
    
    // Enable other game elements
    pauseButton.interactable = true;
    fishPrefab.SetActive(true);
    denominatorBarPanelPrefab.SetActive(true);
    numeratorBarPanel.gameObject.SetActive(true);
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