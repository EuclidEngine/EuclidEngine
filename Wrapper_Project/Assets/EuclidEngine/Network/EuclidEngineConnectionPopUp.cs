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

    public void encodeMdp()
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(mdp);
        mdp = System.Convert.ToBase64String(plainTextBytes);
    }
    public void encodeUser()
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(user);
        user = System.Convert.ToBase64String(plainTextBytes);
    }

    public string getDecodedString(string encoded)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(encoded);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public void setUser(string usergiven)
    {
        user = usergiven;
    }

    public void setMdp(string mdpgiven)
    {
        mdp = mdpgiven;
    }

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

        HttpWebResponse response = EuclidEngineAPI.Login(user, mdp);
        if (response.StatusCode == HttpStatusCode.OK)
            return true;
        else
            return false;
    }

    // Does credential already exist
    static public bool LoadCredential()
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
        data.setUser(data.getDecodedString(data.getUser()));
        data.setMdp(data.getDecodedString(data.getMdp()));

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
                        "You seem not to be logged in to your Euclid Engine account, to ensure the quality of our services please connect to your account using Euclid Engine/Connection", "I understand");
            }
        }
    }
}