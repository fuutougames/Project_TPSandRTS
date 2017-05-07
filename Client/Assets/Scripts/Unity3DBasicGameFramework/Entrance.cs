using UnityEngine;
using System.Collections;

public class Entrance : MonoBehaviour {

	// Use this for initialization
    private int count;
    private Rendering.PostProcessUnits.PPUBlackAndWhite bawppu;

	void Start () {
        DontDestroyOnLoad(gameObject);
        Rendering.RenderingMgr.Instance.Init();
        bawppu = new Rendering.PostProcessUnits.PPUBlackAndWhite();
        bawppu.Init();
        WindowMgr.Instance.Init();

        //Dispatcher.Dispatch<int>(GameEvents.WindowStartUpEvent.EVT_NAME, UIModule.LOGIN);
        
        //Rendering.PostProcessUnits.PPUBlackAndWhite testppu1 = new Rendering.PostProcessUnits.PPUBlackAndWhite();
        //Rendering.RenderingMgr.Instance.AddUnitAtLast(testppu1);
        count = 0;

        GameObject camgo = new GameObject();
        camgo.transform.position = new Vector3(1000, 1000, 1000);
        Camera cam = camgo.AddComponent<Camera>();
        cam.enabled = false;
        Rendering.RenderingUnit mainCamUnit = new Rendering.RenderingUnit("MainCam", cam);
        Rendering.RenderingMgr.Instance.AddCrucialUnitAtFirst("MainCam", mainCamUnit);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(cam.transform, false);
        cube.transform.localPosition = new Vector3(.0f, .0f, 5.0f);

        //Rendering.RenderingMgr.Instance.AddUnitAtLast(bawppu);
    }

    public void AddBAWPPU()
    {
        Rendering.RenderingMgr.Instance.AddUnitAtLast(bawppu);
    }
	
    public void RemoveBAWPPU()
    {
        Rendering.RenderingMgr.Instance.RemoveNode(bawppu);
    }

	// Update is called once per frame
	void Update () {
        return;
        count++;
        if (count == 150)
            Rendering.RenderingMgr.Instance.AddUnitAtLast(bawppu);

        if (count == 300)
            Rendering.RenderingMgr.Instance.RemoveNode(bawppu);

        if (count == 500)
            Rendering.RenderingMgr.Instance.PauseRendering();

        if (count == 700)
            Rendering.RenderingMgr.Instance.ResumeRendering();

        if (count == 900)
            Rendering.RenderingMgr.Instance.PauseRendering(true);

        if (count == 1100)
            Rendering.RenderingMgr.Instance.ResumeRendering();

        if (count == 1300)
            Rendering.RenderingMgr.Instance.AddUnitAtLast(bawppu);
    }
}
