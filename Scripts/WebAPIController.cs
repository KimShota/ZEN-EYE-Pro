using System.Collections;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebAPIController : MonoBehaviour
{
    public static WebAPIController Instance;
    // Button finishButton;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     finishButton = GameObject.Find("Finish").GetComponent<Button>();
    //     finishButton.onClick.AddListener(OnClick);
    // }

    private void Awake()
    {
        Instance = this;
    }

    // public void OnClick()
    // {
    //     Handler("POST", "", 0.0f, "{0,0,0}");
    // }

    public static void Handler(string method, string type, float score, string rawData)
    {
        string macAddress = GetMacAddress();

        // Fetch the target API URL from the provisioning API
        // Then, send the data to the target API URL
        Instance.StartCoroutine(Instance.GetAPIURL(macAddress, apiURL => {
            Instance.StartCoroutine(Instance.CallAPI(method, type, score, rawData, macAddress, apiURL));
        }));

        //Instance.StartCoroutine(Instance.CallAPI(method, type, score, rawData, macAddress));
    }

        /// <summary>
        /// This method handles the call to web APIs
        /// </summary>
        /// <param name="method"> A string that can take one of the following values
        /// CREATE
        /// DELETE
        /// GET
        /// HEAD
        /// POST
        /// PUT
        /// </param>
        /// <param name="rawData"> The data to store</param>
        /// <returns></returns>
    IEnumerator CallAPI(string method, string type, float score, string rawData, string macAddress, string apiURL)
    {
        APIData sendData = new();
        // Start Deprecated
        sendData.token = "token1234";
        sendData.userId = "52ac9242-fa73-4ae9-b548-be9bae21b727";
        // End Deprecated
        sendData.deviceId = macAddress;
        sendData.type = type;
        sendData.score = score;
        sendData.rawData = rawData;

        // Demo API
        // string apiURL = "https://ih8ynj24zl.execute-api.us-west-2.amazonaws.com/api/store";
        // Barcelona (Gallery) API
        // string apiURL = "https://ln6keufg01.execute-api.ap-northeast-1.amazonaws.com/api/store";

        string targetAPIPath = apiURL + "/api/store";

        string jsonData = JsonUtility.ToJson(sendData);
        byte[] bytePostData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(targetAPIPath);
        request.method = method;
        request.uploadHandler = new UploadHandlerRaw(bytePostData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            BodyType data = JsonUtility.FromJson<BodyType>(request.downloadHandler.text);
        }
    }

    static string GetMacAddress()
    {
        string result = "";
        foreach (NetworkInterface ninf in NetworkInterface.GetAllNetworkInterfaces())
        {
            /// TODO: Determine why this logic stopped working (Currently a temporal solution is to use the first interface) 
            //if (ninf.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
            //if (ninf.OperationalStatus == OperationalStatus.Up)
            //{
                result += ninf.GetPhysicalAddress().ToString();
                break;
            //}
        }
        return result;
    }

    IEnumerator GetAPIURL(string deviceId, System.Action<string> callback = null)
    {
        string provisioningAPI = "https://fhmxbuamme.execute-api.ap-northeast-1.amazonaws.com/api/get-url?deviceId=" + deviceId;

        UnityWebRequest request = new UnityWebRequest(provisioningAPI);
        request.method = UnityWebRequest.kHttpVerbGET;
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        // Check error
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                string url = request.downloadHandler.text;
                callback?.Invoke(url);
            }
        }
    }

}
 public class APIData
{
     public string token;
    // TODO remove userId if not needed anymore
     public string userId;
     public string deviceId;
     public string type;
     public float score;
     public string rawData;
}
public class BodyType
{
    public APIData Body;
}
