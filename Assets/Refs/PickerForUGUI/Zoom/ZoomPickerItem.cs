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
	
	[AddComponentMenu("UI/Picker/ZoomPickerItem",1011), DisallowMultipleComponent]
	public class ZoomPickerItem : PickerItem
	{
		public Transform		zoomItem;
		
		protected override void Awake ()
		{
			base.Awake ();

			if( Application.isPlaying )
			{
				Setup();
			}
		}

		protected override void OnBeforeTransformParentChanged ()
		{
			base.OnBeforeTransformParentChanged ();

			for( int i = 0; i < zoomGraphicList.Count; ++i )
			{
				ZoomGraphic zoom = zoomGraphicList[i];
				zoom.Destroy();
			}
			
			zoomGraphicList.Clear();
		}

		protected override void OnTransformParentChanged ()
		{
			base.OnTransformParentChanged ();

			if( Application.isPlaying )
			{
				Setup();
			}
		}

		protected void Setup()
		{
			if( transform.parent == null )
			{
				return;
			}

			ZoomPickerLayoutGroup group = transform.parent.GetComponent<ZoomPickerLayoutGroup>();
			
			if( group == null )
			{
				return;
			}

			Transform zoomItemParent = group.zoomItemParent;

			if( zoomItemParent == null )
			{
				return;
			}

			if( zoomItem != null )
			{
				foreach( Graphic graphic in zoomItem.GetComponentsInChildren<Graphic>(true) )
				{
					SetupZoomGraphic(graphic,zoomItemParent);
				}
			}
		}

		class ZoomGraphic
		{
			public ZoomItemEffect		zoomItemEffect;
			public SyncGraphic			graphic;

			public bool IsEnabled()
			{
				return zoomItemEffect != null && graphic != null;
			}

			public void Destroy()
			{
				Util.DestroyObject(zoomItemEffect);
				Util.DestroyObject(graphic.gameObject);
			}
		}

		List<ZoomGraphic>	zoomGraphicList = new List<ZoomGraphic>();

		void SetupZoomGraphic( Graphic graphic, Transform zoomItemParent )
		{
			ZoomItemEffect effect = graphic.GetComponent<ZoomItemEffect>();
			
			if( effect == null )
			{
				effect = graphic.gameObject.AddComponent<ZoomItemEffect>();
			}

			GameObject zoomItem = new GameObject(graphic.name + "(ZoomItemGraphic)");
			zoomItem.transform.SetParent(zoomItemParent);

			SyncGraphic syncGraphic = zoomItem.AddComponent<SyncGraphic>();
			syncGraphic.Setup(graphic);

			effect.Setup( graphic, syncGraphic );

			zoomGraphicList.Add( new ZoomGraphic(){ zoomItemEffect = effect, graphic = syncGraphic} );
		}
	}
}
