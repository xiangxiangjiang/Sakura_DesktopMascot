using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField]
    Transform _sakura;
    [SerializeField]
    Transform _rotateTarget;
    [SerializeField]
    float _horRotSpeed = 1f;
    [SerializeField]
    float _verRotSpeed = 1f;
    [SerializeField]
    [Range(30, 90)]
    float _elevationUp = 60f;
    [SerializeField]
    [Range(30, 90)]
    float _elevationDown = 60f;

    [SerializeField]
    float _moveSpeed = 1f;
    [SerializeField]
    float _nearestDis = 1.5f;
    [SerializeField]
    float _farthestDis = 8f;
    // [SerializeField]
    // float _highest = 0.7f;
    // [SerializeField]
    // float _lowest = -1f;

    float _lastScroll = 0f;
    float _distance = 0f;
    float _rotate = 0f;
    float _viewAngle = 0f;
    float _yOffset = 0f;
    Vector3 _screen2WorldOffset = Vector3.zero;

    class PosData
    {
        public float sakuraPosY;
        public float sakuraRotY;
        public float camPosX;
        public float camPosY;
        public float camPosZ;
        public float camRotX;
        public float camRotY;
        public float camRotZ;
        public float camRotW;

        public PosData(float sakuraPosY, float sakuraRotY, float camPosX, float camPosY, float camPosZ, float camRotX, float camRotY, float camRotZ, float camRotW)
        {
            this.sakuraPosY = sakuraPosY;
            this.sakuraRotY = sakuraRotY;
            this.camPosX = camPosX;
            this.camPosY = camPosY;
            this.camPosZ = camPosZ;
            this.camRotX = camRotX;
            this.camRotY = camRotY;
            this.camRotZ = camRotZ;
            this.camRotW = camRotW;
        }
    }

    private void Awake()
    {
        DataModel.Instance.Init(_sakura.position, _sakura.rotation, transform.position, transform.rotation);
        var data = DataModel.Instance.Data;
        transform.position = data.cameraPos;
        transform.rotation = data.cameraRot;
        _sakura.position = data.rolePos;
        _sakura.rotation = data.roleRot;
    }

    private void LateUpdate()
    {
        // 旋转中
        if (Input.GetMouseButton(1))
            Rotating();
        // 缩放中
        if (_lastScroll != 0)
            Scaling();

        // 拖动平移开始
        if (Input.GetMouseButtonDown(2))
            _screen2WorldOffset = GetScreen2WorldOffset();
        // 拖动平移中
        if (Input.GetMouseButton(2))
            Translation();

        // 松开，保存
        if (Input.GetMouseButtonUp(1)
            || Input.GetMouseButtonUp(2)
            // 使用上一帧的滚轮计算摄像机的位移并判断此帧为松手
            || (_lastScroll != 0 && Input.mouseScrollDelta.y == 0))
        {
            DataModel.Instance.UpdateTransformData(_sakura, transform);
            DataModel.Instance.SaveData();
        }
        _lastScroll = Input.mouseScrollDelta.y;
    }

    private void Scaling()
    {
        // if (Input.GetMouseButton(1))
        // {
        //     // 高度
        //     _yOffset = -_sakura.position.y;
        //     if (_lastScroll > 0 ?
        //     _yOffset < _highest :
        //     _yOffset > _lowest)
        //     {
        //         _sakura.Translate(Vector3.up * _lastScroll * Time.deltaTime * -5 * _moveSpeed);
        //     }
        // }
        // else
        {
            // 距离
            _distance = Vector3.Distance(transform.position, _rotateTarget.position);
            if (_lastScroll > 0 ? _distance > _nearestDis : _distance < _farthestDis)
            {
                transform.Translate(Vector3.forward * _lastScroll * _moveSpeed * Time.deltaTime * 10, Space.Self);
            }
        }
    }

    private void Rotating()
    {
        // 左右滑八重樱左右转，上下滑摄像机上下绕转
        _sakura.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * _horRotSpeed * -100, 0));
        _rotate = Input.GetAxis("Mouse Y");
        // 垂直正方向与至摄像机向量角度
        _viewAngle = 180 - (Vector3.Dot(_sakura.up, Vector3.Normalize(transform.position - _rotateTarget.position)) + 1) * 90;
        if (_rotate > 0 ?
            _viewAngle < (90 + _elevationDown) :
            _viewAngle > (90 - _elevationUp))
        {
            transform.RotateAround(_rotateTarget.position, transform.right, _rotate * Time.deltaTime * _verRotSpeed * -100);
        }
    }

    // 第一帧计算鼠标点到角色位置之差
    Vector3 GetScreen2WorldOffset()
    {
        var mousePosV2 = TransparentWindow.Instance.GetMousePosW2U();
        var rolePos = Camera.main.WorldToScreenPoint(_sakura.position);
        return rolePos - new Vector3(mousePosV2.x, mousePosV2.y, 0);
    }

    // 再根据鼠标位置加上offset算出角色位置
    void Translation()
    {
        var mousePosV2 = TransparentWindow.Instance.GetMousePosW2U();
        var newPos = new Vector3(mousePosV2.x, mousePosV2.y, 0) + _screen2WorldOffset;
        var h = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
        var w = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
        newPos.x = Mathf.Clamp(newPos.x, 0, w);
        newPos.y = Mathf.Clamp(newPos.y, 0, h - 200);
        _sakura.position = Camera.main.ScreenToWorldPoint(newPos);
    }
}
