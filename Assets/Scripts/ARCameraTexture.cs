using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCameraTextureAssigner : MonoBehaviour
{
    public Material arMaterial; // Assign the Shader Graph Material
    private ARCameraBackground arCameraBackground;
    private ARCameraManager arManager;
    RenderTexture target;

    private void Awake() {
        arCameraBackground = FindFirstObjectByType<ARCameraBackground>();
        arManager = FindFirstObjectByType<ARCameraManager>();
        arManager.frameReceived += ArFrameReceived;
    }
    void ArFrameReceived(ARCameraFrameEventArgs args)
    {
        Graphics.Blit(args.textures[0], target, arCameraBackground.material);
        arMaterial.SetTexture("_CameraTexture", target);
        // Debug.Log("#Raul: Camera texture applied? Textures count: " + args.textures.Count);
    }
}
