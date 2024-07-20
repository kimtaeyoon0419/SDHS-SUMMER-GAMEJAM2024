using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;

    private void Awake()
    {
        // 싱글톤 패턴을 사용하여 전역 접근 가능하도록 설정
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
        // CinemachineBasicMultiChannelPerlin 컴포넌트를 가져옴
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        // 흔들림 강도 설정
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
                // 흔들림 종료
                CinemachineBasicMultiChannelPerlin cinemachinePerlin =
                    virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachinePerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
