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
	
	[RequireComponent(typeof(RectTransform)), ExecuteInEditMode(), AddComponentMenu("UI/Picker/MassivePickerScrollRect",1005), DisallowMultipleComponent]
	public class MassivePickerScrollRect : ScrollRect
	{
		protected RectTransform m_RectTransform;

		public RectTransform rectTransform
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

		[SerializeField]
		protected bool			m_WheelEffect;

		public bool				wheelEffect
		{
			get
			{
				return m_WheelEffect;
			}

			set
			{
				if( m_WheelEffect == value )
				{
					return;
				}

				m_WheelEffect = value;
				UpdateLayout();
			}
		}

		public bool				infiniteScroll
		{
			get
			{
				return movementType == MovementType.Unrestricted;
			}
		}
		
		
		public Vector2 windowSize
		{
			get
			{
				return rectTransform.rect.size;
			}
		}

		[SerializeField]
		protected Vector2		m_ItemSize;

		public Vector2 itemSize
		{
			get
			{
				return m_ItemSize;
			}

			set
			{
				if( m_ItemSize == value )
				{
					return;
				}

				m_ItemSize = value;
				UpdateLayout();
			}
		}

		[SerializeField]
		protected bool			m_DeactiveItemOnAwake = false;

		public bool		deactiveItemOnAwake
		{
			get
			{
				return m_DeactiveItemOnAwake;
			}

			set
			{
				if( m_DeactiveItemOnAwake == value )
				{
					return;
				}

				m_DeactiveItemOnAwake = value;
			}
		}

		[SerializeField]
		protected GameObject	m_ItemSource;

		public GameObject	itemSource
		{
			get
			{
				return m_ItemSource;
			}

			set
			{
				if( m_ItemSource == value )
				{
					return;
				}

				m_ItemSource = value;

				if( pickerLayoutGroup != null )
				{
					pickerLayoutGroup.OnChangeItemSource();
				}
			}
		}

		[SerializeField]
		protected int		m_ItemCount;

		public int	itemCount
		{
			get
			{
				return m_ItemCount;
			}
			
			set
			{
				if( m_ItemCount == value )
				{
					return;
				}
				
				if( value < 0 )
				{
					throw new System.ArgumentOutOfRangeException();
				}
				
				m_ItemCount = value;
				UpdateLayout();
			}
		}

		public int GetSelectedItemIndex()	{ return beforeSelectedItemIndex; }

		public void ScrollAt( int index, bool immdiate = false )
		{
			if( pickerLayoutGroup != null )
			{
				float offset = pickerLayoutGroup.IndexToPosition(index);
				ScrollTo( offset, immdiate );
			}
		}
		
		public void ScrollTo( MassivePickerItem item, bool immdiate = false )
		{
			if( item != null && pickerLayoutGroup != null )
			{
				float position = pickerLayoutGroup.IndexToPosition(item.itemIndex);
				ScrollTo(position, immdiate);
			}
		}
		
		public void ScrollTo( float offset, bool immdiate = false )
		{
			if( content == null )
			{
				return;
			}
			
			int axisIndex = (int)layout;
			Vector3 scrollPosition = content.anchoredPosition;
			m_fromPosition = scrollPosition;
			scrollPosition[axisIndex] = offset;
			scrollPosition[1-axisIndex] = 0;
			
			if( !immdiate )
			{
				m_autoScrollTimeCount = 0;
				velocity = Vector2.zero;
				m_autoScroll = true;
				m_targetPosition = scrollPosition;
			}
			else
			{
				content.anchoredPosition = scrollPosition;
				m_autoScroll = false;
			}
			
			m_slip = false;
			m_dragging = false;
		}
		
		public float GetScrollPosition()
		{
			if( content == null )
			{
				return 0;
			}
			
			return content.anchoredPosition[(int)layout];
		}
		
		public float firstItemPosition
		{
			get
			{
				if( pickerLayoutGroup == null ) return 0f;
				return -pickerLayoutGroup.IndexToPosition(0);
			}
		}
		
		public float lastItemPosition
		{
			get
			{
				if( pickerLayoutGroup == null ) return 0f;
				return -pickerLayoutGroup.IndexToPosition(m_ItemCount-1);
			}
		}
		
		public void ScrollToNearItem( bool immdiate = false )
		{
			if( pickerLayoutGroup == null )
			{
				return;
			}
			
			int index = GetNearItemIndex();
			
			if( index >= 0 )
			{
				ScrollAt(index, immdiate);
			}
		}
		
		public RectTransform.Axis layout
		{
			get
			{
				return horizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			}
			
			set
			{
				bool isHorizontal = (value == RectTransform.Axis.Horizontal);
				
				if( horizontal == isHorizontal && vertical == !isHorizontal )
				{
					return;
				}
				
				if( content != null )
				{
					int index = (int)value;
					Vector2 position = content.anchoredPosition;
					
					if( position[1-index] != 0f )
					{
						content.anchoredPosition = position.Assign(0,1-index);
					}
				}
				
				horizontal = isHorizontal;
				vertical   = !isHorizontal;
				SetContentProperties();
				UpdateLayout();
			}
		}
		
		[SerializeField]
		public float			autoScrollSeconds = 0.65f;
		
		[SerializeField]
		public float 			slipVelocityRate = 0.25f;

		[SerializeField]
		protected float			m_Perspective = 0.25f;
		
		public float			wheelPerspective
		{
			get
			{
				return m_Perspective;
			}
			
			set
			{
				if( m_Perspective == value )
				{
					return;
				}
				
				m_Perspective = value;
				UpdateLayout();
			}
		}
		
		public new MovementType movementType
		{
			get
			{ 
				return base.movementType;
			}
			
			set
			{
				if( base.movementType == value )
				{
					return;
				}
				
				base.movementType = value;
				int index = GetNearItemIndex();
				UpdateLayout();
				
				if( index >= 0 )
				{
					ScrollAt( index, true );
				}
			}
			
		}
		
		[System.Serializable]
		public class ItemSelectEvent : UnityEvent<int> { }
		
		public ItemSelectEvent	onSelectItem = new ItemSelectEvent();
		
		protected int	beforeSelectedItemIndex = -1;
		
		protected DrivenRectTransformTracker 	m_Tracker = new DrivenRectTransformTracker();
		RectTransform							m_BeforeContent = null;

		protected MassivePickerLayoutGroup	m_PickerLayoutGroup;
		
		protected virtual MassivePickerLayoutGroup	pickerLayoutGroup
		{
			get
			{
				if( m_PickerLayoutGroup != null )
				{
					return m_PickerLayoutGroup;
				}
				
				if( content != null )
				{
					m_PickerLayoutGroup = content.GetComponent<MassivePickerLayoutGroup>();
				}
				
				return m_PickerLayoutGroup;
			}
		}
		
		protected RectTransform m_selfRect;
		
		protected RectTransform selfRect
		{
			get
			{
				if( m_selfRect != null )
				{
					return m_selfRect;
				}
				
				return m_selfRect = GetComponent<RectTransform>();
			}
		}

		public InitialPosition	initialPosition = InitialPosition.FirstItem;
		public int				initialPositionItemIndex;
		
		
		protected Vector2 beforeScrollPosition;
		
		protected override void LateUpdate()
		{
			#if UNITY_EDITOR
			if( !Application.isPlaying )
			{
				m_autoScroll = false;
				m_slip = false;
				velocity = Vector2.zero;
			}
			#endif
			
			try
			{
				if( pickerLayoutGroup != null )
				{
					pickerLayoutGroup.LockSetLayout();
				}
				
				if( m_setInitialPosition )
				{
					base.LateUpdate();
				}
				
				AutoScroll();
				
				NotifySelectItem();
				
				if( m_BeforeContent != content )
				{
					m_BeforeContent = content;
					SetContentProperties();
				}

				if( pickerLayoutGroup != null )
				{					
					if( content != null && beforeScrollPosition != content.anchoredPosition )
					{
						pickerLayoutGroup.SetLayoutVertical();
						pickerLayoutGroup.SetLayoutHorizontal();
					}
				}
			}
			finally
			{
				if( pickerLayoutGroup != null ) pickerLayoutGroup.UnlockSetLayoutAndUpdateIfDirty();
			}
			
			if( content != null )
			{
				beforeScrollPosition = content.anchoredPosition;
			}
		}
		
		public void SetInitialPosition()
		{	
			if( m_setInitialPosition || itemCount <= 0 )
			{
				return;
			}
			
			m_setInitialPosition = true;
			
			int targetItemIndex = -1;
			
			switch( initialPosition )
			{
				case InitialPosition.FirstItem:
				{
					targetItemIndex = 0;
					break;
				}
					
				case InitialPosition.MiddleItem:
				{
					targetItemIndex = itemCount / 2;
					break;
				}
					
				case InitialPosition.LastItem:
				{
					targetItemIndex = itemCount - 1;
					break;
				}
					
				case InitialPosition.SelectItemIndex:
				{
					targetItemIndex = Mathf.Clamp( initialPositionItemIndex, 0, itemCount-1 );
					break;
				}
					
				case InitialPosition.NearItem:
				{
					ScrollToNearItem(true);
					onValueChanged.Invoke( normalizedPosition );
					return;
				}
			}
			
			if( infiniteScroll )
			{
				targetItemIndex = (targetItemIndex + itemCount ) % itemCount;
			}

			ScrollAt(targetItemIndex,true);
			NotifySelectItem(targetItemIndex);
			onValueChanged.Invoke( normalizedPosition );
		}
		
		protected virtual void NotifySelectItem()
		{
			if( pickerLayoutGroup == null || content == null )
			{
				return;
			}

			NotifySelectItem(GetNearItemIndex());
		}

		protected void NotifySelectItem( int selectIndex )
		{
			if( selectIndex < 0 || itemCount <= selectIndex )
			{
				return;
			}

			if( beforeSelectedItemIndex != selectIndex )
			{
				beforeSelectedItemIndex = selectIndex;
				onSelectItem.Invoke(selectIndex);
			}
		}
		
		protected bool m_dragging = false;
		
		public override void OnBeginDrag (PointerEventData eventData)
		{
			base.OnBeginDrag (eventData);
			m_dragging = true;
			m_autoScroll = false;
			m_slip = false;
			m_stopAutoScroll = 0f;
		}
		
		public override void OnEndDrag (PointerEventData eventData)
		{
			m_dragging = false;
			base.OnEndDrag (eventData);
			
			if( inertia && Mathf.Abs(velocity[(int)layout]) >= GetSlipVelocity() )
			{
				m_slip = true;
				m_stopAutoScroll = 0f;
			}
			else
			{
				ScrollToNearItem();
			}
		}
		
		protected void SetContentProperties()
		{
			m_Tracker.Clear();
			
			if( content != null )
			{
				m_Tracker.Add(
					this, 
					content, 
					DrivenTransformProperties.Pivot |
					DrivenTransformProperties.Anchors | 
					( layout == RectTransform.Axis.Horizontal ? DrivenTransformProperties.AnchoredPositionY : DrivenTransformProperties.AnchoredPositionX ) | 
					DrivenTransformProperties.Scale |
					DrivenTransformProperties.SizeDelta |
					DrivenTransformProperties.Rotation
					);
				
				if( layout == RectTransform.Axis.Horizontal )
				{
					content.anchorMin = new Vector2(0.5f,0f);
					content.anchorMax = new Vector2(0.5f,1f);
				}
				else
				{
					content.anchorMin = new Vector2(0f,0.5f);
					content.anchorMax = new Vector2(1f,0.5f);
				}
				
				int axisIndex = (int)layout;
				Vector2 size = content.sizeDelta;
				size[axisIndex] = GetComponent<RectTransform>().sizeDelta[axisIndex];
				content.sizeDelta = size;
				content.pivot = new Vector2(0.5f,0.5f);

				content.localScale = Vector3.one;
			}
		}
		
		protected float GetSlipVelocity()
		{
			return slipVelocityRate * selfRect.rect.size[(int)layout];
		}
		
		protected virtual void AutoScroll()
		{
			if( m_stopAutoScroll > 0 )
			{
				m_stopAutoScroll -= Time.deltaTime;
				return;
			}
			
			if( m_autoScroll )
			{
				Vector2 anchoredPosition = content.anchoredPosition;
				
				if( anchoredPosition != m_targetPosition )
				{
					m_autoScrollTimeCount += Time.deltaTime;
					
					if( m_autoScrollTimeCount < autoScrollSeconds )
					{
						//Ease Out Quart
						float t = m_autoScrollTimeCount / autoScrollSeconds -1;
						t *= t;
						t *= t;
						t = t - 1;
						content.anchoredPosition = ( m_fromPosition - m_targetPosition ) * t + m_fromPosition;
					}
					else
					{
						content.anchoredPosition = m_targetPosition;
						velocity = Vector2.zero;
						m_autoScroll = false;
					}
					
					UpdateLayout();
				}
				else
				{
					velocity = Vector2.zero;
					m_autoScroll = false;
				}
			}
			else if( !m_dragging )
			{
				int axisIndex = (int)layout;
				
				if( m_slip )
				{
					float slipVelocity = GetSlipVelocity();
					
					if( Mathf.Abs(velocity[axisIndex]) <= slipVelocity )
					{				
						if( pickerLayoutGroup != null && itemCount > 0 )
						{
							Vector2 scrollPosition = content.anchoredPosition;
							scrollPosition[1-axisIndex] = 0;
							float beforePosition = beforeScrollPosition[axisIndex];
							float currentPosition = scrollPosition[axisIndex];

							int beforeIndex = GetNearItemIndex(-beforePosition);
							int index = GetNearItemIndex(-currentPosition);
							
							if( beforeIndex == index && 0 <= index )
							{
								float itemOffset = pickerLayoutGroup.IndexToPosition(index);
								
								if( (beforePosition - itemOffset) * (currentPosition - itemOffset) <= 0 || beforePosition == currentPosition )
								{

									if( Mathf.Abs(itemOffset-currentPosition) < itemSize[axisIndex] * 0.1f )
									{
										velocity = Vector2.zero;
										m_slip = false;
										content.anchoredPosition = scrollPosition.Assign(itemOffset,axisIndex);
									}
									else
									{
										ScrollTo(itemOffset);
									}
								}
								else if( velocity[axisIndex] * (currentPosition - itemOffset) < 0 || ( index != 0 && index != itemCount-1  ) )
								{
									velocity = velocity.Assign(slipVelocity*Mathf.Sign(velocity[axisIndex]),axisIndex);
								}
							}
							else
							{
								velocity = velocity.Assign(slipVelocity*Mathf.Sign(velocity[axisIndex]),axisIndex);
							}
						}
					}
				}
				else
				{
					if( content != null && pickerLayoutGroup != null )
					{
						int nearItemIndex = GetNearItemIndex();
						float currentPosition = -content.anchoredPosition[axisIndex];
						
						if( 0 <= nearItemIndex && nearItemIndex < itemCount )
						{
							float itemOffset = pickerLayoutGroup.IndexToPosition(nearItemIndex);
							
							if( Mathf.Abs(currentPosition+itemOffset) > content.rect.size[axisIndex] * 0.001f )
							{
								ScrollTo(-itemOffset);
							}
						}
					}
				}
			}
		}

		public void UpdateItemContent( int itemIndex )
		{
			if( pickerLayoutGroup != null )
			{
				pickerLayoutGroup.UpdateItemContent(itemIndex);
			}
		}

		public void UpdateAllItemContent()
		{
			if( pickerLayoutGroup != null )
			{
				pickerLayoutGroup.UpdateAllItemContent();
			}
		}

		protected int GetNearItemIndex()
		{
			Vector2 position = content.anchoredPosition;
			return GetNearItemIndex(-position[(int)layout]);
		}
				
		protected int GetNearItemIndex( float scrollPosition )
		{
			if( pickerLayoutGroup == null )
			{
				return -1;
			}

			return pickerLayoutGroup.PositionToNearItemIndex(scrollPosition);
		}
		
		protected bool		m_autoScroll = false;
		protected bool		m_slip = false;
		protected float		m_autoScrollTimeCount;
		protected Vector2	m_targetPosition;
		protected Vector2	m_fromPosition;
		protected float		m_stopAutoScroll = 0;
		
		protected override void SetContentAnchoredPosition (Vector2 position)
		{
			Vector2 currentPosition = content.anchoredPosition;
			if( !horizontal ) position[0] = currentPosition.x;
			if( !vertical ) position[1] = currentPosition.y;
			
			base.SetContentAnchoredPosition (position);
			
			if( currentPosition != content.anchoredPosition && (wheelEffect || infiniteScroll) )
			{
				UpdateLayout();
			}
		}
		
		public override void OnScroll (PointerEventData data)
		{
			m_stopAutoScroll = 0.4f;
			StopMovement();
			
			base.OnScroll (data);
		}
		
		public override void StopMovement()
		{
			base.StopMovement ();
			m_autoScroll = false;
			m_slip = false;
		}
		
		bool m_setInitialPosition = false;
		
		protected override void Awake ()
		{
			base.Awake();
			
			if( horizontal && vertical )
			{
				//override ScrollPicker value
				horizontal = false;
				vertical = true;
			}
			
			m_setInitialPosition = false;

			if( m_ItemSource != null && m_DeactiveItemOnAwake && Application.isPlaying )
			{
				m_ItemSource.SetActive(false);
			}
		}
		
		public virtual void UpdateLayout()
		{
			ILayoutGroup layoutGroup = pickerLayoutGroup;
			
			if( layoutGroup != null )
			{
				if( horizontal ) layoutGroup.SetLayoutHorizontal();
				if( vertical )   layoutGroup.SetLayoutVertical();
			}
		}
		
		#if UNITY_EDITOR

		protected override void Reset ()
		{
			base.Reset ();

			MassivePickerLayoutGroup layoutGroup = GetComponentInChildren<MassivePickerLayoutGroup>();

			if( layoutGroup != null )
			{
				content = layoutGroup.GetComponent<RectTransform>();
			}
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			
			if( !Application.isPlaying )
			{
				UpdateLayout();
			}
		}

		#endif
		
		#if UNITY_IOS
		
		#pragma warning disable 219
		
		void AOTDummy()
		{
			ItemSelectEvent dummy = new ItemSelectEvent();
		}
		
		#pragma warning restore 219
		
		#endif


        public void ScrollToUp()
        {
            int currentIndex = GetSelectedItemIndex();
            int newIndex = Mathf.Min(currentIndex + 5, itemCount - 1);
            ScrollTo(pickerLayoutGroup.IndexToPosition(newIndex), false);
        }

        public void ScrollToDown()
        {
            int currentIndex = GetSelectedItemIndex();
            int newIndex = Mathf.Max(currentIndex - 5, 0);
            ScrollTo(pickerLayoutGroup.IndexToPosition(newIndex), false);
        }

        public void ScrollToFirst()
        {

            ScrollTo(firstItemPosition, false);
        }

        public void ScrollToLast()
        {

            ScrollTo(lastItemPosition, false);
        }
	
	}
	
}

