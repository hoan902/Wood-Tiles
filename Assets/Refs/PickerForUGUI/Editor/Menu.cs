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
	
	public static class Menu
	{
		public const string ImageDirectory = "Assets/PickerForUGUI/Image/";
		public const string FrameSpritePath				= ImageDirectory + "picker_frame.png";
		public const string CanvasSpritePath			= ImageDirectory + "picker_canvas.png";
		public const string CanvasRotateSpritePath		= ImageDirectory + "picker_canvas_rotate.png";
		public const string CursorSpritePath			= ImageDirectory + "picker_cursor.png";
		public const string CursorRotateSpritePath		= ImageDirectory + "picker_cursor_rotate.png";
		
		[MenuItem("GameObject/UI/Picker/Horizontal Picker", false, 3000)]
		static public void AddHorizontalPicker( MenuCommand command )
		{
			AddPicker(command,"HorizontalPicker",false,true,false);
		}
		
		[MenuItem("GameObject/UI/Picker/Vertical Picker", false, 3001)]
		static public void AddVerticalPicker( MenuCommand command )
		{
			AddPicker(command,"VerticalPicker",false,false,false);
		}

		[MenuItem("GameObject/UI/Picker/Zoom Horizontal Picker", false, 3002)]
		static public void AddZoomHorizontalPicker( MenuCommand command )
		{
			AddPicker(command,"ZoomHorizontalPicker",true,true,false);
		}
		
		[MenuItem("GameObject/UI/Picker/Zoom Vertical Picker", false, 3003)]
		static public void AddZoomVerticalPicker( MenuCommand command )
		{
			AddPicker(command,"ZoomVerticalPicker",true,false,false);
		}

		[MenuItem("GameObject/UI/Picker/StringPicker", false, 2998)]
		static public void AddStringPicker( MenuCommand command )
		{
			GameObject picker = AddPicker(command,"StringPicker",false,false,true);
			StringPicker stringPicker = picker.AddComponent<StringPicker>();
			stringPicker.itemSize = new Vector2(50,20);
			stringPicker.columnList = picker.transform.Find("ColumnList");
			stringPicker.SetColumns(1);
			stringPicker.AddItem("Item1");
			stringPicker.AddItem("Item2");
			stringPicker.AddItem("Item3");
			stringPicker.SyncItemList();
		}

		[MenuItem("GameObject/UI/Picker/ImagePicker", false, 2999)]
		static public void AddImagePicker( MenuCommand command )
		{
			GameObject picker = AddPicker(command,"ImagePicker",false,false,true);
			ImagePicker imagePicker = picker.AddComponent<ImagePicker>();
			imagePicker.itemSize = new Vector2(30,30);
			imagePicker.columnList = picker.transform.Find("ColumnList");
			imagePicker.SyncItemList();
		}

		[MenuItem("GameObject/UI/Picker/Massive StringPicker", false, 3110)]
		static public void AddMassiveStringPicker( MenuCommand command )
		{
			GameObject picker = AddMassivePicker(command,"MassiveStringPicker",false,false,true);
			MassiveStringPicker stringPicker = picker.AddComponent<MassiveStringPicker>();
			stringPicker.itemSize = new Vector2(50,20);
			stringPicker.columnList = picker.transform.Find("ColumnList");

			GameObject itemSource = new GameObject("ItemSource");
			GameObjectUtility.SetParentAndAlign( itemSource, picker );
			itemSource.AddComponent<MassiveStringPickerItem>();
			Text text = itemSource.AddComponent<Text>();
			text.color = Color.black;
			text.alignment = TextAnchor.MiddleCenter;
			text.text = "Item1";
			text.resizeTextForBestFit = true;
			itemSource.GetComponent<RectTransform>().sizeDelta = stringPicker.itemSize;
			itemSource.transform.SetSiblingIndex(1);

			stringPicker.itemSource = itemSource;

			stringPicker.SetColumns(1);
			stringPicker.AddItem("Item1");
			stringPicker.AddItem("Item2");
			stringPicker.AddItem("Item3");
			stringPicker.SyncItemList();
		}

		[MenuItem("GameObject/UI/Picker/Massive ImagePicker", false, 3111)]
		static public void AddMassiveImagePicker( MenuCommand command )
		{
			GameObject picker = AddMassivePicker(command,"MassiveImagePicker",false,false,true);
			MassiveImagePicker imagePicker = picker.AddComponent<MassiveImagePicker>();
			imagePicker.itemSize = new Vector2(50,20);
			imagePicker.columnList = picker.transform.Find("ColumnList");
			
			GameObject itemSource = new GameObject("ItemSource");
			GameObjectUtility.SetParentAndAlign( itemSource, picker );
			itemSource.AddComponent<MassiveImagePickerItem>();
			Image image = itemSource.AddComponent<Image>();
			image.color = Color.white;
			itemSource.GetComponent<RectTransform>().sizeDelta = imagePicker.itemSize;
			itemSource.transform.SetSiblingIndex(1);
			
			imagePicker.itemSource = itemSource;
			
			imagePicker.SetColumns(1);
			imagePicker.SyncItemList();
		}

		[MenuItem("GameObject/UI/Picker/Massive Horizontal Picker", false, 3112)]
		static public void AddMassiveHorizontalPicker( MenuCommand command )
		{
			AddMassivePicker(command,"MassiveHorizontalPicker",false,true,false);
		}
		
		[MenuItem("GameObject/UI/Picker/Massive Vertical Picker", false, 3113)]
		static public void AddMassiveVerticalPicker( MenuCommand command )
		{
			AddMassivePicker(command,"MassiveVerticalPicker",false,false,false);
		}
		
		[MenuItem("GameObject/UI/Picker/Massive Zoom Horizontal Picker", false, 3114)]
		static public void AddMassiveZoomHorizontalPicker( MenuCommand command )
		{
			AddMassivePicker(command,"MassiveZoomHorizontalPicker",true,true,false);
		}
		
		[MenuItem("GameObject/UI/Picker/Massive Zoom Vertical Picker", false, 3115)]
		static public void AddMassiveZoomVerticalPicker( MenuCommand command )
		{
			AddMassivePicker(command,"MassiveZoomVerticalPicker",true,false,false);
		}

		static public GameObject AddPicker( MenuCommand command, string name, bool zoom, bool horizontal, bool noitem )
		{
			GameObject pickerRoot = MenuOptions.CreateUIElementRoot(name,command,MenuOptions.s_ImageGUIElementSize);

			GameObject columnList = new GameObject("ColumnList");
			GameObjectUtility.SetParentAndAlign(columnList, pickerRoot);
			columnList.AddComponent( horizontal ? typeof(VerticalLayoutGroup) : typeof(HorizontalLayoutGroup) );

			RectTransform rect = (RectTransform)columnList.transform;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.sizeDelta = new Vector2(-20,-20);

			PickerScrollRect column = AddColumn(columnList, zoom, horizontal);

			GameObject glass = AddGlass(pickerRoot, zoom, horizontal);
			AddFrame(pickerRoot);

			if( !noitem )
			{
				for( int itemIndex = 0; itemIndex < 3; ++itemIndex )
				{
					AddItem(column.content.gameObject,zoom,horizontal,itemIndex);
				}
			}

			if( zoom )
			{
				column.content.GetComponent<ZoomPickerLayoutGroup>().zoomItemParent = glass.transform;
			}

			return pickerRoot;
		}

		static public GameObject AddMassivePicker( MenuCommand command, string name, bool zoom, bool horizontal, bool noitem )
		{
			GameObject pickerRoot = MenuOptions.CreateUIElementRoot(name,command,MenuOptions.s_ImageGUIElementSize);
			
			GameObject columnList = new GameObject("ColumnList");
			GameObjectUtility.SetParentAndAlign(columnList, pickerRoot);
			columnList.AddComponent( horizontal ? typeof(VerticalLayoutGroup) : typeof(HorizontalLayoutGroup) );
			
			RectTransform rect = (RectTransform)columnList.transform;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.sizeDelta = new Vector2(-20,-20);
			
			MassivePickerScrollRect column = AddMassiveColumn(columnList, zoom, horizontal);
			
			GameObject glass = AddGlass(pickerRoot, zoom, horizontal);
			AddFrame(pickerRoot);

			if( !noitem )
			{
				GameObject itemSource = new GameObject("ItemSource");
				GameObjectUtility.SetParentAndAlign( itemSource, pickerRoot );
				
				Vector2 itemSize = horizontal ? new Vector2(15,50) : new Vector2(50,20);
				
				if( !zoom )
				{
					itemSource.AddComponent<ExampleStringItem>();
				}
				else
				{
					ExampleZoomStringItem item = itemSource.AddComponent<ExampleZoomStringItem>();
					GameObject zoomItem = new GameObject("zoomItem");
					GameObjectUtility.SetParentAndAlign( zoomItem, itemSource );
					SetExampleMassiveItemText(zoomItem,true);
					zoomItem.GetComponent<RectTransform>().sizeDelta = itemSize * 1.4f;
					item.zoomItem = zoomItem.transform;
				}
				
				SetExampleMassiveItemText(itemSource);
				
				RectTransform itemRectTransform = itemSource.GetComponent<RectTransform>();
				itemRectTransform.sizeDelta = itemSize;
				itemRectTransform.SetSiblingIndex( zoom ? 2 : 1 );
				
				column.deactiveItemOnAwake = true;
				column.itemSource = itemSource;
				column.itemSize = itemRectTransform.sizeDelta;
				column.itemCount = 10;
			}

			if( zoom )
			{
				column.content.GetComponent<MassiveZoomPickerLayoutGroup>().zoomItemParent = glass.transform;
			}

			return pickerRoot;
		}

		static void SetExampleMassiveItemText( GameObject obj, bool zoom = false )
		{
			Text text = obj.AddComponent<Text>();
			text.color = Color.black;
			text.alignment = TextAnchor.MiddleCenter;
			text.text = "Item0";
			text.resizeTextForBestFit = true;

			if( zoom )
			{
				text.fontStyle = FontStyle.Bold;
			}
		}

		static public PickerScrollRect AddColumn( GameObject columnList, bool zoom, bool horizontal )
		{
			GameObject column = new GameObject("Column");
			GameObjectUtility.SetParentAndAlign(column, columnList);
			PickerScrollRect scrollRect = column.AddComponent<PickerScrollRect>();
			
			GameObject content = new GameObject("Content");
			GameObjectUtility.SetParentAndAlign(content,column);
			PickerLayoutGroup layoutGroup = (!zoom ? content.AddComponent<PickerLayoutGroup>() : content.AddComponent<ZoomPickerLayoutGroup>());
			
			scrollRect.content = (RectTransform)layoutGroup.transform;
			scrollRect.layout = ( horizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical );
			
			layoutGroup.scrollRect = scrollRect;
			layoutGroup.spacing = 3f;

			Image scrollImage = column.AddComponent<Image>();
			scrollImage.sprite = (Sprite)AssetDatabase.LoadAssetAtPath( !horizontal ? CanvasSpritePath : CanvasRotateSpritePath,typeof(Sprite));
			scrollImage.type = Image.Type.Sliced;
			
			Mask scrollMask = column.AddComponent<Mask>();
			scrollMask.showMaskGraphic = true;

			return scrollRect;
		}

		static public MassivePickerScrollRect AddMassiveColumn( GameObject columnList, bool zoom, bool horizontal )
		{
			GameObject column = new GameObject("Column");
			GameObjectUtility.SetParentAndAlign(column, columnList);
			MassivePickerScrollRect scrollRect = column.AddComponent<MassivePickerScrollRect>();
			
			GameObject content = new GameObject("Content");
			GameObjectUtility.SetParentAndAlign(content,column);
			MassivePickerLayoutGroup layoutGroup = (!zoom ? content.AddComponent<MassivePickerLayoutGroup>() : content.AddComponent<MassiveZoomPickerLayoutGroup>());
			
			scrollRect.content = (RectTransform)layoutGroup.transform;
			scrollRect.layout = ( horizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical );
			
			layoutGroup.scrollRect = scrollRect;
			layoutGroup.spacing = 3f;
			
			Image scrollImage = column.AddComponent<Image>();
			scrollImage.sprite = (Sprite)AssetDatabase.LoadAssetAtPath( !horizontal ? CanvasSpritePath : CanvasRotateSpritePath,typeof(Sprite));
			scrollImage.type = Image.Type.Sliced;
			
			Mask scrollMask = column.AddComponent<Mask>();
			scrollMask.showMaskGraphic = true;
			
			return scrollRect;
		}
		
		static public void AddItem( GameObject parent, bool zoom, bool horizontal, int index )
		{
			string name = "Item" + index;
			GameObject item = new GameObject(name);
			GameObjectUtility.SetParentAndAlign(item, parent);
			
			PickerItem pickerItem = !zoom ? item.AddComponent<PickerItem>() : item.AddComponent<ZoomPickerItem>();
			RectTransform rect = pickerItem.GetComponent<RectTransform>();
			
			if( !horizontal )
			{
				rect.sizeDelta = new Vector2(70,20);
			}
			else
			{
				rect.sizeDelta = new Vector2(15,70);
			}
			
			Text itemText = AddTextComponent(item, item.name);

			if( zoom )
			{
				GameObject zoomItem = new GameObject("ZoomItem");
				GameObjectUtility.SetParentAndAlign( zoomItem, item );
				AddTextComponent(zoomItem,item.name);

				RectTransform zoomTransform = zoomItem.GetComponent<RectTransform>();
				zoomTransform.sizeDelta = rect.sizeDelta * 1.25f;

				itemText.color = Color.gray;

				((ZoomPickerItem)pickerItem).zoomItem = zoomTransform;
			}
		}

		static public Text AddTextComponent( GameObject item, string text )
		{
			Text textComponent = item.AddComponent<Text>();
			textComponent.color = Color.black;
			textComponent.fontStyle = FontStyle.Bold;
			textComponent.alignment = TextAnchor.MiddleCenter;
			textComponent.text = text;
			textComponent.resizeTextForBestFit = true;
			return textComponent;
		}
		
		static public GameObject AddGlass( GameObject parent, bool zoom, bool horizontal )
		{
			if( zoom )
			{
				GameObject glassBg = new GameObject("GlassBg");
				GameObjectUtility.SetParentAndAlign(glassBg,parent);
				DontTouchImage bgImage = glassBg.AddComponent<DontTouchImage>();

				bgImage.sprite = (Sprite)AssetDatabase.LoadAssetAtPath( !horizontal ? CanvasSpritePath : CanvasRotateSpritePath,typeof(Sprite));
				bgImage.type = Image.Type.Sliced;

				RectTransform bgRect = glassBg.GetComponent<RectTransform>();

				if( !horizontal )
				{
					bgRect.anchorMin = new Vector2(0,0.5f);
					bgRect.anchorMax = new Vector2(1,0.5f);
					bgRect.sizeDelta = new Vector2(-20,18);
				}
				else
				{
					bgRect.anchorMin = new Vector2(0.5f,0);
					bgRect.anchorMax = new Vector2(0.5f,1);
					bgRect.sizeDelta = new Vector2(18,-20);
				}
			}

			GameObject glass = new GameObject("Glass");
			GameObjectUtility.SetParentAndAlign(glass,parent);
			
			DontTouchImage image = glass.AddComponent<DontTouchImage>();
			image.color = new Color(1,1,1,0.75f);
			image.sprite = (Sprite)AssetDatabase.LoadAssetAtPath(!horizontal ? CursorSpritePath : CursorRotateSpritePath, typeof(Sprite));
			image.type = Image.Type.Sliced;
			
			RectTransform rect = glass.GetComponent<RectTransform>();
			
			if( !horizontal )
			{
				rect.anchorMin = new Vector2(0,0.5f);
				rect.anchorMax = new Vector2(1,0.5f);
				rect.sizeDelta = new Vector2(-20,20);
			}
			else
			{
				rect.anchorMin = new Vector2(0.5f,0);
				rect.anchorMax = new Vector2(0.5f,1);
				rect.sizeDelta = new Vector2(20,-22);
			}

			if( zoom )
			{
				Mask glassMask = glass.AddComponent<Mask>();
				glassMask.showMaskGraphic = true;
			}

			return glass;
		}
		
		static public  void AddFrame( GameObject parent )
		{
			GameObject frame = new GameObject("Frame");
			GameObjectUtility.SetParentAndAlign(frame,parent);
			
			DontTouchImage image = frame.AddComponent<DontTouchImage>();
			image.fillCenter = false;
			image.sprite = (Sprite)AssetDatabase.LoadAssetAtPath(FrameSpritePath, typeof(Sprite));
			image.type = Image.Type.Sliced;
			
			RectTransform rect = frame.GetComponent<RectTransform>();
			
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f,0.5f);
			rect.sizeDelta = Vector2.zero;
		}
	}
	
}