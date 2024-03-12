using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public static class DivisionProblemLoader
{
    public static string azureFunctionUrl = "https://funtiondivision.azurewebsites.net/api/http_trigger?code=Gz0NyagnzlXr_fjWsHY6XzxsypaakixaDpLMqthg-8qjAzFuFl2arg==";

    public static IEnumerator LoadDivisionProblem(System.Action<DivisionProblem> onSuccess, System.Action<string> onError)
    {
        Debug.Log("Attempting to load division problem from: " + azureFunctionUrl);

        using (UnityWebRequest www = UnityWebRequest.Get(azureFunctionUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = "Failed to load division problem: " + www.error;
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
            }
            else
            {
                string json = www.downloadHandler.text;
                Debug.Log("Received JSON data: " + json);

                try
                {
                    DivisionProblem problem = JsonUtility.FromJson<DivisionProblem>(json);
                    onSuccess?.Invoke(problem);
                }
                catch (System.Exception ex)
                {
                    string errorMessage = "Error parsing JSON data: " + ex.Message;
                    Debug.LogError(errorMessage);
                    onError?.Invoke(errorMessage);
                }
            }
        }
    }
}
