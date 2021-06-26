using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Threading;

public class CSWrapperTests
{
    [Test]
    public void Test1()
    {
        var points = 5;
        var expectedPoints = 10 - 5;

        Assert.That(points, Is.EqualTo(expectedPoints));
    }

    public void CreateAnArea()
    {
        //2,2,2,1,1,1
        try
        {
            EuclidEngineArea TestArea = EuclideEngineArea.Instanciate(new Vector3(2, 2, 2), new Vector3(1, 1, 1), Vector3.zero, Quaternion.identity);
            Assert.That(true);
        }
        catch
        {
            Assert.That(false);
        }

    }

    public void BasicScaleTest()
    {
        try
        {
            EuclidEngineArea TestArea = EuclideEngineArea.Instanciate(new Vector3(2, 2, 2), new Vector3(1, 1, 1), Vector3.zero, Quaternion.identity);
            GameObject TestObject = new GameObject();
            TestObject.transform.position = Vector3.zero;

            while(!Other.test2)
                Thread.Sleep(100);
            Other.test2 = false;

            Assert.That(TestObject.transform.localScale, Is.EqualTo(new Vector3(0.5f,0.5f,0.5f)));
        }
        catch
        {
            Assert.That(false);
        }

    }
}
