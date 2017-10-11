using UnityEngine;

public class GradientBackground : MonoBehaviour
{
    public Color topColor = Color.blue;
    public Color bottomColor = Color.white;
    public int gradientLayer = 7;
    public GameObject gradientPlane;

    void Awake()
    {
        CreateBackground();
    }

    private void CreateBackground()
    {
        gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
        if (!GetComponent<Camera>())
        {
            Debug.LogError("Must attach GradientBackground script to the camera");
            return;
        }

        GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask & ~(1 << gradientLayer);
        Camera gradientCam = new GameObject("Gradient Cam", typeof(Camera)).GetComponent<Camera>();
        gradientCam.depth = GetComponent<Camera>().depth - 1;
        gradientCam.cullingMask = 1 << gradientLayer;

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[4]
                        {new Vector3(-100f, .577f, 1f), new Vector3(100f, .577f, 1f), new Vector3(-100f, -.577f, 1f), new Vector3(100f, -.577f, 1f)};

        mesh.colors = new Color[4] { topColor, topColor, bottomColor, bottomColor };

        mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };

        Material mat = new Material(Shader.Find("Vertex Colors"));
        gradientPlane = new GameObject("Gradient Plane", typeof(MeshFilter), typeof(MeshRenderer));

        ((MeshFilter)gradientPlane.GetComponent(typeof(MeshFilter))).mesh = mesh;
        gradientPlane.GetComponent<Renderer>().material = mat;
        gradientPlane.layer = gradientLayer;
    }

    public void UpdateBackground(Color _topColor, Color _bottomColor)
    {
        topColor = _topColor;
        bottomColor = _bottomColor;
        if (gradientPlane == null) CreateBackground();
        gradientPlane.GetComponent<MeshFilter>().sharedMesh.colors = new Color[4] { topColor, topColor, bottomColor, bottomColor };
    }
}