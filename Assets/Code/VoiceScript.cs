// using UnityEngine;
// using TMPro;
// using Microsoft.CognitiveServices.Speech;

// public class VoiceScript : MonoBehaviour
// {
//     public TextMeshProUGUI questionText; // Reference to your TextMeshProUGUI component

//     private SpeechConfig config;
//     private SpeechSynthesizer synthesizer;
//     private bool isSpeaking = false; // Flag to track if speech synthesis is in progress

//     private void Start()
//     {
//         config = SpeechConfig.FromSubscription("3682d17a97344d87a5af576a55564133", "westus3");
//         synthesizer = new SpeechSynthesizer(config);
//     }

//     public async void Speak()
//     {
//         if (!isSpeaking) // Ensure that speech synthesis is not already in progress
//         {
//             string textToSpeak = questionText.text.Replace("/", " divided by "); // Replace all occurrences of "/" with " divided by"

//             isSpeaking = true; // Set flag to indicate speech synthesis is in progress

//             var result = await synthesizer.SpeakTextAsync(textToSpeak); // Use the modified text

//             isSpeaking = false; // Reset flag after speech synthesis is complete

//             if (result.Reason == ResultReason.SynthesizingAudioCompleted)
//             {
//                 Debug.Log("Speech synthesized to speaker for text [" + textToSpeak + "]");
//             }
//             else if (result.Reason == ResultReason.Canceled)
//             {
//                 var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
//                 Debug.Log($"CANCELED: Reason={cancellation.Reason}");

//                 if (cancellation.Reason == CancellationReason.Error)
//                 {
//                     Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
//                     Debug.Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
//                     Debug.Log("CANCELED: Did you update the subscription info?");
//                 }
//                 else
//                 {
//                     Debug.Log("Speech synthesis canceled.");
//                 }
//             }
//             else
//             {
//                 Debug.Log($"Unexpected ResultReason: {result.Reason}");
//             }
//         }
//         else
//         {
//             // If speech synthesis is already in progress, stop it and start the new synthesis
//             synthesizer.StopSpeakingAsync();
//         }
//     }

//     public void StopSpeaking()
//     {
//         // Stop speech synthesis if it's in progress
//         if (isSpeaking)
//         {
//             synthesizer.StopSpeakingAsync();
//             isSpeaking = false; // Reset the flag
//         }
//     }
// }
