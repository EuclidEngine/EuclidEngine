using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class EuclidEngineConnection : EditorWindow
{
    private string username = "";
    private string mdp = "";

    private void SaveCredential(string user, string mdp)
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        Debug.Log(destination);

        EuclidEngineData data = new EuclidEngineData(user, mdp);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    [MenuItem("Euclid Engine/Connexion")]
    public static void Init()
    {
        EditorWindow window = GetWindow(typeof(EuclidEngineConnection));
        window.titleContent.text = "Login";
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        username = EditorGUILayout.TextField("Username: ", username);
        EditorGUILayout.Space();
        mdp = EditorGUILayout.PasswordField("Password: ", mdp);
        EditorGUILayout.Space();
        //buton to save credentials
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Log in"))
        {
            EuclidEngineAPI.Login(username, mdp);
            SaveCredential(username, mdp);
        }
        //buton to delete credentials
        if (GUILayout.Button("Delete credentials"))
        {
            File.Delete(Application.persistentDataPath + "/save.dat");
        }
        EditorGUILayout.EndHorizontal();
        this.Repaint();
    }
}