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
    static public string bearerToken = "";

    public static HttpWebResponse Login(string email, string password)
    {
        string jsonBody;
        jsonBody = "{" +
            "\"email\": \"" + email + "\"," +
            "\"password\": \"" + password + "\"" +
            "}";
        HttpWebResponse response = SendPostRequest("/login", AuthControllerPort, jsonBody);
        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        bearerToken = responseString;
        Debug.Log("Token is: " + responseString);
        return(response);
    }

    public static HttpWebResponse SendTicket(string ticket_object, string ticket_message, string ticket_image)
    {
        string jsonBody;
        if (ticket_image.Length == 0)
        {
            jsonBody = "{" +
            "\"ticket_object\": \"" + ticket_object + "\"," +
            "\"ticket_message\": \"" + ticket_message + "\"" +
            "}";
        }
        else
        {
            jsonBody = "{" +
            "\"ticket_object\": \"" + ticket_object + "\"," +
            "\"ticket_message\": \"" + ticket_message + "\"," +
            "\"ticket_image\": \"" + ticket_image + "\"" +
            "}";
        }
        HttpWebResponse response = SendPostRequest("/sendTicket", TicketManagerPort, jsonBody);/*, (string response) => {
            Debug.Log("Ticket Response: " + response);
        });*/
        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log("Ticket Response: " + responseString);
        return(response);
    }

    public static HttpWebResponse GetLicence()
    {
        HttpWebResponse response = GetRequest("/getLicence", LicenceManagerPort, "{}", true);
        return(response);
    }

    private static HttpWebResponse GetRequest(string route, string port, string jsonBody, bool needToken)
    {
        string fullUrl = EE_Url + port + EE_ApiComp + route;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
        if (needToken)
            request.Headers.Add("Authorization", "Bearer "+ bearerToken);

        HttpWebResponse response = null;
        try {
            response = (HttpWebResponse)request.GetResponse();
        } catch (WebException ex)
        {
            response = (HttpWebResponse)ex.Response;
        }

        return response;
    }

    private static HttpWebResponse SendPostRequest(string route, string port, string jsonBody/*, Action<string> requestCallback*/)
    {
        string fullUrl = EE_Url + port + EE_ApiComp + route;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
        byte[] data = Encoding.ASCII.GetBytes(jsonBody);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        if (bearerToken.Length != 0)
            request.Headers.Add("Authorization", "Bearer " + bearerToken);
        request.ContentType = "application/json";
        //request.UserAgent = "PostmanRuntime/7.29.0";
        request.Accept = "*/*";
        request.ContentLength = data.Length;
        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }
        HttpWebResponse response = null;
        try {
            response = (HttpWebResponse)request.GetResponse();
        } catch (WebException ex)
        {
            response = (HttpWebResponse)ex.Response;
        }
        return response;


        //www = UnityWebRequest.Post(fullUrl, jsonBody);
        //byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        //www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        //www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //if (bearerToken.Length != 0)
        //    www.SetRequestHeader("Authorization", "Bearer " + bearerToken);
        //www.SetRequestHeader("User-Agent", "PostmanRuntime/7.29.0");
        //www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("Accept", "*/*");
        //www.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        //www.SendWebRequest();

        //EditorApplication.update += EditorUpdate;
        //s_requestCallback = requestCallback;
    }

    //private static void EditorUpdate()
    //{
    //    if (!www.isDone)
    //        return;

    //    if (www.isNetworkError)
    //        Debug.LogError(www.error);
    //    else
    //    {
    //        Debug.Log("Request Code: " + www.responseCode.ToString());
    //        s_requestCallback(www.downloadHandler.text);
    //    }

    //    EditorApplication.update -= EditorUpdate;
    //}
}
