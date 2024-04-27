using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastSelection : MonoBehaviour
{
    [SerializeField] private GameObject rightController;
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private DragTransformManipulation transformManipulation;
    [SerializeField] private float forceGrabDistance;
    private InputAction _button;
    private bool _isActive;
    public Color emissionColorValue;
    [SerializeField] private Color emissionColorValueRaycast;
    [SerializeField] private Color emissionColorValueForceGrab;
    public float intensity;
    public RaycastHit lastHit;
    public bool objSelected;
    public bool forceGrab;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;
        _isActive = false;
        objSelected = false;

        _button = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Select");
        _button.Enable();
        _button.performed += OnSelectActivate;
    }

    // Update is called once per frame
    void Update()
    {

        if (transformManipulation.manipulationMode) return; //raycast selection is blocked if manipulation mode is on

        if (!_isActive)
        {
            objSelected = false;
            if (lastHit.collider)
            {
                lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(0, 0, 0, 0));
            }
            return;
        }
        
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            objSelected = false;
            if (lastHit.collider)
            {
                lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(0, 0, 0, 0));
            }
            return;
        }

        if (lastHit.collider) {
            if (!hit.collider.gameObject.Equals(lastHit.collider.gameObject))
            {
                lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(0, 0, 0, 0));
            }
        }

        if (!hit.collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody component)) return;
        hit.collider.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        if (Vector3.Distance(hit.collider.gameObject.transform.position, rightController.transform.position) < forceGrabDistance) // force grab
        {
            emissionColorValue = emissionColorValueForceGrab;
            forceGrab = true;
        }
        else //regular raycast
        {
            emissionColorValue = emissionColorValueRaycast;
            forceGrab = false;
        }
        hit.collider.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissionColorValue * Mathf.LinearToGammaSpace(intensity));
        lastHit = hit;
        objSelected = true;
    }

    private void OnSelectActivate(InputAction.CallbackContext context)
    {
        if (transformManipulation.manipulationMode) return; 
        if (_isActive)
        {
            rayInteractor.enabled = false;
            _isActive = false;
        }
        else
        {
            rayInteractor.enabled = true;
            _isActive = true;

        }
    }
}

