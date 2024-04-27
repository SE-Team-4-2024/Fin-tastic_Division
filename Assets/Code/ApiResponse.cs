using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ApiResponse
{
    public string Content;
    public string ContentType;
    public int StatusCode;
}

// Define a class to represent the ApiResponseContent structure
[Serializable]
public class ApiResponseContent
{
    // ApiResponseContent contains an array of users and a Content property
    public User[] users; // Assuming User is another class representing user data
}
