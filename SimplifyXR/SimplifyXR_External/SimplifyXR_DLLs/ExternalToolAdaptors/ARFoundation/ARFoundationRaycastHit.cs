using System.Collections.Generic;
using UnityEngine;
#if USING_ARFOUNDATION
using UnityEngine.XR.ARFoundation;

namespace SimplifyXR
{
    /// <summary>
    /// Sends the manipulation event data from a unity event
    /// </summary>
    [DirectiveCategory(DirectiveCategories.Initiator, DirectiveSubCategory.UserInteraction)]
    public class ARFoundationRaycastHit : Initiator
    {
        public ARRaycastManager TheRaycastManager;

        public override List<KnobKeywords> ReceiveKeywords()
        {
            return new List<KnobKeywords>();
        }

        public override List<KnobKeywords> SendKeywords()
        {
            return new List<KnobKeywords>()
            {
                new KnobKeywords("PointHit", typeof(Vector3)),
                new KnobKeywords("Distance", typeof(float)),
            };
        }

        public void Update()
        {
            if (Input.touchCount > 0)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                Touch t = Input.touches[0];
                TheRaycastManager.Raycast(t.position, hits);
                if (hits.Count > 0)
                    SendData(hits[0]);

                Initiate();
            }
        }

        public override void Initiate()
        {
            base.Initiate();
        }

        void SendData(ARRaycastHit theHit)
        {
            var thisData = new List<object> { theHit.pose.position, theHit.distance };
            var thisKeywords = new List<string> { "PointHit", "Distance" };
            AddPassableData(thisKeywords, thisData);
        }
    }
}
#endif