using UnityEngine;
using UnityEditor;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class EuclidEngineData
{
    private string mdp = "";
    private string user = "";

    public EuclidEngineData(string UserName, string UserMdp)
    {
        user = UserName;
        mdp = UserMdp;
    }

    public string getMdp() { return mdp; }

    public string getUser() { return user; }
}

[InitializeOnLoad]
public class EuclidWindow : MonoBehaviour//EditorWindow
{
    private string url = "";

    private static bool IsNullOrEmpty(string s)
    {
        bool result;
        result = s == null || s == string.Empty;
        return result;
    }

    static bool ConnectToEuclid(string user, string mdp)
    {
        // Test string validity
        if (IsNullOrEmpty(user) || IsNullOrEmpty(mdp))
            return false;
        return true;

        // Test account Validity
        // HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    }

    // Does credential already exist
    static bool LoadCredential()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            // File do not exist
            return false;
        }

        BinaryFormatter bf = new BinaryFormatter();
        EuclidEngineData data = (EuclidEngineData)bf.Deserialize(file);
        file.Close();

        if (data == null)
            return false;

        return ConnectToEuclid(data.getUser(), data.getMdp());
    }

    // First function to be executed and call OnInit
    static EuclidWindow()
    {
        EditorApplication.update += OnEnable;
    }

    static void OnEnable()
    {
        EditorApplication.update -= OnEnable;
        if (SessionState.GetBool("FirstInitDone", true))
        {
            SessionState.SetBool("FirstInitDone", false);
            if (!LoadCredential())
            {
                EditorUtility.DisplayDialog("Please login to Euclid Engine",
                        "You seems to be not login to your Euclid Engine account, to ensure the quality of our services please connect to your account using Window/EuclidEngineConnection", "I understand");
            }
        }
    }
}