using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Collections;

public static class UserProfile
{
    public static string baseAzureFunctionUrl = "https://team4-fin.azurewebsites.net/api/user/";
    public static string azureFunctionAuthenticationParams = "code=kBwvubTGVz7PVhfKye_z0qqRZSythQlFKnhG2zso4r2IAzFumD7ejw==&clientId=default";

   public static IEnumerator GetUsers(string deviceID, Action<User[]> onSuccess, Action<string> onError)
    {
        string queryParams = $"{azureFunctionAuthenticationParams}&deviceId={deviceID}";
        string url = $"{baseAzureFunctionUrl}?{queryParams}";
        Debug.Log("Getting List of Users" + url);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = "Failed to fetch user data: " + www.error;
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
                yield break;
            }

            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Parse the 'Content' property as JSON to access the 'users' array
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                Debug.Log("Response Content Found: " + response.Content);

                // Parse the 'Content' property as an array of User object
                ApiResponseContent content = JsonUtility.FromJson<ApiResponseContent>(response.Content);

                // Check if the users array is not null and has at least one element
                if (content.users != null && content.users.Length > 0)
                {
                    onSuccess?.Invoke(content.users);
                }
                else
                {
                    // Handle case where users array is null or empty
                    string errorMessage = "No users found in the response.";
                    Debug.Log(errorMessage);
                    onError?.Invoke(errorMessage);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
            }

        }
    }


   public static IEnumerator GetorCreateUser(string deviceId, string name, string profilePicture, System.Action<string> onSuccess, System.Action<string> onError)
{
    string queryParams = $"{azureFunctionAuthenticationParams}&deviceId={deviceId}&name={name}&profilePicture={profilePicture}";
    string url = $"{baseAzureFunctionUrl}?{queryParams}";

    Debug.Log($"Creating user @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to get user: " + www.error;
            Debug.LogError(errorMessage);
            onError?.Invoke(errorMessage);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                Debug.Log("Received JSON data: " + response.Content);
                onSuccess?.Invoke(response.Content);
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


public static IEnumerator UpdateName(string userId, string name, System.Action<bool> onSuccess, System.Action<bool> onError)
{

    string endpoint = $"{userId}/updateName";
    string queryParams = $"?{azureFunctionAuthenticationParams}&name={name}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating user @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to update name: " + www.error;
            Debug.LogError(errorMessage);
            onError?.Invoke(false);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                onSuccess?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(false);
            }
        }
    }
}


public static IEnumerator UpdateSound(string userId, string isSoundEnabled, System.Action<bool> onSuccess, System.Action<bool> onError)
{

    string endpoint = $"{userId}/updateSound";
    string queryParams = $"?{azureFunctionAuthenticationParams}&isSoundEnabled={isSoundEnabled}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating user @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to update sound: " + www.error;
            Debug.LogError(errorMessage);
            onError?.Invoke(false);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                onSuccess?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(false);
            }
        }
    }
}

public static IEnumerator UpdateMusic(string userId, string isMusicEnabled, System.Action<bool> onSuccess, System.Action<bool> onError)
{

    string endpoint = $"{userId}/updateMusic";
    string queryParams = $"?{azureFunctionAuthenticationParams}&isMusicEnabled={isMusicEnabled}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating user @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to update music: " + www.error;
            Debug.LogError(errorMessage);
            onError?.Invoke(false);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                onSuccess?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(false);
            }
        }
    }
}

public static IEnumerator UpdatePrimaryUser(string userId, string deviceId, System.Action<bool> onSuccess, System.Action<bool> onError)
{

    string endpoint = $"{userId}/updatePrimaryUser";
    string queryParams = $"?{azureFunctionAuthenticationParams}&deviceID={deviceId}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating user @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to update primary user: " + www.error;
            Debug.LogError(errorMessage);
            onError?.Invoke(false);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                onSuccess?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(false);
            }
        }
    }
}


public static IEnumerator UpdateProfilePicture(string userId, string profilePicture, System.Action<bool> onSuccess, System.Action<bool> onError)
{

    string endpoint = $"{userId}/updateProfilePicture";
    string queryParams = $"?{azureFunctionAuthenticationParams}&profilePicture={profilePicture}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating user @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to update profile picture: " + www.error;
            Debug.LogError(errorMessage);
            onError?.Invoke(false);
        }
        else
        {
            string responseBody = www.downloadHandler.text;
            Debug.Log("Received JSON data: " + responseBody);
            try
            {
                // Assuming apiResponse is the JSON string received from the API
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);
                onSuccess?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(false);
            }
        }
    }
}

}