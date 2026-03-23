using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

internal class CameraScreenRaycaster : IScreenRaycaster
{
	public class CameraRayEnumerator : IEnumerator<(Ray, Camera, bool)>, IEnumerator, IDisposable, IEnumerable<(Ray, Camera, bool)>, IEnumerable
	{
		private Camera[] m_Cameras;

		private int m_LayerMask;

		private Vector2 m_MousePosition;

		private int? m_TargetDisplay;

		private int m_Index = -1;

		private Camera m_CurrentCamera;

		private Ray m_CurrentRay;

		private bool m_IsInsideCameraRect;

		public (Ray, Camera, bool) Current => (m_CurrentRay, m_CurrentCamera, m_IsInsideCameraRect);

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			while (++m_Index < m_Cameras.Length)
			{
				m_CurrentCamera = m_Cameras[m_Index];
				if (!IsValid(m_CurrentCamera, m_LayerMask, m_TargetDisplay))
				{
					continue;
				}
				m_IsInsideCameraRect = MakeRay(m_CurrentCamera, m_MousePosition, out m_CurrentRay);
				return true;
			}
			return false;
		}

		public void Reset()
		{
			m_Index = -1;
		}

		public IEnumerator<(Ray, Camera, bool)> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static CameraRayEnumerator GetPooled(Camera[] cameras, int layerMask, Vector2 mousePosition, int? targetDisplay)
		{
			CameraRayEnumerator cameraRayEnumerator = GenericPool<CameraRayEnumerator>.Get();
			cameraRayEnumerator.m_Cameras = cameras;
			cameraRayEnumerator.m_LayerMask = layerMask;
			cameraRayEnumerator.m_MousePosition = mousePosition;
			cameraRayEnumerator.m_TargetDisplay = targetDisplay;
			return cameraRayEnumerator;
		}

		public void Dispose()
		{
			Reset();
			m_Cameras = null;
			GenericPool<CameraRayEnumerator>.Release(this);
		}
	}

	public Camera[] cameras = Array.Empty<Camera>();

	public Camera[] singleCamera = new Camera[1];

	public int layerMask = -1;

	public virtual void Update()
	{
		Array.Sort(cameras, (Camera a, Camera b) => -a.depth.CompareTo(b.depth));
	}

	public IEnumerable<(Ray, Camera, bool)> MakeRay(Vector2 mousePosition, int pointerId, int? targetDisplay)
	{
		return CameraRayEnumerator.GetPooled(((singleCamera[0] = PointerDeviceState.GetCameraWithSoftPointerCapture(pointerId)) != null) ? singleCamera : cameras, layerMask, mousePosition, targetDisplay);
	}

	private static bool IsValid(Camera camera, int layerMask, int? targetDisplay)
	{
		return camera != null && (camera.cullingMask & layerMask) != 0 && (!targetDisplay.HasValue || camera.targetDisplay == targetDisplay);
	}

	private static bool MakeRay(Camera camera, Vector2 mousePosition, out Ray ray)
	{
		Vector2 vector = UIElementsRuntimeUtility.PanelToScreenBottomLeftPosition(mousePosition, camera.targetDisplay);
		ray = camera.ScreenPointToRay(vector);
		return camera.pixelRect.Contains(vector);
	}
}
