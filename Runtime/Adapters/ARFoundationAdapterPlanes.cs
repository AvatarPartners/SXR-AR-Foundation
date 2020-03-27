using System.Collections.Generic;
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
    public class ARFoundationAdapterPlanes : SimplifyXRTargetAdapter
    {
        #region Public Fields
        /// <summary>
        /// The content holder will move to wherever a raycast hit takes place
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("The content holder will move to wherever a raycast hit takes place")]
#endif
        public bool moveContentHolderToRaycastHitLocation = true;
        /// <summary>
        /// The content holder will only be moved by this script after the first hit
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("The content holder will only be moved by this script after the first hit")]
#endif
        public bool stopMovingAfterFirstHit = false;
        //HitTracking
        bool firstHitOccurred;
        /// <summary>
        /// This object will be move to the location of the ARRaycast hits
        /// </summary>
#if UNITY_EDITOR
        [Tooltip("This object will be move to the location of the ARRaycast hits")]
#endif
        public GameObject contentHolder;
        #endregion

#if USING_ARFOUNDATION
        #region Private Fields

        //The AR raycast manager. Must be on the same GameObject as the adapter
        ARRaycastManager theRaycastManager;

        ARPlaneManager planeManager;
        #endregion

        #region Monobehaviours

        //Set the target manager behavior
        protected void Awake()
        {
            theRaycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
            if (theRaycastManager != null)
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorError, "[TARGET] No ARRaycastmanager in the scene. This component is required to use the ARFoundationAdapterPlanes");

            planeManager = GameObject.FindObjectOfType<ARPlaneManager>();
            if (planeManager != null)
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorError, "[TARGET] No ARPlaneManager in the scene. This component is required to use the ARFoundationAdapterPlanes");

            SimplifyXRTargetManager.Instance.SetBehavior(new ARFoundationPlaneManagerBehaviour());
            TrackingManager.Instance.SetTrackingManagerBehavior(new BaseTrackingCameraBehavior());
        }
        //Initial behavior set
        public void Start()
        {
            SimplifyXRTargetManager.Instance.TrackPreloadedObject(this);
            SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorDebug, "[TARGET] New target {0} being tracked", SimplifyXRDebug.Args(GetTargetName()));
            CallPoseLost();
        }
        public void Update()
        {
            if (stopMovingAfterFirstHit && firstHitOccurred)
                return;
            if (Input.touchCount > 0 && moveContentHolderToRaycastHitLocation)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                Touch t = Input.touches[0];
                theRaycastManager.Raycast(t.position, hits);
                MoveContentHolder(hits[0].pose.position);
                CallPoseFound();
                firstHitOccurred = true;
            }
        }
        void MoveContentHolder(Vector3 position)
        {
            contentHolder.transform.position = position;
        }
        //Stop tracking this object
        protected void OnDestroy()
        {
            SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.AuthorDebug, "[TARGET] Target {0} destroyed", SimplifyXRDebug.Args(GetTargetName()));
            SimplifyXRTargetManager.Instance.RemoveObject(GetTargetName());
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get the target name for this TrackableBehaviour
        /// </summary>
        public override string GetTargetName()
        {
            return "Plane Tracking";
        }
        /// <summary>
        /// Starts the tracking on this object
        /// </summary>
        public void StartTracking()
        {
            CallPoseFound();
        }
        /// <summary>
        /// Stops the tracking on any tracked object
        /// </summary>
        public void StopTracking()
        {
            CallPoseLost();
        }
        #endregion
#endif
    }
}