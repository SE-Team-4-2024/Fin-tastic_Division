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
    public GameObject pauseMenuPanel, completeGamePanel, hidingPanel;
    public Button closeButton, restartButton, backToMainMenuButton, pauseButton, resumeButton, playAgainButton, pauseExitToMainMenuButton;
    public GameObject fishPrefab;

    public VoiceScript voiceScript;

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
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
        if(audioSource != null)
        {
            Debug.LogError("No AudioSource Component found on this game object. Please add onr.");
        }
        
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
    questionsText.text = $"{problem.numerator} / {problem.denominator} =?";
    if(voiceScript!=null)
    {
        voiceScript.Speak();
    }
    else
    {
        Debug.LogError("VoiceScript is null");
    }

    // Instantiate fishes for the numerator
    GenerateFishes(problem, () =>
    {
        // Callback to trigger after all fishes have finished animating
        // Generate denominator bar panels and animate fishes to move to the correct denominator bar
        GenerateDenominatorBarPanels(problem.denominator);
        AnimateFishesToDenominator(problem.options[problem.correct_option_index], problem.denominator);
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
void GenerateFishes(DivisionProblem problem, System.Action onComplete)
{
    int numerator = problem.numerator; 
    int denominator = problem.denominator; 
    Debug.Log("Generating fishes. Numerator: " + numerator);

    ClearExistingFishes();

    float panelWidth = numeratorBarPanel.rect.width;
    float fishWidth = fishPrefab.GetComponent<RectTransform>().rect.width;
    float totalFishWidth = numerator * fishWidth;
    float spacing = CalculateSpacing(panelWidth, totalFishWidth, numerator);
    float startXPos = CalculateStartXPos(panelWidth, fishWidth);
    float scaleFactor = CalculateScaleFactor(numerator);

    InstantiateAndAnimateFishes(numerator, denominator, spacing, startXPos, fishWidth, scaleFactor, onComplete);
}

void ClearExistingFishes()
{
    foreach (Transform child in numeratorBarPanel.transform)
    {
        GameObject.Destroy(child.gameObject);
    }
    instantiatedFishes.Clear();
}

float CalculateSpacing(float panelWidth, float totalFishWidth, int numerator)
{
    return (panelWidth - totalFishWidth) / (numerator - 1);
}

float CalculateStartXPos(float panelWidth, float fishWidth)
{
    return -panelWidth / 2f + fishWidth / 2f;
}

float CalculateScaleFactor(int numerator)
{
    float scaleFactor;
    switch (numerator)
    {
        case 4:
            scaleFactor = 2.0f;
            break;
        case 6:
            scaleFactor = 1.7f;
            break;
        case 8:
            scaleFactor = 1.4f;
            break;
        case 9:
            scaleFactor = 1.2f;
            break;
        default:
            scaleFactor = 1.5f;
            break;
    }
    return scaleFactor;
}

void InstantiateAndAnimateFishes(int numerator, int denominator, float spacing, float startXPos, float fishWidth, float scaleFactor, System.Action onComplete)
{
    int quotient = numerator / denominator;
    float animationDuration = 0.5f;
    float delayBeforeMoving = 1.0f;
    int fishesInstantiated = 0;

    for (int i = 0; i < numerator; i += quotient)
    {
        for (int j = 0; j < quotient; j++)
        {
            GameObject fishPrefabToUse = GetFishPrefabByIndex(i / quotient);
            GameObject fish = InstantiateFish(fishPrefabToUse, fishWidth, startXPos, spacing, numerator, i, j);
            
            Vector3 targetPosition = new Vector3(startXPos + (fishWidth + spacing) * i + j * (fishWidth + spacing), 0f, 0f);
            
            fish.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            
            AnimateFishMovement(fish, targetPosition, animationDuration, () =>
            {
                fishesInstantiated++;
                if (fishesInstantiated == numerator)
                {
                    StartCoroutine(DelayBeforeMovingToDenominator(delayBeforeMoving, onComplete));
                }
            });
            
            instantiatedFishes.Add(fish);
        }
    }
}

GameObject InstantiateFish(GameObject fishPrefabToUse, float fishWidth, float startXPos, float spacing, int numerator, int i, int j)
{
    GameObject fish = Instantiate(fishPrefabToUse, numeratorBarPanel);
    fish.transform.localPosition = new Vector3(startXPos + i * (fishWidth + spacing), 0f, 0f);
    return fish;
}

void AnimateFishMovement(GameObject fish, Vector3 targetPosition, float animationDuration, System.Action onComplete)
{
    fish.transform.localPosition = new Vector3(-numeratorBarPanel.rect.width / 2f - fish.GetComponent<RectTransform>().rect.width / 2f, 0f, 0f);
    StartCoroutine(AnimateFish(fish, targetPosition, animationDuration, onComplete));
}



// Helper method to get the fish prefab based on the index
GameObject GetFishPrefabByIndex(int index)
{
    switch (index)
    {
        case 0:
            return fishPrefab;
        case 1:
            return fishPrefab2;
        case 2:
            return fishPrefab3;
        case 3:
            return fishPrefab4;
        default:
            return null; // Default to the second prefab
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
    float horizontalSpacing = panelWidth + 50f; // Increased spacing

    // Increase the vertical spacing between panels
    float verticalSpacing = 250f; // Increased vertical spacing

    // Define the offset from the top of the screen
    float yOffset = 600f; // Adjust this value as needed

    // Clear the existing denominator bar panels
    ClearDenominatorBarPanels();

    // Instantiate and position denominator bar panels
    for (int i = 0; i < denominator; i++)
    {
        // Instantiate the denominator bar panel from the prefab
        GameObject panelObject = Instantiate(denominatorBarPanelPrefab, transform);

        // Add the RectTransform component to the list
        RectTransform panelRectTransform = panelObject.GetComponent<RectTransform>();
        denominatorBarPanels.Add(panelRectTransform);

        // Calculate the position of the panel
        float panelXPosition;
        float panelYPosition;

        if (denominator == 3 && i == 2)
        {
            // Center the third panel below the first two panels
            panelXPosition = 0f; // Center X position
            panelYPosition = yOffset - verticalSpacing; // Adjusted Y position
        }
        else
        {
            // Regular positioning for the other panels
            panelXPosition = i % 2 == 0 ? -panelWidth / 2 : panelWidth / 2;
            panelYPosition = yOffset - (i / 2) * verticalSpacing;
        }

        // Set the position of the panel
        panelRectTransform.localPosition = new Vector3(panelXPosition, panelYPosition, 0f);
    }
}

void AnimateFishesToDenominator(int answer, int denominator)
{
    // Get the total number of fishes
    int totalFishes = numeratorBarPanel.transform.childCount;

    // Calculate total fish width based on fish prefab
    float fishWidth = fishPrefab.GetComponent<RectTransform>().rect.width;
    
    // Define horizontal spacing between fishes
    float spacing = 25f;

    // Ensure the denominator bar panels list contains the required number of panels
    if (denominatorBarPanels.Count < denominator)
    {
        Debug.LogError("Not enough denominator bar panels available.");
        return;
    }

    // Adjust fish size based on denominator bar width
    float fishScaleFactor = denominatorBarPanels[0].rect.width / (2 * fishWidth);

    // Clamp the fishScaleFactor to a reasonable range
    fishScaleFactor = Mathf.Clamp(fishScaleFactor, 1.0f, 2.0f);

    // Distribute exactly 'answer' amount of fishes to each denominator panel
    for (int currentPanelIndex = 0; currentPanelIndex < denominator; currentPanelIndex++)
    {
        // Calculate the initial target position for the fishes in the current denominator bar panel
        float targetXPos = denominatorBarPanels[currentPanelIndex].localPosition.x; // Start from the left edge of the panel
        float targetYPos = denominatorBarPanels[currentPanelIndex].localPosition.y;
        Vector3 targetPosition = new Vector3(targetXPos, targetYPos, 0f);

        // Distribute 'answer' amount of fishes to the current denominator bar panel
        for (int j = 0; j < answer && (currentPanelIndex * answer + j) < totalFishes; j++)
        {
            Transform fishTransform = numeratorBarPanel.transform.GetChild(currentPanelIndex * answer + j);

            if (fishTransform != null)
            {
                GameObject fish = fishTransform.gameObject;

                // Set the fish scale
                fish.transform.localScale = new Vector3(fishScaleFactor, fishScaleFactor, 1f);

                // Animate the fish to the target position within the current denominator bar panel
                StartCoroutine(AnimateFishToDenominator(fish, denominatorBarPanels[currentPanelIndex], targetPosition, () =>
                {
                    // If the last fish for this denominator bar panel has moved, clear the numerator bar
                    if (j == answer - 1 || (currentPanelIndex * answer + j) == totalFishes - 1)
                    {
                        ClearNumeratorBar();
                    }
                }));

                // Update the target position for the next fish in the same denominator bar panel
                targetXPos += fishWidth + spacing; // Add fish width and spacing
                targetPosition = new Vector3(targetXPos, targetYPos, 0f);
            }
            else
            {
                Debug.LogError("Fish transform is null at index: " + (currentPanelIndex * answer + j));
            }
        }
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
    // Get the RectTransform of the canvas
    RectTransform canvasRect = fish.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

    // Get the width and height of the screen
    float screenWidth = Screen.width;
    float screenHeight = Screen.height;

    // Define screen coordinates for the right and left side
    Vector2 screenRight = new Vector2(screenWidth, screenHeight / 2f);
    Vector2 screenLeft = new Vector2(0f, screenHeight / 2f);

    // Convert screen coordinates to local positions of the canvas
    Vector2 localRight;
    Vector2 localLeft;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenRight, null, out localRight);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenLeft, null, out localLeft);

    // Set the starting position to the right side of the canvas
    fish.GetComponent<RectTransform>().anchoredPosition = localRight;

    // Set the target position to the left side of the canvas
    Vector2 targetPosition = localLeft;

    // Set the duration of the animation
    float duration = 1.5f;

    // Initialize elapsed time
    float elapsedTime = 0f;

    // Loop until the animation duration is reached
    while (elapsedTime < duration)
    {
        // Calculate the interpolation factor
        float t = elapsedTime / duration;

        // Interpolate the position between starting and target positions
        fish.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(localRight, targetPosition, t);

        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Wait for the next frame
        yield return null;
    }

    // Ensure the fish reaches the target position exactly
    fish.GetComponent<RectTransform>().anchoredPosition = targetPosition;

    // Disable the fish GameObject
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
    yield return new WaitForSeconds(2f);

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

    hidingPanel.SetActive(true);
    pauseMenuPanel.SetActive(true);
    DisableGameInputs();
}

void ResumeGame()
{
    hidingPanel.SetActive(false);
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
    
    // Hide the denominator bars and fishes inside them
    foreach (RectTransform panel in denominatorBarPanels)
    {
        panel.gameObject.SetActive(false);
    }
    
    // Move numerator bar behind the pause menu panel and hide it
    numeratorBarPanel.gameObject.transform.SetParent(pauseMenuPanel.transform);
    numeratorBarPanel.gameObject.SetActive(false);

    // Disable other game elements
    pauseButton.interactable = false;
    fishPrefab.SetActive(false);
    denominatorBarPanelPrefab.SetActive(false);
}

void EnableGameInputs()
{
    // Move game inputs back to their original parent
    foreach (var button in answerButtons)
    {
        button.gameObject.transform.SetParent(transform);
        button.interactable = true;
    }
    
    // Show the denominator bars and fishes inside them
    foreach (RectTransform panel in denominatorBarPanels)
    {
        panel.gameObject.SetActive(true);
    }
    
    // Move numerator bar back to its original parent and show it
    numeratorBarPanel.gameObject.transform.SetParent(transform);
    numeratorBarPanel.gameObject.SetActive(true);

    // Enable other game elements
    pauseButton.interactable = true;
    fishPrefab.SetActive(true);
    denominatorBarPanelPrefab.SetActive(true);
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