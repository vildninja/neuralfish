﻿using UnityEngine;
using System.Collections;

public class PassTextures : MonoBehaviour
{
    public Texture2D content;
    public Texture2D style;

    public Texture2D output;

    public string outName = "";

    public int gpu;

    private string serverName = "server.dk";

    private bool networkStarted = false;

    // Use this for initialization
    void Update ()
    {
        if (Input.GetButtonDown("Refresh") && !networkStarted)
        {
            networkStarted = true;
            output.SetPixels(content.GetPixels());
            output.Apply();
            PassTexture();
        }
    }

    public void PassTexture()
    {
        var c = content.EncodeToJPG(98);
        var s = style.EncodeToJPG(98);
        StartCoroutine(Post(c, s));
    }

    public IEnumerator Post(byte[] content, byte[] style)
    {
        var form = new WWWForm();


        form.AddBinaryData("content", content, name + "_content.jpg", "image/jpeg");
        form.AddBinaryData("style", style, name + "_style.jpg", "image/jpeg");
        form.AddField("gpu", gpu);
        form.AddField("size", this.content.width);

        var www = new WWW(serverName + ":8180", form);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Web call has errors: " + www.error);
            yield break;
        }

        outName = www.text;
        Debug.Log("Web call is done: " + outName);

        for (int i = 30; i < 1000; i += 30 * 5)
        {
            yield return StartCoroutine(PollTexture("_" + i));
        }

        yield return StartCoroutine(PollTexture(""));
    }

    public IEnumerator PollTexture(string append)
    {
        var form = new WWWForm();
        
        form.AddField("image", outName + append);

        var www = new WWW(serverName + ":8180/live/", form);

        yield return www;
        
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Web call has errors: " + www.error);
            yield break;
        }
        
        output.SetPixels(www.texture.GetPixels());
        output.Apply();

        Debug.Log("Texture " + outName + append + " generated!");
    }

#if UNITY_EDITOR
    [ContextMenu("Create Output")]
    public void CreateOutput()
    {
        output = new Texture2D(content.width, content.height);
        UnityEditor.AssetDatabase.CreateAsset(output, "Assets/" + name + ".asset");
    }

    [ContextMenu("Bake Output")]
    public void BakeOutput()
    {
        var data = output.EncodeToJPG(98);
        var path = UnityEditor.AssetDatabase.GetAssetPath(output);
        System.IO.File.WriteAllBytes(path, data);
    }

    [ContextMenu("Bake Output as New")]
    public void BakeAsNew()
    {
        var data = output.EncodeToJPG(98);
        var path = UnityEditor.AssetDatabase.GetAssetPath(output);
        path = path.Replace(".jpg", "") + "_" + style.name + ".jpg";
        System.IO.File.WriteAllBytes(path, data);
    }
#endif
}
