using System;
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
    bool show = false;
    GUIStyle style = new GUIStyle();

    private HttpStatusCode loginStatuscode;
    private bool clickedLogin = false;
    static private bool isConnected = false;
    private bool licenceFound = false;
    private string bdate;
    private string edate;

    //static Texture2D logoTexture = null;


    private void OnEnable()
    {
        if (logoTexture == null)
            logoTexture = Resources.Load("img/background") as Texture2D;
        style.richText = true;
    }

    static Texture2D logoTexture = null;

    private void SaveCredential(string user, string mdp)
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        Debug.Log(destination);

        EuclidEngineData data = new EuclidEngineData(user, mdp);
        data.encodeUser();
        data.encodeMdp();
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }
    [MenuItem("Euclid Engine/Account")]
    public static void Init()
    {
        EditorWindow window = GetWindow(typeof(EuclidEngineConnection));
        window.titleContent.text = "Account";
        window.Show();

        if (EuclidWindow.LoadCredential())
        {
            isConnected = true;
        }
    }

    void OnGUI()
    {
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(logoTexture, GUILayout.Height(75), GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (isConnected == false)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 30.0f;
            EditorGUILayout.LabelField("Email Address");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            username = EditorGUILayout.TextField(username, GUILayout.Width(175.0f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 3.0f;
            EditorGUILayout.LabelField("Password");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            mdp = EditorGUILayout.PasswordField(mdp, GUILayout.Width(175.0f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
/*<<<<<<< HEAD:Wrapper_Project/Assets/EuclidEngine/Network/EuclidEngineConnectionWindow.cs

=======
>>>>>>> d8a17bbb5c53511fe0820d60204dc5cb9c164b30:Wrapper_Project/Assets/EuclidEngineNetwork/EuclidEngineConnectionWindow.cs*/
        }
        EditorGUILayout.Space();
        //buton to save credentials
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (isConnected == false && GUILayout.Button("Log in", EditorStyles.miniButtonMid, GUILayout.Width(100.0f)))
        {
            HttpWebResponse response = EuclidEngineAPI.Login(username, mdp);
            loginStatuscode = response.StatusCode;
            clickedLogin = true;
            if (loginStatuscode == HttpStatusCode.OK)
            {
                isConnected = true;
                SaveCredential(username, mdp);
            }
        }
        //buton to delete credentials
        if (isConnected && GUILayout.Button("Log off"))
        {
            loginStatuscode = HttpStatusCode.NoContent;
            File.Delete(Application.persistentDataPath + "/save.dat");
            isConnected = false;
            loginStatuscode = HttpStatusCode.NoContent;
            username = "";
            mdp = "";
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (!isConnected && GUILayout.Button("Register", EditorStyles.miniButtonMid, GUILayout.Width(100.0f)))
        {
            Application.OpenURL("https://euclidengine.com/register");
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (loginStatuscode == HttpStatusCode.BadRequest)
        {
            EditorUtility.DisplayDialog("Authentification", "Email or password is wrong.", "OK");
            loginStatuscode = HttpStatusCode.Continue;
        }
        EditorGUILayout.LabelField(loginStatuscode == HttpStatusCode.OK ? "Authentification Successful" : loginStatuscode == HttpStatusCode.BadRequest ? "Authentification Failed" : "");
        if (isConnected && clickedLogin)
        {
            clickedLogin = false;
            CheckLicence();
        }
        if (isConnected)
        {
            EditorGUILayout.LabelField(licenceFound ? "Found Licence valid from " + bdate + " to " + edate : "No licence found");
            if (GUILayout.Button("Refresh Licence"))
            {
                CheckLicence();
            }
        }

//<<<<<<< HEAD:Wrapper_Project/Assets/EuclidEngine/Network/EuclidEngineConnectionWindow.cs
        if (isConnected == false)
        {
            show = EditorGUI.Foldout(new Rect(3, 260, position.width - 6, 15), show, "Usefull links");
            if (show)
            {
                if (Selection.activeTransform)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    LinkButton("Contact us ", "here", "https://euclidengine.com/#Contact");
                    LinkButton("Wanna know more, see our ", "documentation", "https://docs.euclidengine.com");
                    LinkButton("Licence prices ", "here", "https://euclidengine.com/#About");
                }
            }
        }
        GUILayout.FlexibleSpace();


///=======
        GUILayout.FlexibleSpace();
//>>>>>>> d8a17bbb5c53511fe0820d60204dc5cb9c164b30:Wrapper_Project/Assets/EuclidEngineNetwork/EuclidEngineConnectionWindow.cs
        this.Repaint();
    }

    private void LinkButton(string baseurl, string caption, string url)
    {
        var style = GUI.skin.label;
        style.richText = true;
        caption = string.Format("<color=#3944BC>{0}</color>", caption);

        bool bClicked = GUILayout.Button(baseurl + caption, style);

        var rect = GUILayoutUtility.GetLastRect();
        rect.width = style.CalcSize(new GUIContent(caption)).x;
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

        if (bClicked)
            Application.OpenURL(url);
    }

    private void CheckLicence()
    {
        //HttpWebResponse response = EuclidEngineAPI.GetLicence();
        if (username.Length == 0)
            return;

        HttpWebResponse response = EuclidEngineAPI.PostCheckLicense(username);
        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log("response :" + responseString.Split('\"')[3].Split('T')[0]);
        bdate = responseString.Split('\"')[3].Split('T')[0];
        Debug.Log("response :" + responseString.Split('\"')[7].Split('T')[0]);
        edate = responseString.Split('\"')[7].Split('T')[0];
        licenceFound = responseString.Contains("\"is_valid\":true") ? true : false;
    }
}