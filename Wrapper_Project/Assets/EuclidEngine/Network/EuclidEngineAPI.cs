using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

using System;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Text;

[InitializeOnLoad]
[ExecuteInEditMode]
public class EuclidEngineAPI : MonoBehaviour
{
    private const string EE_Url = "http://euclid-engine.francecentral.cloudapp.azure.com:";
    private const string EE_ApiComp = "/api";

    private const string AccountPort        = "8081";
    private const string AuthControllerPort = "8084";
    private const string LicenceManagerPort = "8082";
    private const string TicketManagerPort  = "8083";
    private const string NewsLetterPort     = "8080";
    private const string FeedbackPort       = "8086";

    static UnityWebRequest www;
    static Action<string> s_requestCallback;
    static string bearerToken = "";

    public static void Login(string email, string password)
    {
        string jsonBody;
        jsonBody = "{" +
            "\"email\": \"" + email + "\"," +
            "\"password\": \"" + password + "\"" +
            "}";
        SendPostRequest("/login", AuthControllerPort, jsonBody, (string response) => {
            bearerToken = response;
            Debug.Log("Token is: " + bearerToken);
        });
    }

    public static void SendTicket(string ticket_object, string ticket_message, string ticket_image)
    {
        string jsonBody;
        jsonBody = "{" +
            "\"ticket_object\": \"" + ticket_object + "\"," +
            "\"ticket_message\": \"" + ticket_message + "\"," +
            "\"ticket_image\": \"" + ticket_image + "\"" +
        "}";
        SendPostRequest("/sendTicket", TicketManagerPort, jsonBody, (string response) => {
            Debug.Log("Ticket Response: " + response);
        });
    }

    private static void SendPostRequest(string route, string port, string jsonBody, Action<string> requestCallback)
    {
        string fullUrl = EE_Url + port + EE_ApiComp + route;
        www = UnityWebRequest.Post(fullUrl, jsonBody);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        if (bearerToken.Length != 0)
            www.SetRequestHeader("Authorization", "Bearer " + bearerToken);
        www.SetRequestHeader("User-Agent", "PostmanRuntime/7.29.0");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "*/*");
        www.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        www.SendWebRequest();

        EditorApplication.update += EditorUpdate;
        s_requestCallback = requestCallback;
    }

    private static void EditorUpdate()
    {
        if (!www.isDone)
            return;

        if (www.isNetworkError)
            Debug.LogError(www.error);
        else
        {
            Debug.Log("Request Code: " + www.responseCode.ToString());
            s_requestCallback(www.downloadHandler.text);
        }

        EditorApplication.update -= EditorUpdate;
    }
}
