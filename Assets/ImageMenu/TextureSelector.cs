using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextureSelector : MonoBehaviour
{

    public Texture2D target;

    public Texture2D[] styles;

    public void SetStyle(string style)
    {
        if (string.IsNullOrEmpty(style))
        {
            target.SetPixels(styles[0].GetPixels());
            target.Apply();
            return;
        }

        for (int i = 1; i < styles.Length; i++)
        {
            if (styles[i].name.Contains(style))
            {
                target.SetPixels(styles[i].GetPixels());
                target.Apply();
                return;
            }
        }
    }
}
