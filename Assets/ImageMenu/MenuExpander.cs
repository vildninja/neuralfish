using UnityEngine;
using System.Collections;

public class MenuExpander : MonoBehaviour
{

    public GameObject expanded;
    public GameObject collapsed;

    void Start()
    {
        Expand(false);
    }

    public void Expand(bool expand)
    {
        expanded.SetActive(expand);
        collapsed.SetActive(!expand);
    }
}
