using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.XR.Interaction.Toolkit;


public class TeleportationManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private DragTransformManipulation transformManipulation;
    [SerializeField] private XROrigin player;
    //[SerializeField] private TeleportationProvider provider;
    private InputAction _thumbstick;
    private bool _isActive;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;

        var activate = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Teleport Mode Activate");
        activate.Enable();
        activate.started += OnTeleportActivate;
        activate.canceled += OnTeleportFinished;


        _thumbstick = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Move");
        _thumbstick.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive)
            return;

        if (_thumbstick.triggered)
            return;

        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            rayInteractor.enabled = false;
            _isActive = false;
            return;
        }


        var heightAdjustment = player.Origin.transform.up * player.CameraInOriginSpaceHeight;
        transformManipulation.objPosition += (hit.point + heightAdjustment) - player.Camera.transform.position;
        transformManipulation.ctrlPosition += (hit.point + heightAdjustment) - player.Camera.transform.position;
        player.MoveCameraToWorldLocation(hit.point + heightAdjustment);

        rayInteractor.enabled = false;
        _isActive = false;
        

    }

    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = true;
    }

    private void OnTeleportFinished(InputAction.CallbackContext context)
    {
        _isActive = true;
    }
}
