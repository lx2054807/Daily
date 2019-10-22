using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BDFramework.UI
{
    public class IPageListDrag : IPageList
    {
        private float offsetY = 50f; //上下拖拽多少量,触发子项拖动

        private RectTransform dragParent;
        private RectTransform dragItem;

        private Vector3 originalPos; //拖动的物体原来所在的位置
        private float dragY;

        private bool isLock = false; //true: 锁住PageList

        private Action<PointerEventData> dragBeginAction; //拖动开始回调
        private Action<PointerEventData> dragIngAction; //拖动中回调
        private Action<PointerEventData> dragEndAction; //拖动结束回调

        override public void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            if (null != dragBeginAction)
            {
                dragBeginAction(eventData);
            }

            isLock = false;
            dragY = 0;
        }

        override public void OnDrag(PointerEventData eventData)
        {
            if (!isLock)
            {
                if (null != dragItem)
                {
                    dragY += eventData.delta.y;
                    if (Math.Abs(dragY) >= offsetY)
                    {
                        originalPos = dragItem.localPosition;
                        isLock = true;
                    }
                }

                base.OnDrag(eventData);
            }
            else
            {
                if (null != dragItem)
                {
                    if (dragItem.parent != dragParent)
                    {
                        dragItem.SetParent(dragParent);
                    }

                    dragItem.localPosition =
                        new Vector3(dragItem.localPosition.x + eventData.delta.x,
                            dragItem.localPosition.y + eventData.delta.y, 0);

                    if (null != dragIngAction)
                    {
                        dragIngAction(eventData);
                    }
                }
            }
        }

        override public void OnEndDrag(PointerEventData eventData)
        {
            if (!isLock)
            {
                base.OnEndDrag(eventData);
            }
            else
            {
                if (null != dragItem)
                {
                    if (dragItem.parent != m_Rect)
                    {
                        dragItem.SetParent(m_Rect);
                    }

                    dragItem.localPosition = originalPos;

                    if (null != dragEndAction)
                    {
                        dragEndAction(eventData);
                    }
                }
            }

            isLock = false;
        }


        public void RegisterDragBeginAction(Action<PointerEventData> action)
        {
            this.dragBeginAction = action;
        }

        public void RegisterDragIngAction(Action<PointerEventData> action)
        {
            this.dragIngAction = action;
        }

        public void RegisterDragEndAction(Action<PointerEventData> action)
        {
            this.dragEndAction = action;
        }

        /// <summary>
        /// 需要拖动的物体处于哪个层级
        /// </summary>
        /// <param name="dragParent"></param>
        public void SetDragParent(RectTransform dragParent)
        {
            this.dragParent = dragParent;
        }

        /// <summary>
        /// 需要拖动的物体
        /// </summary>
        /// <param name="dargItem"></param>
        public void SetDragItem(RectTransform dargItem)
        {
            this.dragItem = dargItem;
        }

        /// <summary>
        /// 外界获取该PageList是否锁住
        /// </summary>
        /// <returns></returns>
        public bool GetLockState()
        {
            return isLock;
        }
    }
}