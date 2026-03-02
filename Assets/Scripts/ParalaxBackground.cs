
using UnityEngine;

public class ParalaxBackground : MonoBehaviour
{
   [SerializeField] private Vector2 parallaxEffectMultiplayer;
   [SerializeField] private float rotationSpeed = 0.5f;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()  //to cam already moved
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x *parallaxEffectMultiplayer.x, deltaMovement.y *parallaxEffectMultiplayer.y);
        lastCameraPosition = cameraTransform.position;

        transform.Rotate(0,0,rotationSpeed*Time.deltaTime);
    }
}
