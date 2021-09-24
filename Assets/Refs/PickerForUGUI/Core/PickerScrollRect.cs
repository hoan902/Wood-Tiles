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
    public enum InitialPosition
    {
        FirstItem,
        MiddleItem,
        LastItem,
        SelectItemIndex,
        NearItem,
    }

    [ExecuteInEditMode(), AddComponentMenu("UI/Picker/PickerScrollRect", 1000), DisallowMultipleComponent]
    public class PickerScrollRect : ScrollRect
    {
        public PickerItem GetSelectedPickerItem() { return beforeSelectedItem; }
        public GameObject GetSelectedItem() { return beforeSelectedItem != null ? beforeSelectedItem.gameObject : null; }

        public void ScrollTo(PickerItem item, bool immdiate = false)
        {
            if (item != null)
            {
                ScrollTo(item.position, immdiate);
            }
        }

        public void ScrollTo(float offset, bool immdiate = false)
        {
            if (content == null)
            {
                return;
            }

            int axisIndex = (int)layout;
            Vector3 scrollPosition = content.anchoredPosition;
            m_fromPosition = scrollPosition;
            scrollPosition[axisIndex] = offset;
            scrollPosition[1 - axisIndex] = 0;

            if (!immdiate)
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
            if (content == null)
            {
                return 0;
            }

            return content.anchoredPosition[(int)layout];
        }

        public float firstItemPosition
        {
            get
            {
                Transform transform = m_pickerLayoutGroup.transform;
                if (transform.childCount <= 0) return 0f;
                PickerItem pickerItem = transform.GetChild(0).GetComponent<PickerItem>();
                if (pickerItem == null) return 0;
                return pickerItem.position;
            }
        }

        public float lastItemPosition
        {
            get
            {
                Transform transform = m_pickerLayoutGroup.transform;
                int childCount = transform.childCount;
                if (childCount <= 0) return 0f;
                PickerItem pickerItem = transform.GetChild(childCount - 1).GetComponent<PickerItem>();
                if (pickerItem == null) return 0;
                return pickerItem.position;
            }
        }

        public void ScrollToNearItem(bool immdiate = false)
        {
            if (pickerLayoutGroup == null)
            {
                return;
            }

            PickerItem item = GetNearItem();

            if (item != null)
            {
                ScrollTo(item.position, immdiate);
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

                if (horizontal == isHorizontal && vertical == !isHorizontal)
                {
                    return;
                }

                if (content != null)
                {
                    int index = (int)value;
                    Vector2 position = content.anchoredPosition;

                    if (position[1 - index] != 0f)
                    {
                        content.anchoredPosition = position.Assign(0, 1 - index);
                    }
                }

                horizontal = isHorizontal;
                vertical = !isHorizontal;
                SetContentProperties();
                UpdateLayout();
            }
        }

        [SerializeField]
        public float autoScrollSeconds = 0.65f;

        [SerializeField]
        public float slipVelocityRate = 0.25f;

        [SerializeField]
        protected bool m_wheelEffect;

        public bool wheelEffect
        {
            get
            {
                return m_wheelEffect;
            }

            set
            {
                if (m_wheelEffect == value)
                {
                    return;
                }

                m_wheelEffect = value;
                UpdateLayout();
            }
        }

        [SerializeField]
        protected float m_perspective = 0.25f;

        public float wheelPerspective
        {
            get
            {
                return m_perspective;
            }

            set
            {
                if (m_perspective == value)
                {
                    return;
                }

                m_perspective = value;
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
                if (base.movementType == value)
                {
                    return;
                }

                base.movementType = value;
                PickerItem nearItem = GetNearItem();
                UpdateLayout();

                if (nearItem != null)
                {
                    ScrollTo(nearItem.position);
                }
            }

        }

        public bool infiniteScroll
        {
            get
            {
                return movementType == MovementType.Unrestricted;
            }
        }

        [System.Serializable]
        public class ItemSelectEvent : UnityEvent<GameObject> { }

        public ItemSelectEvent onSelectItem = new ItemSelectEvent();

        protected PickerItem beforeSelectedItem = null;

        protected DrivenRectTransformTracker m_tracker = new DrivenRectTransformTracker();
        RectTransform m_beforeContent = null;

        public List<PickerItem> items
        {
            get
            {
                if (pickerLayoutGroup == null)
                {
                    return null;
                }

                return pickerLayoutGroup.itemList;
            }
        }

        protected PickerLayoutGroup m_pickerLayoutGroup;

        protected virtual PickerLayoutGroup pickerLayoutGroup
        {
            get
            {
                if (m_pickerLayoutGroup != null)
                {
                    return m_pickerLayoutGroup;
                }

                if (content != null)
                {
                    m_pickerLayoutGroup = content.GetComponent<PickerLayoutGroup>();
                }

                return m_pickerLayoutGroup;
            }
        }

        protected RectTransform m_selfRect;

        protected RectTransform selfRect
        {
            get
            {
                if (m_selfRect != null)
                {
                    return m_selfRect;
                }

                return m_selfRect = GetComponent<RectTransform>();
            }
        }

        public InitialPosition initialPosition = InitialPosition.FirstItem;
        public int initialPositionItemIndex;


        protected Vector2 beforeScrollPosition;

        protected override void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_autoScroll = false;
                m_slip = false;
                velocity = Vector2.zero;
            }
#endif

            try
            {
                if (pickerLayoutGroup != null)
                {
                    pickerLayoutGroup.LockSetLayout();
                }

                if (m_setInitialPosition)
                {
                    base.LateUpdate();
                }

                AutoScroll();

                NotifySelectItem();

                if (m_beforeContent != content)
                {
                    m_beforeContent = content;
                    SetContentProperties();
                }
                if (pickerLayoutGroup != null)
                {
                    if (content != null && beforeScrollPosition != content.anchoredPosition)
                    {
                        pickerLayoutGroup.SetLayoutVertical();
                        pickerLayoutGroup.SetLayoutHorizontal();
                    }
                }
            }
            finally
            {
                if (pickerLayoutGroup != null) pickerLayoutGroup.UnlockSetLayoutAndUpdateIfDirty();
            }

            if (content != null)
            {
                beforeScrollPosition = content.anchoredPosition;
            }
        }

        public void SetInitialPosition(int infiniteScrollOffset)
        {
            if (m_setInitialPosition || items == null || items.Count <= 0)
            {
                return;
            }

            m_setInitialPosition = true;

            int targetItemIndex = -1;

            switch (initialPosition)
            {
                case InitialPosition.FirstItem:
                    {
                        targetItemIndex = 0;
                        break;
                    }

                case InitialPosition.MiddleItem:
                    {
                        targetItemIndex = items.Count / 2;
                        break;
                    }

                case InitialPosition.LastItem:
                    {
                        targetItemIndex = items.Count - 1;
                        break;
                    }

                case InitialPosition.SelectItemIndex:
                    {
                        targetItemIndex = Mathf.Clamp(initialPositionItemIndex, 0, items.Count - 1);
                        break;
                    }

                case InitialPosition.NearItem:
                    {
                        ScrollToNearItem(true);
                        onValueChanged.Invoke(normalizedPosition);
                        return;
                    }
            }

            if (vertical)
            {
                targetItemIndex = items.Count - 1 - targetItemIndex;
            }

            if (infiniteScroll)
            {
                targetItemIndex = (targetItemIndex - infiniteScrollOffset + items.Count) % items.Count;
            }

            ScrollTo(items[targetItemIndex], true);
            NotifySelectItem(items[targetItemIndex]);
            onValueChanged.Invoke(normalizedPosition);
        }

        protected virtual void NotifySelectItem()
        {
            if (pickerLayoutGroup == null || content == null)
            {
                return;
            }

            List<float> itemOffsetList = pickerLayoutGroup.itemOffsetList;

            if (itemOffsetList.Count <= 0)
            {
                return;
            }

            PickerItem item = GetNearItem();
            NotifySelectItem(item);
        }

        protected void NotifySelectItem(PickerItem item)
        {
            if (beforeSelectedItem != item)
            {
                beforeSelectedItem = item;

                GameObject notify = (item != null ? item.gameObject : null);
                onSelectItem.Invoke(notify);
            }
        }

        public  void SelectItemPress()
        {
            beforeSelectedItem.onClick.Invoke();
        }


        protected bool m_dragging = false;

        public bool enableDrag = true;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            m_dragging = true;
            m_autoScroll = false;
            m_slip = false;
            m_stopAutoScroll = 0f;
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (enableDrag)
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            m_dragging = false;
            base.OnEndDrag(eventData);

            if (inertia && Mathf.Abs(velocity[(int)layout]) >= GetSlipVelocity())
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
            m_tracker.Clear();

            if (content != null)
            {
                m_tracker.Add(
                    this,
                    content,
                    DrivenTransformProperties.Pivot |
                    DrivenTransformProperties.Anchors |
                    (layout == RectTransform.Axis.Horizontal ? DrivenTransformProperties.AnchoredPositionY : DrivenTransformProperties.AnchoredPositionX) |
                    DrivenTransformProperties.Scale |
                    DrivenTransformProperties.SizeDelta |
                    DrivenTransformProperties.Rotation
                    );

                if (layout == RectTransform.Axis.Horizontal)
                {
                    content.anchorMin = new Vector2(0.5f, 0f);
                    content.anchorMax = new Vector2(0.5f, 1f);
                }
                else
                {
                    content.anchorMin = new Vector2(0f, 0.5f);
                    content.anchorMax = new Vector2(1f, 0.5f);
                }

                int axisIndex = (int)layout;
                Vector2 size = content.sizeDelta;
                size[axisIndex] = GetComponent<RectTransform>().sizeDelta[axisIndex];
                content.sizeDelta = size;
                content.pivot = new Vector2(0.5f, 0.5f);
                content.localScale = Vector3.one;
            }
        }

        protected float GetSlipVelocity()
        {
            return slipVelocityRate * selfRect.rect.size[(int)layout];
        }

        protected virtual void AutoScroll()
        {

            if (m_stopAutoScroll > 0)
            {
                m_stopAutoScroll -= Time.deltaTime;
                return;
            }

            if (m_autoScroll)
            {
                Vector2 anchoredPosition = content.anchoredPosition;

                if (anchoredPosition != m_targetPosition)
                {
                    m_autoScrollTimeCount += Time.deltaTime;

                    if (m_autoScrollTimeCount < autoScrollSeconds)
                    {
                        //Ease Out Quart
                        float t = m_autoScrollTimeCount / autoScrollSeconds - 1;
                        t *= t;
                        t *= t;
                        t = t - 1;
                        content.anchoredPosition = (m_fromPosition - m_targetPosition) * t + m_fromPosition;
                    }
                    else
                    {
                        content.anchoredPosition = m_targetPosition;
                        velocity = Vector2.zero;
                        m_autoScroll = false;
                    }

                    if (wheelEffect || infiniteScroll)
                    {
                        UpdateLayout();
                    }
                }
                else
                {
                    velocity = Vector2.zero;
                    m_autoScroll = false;
                }
            }
            else if (!m_dragging)
            {
                int axisIndex = (int)layout;

                if (m_slip)
                {
                    float slipVelocity = GetSlipVelocity();

                    if (Mathf.Abs(velocity[axisIndex]) <= slipVelocity)
                    {
                        if (pickerLayoutGroup != null)
                        {
                            List<float> itemOffsetList = pickerLayoutGroup.itemOffsetList;

                            if (itemOffsetList != null && itemOffsetList.Count > 0)
                            {
                                Vector2 scrollPosition = content.anchoredPosition;
                                scrollPosition[1 - axisIndex] = 0;
                                float beforePosition = beforeScrollPosition[axisIndex];
                                float currentPosition = scrollPosition[axisIndex];


                                int beforeIndex = GetNearItemIndex(-beforePosition);
                                int index = GetNearItemIndex(-currentPosition);

                                if (beforeIndex == index && 0 <= index)
                                {
                                    float itemOffset = -itemOffsetList[index];

                                    if ((beforePosition - itemOffset) * (currentPosition - itemOffset) <= 0 || beforePosition == currentPosition)
                                    {
                                        if (Mathf.Abs(itemOffset - currentPosition) < content.rect.size[axisIndex] * 0.001f)
                                        {
                                            velocity = Vector2.zero;
                                            m_slip = false;
                                            content.anchoredPosition = scrollPosition.Assign(itemOffset, axisIndex);
                                        }
                                        else
                                        {
                                            ScrollTo(itemOffset);
                                        }
                                    }
                                    else if (velocity[axisIndex] * (currentPosition - itemOffset) < 0 || (index != 0 && index != itemOffsetList.Count - 1))
                                    {
                                        velocity = velocity.Assign(slipVelocity * Mathf.Sign(velocity[axisIndex]), axisIndex);
                                    }
                                }
                                else
                                {
                                    velocity = velocity.Assign(slipVelocity * Mathf.Sign(velocity[axisIndex]), axisIndex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (content != null)
                    {
                        PickerItem nearItem = GetNearItem();
                        float currentPosition = -content.anchoredPosition[axisIndex];

                        if (nearItem != null)
                        {
                            float itemOffset = nearItem.position;

                            if (Mathf.Abs(currentPosition + itemOffset) > content.rect.size[axisIndex] * 0.001f)
                            {
                                ScrollTo(itemOffset);
                            }
                        }
                    }
                }
            }
        }

        protected int GetNearItemIndex()
        {
            Vector2 position = content.anchoredPosition;
            return GetNearItemIndex(-position[(int)layout]);
        }

        protected PickerItem GetNearItem()
        {
            int index = GetNearItemIndex();

            if (index >= 0)
            {
                return pickerLayoutGroup.itemList[index];
            }

            return null;
        }

        protected int GetNearItemIndex(float scrollPosition)
        {
            if (pickerLayoutGroup == null)
            {
                return -1;
            }

            List<float> itemOffsetList = pickerLayoutGroup.itemOffsetList;

            if (itemOffsetList == null || itemOffsetList.Count <= 0)
            {
                return -1;
            }

            int index = itemOffsetList.BinarySearch(scrollPosition);

            if (index < 0)
            {
                if (index != -1)
                {
                    index = ~index - 1;
                }
                else
                {
                    index = ~index;
                }
            }

            if (index + 1 < itemOffsetList.Count)
            {
                if (Mathf.Abs(itemOffsetList[index] - scrollPosition) > Mathf.Abs(itemOffsetList[index + 1] - scrollPosition))
                {
                    index = index + 1;
                }
            }

            return 0 <= index && index < itemOffsetList.Count ? index : -1;
        }

        protected bool m_autoScroll = false;
        protected bool m_slip = false;
        protected float m_autoScrollTimeCount;
        protected Vector2 m_targetPosition;
        protected Vector2 m_fromPosition;
        protected float m_stopAutoScroll = 0;

        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            Vector2 currentPosition = content.anchoredPosition;
            if (!horizontal) position[0] = currentPosition.x;
            if (!vertical) position[1] = currentPosition.y;

            base.SetContentAnchoredPosition(position);

            if (currentPosition != content.anchoredPosition && (wheelEffect || infiniteScroll))
            {
                UpdateLayout();
            }
        }

        public override void OnScroll(PointerEventData data)
        {
            m_stopAutoScroll = 0.4f;
            StopMovement();

            base.OnScroll(data);
        }

        public override void StopMovement()
        {
            base.StopMovement();
            m_autoScroll = false;
            m_slip = false;
        }

        bool m_setInitialPosition = false;

        protected override void Awake()
        {
            base.Awake();

            if (horizontal && vertical)
            {
                //override ScrollPicker value
                horizontal = false;
                vertical = true;
            }

            m_setInitialPosition = false;
        }

        public virtual void UpdateLayout()
        {
            LayoutGroup layoutGroup = pickerLayoutGroup;

            if (layoutGroup != null)
            {
                if (horizontal) layoutGroup.SetLayoutHorizontal();
                if (vertical) layoutGroup.SetLayoutVertical();
            }
        }

        public void ScrollToUp()
        {
            int currentIndex = items.IndexOf(beforeSelectedItem);
            int newIndex = Mathf.Min(currentIndex + 1, items.Count - 1);
            ScrollTo(items[newIndex], false);
        }

        public void ScrollToDown()
        {
            int currentIndex = items.IndexOf(beforeSelectedItem);
            int newIndex = Mathf.Max(currentIndex - 1, 0);
            ScrollTo(items[newIndex], false);
        }

        public void ScrollToFirst()
        {

            ScrollTo(firstItemPosition, false);
        }

        public void ScrollToLast()
        {

            ScrollTo(lastItemPosition, false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!Application.isPlaying)
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


    }



}

