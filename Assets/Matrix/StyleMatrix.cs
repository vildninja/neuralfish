using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class StyleMatrix : MonoBehaviour
{
    public Texture2D[] contents;
    public Texture2D[] styles;

    private Texture2D style;
    private Texture2D content;

    private IEnumerator compare;

    // Use this for initialization
    void Start ()
    {
        compare = CompareLoop();

        foreach (var pass in GetComponents<PassTextures>())
        {
            if (pass.enabled)
                StartCoroutine(Worker(pass));
        }
    }

    IEnumerator CompareLoop()
    {
        foreach (var s in styles)
        {
            style = s;
            foreach (var c in contents)
            {
                content = c;
                yield return s.name + "_" + c.name;
            }
        }
    }

    IEnumerator Worker(PassTextures pass)
    {
        while (compare.MoveNext())
        {
            if (style == content)
                continue;

            var path = "Assets/Matrix/" + content.name + "_" + style.name + ".jpg";

            if (System.IO.File.Exists(path))
                continue;

            Debug.Log("Combine " + style.name + " with " + content.name + " gpu " + pass.gpu);

            var c = content.EncodeToJPG(98);
            var s = style.EncodeToJPG(98);

            yield return StartCoroutine(pass.Post(c, s, content.width));

            if (pass.generated)
            {
                var data = pass.output.EncodeToJPG(98);
                System.IO.File.WriteAllBytes(path, data);
            }
            else
            {
                Debug.LogError("FAILED: " + content.name + "_" + style.name);
            }
        }

        Debug.Log("Worker is done: " + pass.gpu);
    }

    [ContextMenu("Create Showcase")]
    public void CreateShowcase()
    {
        var output = new Texture2D((contents.Length + 1) * 512, (styles.Length + 1) * 512);
        //UnityEditor.AssetDatabase.CreateAsset(output, "Assets/showcase.asset");

        for (int x = 0; x < contents.Length + 1; x++)
        {
            for (int y = 0; y < styles.Length + 1; y++)
            {

                Texture2D src;
                if (y == 0)
                {
                    if (x == 0)
                    {
                        continue;
                    }

                    src = contents[x - 1];
                }
                else if (x == 0)
                {
                    src = styles[y - 1];
                }
                else
                {
                    if (styles[y - 1] == contents[x - 1])
                    {
                        continue;
                    }

                    var path = "Assets/Matrix/" + styles[y - 1].name + "_" + contents[x - 1].name + ".jpg";
                    src = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
                if (src == null)
                {
                    continue;
                }

                output.SetPixels(x * 512, output.height - (y + 1) * 512, 512, 512, src.GetPixels());
            }
        }


        var data = output.EncodeToJPG(98);
        System.IO.File.WriteAllBytes("Assets/showcase.jpg", data);
    }
}
