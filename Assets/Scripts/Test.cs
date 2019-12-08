using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Test : MonoBehaviour
{
    class TestJson
    {
        public float testA;
        public string testB;
        public Vector3 TestPos;
        public Quaternion TestRot;
    }

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


    // void TestMyJson()
    // {
    //     var obj = new TestJson() { testA = 1.0f, testB = "str", TestPos = Vector3.up, TestRot = Quaternion.identity };
    //     var str = JsonUtility.ToJson(obj);
    //     Debug.Log(str);
    //     var obj2 = JsonUtility.FromJson<TestJson>(str);
    //     Debug.Log(obj2.testA);
    //     Debug.Log(obj2.TestPos);
    //     Debug.Log(obj2.TestRot);
    // }
}
