using UnityEngine;
using System;

[Serializable]
public class ApiResponse
{
    public string Content;
    public string ContentType;
    public int StatusCode;
}

[System.Serializable]
public class ApiResponseContent
{
    public User[] users; // Assuming User is another class representing user data

    // Add any other fields or properties as needed
}