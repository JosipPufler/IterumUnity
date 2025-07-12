using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public GameObject? tooltip;

    public float normalSpeed;
    public float fastSpeed;
    public float movementTime;
    public float rotationSpeed;
    public Vector3 zoomAmount;
    private Quaternion newRotation;
    private Vector3 lastPosition;
    private Vector3 lastCameraLocalPosition;

    private float movementSpeed;
    private Vector3 newPosition;
    private Vector3 startingCameraPosition;
    private Vector3 newZoom;
    private Quaternion targetCameraRotation;
    public bool IsCameraMoving { get; private set; } = true;

    private bool isometric = true;

    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        targetCameraRotation = cameraTransform.localRotation;
    }

    void Update()
    {
        HandleMovementInput();
    }

    void HandleMovementInput() {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else { 
            movementSpeed = normalSpeed;
        }

        if (Input.GetKey(KeyCode.W)) {
            newPosition += transform.forward * movementSpeed;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += transform.right * movementSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            newPosition += transform.right * -movementSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            newPosition += transform.forward * -movementSpeed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            IsCameraMoving = true;
            if (isometric)
            {
                newRotation *= Quaternion.Euler(Vector3.up * rotationSpeed);
            }
            else {
                newRotation *= Quaternion.Euler(Vector3.down * -rotationSpeed);
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            IsCameraMoving = true;
            if (isometric)
            {
                newRotation *= Quaternion.Euler(Vector3.up * -rotationSpeed);
            }
            else
            {
                newRotation *= Quaternion.Euler(Vector3.down * rotationSpeed);
            }
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            if (isometric)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }
            else 
            {
                newZoom += Input.mouseScrollDelta.y * new Vector3(0, -10, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isometric)
            {
                isometric = false;
                startingCameraPosition = cameraTransform.localPosition;
                newZoom = new Vector3(0, startingCameraPosition.y, 0);
                targetCameraRotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isometric)
            {
                isometric = true;
                targetCameraRotation = Quaternion.Euler(45f, 0f, 0f);
                newZoom = startingCameraPosition;
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, targetCameraRotation, Time.deltaTime * movementTime);

        transform.position = RoundVector3(transform.position, 0.01f);
        cameraTransform.localPosition = RoundVector3(cameraTransform.localPosition, 0.01f);

        transform.rotation = RoundQuaternion(transform.rotation, 0.01f);
        cameraTransform.localRotation = RoundQuaternion(cameraTransform.localRotation, 0.01f);

        if ((transform.position - newPosition).sqrMagnitude < 0.000001f)
            transform.position = newPosition;

        if (Quaternion.Angle(transform.rotation, newRotation) < 0.01f)
            transform.rotation = newRotation;

        if ((cameraTransform.localPosition - newZoom).sqrMagnitude < 0.000001f)
            cameraTransform.localPosition = newZoom;

        if (Quaternion.Angle(cameraTransform.localRotation, targetCameraRotation) < 0.01f)
            cameraTransform.localRotation = targetCameraRotation;

        float moveThreshold = 0.00001f;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            IsCameraMoving = true;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            IsCameraMoving = true;
        }
        else
        {
            IsCameraMoving =
            (transform.position - lastPosition).sqrMagnitude > moveThreshold ||
            (cameraTransform.localPosition - lastCameraLocalPosition).sqrMagnitude > moveThreshold;

        }

        lastPosition = transform.position;
        lastCameraLocalPosition = cameraTransform.localPosition;

        /*Vector3 rigPosition = transform.position;
        Vector3 desiredCameraWorldPosition = transform.TransformPoint(newZoom);

        Vector3 direction = desiredCameraWorldPosition - rigPosition;
        float distance = direction.magnitude;

        if (Physics.Raycast(rigPosition, direction.normalized, out RaycastHit hit, distance))
        {
            Vector3 hitLocal = transform.InverseTransformPoint(hit.point - direction.normalized * 0.1f);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, hitLocal, Time.deltaTime * movementTime);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        }*/
    }

    private Vector3 RoundVector3(Vector3 v, float snapValue)
    {
        return new Vector3(
            Mathf.Round(v.x / snapValue) * snapValue,
            Mathf.Round(v.y / snapValue) * snapValue,
            Mathf.Round(v.z / snapValue) * snapValue
        );
    }

    private Quaternion RoundQuaternion(Quaternion q, float snapDegrees)
    {
        Vector3 euler = q.eulerAngles;
        euler.x = Mathf.Round(euler.x / snapDegrees) * snapDegrees;
        euler.y = Mathf.Round(euler.y / snapDegrees) * snapDegrees;
        euler.z = Mathf.Round(euler.z / snapDegrees) * snapDegrees;
        return Quaternion.Euler(euler);
    }

    public void FocusOn(Vector3 worldPosition)
    {
        newPosition = worldPosition;
    }
}
