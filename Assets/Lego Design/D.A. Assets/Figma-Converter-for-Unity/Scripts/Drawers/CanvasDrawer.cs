﻿using DA_Assets.FCU.Drawers.CanvasDrawers;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


#if TextMeshPro
using TMPro;
#endif

#pragma warning disable IDE0003
#pragma warning disable CS0649



namespace DA_Assets.FCU.Drawers
{
    public delegate IEnumerator DrawByTag(FObject fobject, FcuTag tag, Action onDraw);

    [Serializable]
    public class CanvasDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator DrawToCanvas(List<FObject> fobjects)
        {
            monoBeh.AssetTools.SelectFcu();

            this.TextDrawer.Init();
            this.ButtonDrawer.Init();
            this.ToggleDrawer.Init();
            this.InputFieldDrawer.Init();
            this.ScrollViewDrawer.Init();
            this.I2LocalizationDrawer.AddLanguageSource();

            yield return DrawComponents(fobjects, DrawByTag);

            yield return this.ButtonDrawer.SetTargetGraphics();
            yield return this.ToggleDrawer.SetTargetGraphics();
            yield return this.InputFieldDrawer.SetTargetGraphics();
            yield return this.ScrollViewDrawer.SetTargetGraphics();

            yield return FixAutolayoutMargins(fobjects);
        }

        private IEnumerator DrawByTag(FObject fobject, FcuTag tag, Action onDraw)
        {
            try
            {
                if (fobject.Data.GameObject == null)
                {
                    yield break;
                }

                switch (tag)
                {
                    case FcuTag.Shadow:
                        this.ShadowDrawer.Draw(fobject);
                        break;
                    case FcuTag.AutoLayoutGroup:
                        this.AutoLayoutDrawer.Draw(fobject);
                        break;
                    case FcuTag.ContentSizeFitter:
                        this.ContentSizeFitterDrawer.Draw(fobject);
                        break;
                    case FcuTag.AspectRatioFitter:
                        this.AspectRatioFitterDrawer.Draw(fobject);
                        break;
                    case FcuTag.InputField:
                    case FcuTag.PasswordField:
                        this.InputFieldDrawer.Draw(fobject);
                        break;
                    case FcuTag.ScrollView:
                        this.ScrollViewDrawer.Draw(fobject);
                        break;
                    case FcuTag.Toggle:
                    case FcuTag.ToggleGroup:
                        this.ToggleDrawer.Draw(fobject);
                        break;
                    case FcuTag.Button:
                        this.ButtonDrawer.Draw(fobject);
                        break;
                    case FcuTag.Mask:
                        this.MaskDrawer.Draw(fobject);
                        break;
                    case FcuTag.CanvasGroup:
                        this.CanvasGroupDrawer.Draw(fobject);
                        break;
                    case FcuTag.Placeholder:
                    case FcuTag.Text:
                        this.TextDrawer.Draw(fobject);
                        this.I2LocalizationDrawer.AddI2Localize(fobject);
                        break;
                    case FcuTag.Image:
                        this.ImageDrawer.Draw(fobject);
                        break;
                }
            }
            catch (Exception ex)
            {
                DALogger.LogError(FcuLocKey.log_cant_draw_object.Localize(fobject.Data.NameHierarchy));
                Debug.LogException(ex);
            }

            onDraw.Invoke();
            yield return null;
        }

        public IEnumerator FixSpriteRenderers(List<FObject> fobjects)
        {
            List<Transform> frames = monoBeh.transform.GetTopLevelChilds();

            int maxOrder = 32767;

            foreach (Transform frame in frames)
            {
                int initialOrder = 0;
                SetOrderInLayerRecursively(frame, ref initialOrder);
            }

            void SetOrderInLayerRecursively(Transform trans, ref int order)
            {
                SpriteRenderer spriteRenderer = trans.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = order;
                    order += monoBeh.Settings.SpriteRendererSettings.NextOrderStep;
                    if (order > maxOrder)
                    {
                        order = maxOrder;
                    }
                }

                for (int i = 0; i < trans.childCount; i++)
                {
                    SetOrderInLayerRecursively(trans.GetChild(i), ref order);
                }
            }

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (!fobject.ContainsTag(FcuTag.Image))
                    continue;

                if (!fobject.Data.GameObject.TryGetComponentSafe(out SpriteRenderer _))
                    continue;

                fobject.Data.GameObject.SetActive(false);
                yield return WaitFor.Delay001();
                fobject.Data.GameObject.SetActive(true);
            }
        }

        public IEnumerator FixDttImages(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (!fobject.ContainsTag(FcuTag.Image))
                    continue;

#if PROCEDURAL_UI_ASSET_STORE_RELEASE
                if (!fobject.Data.GameObject.TryGetComponentSafe(out DTT.UI.ProceduralUI.GradientEffect _))
                    continue;

#if UNITY_EDITOR
                UnityEditor.Selection.activeGameObject = fobject.Data.GameObject;
#endif
                yield return WaitFor.Delay01();
#endif
            }

            monoBeh.AssetTools.SelectFcu();
            Scene activeScene = SceneManager.GetActiveScene();
            activeScene.SetExpanded(false);
            yield return null;
        }

        private IEnumerator FixAutolayoutMargins(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (!fobject.IsInsideAutoLayout(out FObject parent, out var layoutGroup, monoBeh))
                    continue;

                int leftp = layoutGroup.padding.left;
                int rightp = layoutGroup.padding.right;

                float newX = fobject.Size.x;
                float newY = fobject.Size.y;

                float parentSize = parent.Data.Size.x;

                if (leftp + rightp + newX > parentSize)
                {
                    float excess = (leftp + rightp + newX) - parentSize;
                    float totalPadding = leftp + rightp;

                    float leftFactor = leftp / totalPadding;
                    float rightFactor = rightp / totalPadding;

                    int newLeft = leftp - (int)Math.Floor(excess * leftFactor);
                    int newRight = rightp - (int)Math.Floor(excess * rightFactor);

                    if (newLeft > 0 && newRight > 0)
                    {
                        layoutGroup.padding.left = newLeft;
                        layoutGroup.padding.right = newRight;
                    }
                }
            }

            yield return null;
        }

        private IEnumerator FixTextSizes(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (!fobject.ContainsTag(FcuTag.Text))
                    continue;

                if (fobject.Data.GameObject == null)
                    continue;

                RectTransform rt = fobject.Data.GameObject.GetComponent<RectTransform>();

                Vector2 rectSize = new Vector2(rt.rect.width, rt.rect.height);
                Vector2 m = new Vector2((fobject.Size.x - rectSize.x) / 2f, (fobject.Size.y - rectSize.y) / 2f);

                Vector4 marginV4 = new Vector4();

                if (fobject.Size.y > rectSize.y && fobject.Size.x > rectSize.x)
                {
                    marginV4 = new Vector4(m.x, m.y, m.x, m.y);
                }
                else if (fobject.Size.y > rectSize.y)
                {
                    marginV4 = new Vector4(0, m.y, 0, m.y);
                }
                else if (fobject.Size.x > rectSize.x)
                {
                    marginV4 = new Vector4(m.x, 0, m.x, 0);
                }

#if TextMeshPro
                if (fobject.Data.GameObject.TryGetComponentSafe(out TMP_Text text))
                    text.margin = marginV4;
#endif
            }

            yield return null;
        }

        public IEnumerator DrawComponents(List<FObject> fobjects, DrawByTag drawByTag)
        {
            Array fcuTags = Enum.GetValues(typeof(FcuTag));

            foreach (FcuTag tag in fcuTags)
            {
                if (tag.GetTagConfig().HasComponent == false)
                    continue;

                int drawnObjectsCount = 0;
                int objectsToDrawCount = monoBeh.TagSetter.TagsCounter[tag];

                if (objectsToDrawCount < 1)
                    continue;

                DACycles.ForEach(fobjects, fobject =>
                {
                    if (fobject.ContainsTag(tag) == false)
                        return;

                    Action onDraw = () =>
                    {
                        drawnObjectsCount++;
                        monoBeh.Events.OnAddComponent?.Invoke(monoBeh, fobject.Data, tag);
                    };

                    drawByTag(fobject, tag, onDraw).StartDARoutine(monoBeh);

                }, WaitFor.Delay001().WaitTimeF, 150).StartDARoutine(monoBeh);

                int tempCount = -1;
                while (FcuLogger.WriteLogBeforeEqual(
                    ref drawnObjectsCount,
                    ref objectsToDrawCount,
                    FcuLocKey.log_drawn_count.Localize($"{tag}", drawnObjectsCount, objectsToDrawCount),
                    ref tempCount))
                {
                    yield return WaitFor.Delay1();
                }
            }
        }



        // TODO: add referenceResolution
        public void AddCanvasComponent()
        {
            monoBeh.gameObject.TryAddComponent(out Canvas c);
            c.renderMode = RenderMode.ScreenSpaceOverlay;

            if (monoBeh.gameObject.TryGetComponentSafe(out CanvasScaler cs))
                cs.enabled = false;

            monoBeh.gameObject.TryAddComponent(out GraphicRaycaster gr);

            if (MonoBehExtensions.IsExistsOnScene<EventSystem>() == false)
            {
                GameObject go = MonoBehExtensions.CreateEmptyGameObject();
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
                go.name = nameof(EventSystem);
            }
        }

        internal IEnumerator FixJoshPui()
        {
            List<Transform> frames = monoBeh.transform.GetTopLevelChilds();

            foreach (Transform frame in frames)
            {
                frame.gameObject.SetActive(false);
                yield return WaitFor.Delay01();
                frame.gameObject.SetActive(true);
            }
        }

        [SerializeField] ImageDrawer imageDrawer;
        [SerializeProperty(nameof(imageDrawer))]
        public ImageDrawer ImageDrawer => imageDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] TextDrawer textDrawer;
        [SerializeProperty(nameof(textDrawer))]
        public TextDrawer TextDrawer => textDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] AutoLayoutDrawer autoLayoutDrawer;
        [SerializeProperty(nameof(autoLayoutDrawer))]
        public AutoLayoutDrawer AutoLayoutDrawer => autoLayoutDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] ContentSizeFitterDrawer contentSizeFitterDrawer;
        [SerializeProperty(nameof(contentSizeFitterDrawer))]
        public ContentSizeFitterDrawer ContentSizeFitterDrawer => contentSizeFitterDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] AspectRatioFitterDrawer aspectRatioFitterDrawer;
        [SerializeProperty(nameof(aspectRatioFitterDrawer))]
        public AspectRatioFitterDrawer AspectRatioFitterDrawer => aspectRatioFitterDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] MaskDrawer maskDrawer;
        [SerializeProperty(nameof(maskDrawer))]
        public MaskDrawer MaskDrawer => maskDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] ToggleDrawer toggleDrawer;
        [SerializeProperty(nameof(toggleDrawer))]
        public ToggleDrawer ToggleDrawer => toggleDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] ButtonDrawer buttonDrawer;
        [SerializeProperty(nameof(buttonDrawer))]
        public ButtonDrawer ButtonDrawer => buttonDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] ScriptGenerator scriptGenerator;
        [SerializeProperty(nameof(scriptGenerator))]
        public ScriptGenerator ScriptGenerator => scriptGenerator.SetMonoBehaviour(monoBeh);

        [SerializeField] InputFieldDrawer inputFieldDrawer;
        [SerializeProperty(nameof(inputFieldDrawer))]
        public InputFieldDrawer InputFieldDrawer => inputFieldDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] ScrollViewDrawer scrollViewDrawer;
        [SerializeProperty(nameof(scrollViewDrawer))]
        public ScrollViewDrawer ScrollViewDrawer => scrollViewDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] I2LocalizationDrawer i2LocalizationDrawer;
        [SerializeProperty(nameof(i2LocalizationDrawer))]
        public I2LocalizationDrawer I2LocalizationDrawer => i2LocalizationDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] ShadowDrawer shadowDrawer;
        [SerializeProperty(nameof(shadowDrawer))]
        public ShadowDrawer ShadowDrawer => shadowDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] CanvasGroupDrawer canvasGroupDrawer;
        [SerializeProperty(nameof(canvasGroupDrawer))]
        public CanvasGroupDrawer CanvasGroupDrawer => canvasGroupDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] GameObjectDrawer gameObjectDrawer;
        [SerializeProperty(nameof(gameObjectDrawer))]
        public GameObjectDrawer GameObjectDrawer => gameObjectDrawer.SetMonoBehaviour(monoBeh);
    }
}
