using System.Collections;
using System.Collections.Generic;
using BDFramework.UI;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    public static class UICraeteTool
    {

        [MenuItem("GameObject/UI/IButton", false, 2151)]
        static public void AddIButton(MenuCommand menuCommand)
        {
            GameObject go = CreateButton();
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/IPageList", false, 2152)]
        static public void AddIPageList(MenuCommand menuCommand)
        {
            GameObject go = CreatePageList();
            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem("GameObject/UI/SelectPageList",false,2153)]
        public static void AddSPageList(MenuCommand menuCommand)
        {
            GameObject go = CreateSelectPageList();
            PlaceUIElementRoot(go,menuCommand);
        }
        
        public static GameObject CreateButton()
        {
            GameObject root = CreateUIElementRoot("Btn_", new Vector2(200, 100));
            root.AddComponent<IButton>();
            root.AddComponent<Image>();
            return root;
        }

        public static GameObject CreatePageList()
        {
            GameObject root = CreateUIElementRoot("iPageList", new Vector2(100, 100));
            root.AddComponent<IPageList>();
            RectTransform rootRT = root.GetComponent<RectTransform>();
            rootRT.anchorMin = new Vector2(0f, 1);
            rootRT.anchorMax = new Vector2(0f, 1);
            rootRT.pivot = new Vector2(0f, 1);
            GameObject viewPort = CreateUIObject("ViewPort", root);
            viewPort.AddComponent<Image>();
            Mask mask = viewPort.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            RectTransform viewPortRT = viewPort.GetComponent<RectTransform>();
            viewPortRT.anchorMin = new Vector2(0f, 0);
            viewPortRT.anchorMax = new Vector2(1, 1);
            viewPortRT.pivot = new Vector2(0.5f, 0.5f);
            viewPortRT.offsetMax = viewPortRT.offsetMin = Vector2.zero;
            GameObject content = CreateUIObject("Content", viewPort);
            content.AddComponent<IPageGrid>();
            return root;
        }
        
        public static GameObject CreateSelectPageList()
        {
            GameObject root = CreateUIElementRoot("sPageList", new Vector2(100, 100));
            root.AddComponent<SelectPageList>();
            RectTransform rootRT = root.GetComponent<RectTransform>();
            rootRT.anchorMin = new Vector2(0f, 1);
            rootRT.anchorMax = new Vector2(0f, 1);
            rootRT.pivot = new Vector2(0f, 1);
            GameObject viewPort = CreateUIObject("ViewPort", root);
            viewPort.AddComponent<Image>();
            Mask mask = viewPort.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            RectTransform viewPortRT = viewPort.GetComponent<RectTransform>();
            viewPortRT.anchorMin = new Vector2(0f, 0);
            viewPortRT.anchorMax = new Vector2(1, 1);
            viewPortRT.pivot = new Vector2(0.5f, 0.5f);
            viewPortRT.offsetMax = viewPortRT.offsetMin = Vector2.zero;
            GameObject content = CreateUIObject("Content", viewPort);
            content.AddComponent<IPageGrid>();
            return root;
        }

        #region ctrl c ctrl v

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            if (parent != menuCommand.context) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(),
                    element.GetComponent<RectTransform>());

            Selection.activeGameObject = element;
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        static public GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            // CreateEventSystem(false);
            return root;
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform,
                new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) +
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) +
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) -
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) -
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        #endregion
    }
}