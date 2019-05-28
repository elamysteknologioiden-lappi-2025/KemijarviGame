using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tttt : MonoBehaviour
{


    public Vector3[] newVertices = new Vector3[4];

    public Vector2[] newUV = new Vector2[4];

    public int[] newTriangles = new int[6];




    // Start is called before the first frame update
    void Start()
    {

        //GetComponent<MeshFilter>().mesh;
        // mesh.vertices = newVertices;


        ;


        Vector2[] uvs = new Vector2[GetComponent<MeshFilter>().mesh.vertices.Length];

        for (int i = 0; i < uvs.Length; i++) {
            uvs[i] = new Vector2(GetComponent<MeshFilter>().mesh.vertices[i].x, GetComponent<MeshFilter>().mesh.vertices[i].z);
        }
        GetComponent<MeshFilter>().mesh.uv = uvs;
        /*

       // mesh.uv = newUV;
        mesh.triangles = newTriangles;*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
