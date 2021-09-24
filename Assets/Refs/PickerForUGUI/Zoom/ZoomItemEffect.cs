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

using System.Reflection;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System;
#endif

namespace Picker
{
#if USE_BASE_VERTEX_EFFECT
    public class ZoomItemEffect : BaseVertexEffect
#else
    public class ZoomItemEffect : BaseMeshEffect
#endif
    {
        protected Graphic targetGraphic;
        protected SyncGraphic syncGraphic;

        protected RectTransform targetGraphicTransform;
        protected RectTransform syncGraphicTransform;

        protected FieldInfo materialDirtyField;
        protected FieldInfo vertsDirtyField;

        public void Setup(Graphic targetGraphic, SyncGraphic syncGraphic)
        {
            this.targetGraphic = targetGraphic;
            this.syncGraphic = syncGraphic;

            targetGraphicTransform = targetGraphic.rectTransform;
            syncGraphicTransform = syncGraphic.rectTransform;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (syncGraphic != null && Application.isPlaying)
            {
                Util.DestroyObject(syncGraphic.gameObject);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (syncGraphic != null)
            {
                syncGraphic.enabled = true;
            }

            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.FlattenHierarchy;
            materialDirtyField = typeof(Graphic).GetField("m_MaterialDirty", flag);
            vertsDirtyField = typeof(Graphic).GetField("m_VertsDirty", flag);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (syncGraphic != null)
            {
                syncGraphic.enabled = false;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SyncTransform();
        }

        protected virtual void LateUpdate()
        {
            SyncTransform();
        }

        bool prevScaleZero = false;

        protected void SyncTransform()
        {
            RectTransform src = targetGraphicTransform;

            if (src == null)
            {
                return;
            }

            Vector3 scale = src.lossyScale;

            if (scale.x == 0 && scale.y == 0)
            {
                if (prevScaleZero)
                {
                    return;
                }

                prevScaleZero = true;
            }
            else
            {
                prevScaleZero = false;
            }

            RectTransform dist = syncGraphicTransform;

            Vector3 position = src.position;
            if (position != dist.position) dist.position = position;

            Quaternion rotation = src.rotation;
            if (rotation != dist.rotation) dist.rotation = rotation;

            if (scale != dist.lossyScale)
            {
                Vector3 parentScale = dist.parent.lossyScale;

                if (parentScale.x != 0 && parentScale.y != 0 && parentScale.z != 0)
                {
                    dist.localScale = new Vector3(scale.x / parentScale.x, scale.y / parentScale.y, scale.z / parentScale.z);
                }
                else
                {
                    dist.localScale = Vector3.zero;
                }
            }
        }

#if USE_BASE_VERTEX_EFFECT

        public override void ModifyVertices(List<UIVertex> verts)
        {
            if (Application.isPlaying && IsActive())
            {
                verts.Clear();

                if (syncGraphic != null)
                {
                    if (materialDirtyField != null) materialDirtyField.SetValue(syncGraphic, true);
                    if (vertsDirtyField != null) vertsDirtyField.SetValue(syncGraphic, true);
                    syncGraphic.Rebuild(CanvasUpdate.PreRender);
                }
            }
        }

#else
        public override void ModifyMesh( Mesh mesh )
        {
            if( !Application.isPlaying || !IsActive() )
            {
                return;
            }

            _SyncGraphics();

            using( var helper = new VertexHelper() )
            {
                helper.FillMesh( mesh );
            }
        }

        public override void ModifyMesh( VertexHelper vh )
        {
            if( !Application.isPlaying || !IsActive() )
            {
                return;
            }

            _SyncGraphics();

            vh.Clear();
        }

        void _SyncGraphics()
        {
            if( syncGraphic == null )
            {
                return;
            }

            if( materialDirtyField != null )
                materialDirtyField.SetValue( syncGraphic, true );

            if( vertsDirtyField != null )
                vertsDirtyField.SetValue( syncGraphic, true );

            syncGraphic.Rebuild( CanvasUpdate.PreRender );
        }

#endif


    }

}
