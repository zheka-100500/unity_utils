using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

	public static class GameObjectUtils
	{
		public static Dictionary<string, GameObject> GetObjectTree(this GameObject gameObject)
		{
			var Result = new Dictionary<string, GameObject>();
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				var obj = gameObject.transform.GetChild(i).gameObject;
				var name = obj.name;
				GetTreePath(obj, ref name);
				;

				Result.Add(name + $"_{i}", obj);

				if (obj.transform.childCount > 0)
				{
					var nwobjs = GetObjectTree(obj);

					foreach (var item in nwobjs)
					{
						if (!Result.ContainsKey(item.Key))
							Result.Add(item.Key, item.Value);
					}

				}
			}
			return Result;
		}

		public static void GetTreePath(this GameObject obj, ref string name)
		{
			if (obj.transform.parent != null)
			{
				var parent = obj.transform.parent.gameObject;
				name = parent.name + $"/{name}";
				GetTreePath(parent, ref name);
			}
		}

		public static T GetAddComponent<T>(this GameObject go) where T : Component
		{
			T t = go.GetComponent<T>();
			if (t == null)
			{
				t = go.AddComponent<T>();
			}
			return t;
		}


		public static bool RemoveComponent<T>(this GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component != null)
			{
				Object.DestroyImmediate(component);
				return true;
			}
			return false;
		}


		public static T CopyComponent<T>(this GameObject to, GameObject from) where T : Component
		{
			if (from == null)
			{
				return default(T);
			}
			T component = from.GetComponent<T>();
			T addComponent = to.GetAddComponent<T>();
			if (component == null)
			{
				return default(T);
			}
			foreach (FieldInfo fieldInfo in typeof(T).GetFields(BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn))
			{
				fieldInfo.SetValue(addComponent, fieldInfo.GetValue(component));
			}
			return addComponent;
		}


		public static void SetScale(this GameObject gameObject, float scaleX, float scaleY)
		{
			Vector3 localScale = gameObject.transform.localScale;
			localScale.x = scaleX;
			localScale.y = scaleY;
			gameObject.transform.localScale = localScale;
		}


		public static GameObject FindGameObjectInChildren(this GameObject gameObject, string name, bool useBaseName = false)
		{
			if (gameObject == null)
			{
				return null;
			}
			foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
			{
				if (transform.GetName(useBaseName) == name)
				{
					return transform.gameObject;
				}
			}
			return null;
		}

		
		public static void Log(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return;
			}
			Logger.Info("GameObjectUtils", gameObject.GetName(false));
			Logger.Info("GameObjectUtils", gameObject.GetPath(false));
			Logger.Info("GameObjectUtils", "Layer : " + gameObject.layer.ToString());
			Logger.Info("GameObjectUtils", "Position : " + gameObject.transform.position.ToString());
			Logger.Info("GameObjectUtils", "Rotation : " + gameObject.transform.rotation.ToString());
			Logger.Info("GameObjectUtils", "Scale : " + gameObject.transform.localScale.ToString());
			foreach (Component component in gameObject.GetComponents<Component>())
			{
				string str = "Component : ";
				Type type = component.GetType();
				Logger.Info("GameObjectUtils", str + ((type != null) ? type.ToString() : null));
				
			}
		}


		public static void LogWithChildren(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return;
			}
			gameObject.Log();
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.Log();
			}
		}


		public static void PrintAllActiveGameObjectsInScene()
		{
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Log();
			}
		}

		
		public static GameObject Find(this GameObject go, string name)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				GameObject gameObject = go.transform.GetChild(i).gameObject;
				if (gameObject.name == name)
				{
					return gameObject;
				}
			}
			for (int j = 0; j < go.transform.childCount; j++)
			{
				GameObject gameObject2 = go.transform.GetChild(j).gameObject.Find(name);
				if (gameObject2 != null)
				{
					return gameObject2;
				}
			}
			return null;
		}


		public static void FindAllChildren(this GameObject go, List<GameObject> allGoList)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				allGoList.Add(go.transform.GetChild(i).gameObject);
			}
			for (int j = 0; j < go.transform.childCount; j++)
			{
				go.transform.GetChild(j).gameObject.FindAllChildren(allGoList);
			}
		}


		public static void DisableChildren(this GameObject go)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				go.transform.GetChild(i).gameObject.SetActive(false);
			}
		}


		public static List<GameObject> GetAllGameObjects(this Scene scene)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				list.Add(gameObject);
				gameObject.FindAllChildren(list);
			}
			return list;
		}


		public static GameObject GetGameObjectByName(this Scene scene, string name, bool useBaseName = false)
		{
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.GetName(useBaseName) == name)
				{
					return gameObject;
				}
				GameObject gameObject2 = gameObject.FindGameObjectInChildren(name, useBaseName);
				if (gameObject2 != null)
				{
					return gameObject2;
				}
			}
			return null;
		}


		public static GameObject[] GetRootGameObjects()
		{
			return SceneManager.GetActiveScene().GetRootGameObjects();
		}


		public static GameObject GetGameObjectInScene(string name, bool useBaseName = false)
		{
			return SceneManager.GetActiveScene().GetGameObjectByName(name, useBaseName);
		}

		
		public static List<GameObject> GetAllGameObjectsInScene()
		{
			return SceneManager.GetActiveScene().GetAllGameObjects();
		}


		public static string GetName(this Transform t, bool useBaseName = false)
		{
			return t.gameObject.GetName(useBaseName);
		}


		public static string GetName(this GameObject go, bool useBaseName = false)
		{
			string text = go.name;
			if (useBaseName)
			{
				text = text.ToLower();
				text.Replace("(clone)", "");
				text = text.Trim();
				text.Replace("cln", "");
				text = text.Trim();
				text = Regex.Replace(text, "\\([0-9+]+\\)", "");
				text = text.Trim();
				text = Regex.Replace(text, "[0-9+]+$", "");
				text = text.Trim();
				text.Replace("(clone)", "");
				text = text.Trim();
			}
			return text;
		}

		public static string GetPath(this GameObject go, bool useBaseName = false)
		{
			string text = go.GetName(useBaseName);
			GameObject gameObject = go;
			while (gameObject.transform.parent != null && gameObject.transform.parent.gameObject != null)
			{
				gameObject = gameObject.transform.parent.gameObject;
				text = gameObject.GetName(useBaseName) + "/" + text;
			}
			return text;
		}
	}

