using UnityEngine;
using System.Collections;

public class PassTextures : MonoBehaviour
{
    public Texture2D content;
    public Texture2D style;

    public Texture2D output;

    public string outName = "";

    public int gpu;

    private string serverName = "dendron.sportcaster.dk";

    private bool networkStarted = false;

    [HideInInspector]
    public bool generated = false;

    // Use this for initialization
    void Update ()
    {
        if (Input.GetButtonDown("Refresh") && !networkStarted)
        {
            networkStarted = true;
            GenerateTexture();
        }
    }

    public void PassTexture()
    {
        var c = content.EncodeToJPG(98);
        var s = style.EncodeToJPG(98);
        StartCoroutine(Post(c, s, this.content.width));
    }

    public IEnumerator Post(byte[] content, byte[] style, int size)
    {
        var form = new WWWForm();


        form.AddBinaryData("content", content, name + "_content_.jpg", "image/jpeg");
        form.AddBinaryData("style", style, name + "_style_.jpg", "image/jpeg");
        form.AddField("gpu", gpu);
        form.AddField("size", size);

        int iterations = 0;
        form.AddField("iterations", iterations);

        var www = new WWW(serverName + ":8180", form);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Web call has errors: " + www.error);
            yield break;
        }

        outName = www.text;
        Debug.Log("Web call is done: " + outName + " gpu " + gpu);

        if (iterations > 0)
        {
            for (int i = iterations; i < 1000; i += iterations)
            {
                yield return StartCoroutine(PollTexture("_" + i));
            }
        }

        yield return StartCoroutine(PollTexture(""));
    }

    public IEnumerator PollTexture(string append)
    {
        generated = false;
        var form = new WWWForm();
        
        form.AddField("image", outName + append);

        var www = new WWW(serverName + ":8180/live/", form);

        
        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForSeconds(1);
            if (www.isDone)
                break;
        }

        if (!www.isDone)
        {
            Debug.LogError("Error for gpu " + gpu + ": Web call did not return.");
            yield break;
        }

        //yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Error for gpu " + gpu + ": Web call has errors: " + www.error);
            yield break;
        }
        
        Debug.Log("Creating texture on gpu " + gpu);
        if (www.texture.GetPixels().Length == output.GetPixels().Length)
        {
            output.SetPixels(www.texture.GetPixels());
            output.Apply();
            generated = true;
        }
        else
        {
            Debug.LogError("Error for gpu " + gpu + ": Web texture only has " + www.texture.GetPixels().Length + " pixels\n" +
                "Required pixels are " + output.GetPixels().Length + " gpu " + gpu);
        }

        //Debug.Log("Texture " + outName + append + " generated!");
    }

    [ContextMenu("Generate texture")]
    public void GenerateTexture()
    {
        output.SetPixels(content.GetPixels());
        output.Apply();
        PassTexture();
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
