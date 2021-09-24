#if UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1
#define USE_BASE_VERTEX_EFFECT
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{

	public class SyncGraphic : MaskableGraphic
	{
		Graphic		syncGraphic;
#if USE_BASE_VERTEX_EFFECT
        MethodInfo onFillVBOMethod;
#else
        MethodInfo onPopulateMeshMethod;
#endif

        public void Setup( Graphic graphic )
		{
			syncGraphic = graphic;
			enabled = graphic.enabled;
		}

		public override Texture mainTexture 
		{
			get 
			{
				return syncGraphic ? syncGraphic.mainTexture : null;
			}
		}

		public override Material material
		{
			get 
			{
				return syncGraphic ? syncGraphic.material : null;
			}

			set{}
		}


#if USE_BASE_VERTEX_EFFECT

        protected override void OnFillVBO (List<UIVertex> vbo)
		{
			if( !IsActive() )
			{
				return;
			}

			if( syncGraphic != null )
			{
				if( onFillVBOMethod == null )
				{
					System.Type type = syncGraphic.GetType();
					BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
					onFillVBOMethod = type.GetMethod("OnFillVBO", bindingFlags, null,  new System.Type[]{ typeof(List<UIVertex>) }, null);
				}

				if( syncGraphic.enabled && syncGraphic.gameObject.activeInHierarchy )
				{
					onFillVBOMethod.Invoke( syncGraphic, new object[]{vbo} );
        
                    List<Component> modifiers = ComponentListPool.Acquire();
					syncGraphic.GetComponents(typeof(IVertexModifier), modifiers);

					foreach( Component modifier in modifiers )
					{
						if( modifier is ZoomItemEffect )
						{
							continue;
						}

						(modifier as IVertexModifier).ModifyVertices(vbo);
					}
                }
			}
		}

#else

        protected override void OnPopulateMesh( VertexHelper vh )
        {
            if( !IsActive() )
            {
                return;
            }

            if( syncGraphic != null )
            {
                if( onPopulateMeshMethod == null )
                {
                    System.Type type = syncGraphic.GetType();
                    BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                    onPopulateMeshMethod = type.GetMethod( "OnPopulateMesh", bindingFlags, null, new System.Type[] { typeof( VertexHelper ) }, null );
                }

                if( syncGraphic.enabled && syncGraphic.gameObject.activeInHierarchy )
                {
                    onPopulateMeshMethod.Invoke( syncGraphic, new object[] { vh } );


                    List<Component> modifiers = ComponentListPool.Acquire();
                    syncGraphic.GetComponents( typeof( IMeshModifier ), modifiers );

                    foreach( Component modifier in modifiers )
                    {
                        if( modifier is ZoomItemEffect )
                        {
                            continue;
                        }

                        (modifier as IMeshModifier).ModifyMesh( vh );
                    }

                    ComponentListPool.Release( modifiers );
                }
            }
        }

#endif

        public override bool Raycast (Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}
}
