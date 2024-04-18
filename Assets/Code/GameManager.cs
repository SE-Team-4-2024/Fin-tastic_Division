using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameManager
{
    public static string baseAzureFunctionUrl = "https://team4-fin.azurewebsites.net/api/game/";
    public static string azureFunctionAuthenticationParams = "code=kBwvubTGVz7PVhfKye_z0qqRZSythQlFKnhG2zso4r2IAzFumD7ejw==&clientId=default";

    public static IEnumerator UpdateUserResponse(string gameID, bool validUserResponse, System.Action<bool> onSuccess, System.Action<string> onError)
    {
        string endpoint = $"{gameID}/update";
        string queryParams = $"?{azureFunctionAuthenticationParams}&validUserResponse={validUserResponse}";
        string fullUrl = baseAzureFunctionUrl + endpoint + queryParams;

        Debug.Log($"Updating user response as: {validUserResponse} at {fullUrl}");

        using (UnityWebRequest www = UnityWebRequest.Post(fullUrl, new WWWForm()))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = $"Failed to update user response: {www.error}";
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
            }
            else
            {
                Debug.Log("User response updated successfully");
                onSuccess?.Invoke(true);
            }
        }
    }



   public static IEnumerator UpdateGameCompletedStats(string gameId, double accuracy, double completion, System.Action<bool> onSuccess, System.Action<string> onError)
{
    string endpoint = $"{gameId}/complete";
    string queryParams = $"?{azureFunctionAuthenticationParams}&accuracy={accuracy}&completion={completion}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating game completed stats: {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, ""))
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = $"Failed to call API: {www.error}";
            Debug.LogError(errorMessage);
            onError?.Invoke(errorMessage);
        }
        else
        {
            Debug.Log("API call successful");
            onSuccess?.Invoke(true);
        }
    }
}


    public static IEnumerator CreateNewGame(string userID, System.Action<string> onSuccess, System.Action<string> onError)
    {

        string endpoint = "create";
        string queryParams = $"{azureFunctionAuthenticationParams}&userID={userID}";
        string url = $"{baseAzureFunctionUrl}{endpoint}?{queryParams}";
        Debug.Log("Creating New Game" + url);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to create game: " + www.error);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                Database data = JsonUtility.FromJson<Database>(response.Content);
                string gameID = data.gameID;
                onSuccess?.Invoke(gameID);
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