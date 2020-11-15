using UnityEngine;

public class CreatorCamera : MonoBehaviour
{
    class CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

            x += rotatedTranslation.x;
            y += rotatedTranslation.y;
            z += rotatedTranslation.z;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }
    }

    CameraState m_TargetCameraState = new CameraState();
    CameraState m_InterpolatingCameraState = new CameraState();

    [Header("Movement Settings")]
    [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
    public float boost = 3.5f;

    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

    void Start()
    {
        Time.timeScale = 1f;

        try
        {
            transform.position = MeshCreatorLoad.meshcreator.position + new Vector3(0, 2f, -5f);
        }
        catch { }

        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);
    }

    Vector3 GetinputTranslationDirection()
    {
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            direction += Vector3.up;
        }
        return direction;
    }

    public void OnGUI()
    {
        if (locker)
        {
           RightMenu();
        }
        if (actri)
        {
            TriRightMenu();
        }
    }

    void RightMenu()
    {
        if (GUI.Button(new Rect(clickPos.x, Screen.height - clickPos.y + 5, 120, 20), "Create New Node"))
        {
            Debug.Log("Create");

            MeshObject mo = modifyPoint.collider.gameObject.GetComponent<MeshObject>();
            mo.AddPointWithTriangle(modifyPoint.collider.transform.InverseTransformPoint(modifyPoint.point),modifyPoint.triangleIndex);

            locker = false;
        }

        if (GUI.Button(new Rect(clickPos.x, Screen.height - clickPos.y + 25, 120, 20), "Delete Node"))
        {
            MeshObject mo = modifyPoint.collider.gameObject.GetComponent<MeshObject>();

            Vector3 localHit = mo.transform.InverseTransformPoint(modifyPoint.point);
            int ti = modifyPoint.triangleIndex;
            Vector3[] nodes = new Vector3[3] { mo.vertices[mo.trianglesco[ti * 3]], mo.vertices[mo.trianglesco[ti * 3 + 1]], mo.vertices[mo.trianglesco[ti * 3 + 2]] };

            float a = (nodes[0] - localHit).magnitude;
            float b = (nodes[1] - localHit).magnitude;
            float c = (nodes[2] - localHit).magnitude;

            int smallest = a < b ? (a < c ? 0 : 2) : (b < c ? 1 : 2);

            mo.DeleteNode(ti*3 + smallest);
            mo.BuildMesh();

            locker = false;
        }

    }

    void TriRightMenu()
    {
        if (GUI.Button(new Rect(clickPos.x, Screen.height - clickPos.y + 5, 120, 20), "Create Triangle"))
        {
            Debug.Log("Create");

            MeshObject mo = modifyPoint.collider.gameObject.GetComponent<MeshObject>();
            mo.CreateTriangle(new int[3] { mo.trianglesco[verf], mo.trianglesco[vers], mo.trianglesco[vert] });
            mo.BuildMesh();

            actri = false;
        }

    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            locker = false;
        }
    }

    public GameObject visual;

    public bool addMode = false;

    public void SetAddMode(bool s)
    {
        addMode = s;
    }

    public RaycastHit modifyPoint;
    public Vector2 clickPos;

    bool locker = false;
    bool actri = false;
    public GameObject vm;

    public int tri = -1;
    MeshObject mo;

    GameObject vf;
    GameObject vs;
    GameObject vt;
    int verf;
    int vers;
    int vert;

    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            actri = false;
            if (vf != null)
            {
                Destroy(vf);
            }
            if (vs != null)
            {
                Destroy(vs);
            }
            if (vt != null)
            {
                Destroy(vt);
            }

            Ray i_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit i_hitInfo;
            if (Physics.Raycast(i_ray, out i_hitInfo) && i_hitInfo.collider.GetComponent<MeshObject>() != null)
            {
                if (!locker)
                {
                    visual.SetActive(true);
                    visual.transform.position = i_hitInfo.point;

                    if (Input.GetMouseButtonDown(1))
                    {
                        tri = -1;
                        locker = true;
                        clickPos = Input.mousePosition;
                        modifyPoint = i_hitInfo;//.collider.transform.InverseTransformPoint(i_hitInfo.point) // Save modify data
                        visual.transform.position = i_hitInfo.point;
                    }
                    else
                    {
                        mo = i_hitInfo.collider.GetComponent<MeshObject>();
                        if (mo != null)
                        {
                            Vector3 localHit = mo.transform.InverseTransformPoint(i_hitInfo.point);
                            int ti = i_hitInfo.triangleIndex;
                            Vector3[] nodes = new Vector3[3] { mo.vertices[mo.trianglesco[ti * 3]], mo.vertices[mo.trianglesco[ti * 3 + 1]], mo.vertices[mo.trianglesco[ti * 3 + 2]] };

                            float a = (nodes[0] - localHit).magnitude;
                            float b = (nodes[1] - localHit).magnitude;
                            float c = (nodes[2] - localHit).magnitude;

                            int smallest = a < b ? (a < c ? 0 : 2) : (b < c ? 1 : 2);

                            if ((nodes[smallest] - localHit).magnitude < 0.3f)
                            {
                                visual.transform.position = mo.transform.TransformPoint(nodes[smallest]);
                                if (Input.GetMouseButtonDown(0))
                                {
                                    clickPos = Input.mousePosition;
                                    vm.SetActive(true);
                                    vm.transform.position = mo.transform.TransformPoint(nodes[smallest]);
                                    VertexModifier cvm = vm.GetComponent<VertexModifier>();
                                    cvm.mo = mo.gameObject;
                                    cvm.visual = visual;
                                    cvm.index = smallest;
                                    cvm.ti = ti;
                                }
                            }
                            else
                            {
                                //Select triangle
                                if (Input.GetMouseButtonDown(0))
                                {
                                    if (addMode && !vm.activeSelf)
                                    {
                                        MeshObject mo = i_hitInfo.collider.gameObject.GetComponent<MeshObject>();
                                        int index = mo.AddPointWithTriangle(i_hitInfo.collider.transform.InverseTransformPoint(i_hitInfo.point), i_hitInfo.triangleIndex);
                                        vm.SetActive(true);
                                        vm.transform.position = i_hitInfo.point;
                                        VertexModifier cvm = vm.GetComponent<VertexModifier>();
                                        cvm.mo = mo.gameObject;
                                        cvm.visual = visual;
                                        cvm.index = index;
                                        cvm.direct = true;
                                    }
                                    else
                                    {
                                        clickPos = Input.mousePosition;
                                        tri = ti;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    tri = -1;
                    locker = false;
                    Cursor.lockState = CursorLockMode.Locked;//Hide the Cursor
                }
                if (!locker)
                {
                    visual.SetActive(false);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                actri = true;
                clickPos = Input.mousePosition;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                mo = hitInfo.collider.GetComponent<MeshObject>();
                if (mo != null)
                {
                    Vector3 localHit = mo.transform.InverseTransformPoint(hitInfo.point);
                    int ti = hitInfo.triangleIndex;
                    Vector3[] nodes = new Vector3[3] { mo.vertices[mo.trianglesco[ti * 3]], mo.vertices[mo.trianglesco[ti * 3 + 1]], mo.vertices[mo.trianglesco[ti * 3 + 2]] };

                    float a = (nodes[0] - localHit).magnitude;
                    float b = (nodes[1] - localHit).magnitude;
                    float c = (nodes[2] - localHit).magnitude;

                    int smallest = a < b ? (a < c ? 0 : 2) : (b < c ? 1 : 2);

                    if ((nodes[smallest] - localHit).magnitude < 0.3f)
                    {
                        visual.transform.position = mo.transform.TransformPoint(nodes[smallest]);
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (vt == null)
                            {
                                ref GameObject re = ref vt;
                                if (vt == null)
                                {
                                    vert = ti * 3 + smallest;
                                }
                                if (vs == null)
                                {
                                    vert = 0;
                                    re = ref vs;
                                    vers = ti * 3 + smallest;
                                }
                                if (vf == null)
                                {
                                    vers = 0;
                                    vert = 0;
                                    re = ref vf;
                                    verf = ti * 3 + smallest;
                                }
                                re = Instantiate(visual, mo.transform.TransformPoint(nodes[smallest]), Quaternion.identity);
                                re.SetActive(true);
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if(tri != -1)
            {
                mo.RemoveTriangles(tri);
            }
        }

        // Unlock and show cursor when right mouse button released
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Rotation
        if (Input.GetMouseButton(1) && !locker)
        {
            var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
            m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
        }

        // Translation
        var translation = GetinputTranslationDirection() * Time.deltaTime;

        // Speed up movement when shift key held
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            translation *= 10.0f;
        }

        // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
        boost += Input.mouseScrollDelta.y * 0.2f;
        translation *= Mathf.Pow(2.0f, boost);

        m_TargetCameraState.Translate(translation);

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

        m_InterpolatingCameraState.UpdateTransform(transform);
    }
}
