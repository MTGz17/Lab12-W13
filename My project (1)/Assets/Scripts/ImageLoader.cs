using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class ImageLoader : MonoBehaviour
{
    private const string imageUrl1 = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Cat_August_2010-4.jpg/2560px-Cat_August_2010-4.jpg";
    private const string imageUrl2 = "https://upload.wikimedia.org/wikipedia/commons/1/17/La_mort_Saint-Innocent_Louvre_R.F.2625.jpg";
    private const string imageUrl3 = "https://upload.wikimedia.org/wikipedia/commons/4/43/Fernsehturm%2C_Berl%C3%ADn%2C_Alemania%2C_2016-04-22%2C_DD_40-42_HDR.jpg";

    private Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();

    public GameObject billboard1;
    public GameObject billboard2;
    public GameObject billboard3;

    void Start()
    {
        SetTextureOnBillboard(billboard1, imageUrl1);
        SetTextureOnBillboard(billboard2, imageUrl2);
        SetTextureOnBillboard(billboard3, imageUrl3);
    }

    public IEnumerator DownloadImage(string imageUrl, Action<Texture2D> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            callback(texture);
        }
        else
        {
            Debug.LogError("Image download failed: " + request.error);
        }
    }

    public void GetWebImage(string imageUrl, Action<Texture2D> callback)
    {
        if (imageCache.ContainsKey(imageUrl))
        {
            callback(imageCache[imageUrl]);
        }
        else
        {
            StartCoroutine(DownloadImage(imageUrl, texture =>
            {
                imageCache[imageUrl] = texture;
                callback(texture);
            }));
        }
    }

    public void SetTextureOnBillboard(GameObject billboard, string imageUrl)
    {
        GetWebImage(imageUrl, texture =>
        {
            billboard.GetComponent<Renderer>().material.mainTexture = texture;
        });
    }
}