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
	[DisallowMultipleComponent]
	public class MassivePicker<ItemComponent, ItemParamType, ItemListType> : MonoBehaviour 
		where ItemComponent : MassivePickerItem 
		where ItemListType : ItemList<ItemParamType>, new()
	{
		[SerializeField]
		public Vector2								itemSize;

		[SerializeField]
		public Transform							columnList;

		[SerializeField]
		protected GameObject						m_ItemSource;

		public GameObject							itemSource
		{
			set
			{
				m_ItemSource = value;

				if( columns != null )
				{
					foreach( MassivePickerScrollRect scroll in columns )
					{
						scroll.itemSource = m_ItemSource;
						scroll.deactiveItemOnAwake = IsItemChild(scroll);
					}
				}
			}

			get{ return m_ItemSource; }
		}

		[SerializeField]
		protected MassivePickerScrollRect[]		columns
		{
			get
			{
				if( columnList == null )
				{
					return null;
				}

				return columnList.GetComponentsInChildren<MassivePickerScrollRect>();
			}
		}

		protected bool IsItemChild( MassivePickerScrollRect rect )
		{
			GameObject item = rect.itemSource;

			if( item == null )
			{
				return false;
			}

			Transform itemTransform = item.transform;

			while( itemTransform != null )
			{
				itemTransform = itemTransform.parent;

				if( itemTransform == this.transform )
				{
					return true;
				}
			}

			return false;
		}

		public void SetColumns( int num )
		{
			MassivePickerScrollRect[] columns = this.columns;

			if( columns == null || itemList == null || num <= 0 || columns.Length == num && itemList.Count == num || columnList == null )
			{
				return;
			}

			if( itemList.Count < num )
			{
				for( int i = itemList.Count; i < num; ++i )
				{
					itemList.Add( new ItemListType() );
				}
			}
			else if( itemList.Count > num )
			{
				itemList.RemoveRange( num, itemList.Count-num );
			}

			if( columns.Length < num )
			{
				bool zoom = columnList.GetComponentInChildren<MassiveZoomPickerLayoutGroup>() != null;

				MassivePickerScrollRect exampleScrollRect = columnList.GetComponentInChildren<MassivePickerScrollRect>();
				MassivePickerLayoutGroup exampleLayoutGroup = null;
				MassivePickerLayoutGroup exampleZoomLayoutGroup = null;

				if( !zoom )
				{
					exampleLayoutGroup = columnList.GetComponentInChildren<MassivePickerLayoutGroup>();
				}
				else
				{
					exampleZoomLayoutGroup = columnList.GetComponentInChildren<MassiveZoomPickerLayoutGroup>();
				}

				Image exampleImage = columnList.GetComponentInChildren<Image>();

				for( int i = columns.Length; i < num; ++i )
				{
					GameObject column  = new GameObject("Column");
#if UNITY_EDITOR
					Undo.RegisterCreatedObjectUndo(column,"Column");
#endif

					MassivePickerScrollRect scrollRect = column.AddComponent<MassivePickerScrollRect>();

					Image image = column.AddComponent<Image>();
					column.AddComponent<Mask>().showMaskGraphic = true;
					column.transform.SetParent( columnList.transform );
					column.transform.localScale = Vector3.one;

					GameObject content = new GameObject("Content");
					content.transform.SetParent( column.transform );

#if UNITY_EDITOR
					Undo.RegisterCreatedObjectUndo(column,"content");
#endif

					if( !zoom )
					{
						MassivePickerLayoutGroup layoutGroup = content.AddComponent<MassivePickerLayoutGroup>();
						CopyField( exampleLayoutGroup, layoutGroup );
						layoutGroup.scrollRect = scrollRect;
					}
					else
					{
						MassiveZoomPickerLayoutGroup layoutGroup = content.AddComponent<MassiveZoomPickerLayoutGroup>();
						CopyField( exampleZoomLayoutGroup, layoutGroup );
						layoutGroup.scrollRect = scrollRect;
					}
					
					scrollRect.content = (RectTransform)content.transform;

					CopyField( exampleScrollRect, scrollRect );
					CopyField( exampleImage, image );
					
					scrollRect.deactiveItemOnAwake = IsItemChild(scrollRect);

					itemList[i] = new ItemListType();
				}
			}
			else if( columns.Length > num )
			{
				for( int i = columns.Length-1, j = num; i >= j; --i )
				{
					GameObject column = columnList.GetChild(i).gameObject;
					Util.DestroyObject(column);
				}
			}
		}

		public RectTransform.Axis layout
		{
			get
			{
				MassivePickerScrollRect[] columns = this.columns;

				if( columns != null )
				{
					int horizontal = 0;
					int vertical = 0;
					
					for( int i = 0; i < columns.Length; ++i )
					{
						RectTransform.Axis layout = columns[i].layout;
						if( layout == RectTransform.Axis.Horizontal ) ++horizontal;
						if( layout == RectTransform.Axis.Vertical ) ++vertical;
					}
					
					if( horizontal == columns.Length ) return RectTransform.Axis.Horizontal;
					if( vertical == columns.Length ) return RectTransform.Axis.Vertical;
				}

				return (RectTransform.Axis)(-1);
			}

			set
			{
				MassivePickerScrollRect[] columns = this.columns;

				if( columns == null || columnList == null )
				{
					return;
				}

				foreach( MassivePickerScrollRect column in columns )
				{
					column.layout = value;
				}

				if( value == RectTransform.Axis.Horizontal )
				{
					ChangeLayoutGroup<HorizontalLayoutGroup,VerticalLayoutGroup>();
				}

				if( value == RectTransform.Axis.Vertical )
				{
					ChangeLayoutGroup<VerticalLayoutGroup,HorizontalLayoutGroup>();
				}
			}
		}

		protected void ChangeLayoutGroup<BeforeLayoutGroup,AfterLayoutGroup>()
			where BeforeLayoutGroup : Component
			where AfterLayoutGroup : Component
		{
			if( columnList == null )
			{
				return;
			}

			BeforeLayoutGroup before = columnList.GetComponent<BeforeLayoutGroup>();
			if( before != null ) DestroyImmediate( before );

			columnList.gameObject.AddComponent<AfterLayoutGroup>();
		}
			
		public virtual void SyncItemList( bool fromInspector = false )
		{
			MassivePickerScrollRect[] columns = this.columns;

			if( itemList == null || columns == null )
			{
				return;
			}

			SetColumns( itemList.Count );

			for( int columnIndex = 0; columnIndex < itemList.Count; ++columnIndex )
			{
				SetItems( columnIndex, itemList[columnIndex].items, fromInspector );
			}
		}

		[SerializeField]
		protected List<ItemListType>				itemList = new List<ItemListType>();

		public List<ItemListType> 					items	{ get{ return itemList; } }

		public int GetSelectedItemIndex()
		{
			MassivePickerScrollRect[] columns = this.columns;

			if( columns == null )
			{
				return -1;
			}

			return columns[0].GetSelectedItemIndex();
		}

		public int GetColumnIndex( MassivePickerScrollRect scrollRect )
		{
			if( columns == null )
			{
				return -1;
			}

			return System.Array.IndexOf( columns, scrollRect );
		}

		public ItemParamType GetSelectedItem()
		{
			MassivePickerScrollRect[] columns = this.columns;
			
			if( columns == null || columns.Length <= 0 )
			{
				return default(ItemParamType);
			}
			
			int index = columns[0].GetSelectedItemIndex();
			return items[0].items[index];
		}

		public ItemParamType GetSelectedItem( int columnIndex )
		{
			MassivePickerScrollRect[] columns = this.columns;
			
			if( columns == null )
			{
				return default(ItemParamType);
			}
			
			int index = columns[columnIndex].GetSelectedItemIndex();

			if( index >= 0 )
			{
				return items[columnIndex].items[index];
			}

			return default(ItemParamType);
		}

		public ItemParamType[] GetSelectedItems()
		{
			MassivePickerScrollRect[] columns = this.columns;
			
			if( columns == null )
			{
				return new ItemParamType[0];
			}

			ItemParamType[] selected = new ItemParamType[columns.Length];

			for( int i = 0; i < selected.Length; ++i )
			{
				if( columns[i] != null )
				{
					selected[i] = GetSelectedItem(i);
				}
			}

			return selected;
		}

		public void AddItem( ItemParamType param )
		{
			AddItem(0,param);
		}
		
		public void AddItem( int columnIndex, ItemParamType param )
		{
			if( itemList == null )
			{
				return;
			}

			if( columnIndex < 0 || itemList.Count <= columnIndex )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			itemList[columnIndex].items.Add(param);

			MassivePickerScrollRect scroll = GetPickerScrollRect(columnIndex);
			++scroll.itemCount;

			scroll.UpdateAllItemContent();
		}

		public void RemoveItem( int itemIndex )
		{
			RemoveItem(0,itemIndex);
		}

		public void RemoveItem( int columnIndex, int itemIndex )
		{
			MassivePickerScrollRect scrollRect = GetPickerScrollRect(columnIndex);

			if( scrollRect == null )
			{
				return;
			}

			itemList[columnIndex].items.RemoveAt(itemIndex);
			scrollRect.UpdateAllItemContent();
		}

		public ItemParamType GetItemParam( int columnIndex, int itemIndex )
		{
			MassivePickerScrollRect scrollRect = GetPickerScrollRect(columnIndex);

			if( scrollRect == null )
			{
				return default(ItemParamType);
			}

			return itemList[columnIndex].items[itemIndex];
		}

		public void SetItems( IList<ItemParamType> itemParams, bool fromInspector = false )
		{
			SetItems(0,itemParams,fromInspector);
		}
		
		public void SetItems( int columnIndex, IList<ItemParamType> itemParams, bool fromInspector = false )
		{
			MassivePickerScrollRect scroll = GetPickerScrollRect(columnIndex);
			
			if( scroll == null )
			{
				return;
			}

			if( !fromInspector )
			{
				itemList[columnIndex].items = itemParams.ToList();
			}

			scroll.itemSize = itemSize;
			scroll.itemCount = itemList[columnIndex].items.Count;
			scroll.UpdateAllItemContent();
		}

		public MassivePickerScrollRect GetPickerScrollRect( int columnIndex )
		{
			MassivePickerScrollRect[] columns = this.columns;

			if( columns == null || columnIndex < 0 || columns.Length <= columnIndex )
			{
				return null;
			}

			return columns[columnIndex];
		}

		protected virtual void Awake()
		{
			SyncItemList();
		}

		protected static void CopyField( object src, object dst )
		{
			if( src == null || dst == null || src.GetType() != dst.GetType() )
			{
				return;
			}

			foreach( System.Reflection.FieldInfo field in src.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.NonPublic ) )
			{
				field.SetValue( dst, field.GetValue(src) );
			}
		}
	}

}
