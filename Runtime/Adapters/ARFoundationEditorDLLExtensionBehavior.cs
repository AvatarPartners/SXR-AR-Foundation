using UnityEngine;
using SimplifyXR;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SimplifyXR.Runtime")]
namespace SimplifyXR
{
    /// <summary>
    /// Behavior for handling editor methods interacting with AR foundation's DLL
    /// </summary>
    public class ARFoundationEditorDLLExtensionBehavior : IManageARFoundationDLLExtensions
    {
        public void AddARFoundationAdapterComponent(GameObject go)
        {
            if (go != null)
            {
#if USING_ARFOUNDATION
                go.AddComponent<ARFoundationAdapter>();
#endif
            }
        }
    }
}