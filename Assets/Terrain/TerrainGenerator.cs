using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
    public Texture2D map;
    private Mesh mesh;

    private MeshFilter filter;

	// Use this for initialization
	void Start ()
	{
	    filter = GetComponent<MeshFilter>();
        ResetHeigths();
	}

    private void BuildMesh()
    {
        if (mesh != null)
        {
            mesh.Clear();
        }
        else
        {
            mesh = new Mesh();
        }

        var verts = new Vector3[map.width * map.height];
        var uvs = new Vector2[map.width * map.height];
        var tris = new int[(map.width - 1)*(map.height - 1)*6];
        int i = 0;
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                verts[x + y * map.width] = new Vector3(x, 0, y);
                uvs[x + y * map.width] = new Vector2(x / (map.width - 1f), y / (map.height - 1f));
                // norms

                if (x == map.width - 1 || y == map.width -1)
                    continue;

                tris[i++] = x + y * map.width;
                tris[i++] = x + (y + 1) * map.width;
                tris[i++] = x + 1 + (y + 1) * map.width;
                tris[i++] = x + y * map.width;
                tris[i++] = x + 1 + (y + 1) * map.width;
                tris[i++] = x + 1 + y * map.width;
            }
        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        filter.mesh = mesh;
    }

    public void ResetHeigths()
    {
        var colors = map.GetPixels();
        if (mesh == null || mesh.vertices.Length != colors.Length)
            BuildMesh();

        var verts = mesh.vertices;

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                verts[x + y*map.width].y = colors[x+y*map.width].grayscale * 10;
            }
        }

        mesh.vertices = verts;
        
    }
}
