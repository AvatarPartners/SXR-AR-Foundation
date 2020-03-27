using System.Linq;
using SimplifyXR;

namespace SimplifyXR
{
#if USING_ARFOUNDATION

    /// <summary>
    /// Standard VisionLib Behavior
    /// </summary>
    public class ARFoundationPlaneManagerBehaviour : BaseActiveObjectManagerBehaviour<SimplifyXRTargetAdapter>
    {
        #region Overrides
        //Tracks the object
        public override void TrackPreloadedObject(SimplifyXRTargetAdapter theObject)
        {
            myActiveObject.TrackObject(theObject);
        }
        //Method not implemented. Image targets can only be added in a scene ahead of runtime
        protected override void ActuallyAddObject(string toAdd)
        {
            SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.Error, "Cannot Add an SimplifyXRTarget named {0}. This behavior does not add targets at runtime. Create targets in the scene before runtime.", SimplifyXRDebug.Args(toAdd));
        }
        //Used to stop tracking a dataset
        protected override void ActuallyHideObject(SimplifyXRTargetAdapter toHide)
        {
            try
            {
                //see of the object has already been loaded
                if (myActiveObject.GetAllObjects().Contains(toHide))
                {
                    ARFoundationAdapterPlanes adapter = toHide as ARFoundationAdapterPlanes;
                    //TODO stop the tracker
                    adapter.StopTracking();
                    SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.DeveloperDebug, "Stoping Tracking the passed SimplifyXRTarget {0}.", SimplifyXRDebug.Args(toHide.GetTargetName()));
                }
                else
                {
                    SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.DeveloperDebug, "The passed SimplifyXRTarget {0} is being tracked by this manager. Please track the target before calling this method.", SimplifyXRDebug.Args(toHide.GetTargetName()));
                }
            }
            catch
            {
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.DeveloperDebug, "The passed SimplifyXRTarget {0} is not a VisionLib target. Please change target manager behaviors before attempting to unload this target", SimplifyXRDebug.Args(toHide.GetTargetName()));
            }
        }
        //stops tracking an object
        protected override void ActuallyRemoveObject(SimplifyXRTargetAdapter toRemove)
        {
            myActiveObject.StopTrackingObject(toRemove);
        }
        //Not implemented. No concept of active
        protected override void ActuallySetActive(SimplifyXRTargetAdapter toSetActive)
        {
            SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.Error, "Cannot set SimplifyXRTarget {0} as the active target using the current behavior {1}. This behavior does not track active targets.", SimplifyXRDebug.Args(toSetActive.GetTargetName(), this));
        }
        //Used to start tracking a dataset
        protected override void ActuallyShowObject(SimplifyXRTargetAdapter toShow)
        {
            try
            {
                if (!myActiveObject.GetAllObjects().Contains(toShow))
                {
                    myActiveObject.TrackObject(toShow);
                }
                ARFoundationAdapterPlanes adapter = toShow as ARFoundationAdapterPlanes;
                //TODO Start the tracking
                adapter.StartTracking();
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.DeveloperDebug, "Tracking the passed SimplifyXRTarget {0}.", SimplifyXRDebug.Args(toShow.GetTargetName()));
            }
            catch
            {
                SimplifyXRDebug.SimplifyXRLog(SimplifyXRDebug.Type.DeveloperDebug, "The passed SimplifyXRTarget {0} is not a vuforia target. Please change target manager behaviors before attempting to load this target", SimplifyXRDebug.Args(toShow.GetTargetName()));
            }
        }
        //Get an object from a string
        public override SimplifyXRTargetAdapter GetObjectFromString(string nameOfObject)
        {
            return myActiveObject.GetAllObjects().FirstOrDefault(x => x.GetTargetName() == nameOfObject);
        }
        //See if an object is stored
        protected override bool IsObjectStored(string nameOfObject)
        {
            return myActiveObject.GetAllObjects().Contains(GetObjectFromString(nameOfObject));
        }
        #endregion
    }
#endif
}