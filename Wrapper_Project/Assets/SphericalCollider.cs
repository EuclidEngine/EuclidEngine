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
    SphereCollider _sphere;

    // Start is called before the first frame update
    void Start()
    {
        _box = GetComponent<BoxCollider>();
        _sphere = GetComponent<SphereCollider>();
    }

    public bool CheckCollision(IntPtr gv1, IntPtr gv2, SphericalCollider other)
    {
        bool res = false;
        float[] data1, data2;
        if (_box) {
            data1 = new float[6] {
                _box.center.x, _box.center.y, _box.center.z,
                _box.size.x, _box.size.y, _box.size.z,
            };
            //GCHandle gch1 = GCHandle.Alloc(data1);
        } else if (_sphere) {
            data1 = new float[6] {
                _sphere.center.x, _sphere.center.y, _sphere.center.z,
                _sphere.radius * 2, _sphere.radius * 2, _sphere.radius * 2,
            };
        } else return false;
            if (other._box) {
                data2 = new float[6] {
                    other._box.center.x, other._box.center.y, other._box.center.z,
                    other._box.size.x, other._box.size.y, other._box.size.z,
                };
            } else if (other._sphere) {
                data2 = new float[6] {
                    other._sphere.center.x, other._sphere.center.y, other._sphere.center.z,
                    other._sphere.radius * 2, other._sphere.radius * 2, other._sphere.radius * 2,
                };
            } else return false;
                //GCHandle gch2 = GCHandle.Alloc(data2);
                // res = GyroVector_collide(
                //     (int)ColliderType.SBox, gv1, GCHandle.ToIntPtr(gch1),
                //     (int)ColliderType.SBox, gv2, GCHandle.ToIntPtr(gch2));
                SphericalObject.GyroVector_toMatrix(gv1, out Matrix4x4 m1);
                SphericalObject.GyroVector_toMatrix(gv2, out Matrix4x4 m2);

                Vector4[] pts1 = new Vector4[8];
                Vector4[] pts2 = new Vector4[8];
                for (int i = 0; i < 8; ++i) {
                    float[] mult = new float[3] { ((i&0b100)>>2)-.5f, ((i&0b10)>>1)-.5f, (i&0b1)-.5f };
                    pts1[i] = m1 * new Vector4(data1[0]+data1[3]*mult[0], data1[1]+data1[4]*mult[1], data1[2]+data1[5]*mult[2], 1);
                    pts2[i] = m2 * new Vector4(data2[0]+data2[3]*mult[0], data2[1]+data2[4]*mult[1], data2[2]+data2[5]*mult[2], 1);
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
                // print((name, min1, max1));
                // print((other.name, min2, max2));
                // I  1 1 I    1 I 1 I
                if (max1.x >= min2.x && max2.x >= min1.x &&
                    max1.y >= min2.y && max2.y >= min1.y &&
                    max1.z >= min2.z && max2.z >= min1.z)
                    res = true;
                
                // if (min1.x - min2.x < (max1.x-min1.x) + (max2.x-min2.x) &&
                //     min1.y - min2.y < (max1.y-min1.y) + (max2.y-min2.y) &&
                //     min1.z - min2.z < (max1.z-min1.z) + (max2.z-min2.z))
                //     res = true;
                //print((gameObject.name, m * new Vector4(_box.center.x+_box.size.x/2, _box.center.y+_box.size.y/2, _box.center.z+_box.size.z/2, 1)));
                //gch2.Free();

            //}
            //gch1.Free();
        //}
        return res;
    }
}
