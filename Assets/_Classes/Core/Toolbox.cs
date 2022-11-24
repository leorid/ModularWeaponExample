using UnityEngine;
using System.Collections;
using System;

public static class Toolbox
{
	/// <summary>
	/// Generates Unique IDs
	/// </summary>
	public static string GetUniqueID()
	{
		System.Guid guid = System.Guid.NewGuid();
		return guid.ToString();
	}

	public static bool ComputePenetration(Collider col1, Collider col2,
		out Vector3 vector, out float distance)
	{
		return Physics.ComputePenetration(
			col1, col1.transform.position, col1.transform.rotation,
			col2, col2.transform.position, col2.transform.rotation,
			out vector, out distance);
	}

	/// <summary>
	/// Breaks out of infinite while loop by throwing an exception. <br/>
	/// Usage: 
	/// <code>
	/// int breaker = 1000;
	/// while (condition) {
	///	    Toolbox.WhileBreaker(ref breaker, "");
	/// } 
	/// </code>
	/// </summary>
	public static void WhileBreaker(ref int countDown, string debugText)
	{
		countDown--;
		if (countDown < 0)
		{
			throw new System.Exception(debugText);
		}
	}

	public static void SetEnabled(UnityEngine.Object obj, bool enable)
	{
		if (!obj) return;
		if (obj is GameObject) { (obj as GameObject).SetActive(enable); return; }
		if (obj is Behaviour) { (obj as Behaviour).enabled = enable; return; }
		if (obj is Renderer) { (obj as Renderer).enabled = enable; return; }
		Debug.LogError("Cannot disable Object " + obj, obj);
	}

	public static void DrawAxisCross(Vector3 point, float size, Color color,
		float duration = 0, bool depthTest = true)
	{
		Debug.DrawLine(point + Vector3.up * size, point - Vector3.up * size, color,
			duration, depthTest);
		Debug.DrawLine(point + Vector3.right * size, point - Vector3.right * size, color,
			duration, depthTest);
		Debug.DrawLine(point + Vector3.forward * size, point - Vector3.forward * size, color,
			duration, depthTest);
	}


	public static bool CheckIndex(int index, IList list, bool print = true)
	{
		if (index < 0 || index >= list.Count)
		{
			if (print) Debug.LogError($"Index out of Range (0,{list.Count}) -> {index}");
			return false;
		}
		return true;
	}

	public static void DrawDebugTextWithBackground(string text, Vector3 worldPos,
		int textSize = 5, bool worldSize = false,
		Color? textColor = null, Color? backColor = null)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;

		int tempFrameCount = Time.frameCount;

		void Draw(UnityEditor.SceneView sv)
		{
			DrawGizmosTextWithBackground(text, worldPos, textSize,
				worldSize, textColor, backColor);
			if (Time.frameCount != tempFrameCount || !Application.isPlaying)
			{
				UnityEditor.SceneView.duringSceneGui -= Draw;
			}
		}

		UnityEditor.SceneView.duringSceneGui += Draw;
#endif
	}
	public static void DrawGizmosTextWithBackground(string text, Vector3 worldPos, int textSize = 5, bool worldSize = false,
		Color? textColor = null, Color? backColor = null)
	{
#if UNITY_EDITOR
		UnityEditor.Handles.BeginGUI();
		var restoreColor = GUI.color;
		var restoreBackColor = GUI.backgroundColor;

		var view = UnityEditor.SceneView.currentDrawingSceneView;
		if (view != null && view.camera != null)
		{
			Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
			if (screenPos.y < 0 || screenPos.y > Screen.height ||
				screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
			{
				GUI.color = restoreColor;
				UnityEditor.Handles.EndGUI();
				return;
			}

			float distanceToCam = (view.camera.transform.position - worldPos).magnitude;
			textSize = Mathf.RoundToInt(textSize * (50 / distanceToCam));
			if (textSize <= 0) return;

			GUIStyle style = new GUIStyle(GUI.skin.box);
			style.stretchHeight = true;
			style.stretchWidth = true;
			style.normal.textColor = Color.white;
			style.fontSize = textSize;
			style.alignment = TextAnchor.MiddleCenter;
			style.border = new RectOffset(2, 2, 2, 2);

			Vector2 size = style.CalcSize(new GUIContent(text));
			var r = new Rect(screenPos.x - (size.x / 2),
				view.position.height - 80 - screenPos.y,
				size.x, size.y);


			GUI.backgroundColor = backColor ?? Color.black;
			//GUI.Box(r, text, style);
			GUI.color = textColor ?? Color.white;
			GUI.Label(r, text, style);

			GUI.color = restoreColor;
			GUI.backgroundColor = restoreBackColor;
		}
		UnityEditor.Handles.EndGUI();
#endif
	}
}
