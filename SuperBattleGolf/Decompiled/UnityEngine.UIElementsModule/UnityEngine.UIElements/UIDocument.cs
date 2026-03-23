#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements;

[HelpURL("UIE-get-started-with-runtime-ui")]
[DefaultExecutionOrder(-100)]
[AddComponentMenu("UI Toolkit/UI Document")]
[DisallowMultipleComponent]
[ExecuteAlways]
public sealed class UIDocument : MonoBehaviour
{
	public enum WorldSpaceSizeMode
	{
		Dynamic,
		Fixed
	}

	internal const string k_RootStyleClassName = "unity-ui-document__root";

	internal const string k_VisualElementNameSuffix = "-container";

	private const int k_DefaultSortingOrder = 0;

	private static int s_CurrentUIDocumentCounter;

	internal readonly int m_UIDocumentCreationIndex;

	internal static int EnabledDocumentCount;

	[SerializeField]
	private PanelSettings m_PanelSettings;

	private PanelSettings m_PreviousPanelSettings = null;

	[SerializeField]
	private UIDocument m_ParentUI;

	private UIDocumentList m_ChildrenContent = null;

	private List<UIDocument> m_ChildrenContentCopy = null;

	[SerializeField]
	private VisualTreeAsset sourceAsset;

	private UIDocumentRootElement m_RootVisualElement;

	internal int softPointerCaptures = 0;

	private int m_FirstChildInsertIndex;

	[SerializeField]
	private float m_SortingOrder = 0f;

	[SerializeField]
	private Position m_Position = Position.Relative;

	[SerializeField]
	private WorldSpaceSizeMode m_WorldSpaceSizeMode = WorldSpaceSizeMode.Fixed;

	[SerializeField]
	private float m_WorldSpaceWidth = 1920f;

	[SerializeField]
	private float m_WorldSpaceHeight = 1080f;

	[SerializeField]
	private PivotReferenceSize m_PivotReferenceSize;

	[SerializeField]
	private Pivot m_Pivot;

	[HideInInspector]
	[SerializeField]
	private BoxCollider m_WorldSpaceCollider;

	private bool m_RootHasWorldTransform;

	public PanelSettings panelSettings
	{
		get
		{
			return m_PanelSettings;
		}
		set
		{
			if (parentUI == null)
			{
				if (m_PanelSettings == value)
				{
					m_PreviousPanelSettings = m_PanelSettings;
					return;
				}
				if (m_PanelSettings != null)
				{
					m_PanelSettings.DetachUIDocument(this);
				}
				m_PanelSettings = value;
				if (m_PanelSettings != null)
				{
					m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
				}
			}
			else
			{
				Assert.AreEqual(parentUI.m_PanelSettings, value);
				m_PanelSettings = parentUI.m_PanelSettings;
			}
			if (m_ChildrenContent != null)
			{
				foreach (UIDocument attachedUIDocument in m_ChildrenContent.m_AttachedUIDocuments)
				{
					attachedUIDocument.panelSettings = m_PanelSettings;
				}
			}
			m_PreviousPanelSettings = m_PanelSettings;
		}
	}

	public UIDocument parentUI
	{
		get
		{
			return m_ParentUI;
		}
		private set
		{
			m_ParentUI = value;
		}
	}

	public VisualTreeAsset visualTreeAsset
	{
		get
		{
			return sourceAsset;
		}
		set
		{
			sourceAsset = value;
			RecreateUI();
		}
	}

	public VisualElement rootVisualElement
	{
		get
		{
			return m_RootVisualElement;
		}
		private set
		{
			m_RootVisualElement = (UIDocumentRootElement)value;
			focusRing = ((value != null) ? new VisualElementFocusRing(value) : null);
		}
	}

	internal VisualElementFocusRing focusRing { get; private set; } = null;

	internal int firstChildInserIndex => m_FirstChildInsertIndex;

	public Position position
	{
		get
		{
			return m_Position;
		}
		set
		{
			if (m_Position != value)
			{
				m_Position = value;
				SetupPosition();
			}
		}
	}

	public WorldSpaceSizeMode worldSpaceSizeMode
	{
		get
		{
			return m_WorldSpaceSizeMode;
		}
		set
		{
			if (m_WorldSpaceSizeMode != value)
			{
				m_WorldSpaceSizeMode = value;
				SetupWorldSpaceSize();
			}
		}
	}

	public Vector2 worldSpaceSize
	{
		get
		{
			return new Vector2(m_WorldSpaceWidth, m_WorldSpaceHeight);
		}
		set
		{
			if (m_WorldSpaceWidth != value.x || m_WorldSpaceHeight != value.y)
			{
				m_WorldSpaceWidth = value.x;
				m_WorldSpaceHeight = value.y;
				SetupWorldSpaceSize();
			}
		}
	}

	private bool isWorldSpace => m_PanelSettings != null && m_PanelSettings.renderMode == PanelRenderMode.WorldSpace;

	internal bool isTransformControlledByGameObject => isWorldSpace && (m_ParentUI == null || m_Position == Position.Absolute);

	public PivotReferenceSize pivotReferenceSize
	{
		get
		{
			return m_PivotReferenceSize;
		}
		set
		{
			m_PivotReferenceSize = value;
		}
	}

	public Pivot pivot
	{
		get
		{
			return m_Pivot;
		}
		set
		{
			m_Pivot = value;
		}
	}

	public float sortingOrder
	{
		get
		{
			return m_SortingOrder;
		}
		set
		{
			if (m_SortingOrder != value)
			{
				m_SortingOrder = value;
				ApplySortingOrder();
			}
		}
	}

	public IRuntimePanel runtimePanel => containerPanel;

	internal RuntimePanel containerPanel => (RuntimePanel)(rootVisualElement?.elementPanel);

	private float pixelsPerUnit => containerPanel?.pixelsPerUnit ?? 1f;

	internal void ApplySortingOrder()
	{
		AddRootVisualElementToTree();
	}

	internal static UIDocument FindRootUIDocument(VisualElement element)
	{
		UIDocument uIDocument = element.GetFirstOfType<UIDocumentRootElement>()?.document;
		while (uIDocument?.parentUI != null)
		{
			uIDocument = uIDocument.parentUI;
		}
		return uIDocument;
	}

	private UIDocument()
	{
		m_UIDocumentCreationIndex = s_CurrentUIDocumentCounter++;
	}

	private void Awake()
	{
		SetupFromHierarchy();
	}

	private void OnEnable()
	{
		_Enable();
		EnabledDocumentCount++;
	}

	private void _Enable()
	{
		if (parentUI != null && m_PanelSettings == null)
		{
			m_PanelSettings = parentUI.m_PanelSettings;
		}
		if (m_RootVisualElement == null)
		{
			RecreateUI();
		}
		else
		{
			AddRootVisualElementToTree();
		}
		if (TryGetComponent<UIRenderer>(out var component))
		{
			component.enabled = true;
		}
	}

	internal void LateUpdate()
	{
		if (m_RootVisualElement == null || panelSettings == null || panelSettings.panel == null)
		{
			RemoveWorldSpaceCollider();
			return;
		}
		AddOrRemoveRendererComponent();
		if (isWorldSpace)
		{
			if (isTransformControlledByGameObject)
			{
				SetTransform();
			}
			else
			{
				ClearTransform();
			}
			UpdateRenderer();
			if (panelSettings.colliderUpdateMode != ColliderUpdateMode.Keep)
			{
				UpdateWorldSpaceCollider(panelSettings.colliderUpdateMode);
			}
		}
		else
		{
			RemoveWorldSpaceCollider();
			if (m_RootHasWorldTransform)
			{
				ClearTransform();
			}
		}
		UpdateIsWorldSpaceRootFlag();
	}

	private void UpdateRenderer()
	{
		if (!TryGetComponent<UIRenderer>(out var component))
		{
			m_RootVisualElement.uiRenderer = null;
			return;
		}
		m_RootVisualElement.uiRenderer = component;
		component.skipRendering = parentUI != null || pixelsPerUnit < Mathf.Epsilon;
		BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)m_RootVisualElement.panel;
		if (baseRuntimePanel != null)
		{
			Debug.Assert(baseRuntimePanel.drawsInCameras);
			Bounds bounds = SanitizeRendererBounds(rootVisualElement.localBounds3D);
			Matrix4x4 matrix = TransformToGameObjectMatrix();
			VisualElement.TransformAlignedBounds(ref matrix, ref bounds);
			component.localBounds = bounds;
			UpdateIsWorldSpaceRootFlag();
		}
	}

	private Bounds SanitizeRendererBounds(Bounds b)
	{
		if (float.IsNaN(b.size.x) || float.IsNaN(b.size.y) || float.IsNaN(b.size.z))
		{
			b = new Bounds(Vector3.zero, Vector3.zero);
		}
		if (b.size.x < 0f || b.size.y < 0f)
		{
			b.size = Vector3.zero;
		}
		return b;
	}

	private void AddOrRemoveRendererComponent()
	{
		TryGetComponent<UIRenderer>(out var component);
		if (isWorldSpace)
		{
			if (component == null)
			{
				base.gameObject.AddComponent<UIRenderer>();
			}
		}
		else
		{
			UIRUtility.Destroy(component);
		}
	}

	internal void UpdateWorldSpaceCollider(ColliderUpdateMode mode)
	{
		if (parentUI != null)
		{
			return;
		}
		if (containerPanel == null)
		{
			RemoveWorldSpaceCollider();
			return;
		}
		Bounds b;
		if (mode == ColliderUpdateMode.MatchBoundingBox)
		{
			b = WorldSpaceInput.GetPicking3DWorldBounds(rootVisualElement);
		}
		else
		{
			Rect worldBound = rootVisualElement.worldBound;
			b = new Bounds(worldBound.center, worldBound.size);
		}
		if (!IsValidBounds(in b))
		{
			RemoveWorldSpaceCollider();
			return;
		}
		if (m_WorldSpaceCollider == null)
		{
			m_WorldSpaceCollider = base.gameObject.AddComponent<BoxCollider>();
			m_WorldSpaceCollider.isTrigger = panelSettings.colliderIsTrigger;
		}
		if (b.center != m_WorldSpaceCollider.center || b.size != m_WorldSpaceCollider.size)
		{
			m_WorldSpaceCollider.center = b.center;
			m_WorldSpaceCollider.size = b.size;
		}
	}

	internal void RemoveWorldSpaceCollider()
	{
		UIRUtility.Destroy(m_WorldSpaceCollider);
		m_WorldSpaceCollider = null;
	}

	private static bool IsValidBounds(in Bounds b)
	{
		Vector3 extents = b.extents;
		int num = ((extents.x > 0f) ? 1 : 0) + ((extents.y > 0f) ? 1 : 0) + ((extents.z > 0f) ? 1 : 0);
		return num >= 2;
	}

	private void UpdateIsWorldSpaceRootFlag()
	{
		bool flag = !panelSettings.panel.isFlat && parentUI == null;
		if (m_RootVisualElement.isWorldSpaceRootUIDocument != flag)
		{
			m_RootVisualElement.isWorldSpaceRootUIDocument = flag;
			m_RootVisualElement.MarkDirtyRepaint();
		}
	}

	private void SetTransform()
	{
		ComputeTransform(base.transform, out var matrix);
		m_RootVisualElement.style.transformOrigin = new TransformOrigin(Vector3.zero);
		m_RootVisualElement.style.translate = new Translate(matrix.GetPosition());
		m_RootVisualElement.style.rotate = new Rotate(matrix.rotation);
		m_RootVisualElement.style.scale = new Scale(matrix.lossyScale);
		m_RootHasWorldTransform = true;
	}

	private void ClearTransform()
	{
		m_RootVisualElement.style.transformOrigin = StyleKeyword.Null;
		m_RootVisualElement.style.translate = StyleKeyword.Null;
		m_RootVisualElement.style.rotate = StyleKeyword.Null;
		m_RootVisualElement.style.scale = StyleKeyword.Null;
		m_RootHasWorldTransform = false;
	}

	private Matrix4x4 ScaleAndFlipMatrix()
	{
		float num = pixelsPerUnit;
		if (num < Mathf.Epsilon)
		{
			return Matrix4x4.identity;
		}
		float num2 = 1f / num;
		Vector3 s = Vector3.one * num2;
		Quaternion q = Quaternion.AngleAxis(180f, Vector3.right);
		return Matrix4x4.TRS(Vector3.zero, q, s);
	}

	private Bounds LocalBoundsFromPivotSource()
	{
		Bounds localBounds3DWithoutNested3D = m_RootVisualElement.localBounds3DWithoutNested3D;
		Bounds b;
		if (m_PivotReferenceSize == PivotReferenceSize.BoundingBox)
		{
			b = localBounds3DWithoutNested3D;
		}
		else
		{
			Rect layout = m_RootVisualElement.layout;
			Vector2 center = layout.center;
			Vector2 size = layout.size;
			float z = localBounds3DWithoutNested3D.size.z;
			b = new Bounds(new Vector3(center.x, center.y, localBounds3DWithoutNested3D.min.z + z * 0.5f), new Vector3(size.x, size.y, z));
		}
		return SanitizeRendererBounds(b);
	}

	private Vector2 PivotOffset()
	{
		Vector2 pivotAsPercent = GetPivotAsPercent(m_Pivot);
		Bounds bounds = LocalBoundsFromPivotSource();
		return -(Vector2)bounds.min + new Vector2((0f - bounds.size.x) * pivotAsPercent.x, (0f - bounds.size.y) * pivotAsPercent.y);
	}

	private Matrix4x4 TransformToGameObjectMatrix()
	{
		Matrix4x4 m = ScaleAndFlipMatrix();
		MathUtils.PostApply2DOffset(ref m, PivotOffset());
		return m;
	}

	private void ComputeTransform(Transform transform, out Matrix4x4 matrix)
	{
		if (parentUI == null)
		{
			matrix = TransformToGameObjectMatrix();
			return;
		}
		Matrix4x4 matrix4x = parentUI.ScaleAndFlipMatrix();
		Matrix4x4 inverse = matrix4x.inverse;
		Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
		Matrix4x4 worldToLocalMatrix = parentUI.transform.worldToLocalMatrix;
		matrix = inverse * worldToLocalMatrix * localToWorldMatrix * matrix4x;
		MathUtils.PreApply2DOffset(ref matrix, -parentUI.PivotOffset());
		MathUtils.PostApply2DOffset(ref matrix, PivotOffset());
	}

	private static Vector2 GetPivotAsPercent(Pivot origin)
	{
		return origin switch
		{
			Pivot.Center => new Vector2(0.5f, 0.5f), 
			Pivot.TopLeft => new Vector2(0f, 0f), 
			Pivot.TopCenter => new Vector2(0.5f, 0f), 
			Pivot.TopRight => new Vector2(1f, 0f), 
			Pivot.LeftCenter => new Vector2(0f, 0.5f), 
			Pivot.RightCenter => new Vector2(1f, 0.5f), 
			Pivot.BottomLeft => new Vector2(0f, 1f), 
			Pivot.BottomCenter => new Vector2(0.5f, 1f), 
			Pivot.BottomRight => new Vector2(1f, 1f), 
			_ => new Vector2(0.5f, 0.5f), 
		};
	}

	private void SetupFromHierarchy()
	{
		if (parentUI != null)
		{
			parentUI.RemoveChild(this);
		}
		parentUI = FindUIDocumentParent();
	}

	private UIDocument FindUIDocumentParent()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if (parent != null)
		{
			UIDocument[] componentsInParent = parent.GetComponentsInParent<UIDocument>(includeInactive: true);
			if (componentsInParent != null && componentsInParent.Length != 0)
			{
				return componentsInParent[0];
			}
		}
		return null;
	}

	internal void Reset()
	{
		if (parentUI == null)
		{
			m_PreviousPanelSettings?.DetachUIDocument(this);
			panelSettings = null;
		}
		SetupFromHierarchy();
		if (parentUI != null)
		{
			m_PanelSettings = parentUI.m_PanelSettings;
			AddRootVisualElementToTree();
		}
		else if (m_PanelSettings != null)
		{
			AddRootVisualElementToTree();
		}
	}

	internal void AddChildAndInsertContentToVisualTree(UIDocument child)
	{
		if (m_ChildrenContent == null)
		{
			m_ChildrenContent = new UIDocumentList();
		}
		else
		{
			m_ChildrenContent.RemoveFromListAndFromVisualTree(child);
		}
		bool ignoreContentContainer = child.position == Position.Absolute;
		m_ChildrenContent.AddToListAndToVisualTree(child, m_RootVisualElement, ignoreContentContainer, m_FirstChildInsertIndex);
	}

	private void RemoveChild(UIDocument child)
	{
		m_ChildrenContent?.RemoveFromListAndFromVisualTree(child);
	}

	private void RecreateUI()
	{
		if (m_RootVisualElement != null)
		{
			RemoveFromHierarchy();
			rootVisualElement = null;
		}
		if (sourceAsset != null)
		{
			rootVisualElement = new UIDocumentRootElement(this, sourceAsset);
			try
			{
				sourceAsset.CloneTree(m_RootVisualElement);
			}
			catch (Exception exception)
			{
				Debug.LogError("The UXML file set for the UIDocument could not be cloned.");
				Debug.LogException(exception);
			}
		}
		if (m_RootVisualElement == null)
		{
			rootVisualElement = new UIDocumentRootElement(this, null)
			{
				name = base.gameObject.name + "-container"
			};
		}
		else
		{
			m_RootVisualElement.name = base.gameObject.name + "-container";
		}
		m_RootVisualElement.pickingMode = PickingMode.Ignore;
		if (base.isActiveAndEnabled)
		{
			AddRootVisualElementToTree();
		}
		m_FirstChildInsertIndex = m_RootVisualElement.childCount;
		if (m_ChildrenContent != null)
		{
			if (m_ChildrenContentCopy == null)
			{
				m_ChildrenContentCopy = new List<UIDocument>(m_ChildrenContent.m_AttachedUIDocuments);
			}
			else
			{
				m_ChildrenContentCopy.AddRange(m_ChildrenContent.m_AttachedUIDocuments);
			}
			foreach (UIDocument item in m_ChildrenContentCopy)
			{
				if (item.isActiveAndEnabled)
				{
					if (item.m_RootVisualElement == null)
					{
						item.RecreateUI();
					}
					else
					{
						AddChildAndInsertContentToVisualTree(item);
					}
				}
			}
			m_ChildrenContentCopy.Clear();
		}
		SetupRootClassList();
	}

	internal void SetupPosition()
	{
		if (m_RootVisualElement != null && !(m_ParentUI == null))
		{
			if (isTransformControlledByGameObject)
			{
				m_RootVisualElement.style.position = Position.Absolute;
			}
			else
			{
				m_RootVisualElement.style.position = m_Position;
			}
			m_ParentUI.AddChildAndInsertContentToVisualTree(this);
		}
	}

	private void SetupRootClassList()
	{
		if (m_RootVisualElement != null)
		{
			if (!isWorldSpace)
			{
				m_RootVisualElement.EnableInClassList("unity-ui-document__root", parentUI == null);
				m_RootVisualElement.style.position = StyleKeyword.Null;
				m_RootVisualElement.style.width = StyleKeyword.Null;
				m_RootVisualElement.style.height = StyleKeyword.Null;
			}
			else
			{
				SetupWorldSpaceSize();
			}
			SetupPosition();
		}
	}

	private void SetupWorldSpaceSize()
	{
		if (m_RootVisualElement != null)
		{
			if (!isTransformControlledByGameObject)
			{
				m_RootVisualElement.style.width = StyleKeyword.Null;
				m_RootVisualElement.style.height = StyleKeyword.Null;
			}
			else if (m_WorldSpaceSizeMode == WorldSpaceSizeMode.Fixed)
			{
				m_RootVisualElement.style.position = Position.Absolute;
				m_RootVisualElement.style.width = m_WorldSpaceWidth;
				m_RootVisualElement.style.height = m_WorldSpaceHeight;
			}
			else
			{
				m_RootVisualElement.style.position = Position.Absolute;
				m_RootVisualElement.style.width = StyleKeyword.Null;
				m_RootVisualElement.style.height = StyleKeyword.Null;
			}
		}
	}

	private void AddRootVisualElementToTree()
	{
		if (base.enabled)
		{
			if (parentUI != null)
			{
				parentUI.AddChildAndInsertContentToVisualTree(this);
			}
			else if (m_PanelSettings != null)
			{
				m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
			}
		}
	}

	private void RemoveFromHierarchy()
	{
		if (parentUI != null)
		{
			parentUI.RemoveChild(this);
		}
		else if (m_PanelSettings != null)
		{
			m_PanelSettings.DetachUIDocument(this);
		}
	}

	private void OnDisable()
	{
		EnabledDocumentCount--;
		PointerDeviceState.RemoveDocumentData(this);
		RemoveWorldSpaceCollider();
		if (m_RootVisualElement != null)
		{
			RemoveFromHierarchy();
			rootVisualElement = null;
		}
		if (TryGetComponent<UIRenderer>(out var component))
		{
			component.enabled = false;
		}
	}

	private void OnTransformChildrenChanged()
	{
		if (m_ChildrenContent == null)
		{
			return;
		}
		if (m_ChildrenContentCopy == null)
		{
			m_ChildrenContentCopy = new List<UIDocument>(m_ChildrenContent.m_AttachedUIDocuments);
		}
		else
		{
			m_ChildrenContentCopy.AddRange(m_ChildrenContent.m_AttachedUIDocuments);
		}
		foreach (UIDocument item in m_ChildrenContentCopy)
		{
			item.ReactToHierarchyChanged();
		}
		m_ChildrenContentCopy.Clear();
	}

	private void OnTransformParentChanged()
	{
		ReactToHierarchyChanged();
	}

	internal void ReactToHierarchyChanged()
	{
		SetupFromHierarchy();
		if (parentUI != null)
		{
			panelSettings = parentUI.m_PanelSettings;
		}
		m_RootVisualElement?.RemoveFromHierarchy();
		AddRootVisualElementToTree();
		SetupRootClassList();
	}
}
