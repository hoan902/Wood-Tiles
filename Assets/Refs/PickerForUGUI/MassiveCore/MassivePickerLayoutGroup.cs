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

	[ExecuteInEditMode, RequireComponent (typeof(RectTransform)), AddComponentMenu("UI/Picker/MassivePickerLayoutGroup",1006), DisallowMultipleComponent]
	public class MassivePickerLayoutGroup : UIBehaviour, ILayoutGroup
	{

		protected Vector2	m_ItemSize
		{
			get
			{
				if( scrollRect == null )
				{
					return Vector2.zero;
				}

				return scrollRect.itemSize;
			}
		}

		public int	itemCount
		{
			get
			{
				if( scrollRect == null )
				{
					return 0;
				}

				return scrollRect.itemCount; 
			}
		}

		protected DrivenRectTransformTracker m_Tracker;

		public void SetLayoutHorizontal()
		{
			SetLayout(0);
		}

		public void SetLayoutVertical()
		{
			SetLayout(1);
		}

		[SerializeField] protected float m_Spacing = 0;
		public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }
		
		[SerializeField] protected float m_ChildPivot = 0.5f;
		public float childPivot{ get{ return m_ChildPivot; } set{ SetProperty(ref m_ChildPivot, value); } }

		public void UpdateItemContent( int itemIndex )
		{
			if( itemIndex < 0 || itemCount <= itemIndex )
			{
				throw new System.ArgumentOutOfRangeException("index " + itemIndex + " itemCount " + itemCount);
			}

			MassivePickerItem item;

			if( m_Items.TryGetValue(itemIndex, out item) )
			{
				item.SetItemContents(scrollRect,itemIndex);
			}
		}

		public void UpdateAllItemContent()
		{
			foreach( var kv in m_Items )
			{
				int index = kv.Key;
				MassivePickerItem item = kv.Value;

				if( item != null )
				{
					item.SetItemContents(scrollRect,index);
				}
			}
		}

		protected void SetLayout( int axis )
		{
			if( scrollRect == null || scrollRect.layout != (RectTransform.Axis)axis || !Application.isPlaying )
			{
				return;
			}

			int itemCount = this.itemCount;

			if( itemCount <= 0 || scrollRect.itemSource == null )
			{
				foreach( MassivePickerItem item in m_Items.Values )
				{
					ReleaseItemObject( item );
				}

				m_Items.Clear();
				return;
			}

			if( m_ChangedItemSource )
			{
				ClearAlreadyItems();
				m_ChangedItemSource = false;
			}

			RectTransform rectTransform = this.rectTransform;

			Vector2 position = rectTransform.anchoredPosition;
			float itemSize = m_ItemSize[axis];
			float spacing = this.spacing;
			
			Vector2 windowSize = scrollRect.windowSize;

			
			bool wheel = scrollRect.wheelEffect;
			float direction = ( axis == (int)RectTransform.Axis.Horizontal ? 1f : -1f );

			Vector2 areaSize = rectTransform.sizeDelta;
			areaSize[axis] = scrollRect.windowSize[axis] + ( itemSize + spacing ) * (itemCount - 1);
			if( rectTransform.sizeDelta != areaSize ) rectTransform.sizeDelta = areaSize;

			int beginIndex = 0, endIndex = itemCount-1;

			if( itemCount >= 3 )
			{
				float range = !wheel ? windowSize[axis] * 0.5f : windowSize[axis] * 0.25f * Mathf.PI;
				int centerIndex = PositionToNearItemIndex( -position[axis] );
				int indexRange = Mathf.CeilToInt(range / (itemSize + spacing));
				indexRange = Mathf.Min( indexRange, itemCount / 2 );

				if( indexRange+indexRange+1 >= itemCount )
				{
					beginIndex = 0;
					endIndex = itemCount - 1;
				}
				else
				{
					beginIndex = (centerIndex - indexRange + itemCount) % itemCount;
					endIndex = (centerIndex + indexRange + itemCount) % itemCount;
				}
			}
			else
			{
				beginIndex = 0;
				endIndex = itemCount - 1;
			}

			ShrinkItem( beginIndex, endIndex );

			float scrollOffset = position[axis];
			float pivot = m_ChildPivot - 0.5f;

			if( !wheel )
			{
				while(true)
				{
					MassivePickerItem item = GetItem(beginIndex);	
					RectTransform itemRect = item.rectTransform;
					Vector2 itemPosition = itemRect.anchoredPosition;

					itemPosition[axis] = -IndexToPosition(beginIndex);
					
					Vector2 center = itemRect.rect.center;
					float itemPivot = itemRect.pivot[1-axis] - 0.5f;
					float itemAnchorMin = itemRect.anchorMin[1-axis];
					float itemAnchorMax = itemRect.anchorMax[1-axis];
					float revision = (itemPivot * (itemAnchorMax - itemAnchorMin) + (itemAnchorMax+ itemAnchorMin - 1) * 0.5f ) * windowSize[1-axis];
					itemPosition[1-axis] = (windowSize[1-axis] - itemRect.rect.size[1-axis]) * pivot - center[1-axis] - revision;

					itemRect.anchoredPosition = itemPosition;
					if( itemRect.localScale != Vector3.one ) itemRect.localScale = Vector3.one;

					if( beginIndex == endIndex ) break;
					beginIndex = (beginIndex + 1) % itemCount;
				}

			}
			else
			{
				float perspective = m_ScrollRect.wheelPerspective;
				float wheelRadius = windowSize[axis] * 0.5f;
				float circumference = (wheelRadius + wheelRadius) * Mathf.PI;
				float offsetToRad = (Mathf.PI + Mathf.PI) / circumference * -direction;

				while(true)
				{
					MassivePickerItem item = GetItem(beginIndex);
					RectTransform itemRect = item.rectTransform;

					float rad = (IndexToPosition(beginIndex) - scrollOffset) * offsetToRad;

					if( Mathf.Abs(rad) < Mathf.PI * 0.5f )
					{
						float scale  = Mathf.Cos(rad);
						float scale2 = (1-perspective) + scale * perspective;

						Vector3 localScale = itemRect.localScale;
						localScale[axis] = scale;
						localScale[1-axis] = scale2;
						itemRect.localScale = localScale;

						Vector2 itemPosition = itemRect.anchoredPosition;
						Vector2 center = itemRect.rect.center;
						itemPosition[axis] =  Mathf.Sin(-rad) * wheelRadius * -direction - scrollOffset - center[axis] * scale;

						float itemPivot = itemRect.pivot[1-axis] - 0.5f;
						float itemAnchorMin = itemRect.anchorMin[1-axis];
						float itemAnchorMax = itemRect.anchorMax[1-axis];
						float revision = (itemPivot * (itemAnchorMax - itemAnchorMin) + (itemAnchorMax+ itemAnchorMin - 1) * 0.5f ) * windowSize[1-axis];
						itemPosition[1-axis] = (windowSize[1-axis] - itemRect.rect.size[1-axis] * scale2) * pivot - center[1-axis] * scale2 - revision;

						itemRect.anchoredPosition = itemPosition;
					}
					else
					{
						Vector3 scale = itemRect.localScale;

						if( scale.x != 0 || scale.y != 0 )
						{
							scale.x = 0;
							scale.y = 0;
							itemRect.localScale = scale;
						}
					}

					if( beginIndex == endIndex ) break;
					beginIndex = (beginIndex + 1) % itemCount;
				}
			}

			if( scrollRect != null && Application.isPlaying )
			{
				scrollRect.SetInitialPosition();
			}
		}

		protected void ShrinkItem( int begin, int end )
		{
			m_RemovedItemIndex.Clear();

			foreach( int i in m_Items.Keys )
			{
				if( i < begin || end < i )
				{
					m_RemovedItemIndex.Add(i);
				}
			}

			foreach( int i in m_RemovedItemIndex )
			{
				ReleaseItemObject( m_Items[i] );
				m_Items.Remove(i);
			}
		}

		List<int> m_RemovedItemIndex = new List<int>();
		Dictionary<int,MassivePickerItem>	m_Items = new Dictionary<int, MassivePickerItem>();

		protected MassivePickerItem GetItem( int index )
		{
			MassivePickerItem item;

			if( m_Items.TryGetValue(index, out item) )
			{
				if( item != null )
				{
					return item;
				}
			}

			item = AcquireItemObject();
			item.SetItemContents( scrollRect, index );
			item.itemIndex = index;
			m_Items[index] = item;
			m_Tracker.Add( this, item.rectTransform, DrivenTransformProperties.All );
			return item;
		}

		Stack<MassivePickerItem>		m_ItemPool = new Stack<MassivePickerItem>();

		protected MassivePickerItem AcquireItemObject()
		{
			if( m_ItemPool.Count > 0 )
			{
				MassivePickerItem item = m_ItemPool.Pop();

				if( item != null )
				{
					return item;
				}
			}

			GameObject itemObject = (GameObject)Instantiate( scrollRect.itemSource );
			itemObject.SetActive(true);
			MassivePickerItem pickerItem = itemObject.GetComponent<MassivePickerItem>();

			RectTransform itemTransform = pickerItem.rectTransform;
			itemTransform.SetParent( rectTransform );
			itemTransform.localPosition = Vector3.zero;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;

			return pickerItem;
		}

		protected void ReleaseItemObject( MassivePickerItem item )
		{
			m_ItemPool.Push(item);
			item.rectTransform.localScale = Vector3.zero;
		}

		protected MassivePickerScrollRect	m_ScrollRect;

		public MassivePickerScrollRect	scrollRect
		{
			get
			{
				if( m_ScrollRect != null )
				{
					return m_ScrollRect;
				}

				return m_ScrollRect = GetComponentInParent<MassivePickerScrollRect>();
			}

			set
			{
				m_ScrollRect = value;
			}
		}

		protected RectTransform	m_RectTransform;

		protected RectTransform rectTransform
		{
			get
			{
				if( m_RectTransform != null )
				{
					return m_RectTransform;
				}

				return m_RectTransform = GetComponent<RectTransform>();
			}
		}

		protected override void OnTransformParentChanged ()
		{
			base.OnTransformParentChanged();
			m_ScrollRect = GetComponentInParent<MassivePickerScrollRect>();
		}

		public float IndexToPosition( int index )
		{
			if( index < 0 || itemCount <= index )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			int axis = (int)scrollRect.layout;
			float itemSize = m_ItemSize[axis];
			float direction = ( axis == (int)RectTransform.Axis.Horizontal ? 1f : -1f );
			float partition = itemSize + spacing;
			float itemPosition = 0f;

			if( scrollRect.infiniteScroll )
			{
				float areaSize = partition * itemCount;
				float position = rectTransform.anchoredPosition[axis];
				float basePosition;

				if( axis == (int)RectTransform.Axis.Horizontal )
				{
					basePosition = Mathf.Floor( position / areaSize);
				}
				else
				{
					basePosition = Mathf.Ceil( position / areaSize);
				}

				basePosition *= areaSize;

				float a = (index * partition - areaSize * 0.5f) * direction - basePosition;
				float b = a - areaSize * direction;
				itemPosition = Mathf.Abs(a+position) < Mathf.Abs(b+position) ? a : b;
			}
			else
			{
				float windowSize = scrollRect.windowSize[axis];
				float areaSize = windowSize + partition * (itemCount - 1);
				itemPosition = ((windowSize - areaSize) * 0.5f  + partition * index)  * direction;
			}


			return -itemPosition;
		}

		public int PositionToNearItemIndex( float position )
		{
			int itemCount = this.itemCount;

			if( itemCount <= 1 )
			{
				return itemCount == 1 ? 0 : -1;
			}

			int axis = GetAxisIndex();
			float itemSize = m_ItemSize[axis];
			float spacing = this.spacing;
			float partition = itemSize + spacing;
			float direction = ( axis == (int)RectTransform.Axis.Horizontal ? 1f : -1f );
			int index;
			
			if( scrollRect.infiniteScroll )
			{
				float areaSize = partition * itemCount;
				float scrollPosition = rectTransform.anchoredPosition[axis];
				float basePosition;
				
				if( axis == (int)RectTransform.Axis.Horizontal )
				{
					basePosition = Mathf.Floor( scrollPosition / areaSize);
				}
				else
				{
					basePosition = Mathf.Ceil( scrollPosition / areaSize);
				}
				
				basePosition *= areaSize;

				float a = position + basePosition;
				float b = a + areaSize * direction;

				if( Mathf.Abs(a) < Mathf.Abs(b) )
				{
					position = a;
				}
				else
				{
					position = b;
				}

				index = Mathf.RoundToInt((position * direction + areaSize * 0.5f) / partition);
				index = index % itemCount;
			}
			else
			{
				float windowSize = scrollRect.windowSize[axis];
				float firstItemOffset = (windowSize - itemSize - spacing) * 0.5f;
				
				float areaSize = windowSize + partition * (itemCount - 1);
				
				index = Mathf.FloorToInt( (position * direction + areaSize * 0.5f - firstItemOffset) / partition );
				index = Mathf.Clamp( index, 0, itemCount-1 );
			}

			return index;
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

		protected int GetAxisIndex()
		{
			return (int)scrollRect.layout;
		}

		protected void SetProperty<T>(ref T currentValue, T newValue)
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
				return;
			currentValue = newValue;
			SetDirty();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}
		
		protected override void OnDisable()
		{
			m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}
		
		protected override void OnDidApplyAnimationProperties()
		{
			SetDirty();
		}
		
		protected void SetDirty()
		{
			if (!IsActive())
				return;
			
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		public void RebuildLayout()
		{
			SetDirty();
		}

		protected bool m_ChangedItemSource = false;

		public void OnChangeItemSource()
		{
			if( m_ChangedItemSource )
			{
				return;
			}

			m_ChangedItemSource = true;
			RebuildLayout();
		}

		protected void ClearAlreadyItems()
		{
			foreach( MassivePickerItem item in m_Items.Values )
			{
				GameObject.Destroy( item.gameObject );
			}
			
			foreach( MassivePickerItem item in m_ItemPool )
			{
				GameObject.Destroy( item.gameObject );
			}
			
			m_Items.Clear();
			m_ItemPool.Clear();
		}
		
#if UNITY_EDITOR

		protected override void Reset()
		{
			m_ScrollRect = GetComponentInParent<MassivePickerScrollRect>();
		}

#endif

	}
}
