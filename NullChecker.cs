using System;
#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
[InitializeOnLoad]
public static class NullChecker
{
   static NullChecker()
   {
      EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
      SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
   }
   private static void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
   {
      if (EditorApplication.isPlaying) CheckAllForNull(arg1.GetRootGameObjects());
   }
   private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
   {
      if (state == PlayModeStateChange.EnteredPlayMode) CheckAllForNull(SceneManager.GetActiveScene().GetRootGameObjects());
   }
   private static void CheckAllForNull(GameObject[] root)
   {
      var bindingFlags = BindingFlags.Instance |
                         BindingFlags.NonPublic |
                         BindingFlags.Public;
      var components = new List<MonoBehaviour>();
      foreach (var t in root)
      {
         components.AddRange(t.GetComponents<MonoBehaviour>());
         components.AddRange(t.GetComponentsInChildren<MonoBehaviour>().Where(c => !components.Contains(c)));
      }
      foreach (var component in components)
      {
         var fields = component.GetType().GetFields(bindingFlags).Where(f => Attribute.IsDefined(f, typeof(CheckForNull)));
         foreach (var field in fields)
         {
            var value = field.GetValue(component);
            if (value != null && !value.ToString().Equals("null", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value.ToString()))continue;
            Debug.LogError($"Detected null at: ({component.gameObject.name}) {component.GetType().FullName}::{field.Name}");
            EditorApplication.Beep();
            if (EditorUtility.DisplayDialog("Check failed",
                   $"Detected null at: ({component.gameObject.name}) {component.GetType().FullName}::{field.Name}, continue anyway?", "continue",
                   "abort"))
            {
               continue;
            }
            Selection.activeGameObject = component.gameObject;
            Debug.LogError("Aborting!");
            EditorApplication.ExitPlaymode();
            return;
         }
      }
      Debug.Log("All checks passed :)");
   }
}
#endif
public class CheckForNull : Attribute { }
