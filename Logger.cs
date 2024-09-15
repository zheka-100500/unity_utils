using System;
using UnityEngine;

public static class Logger
	{
		public static bool showDebug { get; set; } = Application.isEditor;

		public static void WriteDebug(string tag, string message)
		{
			if (showDebug)
			{
				Write("DEBUG", tag, message, ConsoleColor.Gray);
			}
		}


		public static void WriteError(string tag, string message)
		{
			Write("ERROR", tag, message, ConsoleColor.Red);
		}


		public static void WriteInfo(string tag, string message)
		{
			Write("INFO", tag, message, ConsoleColor.Green);
		}


		public static void WriteWarn(string tag, string message)
		{
			Write("WARN", tag, message, ConsoleColor.Yellow);
		}


		private static void Write(string level, string tag, string message, ConsoleColor logColor)
		{

#if UNITY_SERVER && !UNITY_EDITOR
		var log = $"[{level}] [{tag}] {message}";
		Console.ForegroundColor = logColor;
			Console.WriteLine(log);
			Console.ResetColor();
#else

			UnityEngine.Debug.Log($"<color={logColor.ToString().ToLower()}>[{level}] [{tag}] {message}</color>");
#endif
		}


		public static void Info<T>(this T cl, string msg)
		{
			var type = typeof(T);
			WriteInfo(type.FullName ?? type.Name, msg);
		}

		public static void Warn<T>(this T cl, string msg)
		{
			var type = typeof(T);
			WriteWarn(type.FullName ?? type.Name, msg);
		}

		public static void Error<T>(this T cl, string msg)
		{
			var type = typeof(T);
			WriteError(type.FullName ?? type.Name, msg);
		}

		public static void Debug<T>(this T cl, string msg)
		{
			var type = typeof(T);
			WriteDebug(type.FullName ?? type.Name, msg);
		}

		public static void Info<T>(this T cl, string tag, string msg)
		{
			WriteInfo(tag, msg);
		}

		public static void Warn<T>(this T cl, string tag, string msg)
		{
			WriteWarn(tag, msg);
		}

		public static void Error<T>(this T cl, string tag, string msg)
		{
			WriteError(tag, msg);
		}

		public static void Debug<T>(this T cl, string tag, string msg)
		{
			WriteDebug(tag, msg);
		}
	}
