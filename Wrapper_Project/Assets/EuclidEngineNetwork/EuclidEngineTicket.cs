using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

public class EuclidEngineTicket : EditorWindow
{
    private string ticket_object = "";
    private string ticket_message = "";
    private string ticket_image = "";
    private string imagePath = "";
    private Texture2D imageTexture = null;

    private HttpStatusCode ticketStatuscode;

    [MenuItem("Euclid Engine/Support")]
    public static void Init()
    {
        EditorWindow window = GetWindow(typeof(EuclidEngineTicket));
        window.titleContent.text = "Support";
        window.minSize = new Vector2(500.0f, 280.0f);
        window.Show();
    }

    static Texture2D logoTexture = null;
    private void OnEnable()
    {
        if (logoTexture == null)
            logoTexture = Resources.Load("img/background") as Texture2D;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        ticket_object = EditorGUILayout.TextField("Title: ", ticket_object);
        EditorGUILayout.Space();
        ticket_message = EditorGUILayout.TextField("Message: ", ticket_message, GUILayout.Height(150.0f));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Screenshot (optional): ");
        if (GUILayout.Button("Select", GUILayout.Width(100)))
        {
            imagePath = EditorUtility.OpenFilePanel("Select an image to send for the ticket", "", "gif,jpeg,jpg,png");
            if (imagePath.Length != 0)
            {
                var fileContent = File.ReadAllBytes(imagePath);
                imageTexture = new Texture2D(2, 2);
                imageTexture.LoadImage(fileContent);

                ticket_image = Convert.ToBase64String(fileContent);
            }
        }
        EditorGUILayout.EndHorizontal();
        if (imageTexture != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(imageTexture, GUILayout.MaxHeight(100), GUILayout.MaxWidth(300));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        //button to send tickets
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Send Ticket", GUILayout.Width(150)))
        {
            HttpWebResponse response = EuclidEngineAPI.SendTicket(ticket_object, ticket_message, ticket_image);
            ticketStatuscode = response.StatusCode;
            if (ticketStatuscode == HttpStatusCode.OK)
            {
                EditorUtility.DisplayDialog("Ticket Status", "Ticket successfully sent !", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Ticket Status", "Ticket couldn't be sent, check your connection or Euclid Engine's server status.", "OK");
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        this.Repaint();
    }
}