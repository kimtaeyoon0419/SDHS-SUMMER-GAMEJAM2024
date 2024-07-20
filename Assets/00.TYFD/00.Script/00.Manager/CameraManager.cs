using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera lastvirtualCamera;
    private float shakeTimer;

    public GameObject[] cams;

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

        //virtualCamera = GetComponent<CinemachineVirtualCamera>();
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

    public void ChageCam(int index)
    {
        cams[index].gameObject.SetActive(true);
    }
}
