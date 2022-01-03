using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SphericalObjCreat : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject go;
    public Vector4 tileHyperOrigin;

    private GameObject goX;
    private GameObject goY;
    private GameObject goZ;
    private GameObject goW;

    void Start()
    {
        goX = Instantiate(go, new Vector3(.5f,0,0), new Quaternion(), transform);
        goX.SetActive(true);
        goX.GetComponent<SphericalObject>().SetHyperOrigin(tileHyperOrigin);
        goY = Instantiate(go, new Vector3(0,.5f,0), new Quaternion(), transform);
        goY.SetActive(true);
        goY.GetComponent<SphericalObject>().SetHyperOrigin(tileHyperOrigin);
        goZ = Instantiate(go, new Vector3(0,0,.5f), new Quaternion(), transform);
        goZ.SetActive(true);
        goZ.GetComponent<SphericalObject>().SetHyperOrigin(tileHyperOrigin);
        goW = Instantiate(go, new Vector3(0,0,0), new Quaternion(), transform);
        goW.SetActive(true);
        goW.GetComponent<SphericalObject>().SetHyperOrigin(tileHyperOrigin);

    #if false
        Mesh mesh = new Mesh();
        goX.GetComponent<MeshFilter>().mesh = mesh;

        mesh.SetVertexBufferParams(24, new[]{ new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4) });
        mesh.SetVertexBufferData(new Vector4[]{
            new Vector4(-0.5f,  0.5f, 0f, 0.5f),// Left,  Top,    Front
            new Vector4(-0.5f, -0.5f, 0f, 0.5f),// Left,  Bottom, Front
            new Vector4( 0.5f,  0.5f, 0f, 0.5f),// Right, Top,    Front
            new Vector4( 0.5f, -0.5f, 0f, 0.5f),// Right, Bottom, Front

            new Vector4(-0.5f,  0.5f, 0f, 1.5f),// Left,  Top,    Back
            new Vector4(-0.5f, -0.5f, 0f, 1.5f),// Left,  Bottom, Back
            new Vector4( 0.5f,  0.5f, 0f, 1.5f),// Right, Top,    Back
            new Vector4( 0.5f, -0.5f, 0f, 1.5f),// Right, Bottom, Back

            new Vector4(-0.5f,  0.5f, 0f, 0.5f),// Left, Top,    Front
            new Vector4(-0.5f, -0.5f, 0f, 0.5f),// Left, Bottom, Front
            new Vector4(-0.5f,  0.5f, 0f, 1.5f),// Left, Top,    Back
            new Vector4(-0.5f, -0.5f, 0f, 1.5f),// Left, Bottom, Back

            new Vector4(0.5f,  0.5f, 0f, 0.5f),// Right, Top,    Front
            new Vector4(0.5f, -0.5f, 0f, 0.5f),// Right, Bottom, Front
            new Vector4(0.5f,  0.5f, 0f, 1.5f),// Right, Top,    Back
            new Vector4(0.5f, -0.5f, 0f, 1.5f),// Right, Bottom, Back

            new Vector4(-0.5f, 0.5f, 0f, 0.5f),// Left,  Top, Front
            new Vector4(-0.5f, 0.5f, 0f, 1.5f),// Left,  Top, Back
            new Vector4( 0.5f, 0.5f, 0f, 0.5f),// Right, Top, Front
            new Vector4( 0.5f, 0.5f, 0f, 1.5f),// Right, Top, Back

            new Vector4(-0.5f, -0.5f, 0f, 0.5f),// Left,  Bottom, Front
            new Vector4(-0.5f, -0.5f, 0f, 1.5f),// Left,  Bottom, Back
            new Vector4( 0.5f, -0.5f, 0f, 0.5f),// Right, Bottom, Front
            new Vector4( 0.5f, -0.5f, 0f, 1.5f),// Right, Bottom, Back
        }, 0, 0, 24);
        mesh.triangles = new int[] {
            0, 2, 1,
            1, 2, 3,

            4, 5, 6,
            6, 5, 7,

            8, 9, 10,
            10, 9, 11,

            12, 14, 13,
            13, 14, 15,

            16, 17, 18,
            18, 17, 19,

            20, 22, 21,
            21, 22, 23,
        };
    #endif
    

    }

    /*void Start()
    {
        goX = Instantiate(go, transform);
        goY = Instantiate(go, transform);
        goZ = Instantiate(go, transform);
        goW = Instantiate(go, transform);

        Mesh mesh = goX.GetComponent<MeshFilter>().mesh;

        var layout = mesh.GetVertexAttributes();
        int size = 0;
        for (int i = 0; i != mesh.vertexAttributeCount; ++i) {
            if (layout[i].attribute == VertexAttribute.Position)
                layout[i].dimension = 4;
            switch (layout[i].format) {
                case VertexAttributeFormat.UNorm8:
                case VertexAttributeFormat.SNorm8:
                case VertexAttributeFormat.UInt8:
                case VertexAttributeFormat.SInt8:
                    size += 8 * layout[i].dimension / 8;
                    break;
                case VertexAttributeFormat.UNorm16:
                case VertexAttributeFormat.SNorm16:
                case VertexAttributeFormat.UInt16:
                case VertexAttributeFormat.SInt16:
                case VertexAttributeFormat.Float16:
                    size += 16 * layout[i].dimension / 8;
                    break;
                case VertexAttributeFormat.UInt32:
                case VertexAttributeFormat.SInt32:
                case VertexAttributeFormat.Float32:
                    size += 32 * layout[i].dimension / 8;
                    break;
                default:
                    throw new System.Exception("Unsupported mesh data type");
            }
        }

        byte[] data = new byte[size * mesh.vertexCount];
        int prev = -1;
        for (int vi = 0; vi != mesh.vertexCount; ++vi) {
            int attribOffset = 0;
            for (int ai = 0; ai != mesh.vertexAttributeCount; ++ai) {
                int formatSize = 0;
                switch (layout[ai].format) {
                    case VertexAttributeFormat.UNorm8:
                    case VertexAttributeFormat.SNorm8:
                    case VertexAttributeFormat.UInt8:
                    case VertexAttributeFormat.SInt8:
                        formatSize = 8 / 8;
                        break;
                    case VertexAttributeFormat.UNorm16:
                    case VertexAttributeFormat.SNorm16:
                    case VertexAttributeFormat.UInt16:
                    case VertexAttributeFormat.SInt16:
                    case VertexAttributeFormat.Float16:
                        formatSize = 16 / 8;
                        break;
                    case VertexAttributeFormat.UInt32:
                    case VertexAttributeFormat.SInt32:
                    case VertexAttributeFormat.Float32:
                        formatSize = 32 / 8;
                        break;
                }
                for (int di = 0; di != layout[ai].dimension; ++di) {
                    for (int fi = 0; fi != formatSize; ++fi) {
                        switch (layout[ai].attribute) {

                        }
                        data[vi*size + attribOffset + di * formatSize + si] = ;
                    }
                }
                attribOffset += layout[ai].dimension * formatSize;
            }
        }

        var vertices = mesh.vertices;
        Vector4[] newVertices = new Vector4[mesh.vertexCount];
        for (int i = 0; i != mesh.vertexCount; ++i)
            newVertices[i] = new Vector4(1,0,0,0) + (Vector4)vertices[i];

        mesh.SetVertexBufferParams(mesh.vertexCount, layout);
        //mesh.SetVertices(newVertices, 0, mesh.vertexCount);
        foreach (var lay in layout)
            print(lay);
        //print((v1, v2));
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
