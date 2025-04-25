using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
//using WebSocketSharp;

[System.Serializable]
public class QRData
{
    public string id;
    public float x, y, z;
}

//namespace UnityEngine.XR.ARFoundation.Samples
//{
    /// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
    /// and overlays some information as well as the source Texture2D on top of the
    /// detected image.
    /// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracker : MonoBehaviour    
{
    public Action<string> OnQRCodeIdentified;

    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    /// <summary>
    /// The prefab has a world space UI canvas,
    /// which requires a camera to function properly.
    /// </summary>
    public Camera worldSpaceCanvasCamera
    {
        get { return m_WorldSpaceCanvasCamera; }
        set { m_WorldSpaceCanvasCamera = value; }
    }

    [SerializeField]
    [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
    Texture2D m_DefaultTexture;

    //WebSocket ws;

    /// <summary>
    /// If an image is detected but no source texture can be found,
    /// this texture is used instead.
    /// </summary>
    public Texture2D defaultTexture
    {
        get { return m_DefaultTexture; }
        set { m_DefaultTexture = value; }
    }

    ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void Start()
    {
        // WebSocket 클라이언트 초기화
        /*ws = new WebSocket("ws://192.168.0.1:8081/qr"); // Quest IP
        //ws = new WebSocket("ws://192.168.1.101:4649/qr"); // Quest IP
            
        ws.OnOpen += (sender, e) => Debug.Log("WebSocket 연결 성공");
        ws.OnError += (sender, e) => Debug.LogError("WebSocket 오류: " + e.Message);
        ws.OnClose += (sender, e) => Debug.Log($"WebSocket 연결 종료: {e.Reason}, {e.Code}");
        ws.Log.Level = LogLevel.Trace;

        ws.Connect();
        Debug.Log("Web Socket Started....");
        */
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        // Update information about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
            "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3} cm\nDetected size: {4} cm\nx{5},y{6},z{7}",
            trackedImage.referenceImage.name,
            trackedImage.trackingState,
            trackedImage.referenceImage.guid,
            trackedImage.referenceImage.size * 100f,
            trackedImage.size * 100f, 
            trackedImage.transform.position.x, trackedImage.transform.position.y, trackedImage.transform.position.z);


        var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
        var planeGo = planeParentGo.transform.GetChild(0).gameObject;

        // Disable the visual plane if it is not being tracked
        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGo.SetActive(true);

            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);

            // Set the texture
            var material = planeGo.GetComponentInChildren<MeshRenderer>().material;
            material.mainTexture = (trackedImage.referenceImage.texture == null) ? defaultTexture : trackedImage.referenceImage.texture;


            QRData data = new QRData();
            data.id = "1";
            data.x = trackedImage.transform.position.x;
            data.y = trackedImage.transform.position.y;
            data.z = trackedImage.transform.position.z;
            string jsonData = JsonUtility.ToJson(data);
            OnQRCodeIdentified?.Invoke(jsonData);

            // if(ws.IsAlive)
            //{ 
            //    ws.Send(jsonData);
            //    Debug.Log("Quest로 전송: " + jsonData);
            //}
        }
        else
        {
            planeGo.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
            UpdateInfo(trackedImage);
    }

    private void OnDestroy()
    {
        //if(ws != null) ws.Close();
    }
}
//}