using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform Target;

    private Vector3 _distance;

    void Start()
    {
        _distance = Target.position - transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ī�޶��� ��ġ�� Ÿ�����κ��� ���� �Ÿ� �������� �Ѵ�
        transform.position = Target.position - _distance;
    }
}
