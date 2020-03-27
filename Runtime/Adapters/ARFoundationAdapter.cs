using UnityEngine;
using SimplifyXR;
#if USING_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace SimplifyXR
{
    /// <summary>
    /// This adapter is the main interface for Simplify to AR Foundation
    /// </summary>
    public class ARFoundationAdapter : SimplifyXRTargetAdapter
    {
        #region Public Fields
        /// <summary>
        /// If true, the camera will start tracking this target when the target is active in the scene. If false, use the StartTrackingTarget Action to track this target.
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("If true, the camera will start tracking this target when the target is active in the scene. If false, use the StartTrackingTarget Action to track this target.")]
#endif
        public bool StartTrackingImmediately = true;
        /// <summary>
        /// Tracks all of the images in the Reference Image Library
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("Tracks all of the images in the Reference Image Library")]
#endif
        public bool TrackAll;
        /// <summary>
        /// The name of the tracker file that this adapter will monitor
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("The name of the tracker file that this adapter will monitor")]
#endif
        public string ImageTargetName = "";

        [SerializeField]
        [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
        Texture2D m_DefaultTexture;

        /// <summary>
        /// If an image is detected but no source texture can be found,
        /// this texture is used instead.
        /// </summary>
        public Texture2D defaultTexture
        {
            get { return m_DefaultTexture; }
            set { m_DefaultTexture = value; }
        }
        /// <summary>
        /// The object to move with the image target
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("The object to move with the image target")]
#endif
        public GameObject contentHolder;
        #endregion

#if USING_ARFOUNDATION
        #region Private Fields
        //Stores the previous state of the target so that it can be compared against
        TrackingState previousState;

        //AR Tracked Image Manager instance ref
        ARTrackedImageManager m_TrackedImageManager;
        #endregion

        #region Monobehaviours
        //Set the target manager behavior
        protected void Awake()
        {
            m_TrackedImageManager = GameObject.FindObjectOfType<ARTrackedImageManager>();

            if (m_TrackedImageManager == null)
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorError, "[TARGET] No ARTrackedImageManager in the scene. This component is required to use the ARFoundationAdapter");

            SimplifyXRTargetManager.Instance.SetBehavior(new ARFoundationImageTargetManagerBehaviour());
            TrackingManager.Instance.SetTrackingManagerBehavior(new BaseTrackingCameraBehavior());
        }
        //Initial behavior set
        public void Start()
        {
            SimplifyXRTargetManager.Instance.TrackPreloadedObject(this);

            SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorDebug, "[TARGET] New target {0} being tracked", SimplifyXRDebug.Args(GetTargetName()));

            //Determine if the user would like to start tracking immediately or not
            if (StartTrackingImmediately)
            {
                StartTracking();
            }

            CallPoseLost();
        }
        //Event subscription
        protected void OnEnable()
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
        //Unsubscribe from events
        protected void OnDisable()
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
        //Stop tracking this object
        protected void OnDestroy()
        {
            SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorDebug, "[TARGET] Target {0} destroyed", SimplifyXRDebug.Args(GetTargetName()));
            SimplifyXRTargetManager.Instance.RemoveObject(GetTargetName());
        }
        #endregion

        #region Public methods
        void UpdateInfo(ARTrackedImage trackedImage)
        {
            Debug.Log("UpdateInfo: " + trackedImage.name);

            if (trackedImage.referenceImage.name.Equals(ImageTargetName) || TrackAll)
            {
                if ((trackedImage.trackingState == TrackingState.Tracking))
                {
                    if (this.previousState != TrackingState.Tracking)
                    {
                        Debug.Log("Tracking: " + trackedImage.referenceImage.name);
                        CallPoseFound();
                        this.previousState = TrackingState.Tracking;
                    }
                    contentHolder.transform.position = trackedImage.transform.position;
                    contentHolder.transform.rotation = trackedImage.transform.rotation;
                }
                else if (trackedImage.trackingState == TrackingState.Limited)
                {
                    contentHolder.transform.position = trackedImage.transform.position;
                    contentHolder.transform.rotation = trackedImage.transform.rotation;
                }
                else if (trackedImage.trackingState == TrackingState.None)
                {
                    if (this.previousState != TrackingState.None)
                    {
                        Debug.Log("No tracking");
                        CallPoseLost();
                        this.previousState = TrackingState.None;
                    }
                }
            }
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            Debug.Log("OnTrackedImagesChanged");

            foreach (var trackedImage in eventArgs.added)
            {
                // Give the initial image a reasonable default scale
                trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
                UpdateInfo(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
                UpdateInfo(trackedImage);
        }

        /// <summary>
        /// Get the target name for this TrackableBehaviour
        /// </summary>
        public override string GetTargetName()
        {
            return ImageTargetName;
        }
        /// <summary>
        /// Starts the tracking on this object
        /// </summary>
        public void StartTracking()
        {
            m_TrackedImageManager.enabled = true;
        }
        /// <summary>
        /// Stops the tracking on any tracked object
        /// </summary>
        public void StopTracking()
        {
            m_TrackedImageManager.enabled = false;
        }
        #endregion
#endif
    }
}