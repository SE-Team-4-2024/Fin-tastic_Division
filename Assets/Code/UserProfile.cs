using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class UserProfile
{
    public static string baseAzureFunctionUrl = "https://team4-fin.azurewebsites.net/api/user/";
    public static string azureFunctionAuthenticationParams = "code=kBwvubTGVz7PVhfKye_z0qqRZSythQlFKnhG2zso4r2IAzFumD7ejw==&clientId=default";

   public static IEnumerator GetUsers(string deviceID, Action<User[]> onSuccess, Action<string> onError)
    {
        string queryParams = $"{azureFunctionAuthenticationParams}&deviceId={deviceID}";
        string url = $"{baseAzureFunctionUrl}?{queryParams}";
        Debug.Log("Getting List of Users: " + url);

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
                // Deserialize the JSON response
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);

                // Parse the 'Content' property as an array of User object
                ApiResponseContent content = JsonUtility.FromJson<ApiResponseContent>(response.Content);

                if (content == null || (content.users == null || content.users.Length <= 0))
                {
                    onSuccess?.Invoke(new User[0]); // Assuming onError is a delegate that accepts a User[] parameter
                    yield break;
                }

                List<User> userList = new List<User>();

                foreach (User user in content.users)
                {
                    userList.Add(user);
                }

                onSuccess?.Invoke(userList.ToArray());
            }
            catch (Exception ex)
            {
                string errorMessage = "Error parsing JSON data: " + ex.Message;
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
            }
        }
    }

    public static User[] GetUsersSync(string deviceID)
{
    string queryParams = $"{azureFunctionAuthenticationParams}&deviceId={deviceID}";
    string url = $"{baseAzureFunctionUrl}?{queryParams}";
    Debug.Log("Getting List of Users: " + url);

    using (UnityWebRequest www = UnityWebRequest.Get(url))
    {
        www.SendWebRequest();

        while (!www.isDone)
        {
            // Wait for the request to complete
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to fetch user data: " + www.error;
            Debug.LogError(errorMessage);
            return new User[0];
        }

        string responseBody = www.downloadHandler.text;
        Debug.Log("Received JSON data: " + responseBody);

        try
        {
            // Deserialize the JSON response
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(responseBody);

            // Parse the 'Content' property as an array of User object
            ApiResponseContent content = JsonUtility.FromJson<ApiResponseContent>(response.Content);

            if (content == null || content.users == null || content.users.Length <= 0)
            {
                return new User[0];
            }

            return content.users;
        }
        catch (Exception ex)
        {
            string errorMessage = "Error parsing JSON data: " + ex.Message;
            Debug.LogError(errorMessage);
            return new User[0];
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


public static IEnumerator UpdateUserDetails(string userId, string name,string profilePicture, string isMusicEnabled, string isSoundEnabled, System.Action<bool> onSuccess, System.Action<bool> onError)
{

    string endpoint = $"{userId}/updateDetails";
    string queryParams = $"?{azureFunctionAuthenticationParams}&name={name}&profilePicture={profilePicture}&isMusicEnabled={isMusicEnabled}&isSoundEnabled={isSoundEnabled}";
    string url = $"{baseAzureFunctionUrl}{endpoint}{queryParams}";

    Debug.Log($"Updating user details @ {url}");

    using (UnityWebRequest www = UnityWebRequest.Put(url, new byte[0])) // Use byte[0] as the request body for PUT requests
    {
        yield return www.SendWebRequest();
        Debug.Log("Request body.");

        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorMessage = "Failed to update details: " + www.error;
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

