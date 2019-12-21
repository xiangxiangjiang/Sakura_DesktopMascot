using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAutoAnimation : StateMachineBehaviour
{
    public AudioSource audioPrefeb;
    FaceCtrl face;
    AudioSource audio;
    float[] spectrumData = new float[128];

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //进入状态加载音频
        audio = Instantiate(audioPrefeb, GameObject.Find("/Audio").transform);
        face = GameObject.Find("/Sakura").GetComponent<FaceCtrl>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //获取音频数据，精度128
        audio.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        float max = 0;
        //截取前8个数据中最高的，0.1》1  0.2》2  0.3》3  0.4》4
        for (int i = 0; i < 8; i++)
        {
            if (max < spectrumData[i])
            {
                max = spectrumData[i];
            }
        }
        if (max < 0.01f)
            face.speek = 0;
        else if (max < 0.02f)
            face.speek = 1;

        else if (max < 0.03f)
            face.speek = 2;

        else if (max < 0.04f)
            face.speek = 3;
        else
            face.speek = 4;

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //重置TouchID
        animator.SetInteger("TouchID", 0);
        //退出销毁当前音频
        Destroy(audio.gameObject);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
