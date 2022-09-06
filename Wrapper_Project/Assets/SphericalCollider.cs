using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SphericalCollider : MonoBehaviour
{
    enum ColliderType {
        SBox = 1, // Square Box
        RBox = 2, // Round Box
        Plane = 3,
    }

    [DllImport(SphericalObject.plugin)] public static extern bool GyroVector_collide(int type1, IntPtr gv1, IntPtr data1, int type2, IntPtr gv2, IntPtr data2);

    BoxCollider _box;

    // Start is called before the first frame update
    void Start()
    {
        _box = GetComponent<BoxCollider>();
    }

    public bool CheckCollision(IntPtr gv1, IntPtr gv2, SphericalCollider other)
    {
        bool res = false;
        if (_box) {
            float[] data1 = new float[6] {
                _box.center.x, _box.center.y, _box.center.z,
                _box.size.x, _box.size.y, _box.size.z,
            };
            GCHandle gch1 = GCHandle.Alloc(data1);
            if (other._box) {
                float[] data2 = new float[6] {
                    other._box.center.x, other._box.center.y, other._box.center.z,
                    other._box.size.x, other._box.size.y, other._box.size.z,
                };
                GCHandle gch2 = GCHandle.Alloc(data2);
                // res = GyroVector_collide(
                //     (int)ColliderType.SBox, gv1, GCHandle.ToIntPtr(gch1),
                //     (int)ColliderType.SBox, gv2, GCHandle.ToIntPtr(gch2));
                SphericalObject.GyroVector_toMatrix(gv1, out Matrix4x4 m1);
                SphericalObject.GyroVector_toMatrix(gv2, out Matrix4x4 m2);

                Vector4[] pts1 = new Vector4[8];
                Vector4[] pts2 = new Vector4[8];
                for (int i = 0; i < 8; ++i) {
                    float[] mult = new float[3] { ((i&0b100)>>2)-.5f, ((i&0b10)>>1)-.5f, (i&0b1)-.5f };
                    pts1[i] = m1 * new Vector4(_box.center.x+_box.size.x*mult[0], _box.center.y+_box.size.y*mult[1], _box.center.z+_box.size.z*mult[2], 1);
                    pts2[i] = m2 * new Vector4(other._box.center.x+other._box.size.x*mult[0], other._box.center.y+other._box.size.y*mult[1], other._box.center.z+other._box.size.z*mult[2], 1);
                }

                Vector3 min1 = new Vector3(Mathf.Min(pts1[0].x,pts1[1].x,pts1[2].x,pts1[3].x,pts1[4].x,pts1[5].x,pts1[6].x,pts1[7].x),
                                           Mathf.Min(pts1[0].y,pts1[1].y,pts1[2].y,pts1[3].y,pts1[4].y,pts1[5].y,pts1[6].y,pts1[7].y),
                                           Mathf.Min(pts1[0].z,pts1[1].z,pts1[2].z,pts1[3].z,pts1[4].z,pts1[5].z,pts1[6].z,pts1[7].z));
                Vector3 min2 = new Vector3(Mathf.Min(pts2[0].x,pts2[1].x,pts2[2].x,pts2[3].x,pts2[4].x,pts2[5].x,pts2[6].x,pts2[7].x),
                                           Mathf.Min(pts2[0].y,pts2[1].y,pts2[2].y,pts2[3].y,pts2[4].y,pts2[5].y,pts2[6].y,pts2[7].y),
                                           Mathf.Min(pts2[0].z,pts2[1].z,pts2[2].z,pts2[3].z,pts2[4].z,pts2[5].z,pts2[6].z,pts2[7].z));
                Vector3 max1 = new Vector3(Mathf.Max(pts1[0].x,pts1[1].x,pts1[2].x,pts1[3].x,pts1[4].x,pts1[5].x,pts1[6].x,pts1[7].x),
                                           Mathf.Max(pts1[0].y,pts1[1].y,pts1[2].y,pts1[3].y,pts1[4].y,pts1[5].y,pts1[6].y,pts1[7].y),
                                           Mathf.Max(pts1[0].z,pts1[1].z,pts1[2].z,pts1[3].z,pts1[4].z,pts1[5].z,pts1[6].z,pts1[7].z));
                Vector3 max2 = new Vector3(Mathf.Max(pts2[0].x,pts2[1].x,pts2[2].x,pts2[3].x,pts2[4].x,pts2[5].x,pts2[6].x,pts2[7].x),
                                           Mathf.Max(pts2[0].y,pts2[1].y,pts2[2].y,pts2[3].y,pts2[4].y,pts2[5].y,pts2[6].y,pts2[7].y),
                                           Mathf.Max(pts2[0].z,pts2[1].z,pts2[2].z,pts2[3].z,pts2[4].z,pts2[5].z,pts2[6].z,pts2[7].z));
                if (min1.x - min2.x < (max1.x-min1.x) + (max2.x-min2.x) &&
                    min1.y - min2.y < (max1.y-min1.y) + (max2.y-min2.y) &&
                    min1.z - min2.z < (max1.z-min1.z) + (max2.z-min2.z))
                    res = true;
                //print((gameObject.name, m * new Vector4(_box.center.x+_box.size.x/2, _box.center.y+_box.size.y/2, _box.center.z+_box.size.z/2, 1)));
                gch2.Free();

            }
            gch1.Free();
        }
        return res;
    }
}
