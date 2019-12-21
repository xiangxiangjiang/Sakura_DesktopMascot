using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{
    class TestJson
    {
        public float testA;
        public long testB;
    }

    public GameObject[] go;
    public float test;

    void Start()
    {
        // TestMyJson();
        // EXETest();
    }

    void EXETest()
    {
        System.Diagnostics.Process exep = new System.Diagnostics.Process();
        exep.StartInfo.FileName = "calc.exe";
        exep.StartInfo.Arguments = "";
        exep.Start();
    }


    void TestMyJson()
    {
        DateTime t1 = DateTime.Now;
        var str = JsonUtility.ToJson(t1);
        Debug.Log(t1.ToFileTime());
        Debug.Log(DateTime.FromFileTime(t1.ToFileTime()));
        Debug.Log(JsonUtility.ToJson(new TestJson() { testA = 6.66f, testB = 9999999 }));
        var obj = JsonUtility.FromJson<TestJson>("{\"testA\":6.659999847412109}");
        Debug.Log(obj.testA);
        Debug.Log(obj.testB);
    }
}
