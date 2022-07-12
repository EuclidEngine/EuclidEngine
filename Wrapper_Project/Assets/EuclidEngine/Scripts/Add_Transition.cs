using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EuclidEngine
{
    [RequireComponent(typeof(Area))]
    public class Add_Transition : MonoBehaviour
    {
        // Start is called before the first frame update

        public Vector3 internalSize = new Vector3(1.5f, 0.5f, 1.5f);

        Area Area;
        void Start()
        {
            Area = gameObject.GetComponent<Area>();
            Area.SetSize(Area.size, internalSize, Area.transitionSize);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
};