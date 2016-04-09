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

    // Use this for initialization
    void Start ()
    {
        output.SetPixels(content.GetPixels());
        output.Apply();
	    PassTexture();
	}
	
	// Update is called once per frame
	void Update ()
	{
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

        var www = new WWW(serverName + ":8180", form);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Web call has errors: " + www.error);
        }

        outName = www.text;
        Debug.Log("Web call is done: " + outName);

        for (int i = 1; i < 10; i++)
        {
            yield return StartCoroutine(PollTexture("_" + (i*100)));
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
        }
        
        output.SetPixels(www.texture.GetPixels());
        output.Apply();
        Debug.Log("Texture " + outName + append + " generated!");
    }

#if UNITY_EDITOR
    [ContextMenu("CreateOutput")]
    public void CreateOutput()
    {
        output = new Texture2D(512, 512);
        UnityEditor.AssetDatabase.CreateAsset(output, "Assets/" + name + ".asset");
    }
#endif
}
