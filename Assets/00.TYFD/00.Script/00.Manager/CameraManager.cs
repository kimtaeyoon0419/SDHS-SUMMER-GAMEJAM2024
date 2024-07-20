using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;

    private void Awake()
    {
        // �̱��� ������ ����Ͽ� ���� ���� �����ϵ��� ����
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void CameraShake(float intensity, float time)
    {
        // CinemachineBasicMultiChannelPerlin ������Ʈ�� ������
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        // ��鸲 ���� ����
        cinemachinePerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                // ��鸲 ����
                CinemachineBasicMultiChannelPerlin cinemachinePerlin =
                    virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachinePerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
