using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{

	[ExecuteInEditMode, RequireComponent (typeof(RectTransform)), AddComponentMenu("UI/Picker/PickerLayoutGroup",1001), DisallowMultipleComponent]
	public class PickerLayoutGroup : LayoutGroup
	{
		public void RebuildLayout()
		{
			SetDirty();
		}

		[SerializeField] protected PickerScrollRect m_ScrollRect;
		public PickerScrollRect scrollRect{ get{ return m_ScrollRect; } set{ SetProperty(ref m_ScrollRect, value); } }
				
		[SerializeField] protected float m_Spacing = 0;
		public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

		[SerializeField] protected float m_ChildPivot = 0.5f;
		public float childPivot{ get{ return m_ChildPivot; } set{ SetProperty( ref m_ChildPivot, value); } }
		
		List<float>	m_itemOffsetList = new List<float>();
		public List<float> itemOffsetList{ get{ return m_itemOffsetList; } }

		List<PickerItem> m_pickerItemList = new List<PickerItem>();
		public List<PickerItem> itemList{ get{ return m_pickerItemList; } }

		List<RectTransform> m_childTransformList = new List<RectTransform>();


		protected int GetAxisIndex()
		{
			return (int)scrollRect.layout;
		}

		public override void CalculateLayoutInputHorizontal()
		{
			CalculateLayoutInput(0);
		}
		
		public override void CalculateLayoutInputVertical()
		{
			CalculateLayoutInput(1);
		}

		protected void CalculateLayoutInput( int axis )
		{	
			float size = GetScrollAreaSize()[axis];
			SetLayoutInputForAxis(size, size, size, axis);
		}
		
		public override void SetLayoutHorizontal()
		{
			SetLayout(0);
		}
		
		public override void SetLayoutVertical()
		{
			SetLayout(1);
		}

		protected virtual void SetContentRectSize( int axis, float scrollSize )
		{
			Vector2 size = Vector2.zero;
			size[axis] = scrollSize;

			if( rectTransform.sizeDelta != size )
			{
				rectTransform.sizeDelta = size;
			}

			Vector2 position = rectTransform.anchoredPosition;

			if( position[1-axis] != 0 )
			{
				rectTransform.anchoredPosition = position;
			}
		}

		protected virtual Vector2 GetScrollAreaSize()
		{
			if( scrollRect == null ) return Vector2.zero;
			return scrollRect.GetComponent<RectTransform>().rect.size;
		}

		protected DrivenTransformProperties GetChildPropertyDriven( int axis, bool wheelEffect )
		{
			DrivenTransformProperties driven = DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY;
			
			if( axis == 0 )
			{
				driven |= DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX;
			}
			else
			{
				driven |= DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY;
			}
			
			if( wheelEffect )
			{
				driven |= DrivenTransformProperties.ScaleX | DrivenTransformProperties.ScaleY;
			}

			return driven;
		}

		protected bool m_LockSetLayout = false;
		protected bool m_DirtyLayout = false;

		public void LockSetLayout()
		{
			m_LockSetLayout = true;
			m_DirtyLayout = false;
		}

		public void UnlockSetLayoutAndUpdateIfDirty()
		{
			if( !m_LockSetLayout )
			{
				return;
			}

			m_LockSetLayout = false;

			if( m_DirtyLayout )
			{
				m_DirtyLayout = false;
				SetLayout( GetAxisIndex() );
			}
		}

		static List<Rect> cacheRect = new List<Rect>();

		protected virtual void SetLayout( int axis )
		{
			if( scrollRect == null )
			{
				return;
			}

			if( axis != GetAxisIndex() )
			{
				return;
			}

			if( m_LockSetLayout )
			{
				m_DirtyLayout = true;
				return;
			}

			Vector2 scrollAreaSize = GetScrollAreaSize();

			m_Tracker.Clear();
			rectChildren.Clear();
			m_itemOffsetList.Clear();
			m_pickerItemList.Clear();
			m_childTransformList.Clear();
			cacheRect.Clear();

			bool infinite = m_ScrollRect.infiniteScroll;
			bool wheelEffect = m_ScrollRect.wheelEffect;

			DrivenTransformProperties childDriven = GetChildPropertyDriven(axis,wheelEffect);
			float position = ( !infinite ? scrollAreaSize[axis] * 0.5f : 0 );
			float scrollSize = position;
			
			for (int childIndex = 0; childIndex < rectTransform.childCount; childIndex++)
			{
				Transform childTransform = rectTransform.GetChild(childIndex);
				RectTransform childRectTransform;
				ItemComponent itemComponent;
				PickerItem item;

				if( !m_ChildItemTable.TryGetValue(childTransform,out itemComponent) )
				{
					childRectTransform = childTransform as RectTransform;
					item = childRectTransform.GetComponent<PickerItem>();
					m_ChildItemTable[childTransform] = new ItemComponent(){ item = item, rectTransform = childRectTransform };
				}
				else
				{
					childRectTransform = itemComponent.rectTransform;
					item = itemComponent.item;
				}

				if( item == null || !item.enabled || !childRectTransform.gameObject.activeInHierarchy )
				{
					continue;
				}

				rectChildren.Add(childRectTransform);

				m_pickerItemList.Add(item);
				m_itemOffsetList.Add(0);
				m_childTransformList.Add(childRectTransform);

				m_Tracker.Add(this, childRectTransform, childDriven );

				{
					Vector2 anchorMin = childRectTransform.anchorMin;

					if( anchorMin[axis] != 0.5f )
					{
						anchorMin[axis] = 0.5f;
						childRectTransform.anchorMin = anchorMin;
					}

					Vector2 anchorMax = childRectTransform.anchorMax;

					if( anchorMax[axis] != 0.5f )
					{
						anchorMax[axis] = 0.5f;
						childRectTransform.anchorMax = anchorMax;
					}
				}

				Rect rect = childRectTransform.rect;
				cacheRect.Add( rect );
				float size = rect.size[axis];
				scrollSize += spacing + size;
			}

			if( m_childTransformList.Count > 0 && !infinite )
			{
				float size = m_childTransformList[0].rect.size[axis];
				position -= size * 0.5f;
				scrollSize -= size * 0.5f;

				size = m_childTransformList[m_childTransformList.Count-1].rect.size[axis];
				scrollSize -= size * 0.5f;

				scrollSize -= spacing;
			}

			if( !infinite )
			{
				float flex = Mathf.Min(scrollSize * 0.002f,0.1f);
				scrollSize += scrollAreaSize[axis] * 0.5f + flex + flex;
				position += flex;
			}

			SetContentRectSize( axis, scrollSize );

			position += scrollSize * -0.5f;

			float pivot = m_ChildPivot - 0.5f;
			float direction = ( axis == 0 ? 1f : -1f );
			float wheelPosition = scrollRect.content.localPosition[axis] * -direction;

			float infiniteOffsetFloor = 0;

			if( infinite )
			{
				infiniteOffsetFloor = Mathf.Floor(wheelPosition / scrollSize) * scrollSize;
			}

			if( wheelEffect )
			{
				float perspective = m_ScrollRect.wheelPerspective;
				float wheelRadius = scrollAreaSize[axis] * 0.5f;
				float circumference = (wheelRadius + wheelRadius) * Mathf.PI;
				float ru = (Mathf.PI + Mathf.PI) / circumference * direction;

				for( int childIndex = 0; childIndex < m_childTransformList.Count; ++childIndex )
				{
					RectTransform childTransform = m_childTransformList[childIndex];

					float size = cacheRect[childIndex].size[axis];
					
					float tmp;

					if( !infinite )
					{
						tmp = position + size * 0.5f;
					}
					else
					{
						tmp = infiniteOffsetFloor + position + size * 0.5f;
						
						if( Mathf.Abs(tmp - wheelPosition) > Mathf.Abs(tmp + scrollSize - wheelPosition) )
						{
							tmp += scrollSize;
						}
					}

					position += size + spacing;
					float rad = (tmp - wheelPosition) * ru;

					float offset = tmp * direction;
					m_itemOffsetList[childIndex] = offset;
					PickerItem item = m_pickerItemList[childIndex];
					item.position = -offset;

					if( Mathf.Abs(rad) < Mathf.PI * 0.5f )
					{
						float scale = Mathf.Cos (rad);
						Vector3 localScale = childTransform.localScale;
				     	localScale[axis] = scale;

						float scale2 = (1-perspective) + scale * perspective;
						localScale[1-axis] = scale2;
                        localScale.x = localScale.y;       //DatPT
						childTransform.localScale = localScale;

						Vector2 center = cacheRect[childIndex].center;
						Vector2 childPosition = Vector2.zero;
						childPosition[axis] = Mathf.Sin(rad) * wheelRadius + wheelPosition * direction - center[axis] * scale;

						float childPivot = childTransform.pivot[1-axis] - 0.5f;
						float childAnchorMin = childTransform.anchorMin[1-axis];
						float childAnchorMax = childTransform.anchorMax[1-axis];
						float revision = (childPivot * (childAnchorMax - childAnchorMin) + (childAnchorMax + childAnchorMin - 1) * 0.5f ) * scrollAreaSize[1-axis];
						childPosition[1-axis] = (scrollAreaSize[1-axis] - cacheRect[childIndex].size[1-axis] * scale2) * pivot - center[1-axis] * scale2 - revision;

						childTransform.anchoredPosition = childPosition ;
                     //   Debug.Log(childTransform.gameObject.name + childTransform.anchoredPosition);
					}
					else
					{
						Vector3 scale = childTransform.localScale;

						if( scale.x != 0 || scale.y != 0 )
						{
							scale.x = 0;
							scale.y = 0;
							childTransform.localScale = scale;
						}
					}
				}
			}
			else
			{
				for( int childIndex = 0; childIndex < m_childTransformList.Count; ++childIndex )
				{
					RectTransform childTransform = m_childTransformList[childIndex];
					PickerItem item = m_pickerItemList[childIndex];
					
					Rect rect = childTransform.rect;
					float size = rect.size[axis];
					float tmp;

					if( !infinite )
					{
						tmp = position + size * 0.5f;
					}
					else
					{
						tmp = infiniteOffsetFloor + position + size * 0.5f;

						if( Mathf.Abs(tmp - wheelPosition) > Mathf.Abs(tmp + scrollSize - wheelPosition) )
						{
							tmp += scrollSize;
						}
					}

					float offset = tmp * direction;
					m_itemOffsetList[childIndex] = offset;
					item.position = -offset;
					position += size + spacing;

					Vector2 childPosition = childTransform.anchoredPosition;
					childPosition[axis] = tmp * direction - rect.center[axis];

					float childPivot = childTransform.pivot[1-axis] - 0.5f;
					float childAnchorMin = childTransform.anchorMin[1-axis];
					float childAnchorMax = childTransform.anchorMax[1-axis];
					float revision = (childPivot * (childAnchorMax - childAnchorMin) + (childAnchorMax + childAnchorMin - 1) * 0.5f ) * scrollAreaSize[1-axis];

					childPosition[1-axis] = (scrollAreaSize[1-axis] - rect.size[1-axis]) * pivot - rect.center[1-axis] - revision;
					childTransform.anchoredPosition = childPosition;
					
					childTransform.localScale = Vector3.one;
				}
			}

			if( direction < 0 )
			{
				m_itemOffsetList.Reverse();
				m_pickerItemList.Reverse();
			}

			int infiniteScrollOffset = 0;

			if( infinite )
			{
				//sort offset & item
				int i;
				int count = m_itemOffsetList.Count;

				if( count > 1 )
				{
					for( i = 1; i < count; ++i )
					{
						if( m_itemOffsetList[i-1] >= m_itemOffsetList[i] )
						{
							break;
						} 
					}

					infiniteScrollOffset = i;

					if( i < count )
					{
						if( swapBufferOffset == null || i > swapBufferOffset.Length )
						{
							swapBufferOffset = new float[i];
							swapBufferItem = new PickerItem[i];
						}

						m_itemOffsetList.CopyTo( 0, swapBufferOffset, 0, i );
						m_pickerItemList.CopyTo( 0, swapBufferItem, 0, i );

						int j;

						for( j = 0; j+i < count; ++j )
						{
							m_itemOffsetList[j] = m_itemOffsetList[j+i];
							m_pickerItemList[j] = m_pickerItemList[j+i];
						}

						for( int k = 0; k+j < count; ++k )
						{
							m_itemOffsetList[k+j] = swapBufferOffset[k];
							m_pickerItemList[k+j] = swapBufferItem[k];
						}
					}
				}
			}

			if( scrollRect != null && Application.isPlaying )
			{
				scrollRect.SetInitialPosition(infiniteScrollOffset);
			}
		}

		static float[]		swapBufferOffset;
		static PickerItem[]	swapBufferItem;

		protected override void OnRectTransformDimensionsChange ()
		{
			base.OnRectTransformDimensionsChange ();

#if UNITY_EDITOR
			if( !Application.isPlaying && scrollRect != null )
			{
				SetLayout(GetAxisIndex());
			}
#endif
		}

		protected struct ItemComponent
		{
			public PickerItem		item;
			public RectTransform	rectTransform;
		}

		protected Dictionary<Transform,ItemComponent>	m_ChildItemTable = new Dictionary<Transform, ItemComponent>();

		public void RegisterItem( PickerItem item )
		{
			if( Application.isPlaying )
			{
				Transform key = item.transform;

				if( !m_ChildItemTable.ContainsKey(key) )
				{
					m_ChildItemTable[key] = new ItemComponent(){ item = item, rectTransform = item.GetComponent<RectTransform>() };
				}
			}
		}

		public void UnregisterItem( PickerItem item )
		{
			if( Application.isPlaying )
			{
				m_ChildItemTable.Remove(item.transform);
			}
		}

		protected override void Awake ()
		{
			base.Awake();
		}
	}

	static public class Util
	{
		static public Vector2 Assign( this Vector2 vec, float value, int axis )
		{
			vec[axis] = value;
			return vec;
		}

		static public Vector3 Assign( this Vector3 vec, float value, int axis )
		{
			vec[axis] = value;
			return vec;
		}

		static public void DestroyObject( Object obj )
		{
			if( Application.isPlaying )
			{
				Object.Destroy(obj);
			}
			else
			{
				Object.DestroyImmediate(obj);
			}
		}
	}

	public class ComponentListPool
	{
		static Stack<List<Component>> poolObjects = new Stack<List<Component>>();

		public static List<Component> Acquire()
		{
			if( poolObjects.Count > 0 )
			{
				return poolObjects.Pop();
			}

			return new List<Component>();
		}

		public static void Release( List<Component> list )
		{
			poolObjects.Push(list);
		}
	}
}
