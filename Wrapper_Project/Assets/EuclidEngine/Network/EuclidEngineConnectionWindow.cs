using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

class EuclidEngineConnection : EditorWindow
{
    private string username = "";
    private string mdp = "";


    private HttpStatusCode loginStatuscode;
    private bool clickedLogin = false;
    static private bool isConnected = false;
    private bool licenceFound = false;

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

        if (EuclidWindow.LoadCredential())
        {
            isConnected = true;
        }
    }

    void OnGUI()
    {
        if(isConnected == false)
        {
            EditorGUILayout.Space();
            username = EditorGUILayout.TextField("Username: ", username);
            EditorGUILayout.Space();
            mdp = EditorGUILayout.PasswordField("Password: ", mdp);
        }
        EditorGUILayout.Space();
        //buton to save credentials
        EditorGUILayout.BeginHorizontal();
        if (isConnected == false && GUILayout.Button("Log in"))
        {
            HttpWebResponse response = EuclidEngineAPI.Login(username, mdp);
            loginStatuscode = response.StatusCode;
            clickedLogin = true;
            if (loginStatuscode == HttpStatusCode.OK) {
                isConnected = true;
                SaveCredential(username, mdp);
            }
        }
        //buton to delete credentials
        if (GUILayout.Button("Delete credentials"))
        {
            File.Delete(Application.persistentDataPath + "/save.dat");
            isConnected = false;
            loginStatuscode = HttpStatusCode.NoContent;
            username = "";
            mdp = "";
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(loginStatuscode == HttpStatusCode.OK ? "Authentification Successful (code: 200)" : loginStatuscode == HttpStatusCode.BadRequest ? "Not Found (code: 400)" : "");
        if (isConnected && clickedLogin)
        {
            clickedLogin = false;
            CheckLicence();
        }
        if (isConnected) {
            EditorGUILayout.LabelField(licenceFound ? "Licence found" : "No licence found");
            if (GUILayout.Button("Refresh Licence"))
            {
                CheckLicence();
            }
        }
        this.Repaint();
    }

    private void CheckLicence()
    {
        HttpWebResponse response = EuclidEngineAPI.GetLicence();
        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        licenceFound = responseString.Contains("\"is_valid\":true") ? true : false;
    }
}