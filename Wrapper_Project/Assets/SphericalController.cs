using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SphericalController : SphericalObject
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        transform.position = new Vector3(0,1f,0);
        delete_GyroVector(_gv);

        _gv = c.gyroVector;
        // SphericalObject.GyroVector_alignUp(_gv);
    }

    void OnDestroy()
    {
        _gv = IntPtr.Zero;
    }

    public Vector3 velocity = Vector3.zero;
    public bool isGrounded = false;
    public int jumped = 0;
    // Update is called once per frame
    void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");
        float dcx = Input.GetAxis("Mouse X");
        float dcy = Input.GetAxis("Mouse Y");

        velocity = Vector3.Lerp(velocity, new Vector3(dx, velocity.y, dy), 0.7f);
        if (jumped == 0 && jump > 0) {
            jumped = 150;
            velocity.y = 1;
        } else if (jumped == 1) {
            if (velocity.y > 0) {
                velocity.y = -1;
                jumped = 150;
            } else {
                velocity.y = 0;
                jumped = 0;
            }
        } else if (jumped > 0)
            jumped--;
        // velocity.y += (!isGrounded&&velocity.y<=1e-6? (velocity.y-1) * Time.deltaTime*5 : velocity.y*velocity.y-velocity.y);
        // if (isGrounded && jump > 0) {
        //     velocity.y = Mathf.Min(jump,1-1e-6f);
        //     isGrounded = false;
        // }
        //Quaternion rot = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward + (/*transform.up*dcy*/ - transform.right*dcx) * Time.deltaTime), 360);
        //SphericalObject.GyroVector_rotate(_gv, in rot);
        transform.LookAt(transform.position + transform.forward + (Vector3.up*dcy + transform.right*dcx) * Time.deltaTime*5);

        if (velocity.sqrMagnitude > 0) {
            Vector3 tmp = velocity.normalized / 2.0f / worldRadius;
            tmp = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0) * tmp * Time.deltaTime;
            tmp *= -1;
            // print((tmp.x,tmp.y,tmp.z));
            //v *= Mathf.Tan(v.magnitude) / v.magnitude;
            SphericalObject.GyroVector_move(_gv, in tmp);
            SphericalObject.GyroVector_alignUp(_gv);
        }
    }

    override public void OnTriggerEnter(Collider obj)
    {
        // velocity.y = 0;
        isGrounded = true;
        print(("Player Collision", name, obj.name));
    }
}
