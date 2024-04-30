using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;


public class PlayScene : MonoBehaviour
{
    private float startTime;
    private float endTime;
    public TextMeshProUGUI questionsText, stageText, accuracyText, wrongText, rateText;
    public Button[] answerButtons;
    public GameObject pauseMenuPanel, completeGamePanel, hidingPanel, previousRecordsPanel;
    public GameObject textBoxPrefab; // Reference to the prefab of your text box
   
    public Button closeButton, restartButton, backToMainMenuButton, pauseButton, resumeButton, playAgainButton, pauseExitToMainMenuButton, historyButton, closeButtonPrev;
    public GameObject fishPrefab;
    private Game[] fetchedGames; 

    public VoiceScript voiceScript;

     private UserManager userManagerInstance;

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
    private AudioSource audioSource, buttonClickAudioSource;

    public AudioClip correctAnswerAudio, wrongAnswerAudio, buttonClickAudioClip;
    public RectTransform numeratorBarPanel; // Reference to the numerator bar panel
    private bool isAnimating = false;
    private bool isSoundOn;
    private AudioController audioController; // Reference to AudioController



    private List<RectTransform> denominatorBarPanels = new List<RectTransform>(); // List to store references to denominator bar panels
    // Declare a list to keep track of instantiated fish objects
    private List<GameObject> instantiatedFishes = new List<GameObject>();
    float originalTimeScale;
    private List<GameObject> createdTextBoxes = new List<GameObject>(); // List to store references to created text boxes
    void Start()
    {
        originalTimeScale = Time.timeScale; //Store the original time scale
        // Ensure that both fish animations are initially inactive
        happyFishAnim.SetActive(false);
        sadFishAnim.SetActive(false);
        pauseMenuPanel.SetActive(false);
        completeGamePanel.SetActive(false);
        audioController = FindObjectOfType<AudioController>(); // Find the AudioController in the scene

        // Load sound settings from PlayerPrefs
        isSoundOn = PlayerPrefs.GetInt(UserManager.SOUND_KEY, 1) == 1; // Default is true

        pauseButton.onClick.AddListener(() => { PauseGame(); PlayClickSound(); });
        closeButton.onClick.AddListener(() => { ResumeGame(); PlayClickSound(); });
        resumeButton.onClick.AddListener(() => { ResumeGame(); PlayClickSound(); });
        restartButton.onClick.AddListener(() => { RestartGame(); PlayClickSound(); });
        backToMainMenuButton.onClick.AddListener(() => { BackToMainMenu(); PlayClickSound(); });
        playAgainButton.onClick.AddListener(() => { PlayAgainButton(); PlayClickSound(); });
        pauseExitToMainMenuButton.onClick.AddListener(() => { BackToMainMenu(); PlayClickSound(); });
        historyButton.onClick.AddListener(() => { ShowPreviousRecords(); PlayClickSound(); });
        closeButtonPrev.onClick.AddListener(() => {MoveToCompleteGamePanel(); PlayClickSound(); });
        buttonClickAudioSource = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource Component found on this game object. Please add one.");
        }

        CreateNewGame(); // To create a new game to store in database
        startTime = Time.time; // Track the start time when the game starts
        LoadNextProblem(); // Start loading the first problem
        Debug.Log("------------START---------------------");
    }


    public void PlayClickSound()
    {
        if (buttonClickAudioSource != null && buttonClickAudioClip != null && isSoundOn)
        {
            buttonClickAudioSource.clip = buttonClickAudioClip;
            buttonClickAudioSource.Play();
        }
    }


    IEnumerator PlayAudioSoundAfterDelay(bool isAnswerCorrect)
    {
        if (audioSource != null && correctAnswerAudio != null && wrongAnswerAudio != null && isSoundOn)
        {
            yield return new WaitForSeconds(0.5f);
            audioSource.clip = isAnswerCorrect ? correctAnswerAudio : wrongAnswerAudio;
            audioSource.Play();
        }
        
    }


    void CreateNewGame()
    {
        string userID = PlayerPrefs.GetString(UserManager.USERID_KEY);
        Debug.Log("Creating New Game for the userID" + userID);

        // Call the asynchronous method and pass onSuccess and onError callbacks
        StartCoroutine(GameManager.CreateNewGame(userID, onSuccess, OnError));
    }

    void onSuccess(string gameID)
    {
        Debug.Log("Game Succesfully created with gameId" + gameID);
        PlayerPrefs.SetString(UserManager.GAME_KEY, gameID);
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
        if (voiceScript != null && isSoundOn)
        {
            voiceScript.Speak();
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
            answerButtons[i].onClick.AddListener(() => { PlayClickSound(); AnswerSelected(optionIndex); StartCoroutine(PlayAudioSoundAfterDelay(optionIndex == correctAnswerIndex)); });
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
        float verticalSpacing = 500f; // Increased vertical spacing

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
        if (fish == null || targetPanel == null)
        {
            Debug.LogWarning("Fish or target panel is null. Aborting fish animation.");
            yield break; // Exit the coroutine
        }

        float animationDuration = 1.0f; // Duration of the animation in seconds
        float elapsedTime = 0f;
        Vector3 startingPosition = fish.transform.localPosition;

        // Calculate the distance between starting and target positions
        float distance = Vector3.Distance(startingPosition, targetPosition);

        while (elapsedTime < animationDuration)
        {
            // Check if the target panel reference is still valid
            if (targetPanel == null)
            {
                Debug.LogWarning("Target panel is null during fish animation. Aborting animation.");
                yield break; // Exit the coroutine
            }

            // Check if the fish GameObject is still valid
            if (fish == null)
            {
                Debug.LogWarning("Fish GameObject is null during fish animation. Aborting animation.");
                yield break; // Exit the coroutine
            }

            // Calculate the interpolation factor (0 to 1)
            float t = elapsedTime / animationDuration;

            // Ease in-out interpolation function for smoother animation
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Interpolate the position between starting and target positions using smooth step
            Vector3 newPosition = Vector3.Lerp(startingPosition, targetPosition, smoothT);

            // Ensure the fish stays within the bounds of the target panel
            if (targetPanel != null)
            {
                float panelWidth = targetPanel.rect.width;
                float halfPanelWidth = panelWidth / 2f;
                float minX = targetPanel.localPosition.x - halfPanelWidth;
                float maxX = targetPanel.localPosition.x + halfPanelWidth;
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            }

            // Update the position of the fish
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
        // Check if an animation is already in progress
        if (isAnimating)
        {
            // Stop the animation coroutine for moving fishes to the denominator bar
            StopCoroutine("AnimateFishToDenominator");

            // Clear the numerator bar to remove any instantiated fish
            ClearNumeratorBar();
        }

        // Set the flag to indicate that an animation is starting
        isAnimating = true;

        // Reset button colors before processing the selected answer
        ResetButtonColors();
        // Stop fish animation coroutine if it's currently running
        StopCoroutine("AnimateFishToDenominator");

        // Destroy instantiated fish objects
        foreach (GameObject fish in instantiatedFishes)
        {
            // Check if the fish object is null or has been destroyed
            if (fish != null)
            {
                Destroy(fish);
            }
        }
        instantiatedFishes.Clear(); // Clear the list after destroying fish objects

        bool isCorrect = false; // Declare isCorrect variable outside the if statement

        // Ensure that the answer buttons array is not null and the index is valid
        if (answerButtons != null && index >= 0 && index < answerButtons.Length)
        {
            isCorrect = index == correctAnswerIndex; // Assign isCorrect value here
            answerButtons[index].GetComponent<Image>().color = isCorrect ? Color.green : Color.red;
            answerButtons[correctAnswerIndex].GetComponent<Image>().color = Color.green;
            correctlyAnswered += isCorrect ? 1 : 0;
            //PlayAudioSoundAfterDelay(isCorrect);
            voiceScript.StopSpeaking();
            // Activate the corresponding fish animation based on the user's answer
            if (isCorrect)
            {
                AnimateFish(true); // Happy fish animation
            }
            else
            {
                AnimateFish(false); // Sad fish animation
            }
        }
        else
        {
            Debug.LogError("Answer button array is null or index is out of bounds.");
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
        Vector2 screenRight = new Vector2(screenWidth, screenHeight / 1.5f);
        Vector2 screenLeft = new Vector2(0f, screenHeight / 1.5f);

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
        string gameID = PlayerPrefs.GetString(UserManager.GAME_KEY);
        yield return GameManager.UpdateUserResponse(gameID, isCorrect, onSuccessfulUpdate, OnError);
    }

    void onSuccessfulUpdate(bool success)
    {
        Debug.Log("User response updation" + success);
    }


    IEnumerator ContinueAfterFeedback(bool isCorrect, int index)
    {
        yield return new WaitForSeconds(2f);

        currentQuestionIndex++;
        if (currentQuestionIndex < totalQuestions)
        {
            LoadNextProblem();
        }
        else if (!pauseMenuPanel.activeSelf)
        {
            CompleteGame();
        }

        // Reset the flag to indicate that the animation is complete
        isAnimating = false;
    }

    void CompleteGame()
    {
        pauseMenuPanel.SetActive(false);
        hidingPanel.SetActive(true);
        completeGamePanel.SetActive(true);
        endTime = Time.time; // Track the end time when the game completes
                             // Calculate the total time taken in minutes
        float totalTimeTakenInMinutes = (endTime - startTime) / 60f;
        // Calculate the rate (questions answered per minute)
        float rate = totalQuestions / totalTimeTakenInMinutes;
        int roundedRate = Mathf.RoundToInt(rate); // Round rate to the nearest whole number
        rateText.text = "Rate: " + roundedRate + "/min";
        double accuracy = ((double)correctlyAnswered / totalQuestions) * 100;
        accuracyText.text = "Accuracy:" + $"{accuracy}%";
        wrongText.text = "Wrong:" + (totalQuestions - correctlyAnswered);
        StartCoroutine(UpdateGameCompletionStats(accuracy, rate));
        DisableGameInputs();
    }

    IEnumerator UpdateGameCompletionStats(double accuracy, double completionRate)
    {
        string gameID = PlayerPrefs.GetString(UserManager.GAME_KEY);
        // Define the success and error callbacks
        System.Action<bool> onSuccess = (bool success) =>
        {
            // Handle onSuccess callback if needed
            FetchGameStats();
            Debug.Log("Updated in database as game completed");
        };
        yield return GameManager.UpdateGameCompletedStats(gameID, accuracy, completionRate, onSuccess, OnError);
    }


    void PauseGame()
    {
        hidingPanel.SetActive(true);
        pauseMenuPanel.SetActive(true);
        DisableGameInputs();
        Time.timeScale = 0; //Freeze the game
    }

    void ResumeGame()
    {
        Time.timeScale = originalTimeScale; // Unfreeze the game
        if (currentQuestionIndex == totalQuestions)
        {
            // If the game has been completed, show the complete panel
            CompleteGame();
        }
        else
        {
            hidingPanel.SetActive(false);
            pauseMenuPanel.SetActive(false);
            EnableGameInputs();
        }
    }

    void BackToMainMenu()
    {
        Time.timeScale = originalTimeScale; // Unfreeze the game
        PlayClickSound(); // Play the click sound first
        StartCoroutine(DelayBeforeMainMenuTransition());
    }

    IEnumerator DelayBeforeMainMenuTransition()
    {
        yield return new WaitForSeconds(0.3f); // Adjust the delay time as needed
        SceneManager.LoadScene("HomeViewController"); // Transition to the main menu
    }


    void RestartGame()
    {
        Time.timeScale = originalTimeScale; // Unfreeze the game
        completeGamePanel.SetActive(false);
        hidingPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        EnableGameInputs();
        currentQuestionIndex = 0;
        correctlyAnswered = 0;
        ResumeGame();
        CreateNewGame();
        LoadNextProblem();
    }

    void DisableGameInputs()
    {
        // Move game inputs behind the pause menu panel
        foreach (var button in answerButtons)
        {
            button.gameObject.transform.SetParent(pauseMenuPanel.transform);
            button.interactable = false;
            button.gameObject.SetActive(false);
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
            button.gameObject.SetActive(true);
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
        Time.timeScale = originalTimeScale; // Unfreeze the game
        currentQuestionIndex = 0;
        correctlyAnswered = 0;
        pauseMenuPanel.SetActive(false);
        completeGamePanel.SetActive(false);
        pauseButton.interactable = true;
        hidingPanel.SetActive(false);
        EnableGameInputs();
        LoadNextProblem();
    }

    void MoveToCompleteGamePanel(){
        DestroyTextBoxes();
        previousRecordsPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        hidingPanel.SetActive(true);
        completeGamePanel.SetActive(true);
    }

    void ShowPreviousRecords()
    {
        completeGamePanel.SetActive(false);
        previousRecordsPanel.SetActive(true);
        CreateTextBoxes(5);
    }

    void CreateTextBoxes(int limit)
    {
        textBoxPrefab.SetActive(true);
        float verticalSpacing = 10f;
        // Get the position of the prefab text box
        Vector3 prefabPosition = textBoxPrefab.transform.position;

        // Get the height of the prefab text box
        float textBoxHeight = textBoxPrefab.GetComponent<TextMeshProUGUI>().rectTransform.rect.height;
        // Get the font size of the prefab text box
        float fontSize = textBoxPrefab.GetComponent<TextMeshProUGUI>().fontSize;
        // Loop through the number of rows
        Debug.Log(fetchedGames.Length + "Length of stats");
        for (int i = 0; i < fetchedGames.Length && i < limit; i++)
        {
            // Instantiate a new text box prefab
            GameObject newTextBox = Instantiate(textBoxPrefab, transform);
            createdTextBoxes.Add(newTextBox); 
            // Set font size for the new text box
            TextMeshProUGUI textComponent = newTextBox.GetComponent<TextMeshProUGUI>();
            textComponent.fontSize = fontSize;
            textComponent.text = (i + 1).ToString() + ". Score: " + fetchedGames[i].noOfCorrectAnswers.ToString() + "/" +  totalQuestions.ToString();

            // Calculate position for the new text box
            float newY = prefabPosition.y - ((i + 1) * (textBoxHeight + verticalSpacing)); // Adding 1 to i because we want the new boxes to be below the prefab
            Vector3 newPosition = new Vector3(prefabPosition.x, newY, prefabPosition.z);

            // Set position for the new text box
            newTextBox.transform.position = newPosition;

            // You might want to modify other properties of the text box (like text content) here
        }
        // Deactivate the initial prefab
        textBoxPrefab.SetActive(false);
    }

     void DestroyTextBoxes()
    {   
        foreach (var textBox in createdTextBoxes)
        {
            Destroy(textBox);
        }
        createdTextBoxes.Clear();
    }


    public void FetchGameStats()
    {
        Debug.Log("Fetching games Stasts");
        userManagerInstance = FindObjectOfType<UserManager>();
        fetchedGames = userManagerInstance.FetchGameStats();
    }
}