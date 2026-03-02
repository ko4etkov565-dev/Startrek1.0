using System;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{

    public const float NORMAL_ORTHOGRAPHIC_SIZE = 25f;
    public static CinemachineCameraZoom2D Instance {get; private set;}

    [SerializeField] private CinemachineCamera cinemachineCamera;
    private float targetOrthographicSize = 15f;

    private void Awake()    {
        Instance = this;
        targetOrthographicSize = NORMAL_ORTHOGRAPHIC_SIZE;
    }

    private void Update() {
        float zoomSpeed = .5f;
        cinemachineCamera.Lens.OrthographicSize = 
            Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
    }

    public void SetTargetOrthographicSize(float targetOrthographicSize) {
        this.targetOrthographicSize = targetOrthographicSize;        
    }

    public void SetNormalOrthographicSize() {
        SetTargetOrthographicSize(NORMAL_ORTHOGRAPHIC_SIZE);
    }
}
