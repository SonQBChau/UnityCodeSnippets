using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System;

 //https://answers.unity.com/questions/773464/webcamtexture-correct-resolution-and-ratio.html?childToView=1155328#answer-1155328
 public class DeviceCameraController : MonoBehaviour
 {
     public RawImage image;
     private RawImage imageToCopy;
     public RectTransform imageParent;
     public AspectRatioFitter imageFitter;
 
     // Device cameras
     WebCamDevice frontCameraDevice;
     WebCamDevice backCameraDevice;
     WebCamDevice activeCameraDevice;
 
     WebCamTexture frontCameraTexture;
     WebCamTexture backCameraTexture;
     WebCamTexture activeCameraTexture;


 
     // Image rotation
     Vector3 rotationVector = new Vector3(0f, 0f, 0f);
 
     // Image uvRect
     Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
     Rect fixedRect = new Rect(0f, 1f, 1f, -1f);
 
     // Image Parent's scale
     Vector3 defaultScale = new Vector3(1f, 1f, 1f);
     Vector3 fixedScale = new Vector3(-1f, 1f, 1f);
 
 
     void Start()
     {
         // Check for device cameras
         if (WebCamTexture.devices.Length == 0)
         {
             Debug.Log("No devices cameras found");
             return;
         }
 
         // Get the device's cameras and create WebCamTextures with them
         frontCameraDevice = WebCamTexture.devices.Last();
         backCameraDevice = WebCamTexture.devices.First();
 
         frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
         backCameraTexture = new WebCamTexture(backCameraDevice.name);
 
         // Set camera filter modes for a smoother looking image
         frontCameraTexture.filterMode = FilterMode.Trilinear;
         backCameraTexture.filterMode = FilterMode.Trilinear;
 
         // Set the camera to use by default
         SetActiveCamera(frontCameraTexture);
         AdjustCameraOrientation(frontCameraTexture);
     }
 
     // Set the device camera to use and start it
     public void SetActiveCamera(WebCamTexture cameraToUse)
     {
         if (activeCameraTexture != null)
         {
             activeCameraTexture.Stop();
         }
             
         activeCameraTexture = cameraToUse;
         activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device => 
             device.name == cameraToUse.deviceName);

         image.texture = activeCameraTexture;
         image.material.mainTexture = activeCameraTexture;
        
 
        
        // the camera resolution of front camera
        // Aspect ratio is 1.777
        // Desktop is 1280x720
        // Pixel 2 XL is 3264*2488
         activeCameraTexture.requestedWidth = 1920; 
         activeCameraTexture.requestedHeight = 1080; 
         activeCameraTexture.Play();
     }
     public void AdjustCameraOrientation(WebCamTexture cameraToUse)
     {
        // Rotate image to show correct orientation 
        rotationVector.z = -activeCameraTexture.videoRotationAngle;
        image.rectTransform.localEulerAngles = rotationVector;
        if(WebCamTexture.devices.Length > 1){
            image.transform.Rotate(180,0,0); // for mobile
        }
        else{
            image.transform.Rotate(0,180,0); // for desktop
        }
 
         // Set AspectRatioFitter's ratio
         float videoRatio = 
             (float)activeCameraTexture.width / (float)activeCameraTexture.height;
         imageFitter.aspectRatio = videoRatio;
 
         // Unflip if vertically flipped
         image.uvRect = 
             activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect; //false
        
 
         // Mirror front-facing camera's image horizontally to look more natural
         imageParent.localScale = 
             activeCameraDevice.isFrontFacing ? fixedScale : defaultScale; //true

       
     }
 
     // Switch between the device's front and back camera
     public void SwitchCamera()
     {
         SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ? 
             backCameraTexture : frontCameraTexture);
     }
         
     void Update()
     {
        //    Debug.Log(activeCameraTexture.width);
        // Debug.Log(activeCameraTexture.height);
     }

    public void TakeSnapshot()
    {
        // full picture
        Texture2D snap = new Texture2D(activeCameraTexture.width, activeCameraTexture.height);
        snap.SetPixels(activeCameraTexture.GetPixels());
        snap.Apply();
     
        //Create our byte array and fill it with the necessary data to save our screenshot
        byte[] fileData = null;
        fileData = snap.EncodeToJPG();
        string filePath = GetScreenshotFileName(DateTime.Now);
        //Spin up a thread to write the information to the folder and then close it
        new System.Threading.Thread(() =>
        {
            var f = System.IO.File.Create(filePath);

            f.Write(fileData, 0, fileData.Length);
            f.Close();
            Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filePath, fileData.Length));
        }).Start();
    }
    private string GetScreenshotFileName(DateTime dateTime)
    {
        string filename = "";
        string folderPath = getPath();
        folderPath +=  @"/ScreenShots";
         // make sure directoroy exists
        Directory.CreateDirectory(folderPath);

        filename = folderPath + @"/" + dateTime.ToString("yyyy-MM-ddTHH:mm:ss").Replace(":", "-") + "_ScreenCapture.jpg";

        return filename;
    }
    private static string getPath(){
		#if UNITY_EDITOR
		return Application.dataPath;
		#elif UNITY_ANDROID
		return Application.persistentDataPath;// +fileName;
		#else
		return Application.dataPath;// +"/"+ fileName;
		#endif
	}
 }