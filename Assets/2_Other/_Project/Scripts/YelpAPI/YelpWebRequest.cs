using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public static class YelpWebRequest
{
    private static string APIKey = 
        "ubBOhXtTPA18rNZvA5dqIMCr41jMc90ooq-bcr3w1GGkai73pahmTHE79XOz8HJCFVoVbtFOHAotqmugOTMnBbt6edjTZzA54tZ3bSmVSOvf7PmyVyTNSuai3iSOY3Yx";

    public static async Task<JSONNode> GetWebData(string url)
    {
        UnityWebRequest webReq = new UnityWebRequest();
        
        webReq.SetRequestHeader("Authorization", $"Bearer {APIKey}");
        webReq.SetRequestHeader("accept", $"application/json");
        webReq.url = url;
        webReq.downloadHandler = new DownloadHandlerBuffer();
        webReq.SendWebRequest();
        while (!webReq.isDone)
        {
            await Task.Yield();
        }

        if (webReq.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log($"Error While Sending: {webReq.error}");
            return null;
        }
        else
        {
            string rawJson = Encoding.Default.GetString(webReq.downloadHandler.data);
            var jsonResult = JSON.Parse(rawJson);
            return jsonResult;
        }
    }
    
}
