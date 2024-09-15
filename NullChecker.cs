using System;

#if UNITY_EDITOR
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
   }

   private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
   {
      if (state != PlayModeStateChange.EnteredPlayMode) return;
      if(CheckAllForNull()) return;
      EditorApplication.ExitPlaymode();
   }

   private static bool CheckAllForNull()
   {
      var root = SceneManager.GetActiveScene().GetRootGameObjects();
      var components = new List<MonoBehaviour>();
      foreach (var t in root)
      {
         components.AddRange(t.GetComponents<MonoBehaviour>());
         components.AddRange(t.GetComponentsInChildren<MonoBehaviour>().Where(c => !components.Contains(c)));
      }
      foreach (var component in components)
      {
         var fields = component.GetType().GetFields().Where(f => Attribute.IsDefined(f, typeof(CheckForNull)));
         foreach (var field in fields)
         {

            var value = $"{field.GetValue(component)}";
            if (!value.Equals("null", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))continue;
            Debug.LogError($"Detected null at: {component.GetType().FullName}::{field.Name}");
            EditorApplication.Beep();
            if (EditorUtility.DisplayDialog("Check failed",
                   $"Detected null in: {component.GetType().FullName}::{field.Name}, continue anyway?", "continue",
                   "abort"))
            {
               continue;
            }
            Selection.activeGameObject = component.gameObject;
            Debug.LogError("Abort startup!");
            return false;
         }
         
      }
      Debug.Log("All checks passed, start allowed :)");
      return true;
   }
}


#endif

public class CheckForNull : Attribute
{
   
}
