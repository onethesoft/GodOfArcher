using Assets.HeroEditor.Common.CommonScripts;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Assets.HeroEditor.Common.Editor
{
    /// <summary>
    /// Add "Refresh" button to IconCollection script
    /// </summary>
    [CustomEditor(typeof(IconCollection))]
    public class IconCollectionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var collection = (IconCollection) target;

            if (GUILayout.Button("Refresh"))
            {
				collection.Refresh();
            }
        }
    }
}
#endif