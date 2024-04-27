using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

public class DragTransformManipulation : MonoBehaviour
{
    [SerializeField] private GameObject rightController;
    [SerializeField] private GameObject leftController;
    [SerializeField] private RaycastSelection raycastSelection;
	[SerializeField] private XRRayInteractor rayInteractor;
	[SerializeField] private InputActionAsset actionAsset;
	[SerializeField] private float scaleFactor;
    private float translateFactor;
    [SerializeField] private float translateFactorRaycast;
    [SerializeField] private float translateFactorForceGrab;

    private Vector3 scaleValue;
    public bool manipulationMode;
	private InputAction _activate_button;
    private InputAction scaleUp;
    private InputAction scaleDown;
    public Color emissionColorValue;
	public float intensity;
	public Vector3 objPosition, ctrlPosition; //position of selected object/controller
	private Quaternion objRotation, ctrlRotation; //rotation of selected object/controller
	private bool isScalingUp;
	private bool isScalingDown;

    // Start is called before the first frame update
    void Start()
	{
        translateFactor = translateFactorRaycast;
		isScalingUp = false;
		isScalingDown = false;
		manipulationMode = false;
		scaleValue = Vector3.one;

		_activate_button = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Manipulate");
		_activate_button.Enable();
		_activate_button.performed += OnGripPressed;

        scaleUp = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Scale Up");
        scaleUp.Enable();
        scaleUp.started += OnScaleUpPressed;
        scaleUp.canceled += OnScaleUpFinished;

        scaleDown = actionAsset.FindActionMap("XRI LeftHand Interaction").FindAction("Scale Down");
        scaleDown.Enable();
        scaleDown.started += OnScaleDownPressed;
        scaleDown.canceled += OnScaleDownFinished;

    }

	// Update is called once per frame
	void Update()
	{
		if (manipulationMode)
		{
			raycastSelection.lastHit.collider.gameObject.transform.position = objPosition + translateFactor*(rightController.transform.position - ctrlPosition);
			raycastSelection.lastHit.collider.gameObject.transform.rotation = (rightController.transform.rotation * Quaternion.Inverse(ctrlRotation)) * objRotation;

			if (isScalingUp)
			{
				raycastSelection.lastHit.collider.gameObject.transform.localScale += scaleFactor * scaleValue;
			}

			if (isScalingDown)
			{
				if (raycastSelection.lastHit.collider.gameObject.transform.localScale.x > 20*scaleFactor*scaleValue.x)
				{
                    raycastSelection.lastHit.collider.gameObject.transform.localScale -= scaleFactor * scaleValue;
                }
            }
		}
		else
		{
			isScalingDown = false;
			isScalingUp = false;
		}
	}

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log("sorry im dumb");
        if (!rayInteractor.enabled) // this means we are using grab transform manipulation
        {
            Debug.Log(other.gameObject.name);
            if (!other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody component)) return;
            Debug.Log("I hit an object!!!");
            other.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            other.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", raycastSelection.emissionColorValueRaycast * Mathf.LinearToGammaSpace(raycastSelection.intensity));
        }
    }*/

    void OnGripPressed(InputAction.CallbackContext context)
	{
		if(raycastSelection.objSelected) // this means we are using drag transform manipulation (aka raycaster manipulation)
        {
            if (manipulationMode) // exit manipulation mode
            {
                manipulationMode = false;
                raycastSelection.lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", raycastSelection.emissionColorValue * Mathf.LinearToGammaSpace(raycastSelection.intensity));
                raycastSelection.lastHit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                rayInteractor.enabled = true;
                return;
            }
            else
            {
                manipulationMode = true;
                raycastSelection.lastHit.collider.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissionColorValue * Mathf.LinearToGammaSpace(intensity));
                raycastSelection.lastHit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                rayInteractor.enabled = false;
                objPosition = raycastSelection.lastHit.collider.gameObject.transform.position;
                objRotation = raycastSelection.lastHit.collider.gameObject.transform.rotation;
                ctrlPosition = rightController.transform.position;
                ctrlRotation = rightController.transform.rotation;
            }
            // if the object is close to you snap to hand
            if(raycastSelection.forceGrab)
            {
                raycastSelection.lastHit.collider.gameObject.transform.position = ctrlPosition;
                objPosition = ctrlPosition;
                translateFactor = translateFactorForceGrab;
            }
            else
            {
                translateFactor = translateFactorRaycast;
            }
        }
    }
    void OnScaleUpPressed(InputAction.CallbackContext context)
    {
		if (!isScalingDown)
		{
			isScalingUp = true;
		}
    }

    void OnScaleDownPressed(InputAction.CallbackContext context)
    {
		if (!isScalingUp)
		{
			isScalingDown = true;
		}
    }

    void OnScaleUpFinished(InputAction.CallbackContext context)
    {
		isScalingUp = false;
    }

    void OnScaleDownFinished(InputAction.CallbackContext context)
    {
		isScalingDown = false;
    }
}
