using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EuclidEngineTicket : EditorWindow
{
    private string ticket_object = "";
    private string ticket_message = "";
    private string ticket_image = "";
    private string imagePath = "";

    [MenuItem("Euclid Engine/Tickets")]
    public static void Init()
    {
        EditorWindow window = GetWindow(typeof(EuclidEngineTicket));
        window.titleContent.text = "Ticket Requests";
        window.minSize = new Vector2(500.0f, 280.0f);
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        ticket_object = EditorGUILayout.TextField("Ticket Object/Title: ", ticket_object);
        EditorGUILayout.Space();
        ticket_message = EditorGUILayout.TextField("Message: ", ticket_message, GUILayout.Height(150.0f));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Screenshot (if wanted): " + (ticket_image.Length == 0 ? "" : imagePath));
        if (GUILayout.Button("Select"))
        {
            imagePath = EditorUtility.OpenFilePanel("Select an image to send for the ticket", "", "gif,jpeg,jpg,png");
            if (imagePath.Length != 0)
            {
                var fileContent = File.ReadAllBytes(imagePath);

                ticket_image = Convert.ToBase64String(fileContent);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //button to send tickets
        if (GUILayout.Button("Send Ticket"))
        {
            EuclidEngineAPI.SendTicket(ticket_object, ticket_message, ticket_image);
        }
        this.Repaint();
    }
}