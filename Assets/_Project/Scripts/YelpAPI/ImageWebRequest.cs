using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public static class ImageWebRequest
{
    public static async Task<Texture2D> GetWebImage(string url)
    {
        using (UnityWebRequest webReq = UnityWebRequestTexture.GetTexture(url))
        {
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
                var texture = DownloadHandlerTexture.GetContent(webReq);
                return texture;
            }
        }
    }
}
