using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AreaTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void AreaTestsSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CreateAnArea()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(2, 2, 2), new Vector3(1, 1, 1), Vector3.zero, Quaternion.identity);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            Assert.IsNotNull(TestArea);
        }

        [UnityTest]
        public IEnumerator BasicScaleTest()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(2, 2, 2), new Vector3(1, 1, 1), Vector3.zero, Quaternion.identity);
            Debug.Log(TestArea.GetComponent<BoxCollider>().bounds);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(100,100,100);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            Debug.Log(cube.GetComponent<BoxCollider>().bounds);
            cube.transform.position = new Vector3(0,0,0);
            Debug.Log(cube.GetComponent<BoxCollider>().bounds);
            yield return null;

            Debug.Log(cube.GetComponent<BoxCollider>().bounds);
            Assert.AreEqual(new Vector3(0.5f,0.5f,0.5f), cube.transform.lossyScale);
        }
    }
}
