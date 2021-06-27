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
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(2, 2, 2),
                                                                     new Vector3(1, 1, 1),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            Assert.IsNotNull(TestArea);
        }

        [UnityTest]
        public IEnumerator BasicScaleUpTest()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(2, 2, 2),
                                                                     new Vector3(1, 1, 1),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0,0,0);
            yield return null;

            Assert.AreEqual(new Vector3(2, 2, 2), cube.transform.lossyScale);
        }

        [UnityTest]
        public IEnumerator BasicScaleDownTest()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(10, 10, 10),
                                                                     new Vector3(20, 20, 20),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0,0,0);
            yield return null;

            Assert.AreEqual(new Vector3(0.5f, 0.5f, 0.5f), cube.transform.lossyScale);
        }

        [UnityTest]
        public IEnumerator BasicNoScaleTest()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(10, 10, 10),
                                                                     new Vector3(10, 10, 10),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0,0,0);
            yield return null;

            Assert.AreEqual(new Vector3(1, 1, 1), cube.transform.lossyScale);
        }

        [UnityTest]
        public IEnumerator BasicScaleBothTest()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(10, 10, 10),
                                                                     new Vector3(20, 10, 5),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0,0,0);
            yield return null;

            Assert.AreEqual(new Vector3(0.5f, 1, 2), cube.transform.lossyScale);
        }

        [UnityTest]
        public IEnumerator OutsideArea()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(10, 10, 10),
                                                                     new Vector3(20, 20, 20),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(100,100,100);
            yield return null;

            Assert.AreEqual(new Vector3(1,1,1), cube.transform.lossyScale);
        }

        [UnityTest]
        public IEnumerator BasicScaleTransitionTest()
        {
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(20, 20, 20),
                                                                     new Vector3(10, 10, 10),
                                                                     new Vector3(5, 5, 5),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(12.5f,12.5f,12.5f);
            yield return null;

            Assert.AreEqual(new Vector3(1.5f,1.5f,1.5f), cube.transform.lossyScale);
        }

        [UnityTest]
        public IEnumerator MovingPositionArea()
        {
            
            EuclidEngineArea TestArea = EuclidEngineArea.Instantiate(new Vector3(20, 20, 20),
                                                                     new Vector3(10, 10, 10),
                                                                     new Vector3(5, 5, 5),
                                                                     Vector3.zero,
                                                                     Quaternion.identity);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            cube.transform.position = new Vector3(100,100,100);
            yield return null;
            Assert.AreEqual(new Vector3(1,1,1), cube.transform.lossyScale);

            cube.transform.position = new Vector3(12.5f,12.5f,12.5f);
            yield return null;
            Assert.AreEqual(new Vector3(1.5f,1.5f,1.5f), cube.transform.lossyScale);

            cube.transform.position = new Vector3(20,20,20);
            yield return null;
            Assert.AreEqual(new Vector3(2, 2, 2), cube.transform.lossyScale);
        }
    }
}
