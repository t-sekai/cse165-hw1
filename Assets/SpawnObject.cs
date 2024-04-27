using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnObject : MonoBehaviour
{
    [SerializeField] private GameObject rightController;
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private GameObject object1;
    [SerializeField] private GameObject object2;
    private InputAction spawn1;
    private InputAction spawn2;
    private InputAction despawn;
    private GameObject curObject;
    private bool isSpawning;

    // Start is called before the first frame update
    void Start()
    {
        curObject = null;

        spawn1 = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Spawn1");
        spawn1.Enable();
        spawn1.started += OnSpawnActivate1;
        spawn1.canceled += OnSpawnRelease;

        spawn2 = actionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Spawn2");
        spawn2.Enable();
        spawn2.started += OnSpawnActivate2;
        spawn2.canceled += OnSpawnRelease;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawning)
        {
            curObject.transform.position = rightController.transform.position;
            curObject.transform.rotation = rightController.transform.rotation;
        }
    }

    private void OnSpawnRelease(InputAction.CallbackContext context)
    {
        curObject.GetComponent<Rigidbody>().isKinematic = false;
        curObject = null;
        isSpawning = false;
    }

    private void OnSpawnActivate1(InputAction.CallbackContext context)
    {
        spawnItem(object1);
        isSpawning = true;
    }

    private void OnSpawnActivate2(InputAction.CallbackContext context)
    {
        spawnItem(object2);
        isSpawning = true;
    }

    private GameObject spawnItem(GameObject toSpawn) {
        curObject = Instantiate(toSpawn, rightController.transform.position, rightController.transform.rotation);
        curObject.AddComponent<MeshCollider>();
        curObject.GetComponent<MeshCollider>().convex = true;
        curObject.AddComponent<Rigidbody>();
        curObject.GetComponent<Rigidbody>().isKinematic = true; 
        return curObject;
    }
}
