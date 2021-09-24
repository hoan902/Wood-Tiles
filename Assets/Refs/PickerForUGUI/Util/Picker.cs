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
	[System.Serializable]
	public class ItemList<ParamType>
	{
		[SerializeField]
		public List<ParamType>			items = new List<ParamType>();
	}

	[DisallowMultipleComponent]
	public class Picker<ItemComponent, ItemParamType, ItemListType> : MonoBehaviour 
		where ItemComponent : PickerItem 
		where ItemListType : ItemList<ItemParamType>, new()
	{
		[SerializeField]
		public Vector2								itemSize;

		[SerializeField]
		public Transform							columnList;

		[SerializeField]
		protected PickerScrollRect[]		columns
		{
			get
			{
				if( columnList == null )
				{
					return null;
				}

				return columnList.GetComponentsInChildren<PickerScrollRect>();
			}
		}

		public void SetColumns( int num )
		{
			PickerScrollRect[] columns = this.columns;

			if( columns == null || itemList == null || num < 1  || columns.Length == num && itemList.Count == num || columnList == null )
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
				bool zoom = columnList.GetComponentInChildren<ZoomPickerLayoutGroup>() != null;

				PickerScrollRect exampleScrollRect = columnList.GetComponentInChildren<PickerScrollRect>();
				PickerLayoutGroup exampleLayoutGroup = null;
				PickerLayoutGroup exampleZoomLayoutGroup = null;

				if( !zoom )
				{
					exampleLayoutGroup = columnList.GetComponentInChildren<PickerLayoutGroup>();
				}
				else
				{
					exampleZoomLayoutGroup = columnList.GetComponentInChildren<ZoomPickerLayoutGroup>();
				}

				Image exampleImage = columnList.GetComponentInChildren<Image>();


				for( int i = columns.Length; i < num; ++i )
				{
					GameObject column  = new GameObject("Column");
#if UNITY_EDITOR
					Undo.RegisterCreatedObjectUndo(column,"Column");
#endif

					PickerScrollRect scrollRect = column.AddComponent<PickerScrollRect>();
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
						PickerLayoutGroup layoutGroup = content.AddComponent<PickerLayoutGroup>();
						CopyField( exampleLayoutGroup, layoutGroup );
						layoutGroup.scrollRect = scrollRect;
					}
					else
					{
						ZoomPickerLayoutGroup layoutGroup = content.AddComponent<ZoomPickerLayoutGroup>();
						CopyField( exampleZoomLayoutGroup, layoutGroup );
						layoutGroup.scrollRect = scrollRect;
					}
					
					scrollRect.content = (RectTransform)content.transform;

					CopyField( exampleScrollRect, scrollRect );
					CopyField( exampleImage, image );

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
				PickerScrollRect[] columns = this.columns;

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
				PickerScrollRect[] columns = this.columns;

				if( columns == null || columnList == null )
				{
					return;
				}

				foreach( PickerScrollRect column in columns )
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
			PickerScrollRect[] columns = this.columns;

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

		protected virtual void SetParameter( ItemComponent item, ItemParamType param )
		{/*empty*/}

		protected void UpdateItem( ItemComponent item, ItemParamType param )
		{
			if( param != null )
			{
				item.name = param.ToString();
			}

			RectTransform itemTransform = item.GetComponent<RectTransform>();

			Vector2 size = Vector3.zero;

			for( int i = 0; i < 2; ++i )
			{
				size[i] = itemSize[i];

				if( itemSize[i] == 0 )
				{
					itemTransform.anchorMin = itemTransform.anchorMin.Assign(0,i);
					itemTransform.anchorMax = itemTransform.anchorMax.Assign(1,i);
				}
				else
				{
					itemTransform.anchorMin = itemTransform.anchorMin.Assign(0.5f,i);
					itemTransform.anchorMax = itemTransform.anchorMax.Assign(0.5f,i);
				}
			}

			itemTransform.sizeDelta = size;

			item.userData = param;
			SetParameter(item,param);
		}

		protected GameObject CreateItem( ItemParamType param )
		{
			GameObject itemObject = new GameObject();
			ItemComponent item = itemObject.AddComponent<ItemComponent>();

			RectTransform itemTransform = itemObject.GetComponent<RectTransform>();
			itemTransform.sizeDelta = itemSize;
			itemTransform.anchorMin = itemTransform.anchorMax = new Vector2(0.5f,0.5f);

			UpdateItem(item, param);
			
			return itemObject;
		}

		public PickerItem GetSelectedItem()
		{
			PickerScrollRect[] columns = this.columns;

			if( columns == null )
			{
				return null;
			}

			return columns[0].GetSelectedPickerItem();
		}

		public PickerItem[] GetSelectedItems()
		{
			PickerScrollRect[] columns = this.columns;
			
			if( columns == null )
			{
				return new PickerItem[0];
			}

			PickerItem[] selected = new PickerItem[columns.Length];

			for( int i = 0; i < selected.Length; ++i )
			{
				if( columns[i] != null )
				{
					selected[i] = columns[i].GetSelectedPickerItem();
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
				throw new System.ArgumentException();
			}

			GameObject item = CreateItem(param);
			
			if( item != null )
			{
				if( _AddItem( columnIndex, item ) )
				{
					itemList[columnIndex].items.Add(param);
				}
			}
		}

		public void RemoveItem( int itemIndex )
		{
			RemoveItem(0,itemIndex);
		}

		public void RemoveItem( int columnIndex, int itemIndex )
		{
			PickerScrollRect scrollRect = GetPickerScrollRect(columnIndex);

			if( scrollRect == null )
			{
				return;
			}

			Transform scrollTransform = scrollRect.transform;

			if( itemIndex < 0 || scrollTransform.childCount <= itemIndex )
			{
				return;
			}

			Transform itemTransform = scrollTransform.GetChild(itemIndex);
			Util.DestroyObject( itemTransform.gameObject );

			itemList[columnIndex].items.RemoveAt(itemIndex);
		}

		public void SetItems( IList<ItemParamType> itemParams, bool fromInspector = false )
		{
			SetItems(0,itemParams,fromInspector);
		}
		
		public void SetItems( int columnIndex, IList<ItemParamType> itemParams, bool fromInspector = false )
		{
			PickerScrollRect scroll = GetPickerScrollRect(columnIndex);
			
			if( scroll == null )
			{
				return;
			}
			
			RectTransform rectTransform = scroll.content;
			
			if( rectTransform == null )
			{
				return;
			}

			if( !fromInspector )
			{
				itemList[columnIndex].items = itemParams.ToList();
			}
			
			int childCount = rectTransform.childCount;
			
			if( itemParams.Count < childCount )
			{
				for( int i = childCount-1; i >= itemParams.Count; --i )
				{
					Util.DestroyObject(rectTransform.GetChild(i).gameObject);
				}
				
				childCount = itemParams.Count;
			}
			
			int itemIndex = 0;
			
			for( int itemCount = Mathf.Min(itemParams.Count,childCount) ; itemIndex < itemCount; ++itemIndex )
			{
				Transform childTransform = rectTransform.GetChild(itemIndex);
				ItemComponent item = childTransform.GetComponent<ItemComponent>();
				UpdateItem(item, itemParams[itemIndex]);
			}
			
			for( int i = itemIndex; itemIndex < itemParams.Count; ++itemIndex )
			{
				GameObject item = CreateItem(itemParams[i]);
				
				if( item != null )
				{
					_AddItem( columnIndex, item );
				}
			}
		}

		protected bool _AddItem( int columnIndex, GameObject item )
		{
			if( item == null )
			{
				return false;
			}

			PickerScrollRect scroll = GetPickerScrollRect(columnIndex);
			
			if( scroll == null )
			{
				return false;
			}

			item.transform.SetParent(scroll.content.transform);

			return true;
		}

		public PickerScrollRect GetPickerScrollRect( int columnIndex )
		{
			PickerScrollRect[] columns = this.columns;

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
