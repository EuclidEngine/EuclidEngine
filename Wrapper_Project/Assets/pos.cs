using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        const float _Radius = 2.83f;
        Vector4 _Origin = new Vector4(0,0,0,0);
        float len = 2 * _Radius / Mathf.Sqrt(2);

        if (transform.position.x < -1.5f * len || transform.position.x > 1.5f * len || transform.position.y < -1.5f * len || transform.position.y > 1.5f * len)
            return;
        Vector4 facePos = transform.position;
        facePos.x = (transform.position.x + 1.5f * len) % len - len/2;
        facePos.z = (transform.position.z + 1.5f * len) % len - len/2;

        Vector4 vec = _Origin + new Vector4(0,_Radius,0,0) - facePos;
        vec.Normalize();
        vec *= (_Origin.y + _Radius - facePos.y);
        
        //if (pos.x < -0.5 * len)
        //    return float4(center.x - vec.y, center.y - vec.x, center.z - vec.z, center.w - vec.w);
        
        if (transform.position.z < -0.5 * len)
            print(_Origin + new Vector4(0,_Radius,0,0) - new Vector4(vec.x, -vec.z, vec.y, vec.w));
        else
            print(_Origin + new Vector4(0,_Radius,0,0) - vec);


        /*float radius = 5;

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        float len = Mathf.Sqrt(x*x + z*z);
        float angle = Mathf.Atan2(z, x);

        float longitude = angle;
        float lattitude = Mathf.PI / 2 - 2*Mathf.Atan(len / 2 / radius);

        float w = Mathf.Sin(lattitude) * radius;
        float latRad = Mathf.Cos(lattitude) * radius;
        float x2 = Mathf.Cos(longitude) * latRad;
        float z2 = Mathf.Sin(longitude) * latRad;

        print((x, z, len, angle * Mathf.Rad2Deg, longitude * Mathf.Rad2Deg, lattitude * Mathf.Rad2Deg, x2, z2, w));*/
    }

    bool sens = true;
    void Update()
    {
        const float _Radius = 2.83f;
        Vector4 _Origin = new Vector4(0,0,0,0);

        //if (transform.position.x > 0.5*_Radius*Mathf.Sqrt(2))
        //    sens = false;
        //else if (transform.position.x < -1.5*_Radius*Mathf.Sqrt(2))
        //    sens = true;
        //transform.position = new Vector3(transform.position.x + 0.02f * (sens ? 1: -1), transform.position.y, transform.position.z);
        if (transform.position.z > 0.5*_Radius*Mathf.Sqrt(2))
            sens = false;
        else if (transform.position.z < -0.5*_Radius*Mathf.Sqrt(2))
            sens = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.02f * (sens ? 1: -1));

        Vector4 pos = transform.position;



        float len = _Radius * Mathf.Sqrt(2);
        pos.y /= 100.0f;
        Vector4 facePos = 2 * pos / len;
        if (facePos.x < -3 || facePos.x > 3 || facePos.z < -3 || facePos.z > 3)
            return;
        else if (facePos.x < -1) {
            facePos.y = -facePos.x - 1;
            facePos.x = -1 + pos.y / len;
        } else if (facePos.x > 1) {
            facePos.y = facePos.x - 1;
            facePos.x = 1 - pos.y / len;
        } else if (facePos.z < -1) {
            facePos.y = -facePos.z - 1;
            facePos.z = -1 + pos.y / len;
        } else if (facePos.z > 1) {
            facePos.y = facePos.z - 1;
            facePos.z = 1 - pos.y / len;
        } else
            facePos.y /= 2;

        Vector4 vec = (facePos - new Vector4(0,1,0,0)).normalized;
        print((vec, vec*_Radius, vec * _Radius + new Vector4(0,_Radius,0,0)));



        /*Vector4 center = _Origin;// + new Vector4(0,_Radius,0,0);
        float len = _Radius * Mathf.Sqrt(2);
        float len2 = Mathf.Sqrt(2) / 2;
        //float len2 = (_Radius + _Origin.y - pos.y) * Mathf.Sqrt(2);
        // x, y, z -> lattitude, longitude, profondeur
        if (pos.x < -1.5 * len || pos.x > 1.5 * len || pos.z < -1.5 * len || pos.z > 1.5 * len)
            return;
        //pos.y = (pos.y - _Origin.y) * _Radius / /*_Height* /len*2 + _Origin.y;
        //if (pos.y > len/2) pos.y = len/2;
        
        // pos.x = [-1,1] -> pos.x = [-sqrt(2)/2, sqrt(2)/2]
        
        Vector4 facePos = pos / _Radius;
        if (facePos.x < -len2) {
            facePos.x = pos.y / len - 1;
            facePos.y = -2 * (pos.x+len) / len;
        }
        //facePos.x *= len2 / len;
        //facePos.y *= len2 / len;
        //facePos.z *= len2 / len;

        Vector4 vec = (facePos - center).normalized;
        //vec *= (center.y - facePos.y);
        print((facePos, vec, center + vec*(center.y-facePos.y)));*/

        /*
        const float _Radius = 2.83f;
        Vector4 _Origin = new Vector4(0,0,0,0);
        float len = 2 * _Radius;

        if (transform.position.x < -1.5f * len || transform.position.x > 1.5f * len || transform.position.y < -1.5f * len || transform.position.y > 1.5f * len)
            return;
        Vector4 facePos = transform.position;
        facePos.x = (transform.position.x + 1.5f * len) % len - len/2;
        facePos.z = (transform.position.z + 1.5f * len) % len - len/2;

        Vector4 vec = _Origin + new Vector4(0,_Radius,0,0) - facePos;
        vec.Normalize();
        vec *= (_Origin.y + _Radius - facePos.y);
        
        //if (pos.x < -0.5 * len)
        //    return float4(center.x - vec.y, center.y - vec.x, center.z - vec.z, center.w - vec.w);
        
        if (transform.position.z < -0.5 * len)
            print(_Origin + new Vector4(0,_Radius,0,0) - new Vector4(vec.x, -vec.z, vec.y, vec.w));*/
    }
}
