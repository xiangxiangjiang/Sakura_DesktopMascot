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

    float _lastScroll = 0f;
    float _distance = 0f;
    float _rotate = 0f;
    float _viewAngle = 0f;
    Vector3 _screen2WorldOffset = Vector3.zero;



    private void Awake()
    {
        Application.targetFrameRate = 120;
        DataModel.Instance.Init(_sakura.position, _sakura.rotation, transform.position, transform.rotation);
        var data = DataModel.Instance.Data;
        // 累计误差达到一定值则重置位置
        if (data.cameraPos.magnitude + data.rolePos.magnitude > 10000f)
            return;
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
        // 距离
        _distance = Vector3.Distance(transform.position, _rotateTarget.position);
        if (_lastScroll > 0 ? _distance > _nearestDis : _distance < _farthestDis)
        {
            // 向朝向角色的方向移动
            var dir = Vector3.Normalize(_rotateTarget.position - transform.position);
            transform.Translate(dir * _lastScroll * _moveSpeed * Time.deltaTime * _distance * 0.75f, Space.World);
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
        // 限制上下位置
        var rotateTargetPos = Camera.main.WorldToScreenPoint(_rotateTarget.position);
        var rolePos = Camera.main.WorldToScreenPoint(_sakura.position);
        var dis = (new Vector2(rotateTargetPos.x, rotateTargetPos.y) - new Vector2(rolePos.x, rolePos.y)).magnitude;
        newPos.x = Mathf.Clamp(newPos.x, 0, System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width);
        newPos.y = Mathf.Clamp(newPos.y, 0 - dis, System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - dis);
        _sakura.position = Camera.main.ScreenToWorldPoint(newPos);

    }
}
