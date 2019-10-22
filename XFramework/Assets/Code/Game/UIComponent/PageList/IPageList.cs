﻿using System;
using System.Collections.Generic;
using BDFramework.ResourceMgr;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BDFramework.UI
{
    public class PageListItemPool
    {
        private Queue<RectTransform> pool;

        private RectTransform item;

        public PageListItemPool(string poolName)
        {
            item = BResources.Load<GameObject>(poolName).transform as RectTransform;
            sizeDelta = new Vector2(item.sizeDelta.x, item.sizeDelta.y);
            pool = new Queue<RectTransform>();
        }

        public Vector2 sizeDelta;

        public void Push(RectTransform rt)
        {
            pool.Enqueue(rt);
        }

        public RectTransform Pop()
        {
            RectTransform rt;
            if (pool.Count == 0)
            {
                rt = GameObject.Instantiate(item).transform as RectTransform;
            }
            else
            {
                rt = pool.Dequeue();
            }

            return rt;
        }
    }

    public class IPageList : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Scrollbar scrollbar = null;
        public Slider slider = null;

        [SerializeField] public float stdWidth = 0;
        [SerializeField] public float stdHeight = 0;


        public static RectTransform GetItem(string name)
        {
            PageListItemPool pool;
            if (!dict.TryGetValue(name, out pool))
            {
                pool = new PageListItemPool(name);
                dict.Add(name, pool);
            }

            return pool.Pop();
        }


        public static void RecycleItem(string name, RectTransform rt)
        {
            PageListItemPool pool;
            if (!dict.TryGetValue(name, out pool))
            {
                pool = new PageListItemPool(name);
                dict.Add(name, pool);
            }

            pool.Push(rt);
        }

        public static Vector2 GetCellSizeDelta(string name)
        {
            PageListItemPool pool;
            if (!dict.TryGetValue(name, out pool))
            {
                pool = new PageListItemPool(name);
            }

            return pool.sizeDelta;
        }

        private static Dictionary<string, PageListItemPool> dict = new Dictionary<string, PageListItemPool>();

        //这个是给杰哥做的 已经被废弃 
        public float scaleX = 1;
        public float scaleY = 1;


        /// <summary>
        /// 滚动类型
        /// </summary>
        enum Direction
        {
            Horizontal, //水平方向
            Vertical //垂直方向
        }

        /// <summary>
        /// 滚动方向
        /// </summary>
        [SerializeField] Direction direction = Direction.Horizontal;

        private List<RectTransform> m_InstantiateItems = new List<RectTransform>();

        public System.Action<int, GameObject> onItemUpdateAction = null;
        public System.Action<int, GameObject> onItemRemoveAction = null;

        public System.Action onTopAction = null;
        public System.Action onBotAction = null;

        /// <summary>
        /// 不在显示区域多加载的数量(垂直滚动=>增加行 水平滚动=>增加列)
        /// </summary>
        [SerializeField, Range(2, 5)] private int m_BufferNo;

        /// <summary>
        /// 需要实例化的行列
        /// </summary>
        private Vector2 m_InstantiateSize = Vector2.zero;

        /// <summary>
        /// PageGrid数据
        /// </summary>
        private Vector2 m_Page;

        private Vector2 cellGap;

        private int dataCount;

        /// <summary>
        /// 
        /// </summary>
        private float m_PrevPos = 0;

        public Vector2 InstantiateSize
        {
            get
            {
                if (m_InstantiateSize == Vector2.zero)
                {
                    float rows, cols;
                    if (direction == Direction.Horizontal)
                    {
                        rows = m_Page.x;
                        cols = m_Page.y + (float) m_BufferNo;
                    }
                    else
                    {
                        rows = m_Page.x + (float) m_BufferNo;
                        cols = m_Page.y;
                    }

                    m_InstantiateSize = new Vector2(rows, cols);
                }

                return m_InstantiateSize;
            }
        }

        //一个item的宽高
        public Vector2 CellRect
        {
            get
            {
                return m_Cell != null
                    ? new Vector2(cellGap.x + m_Cell.x * cellScale.x,
                        cellGap.y + m_Cell.y * cellScale.y)
                    : new Vector2(100, 100);
            }
        }

        private float scale
        {
            get { return direction == Direction.Horizontal ? 1f : -1f; }
        }

        public int PageScale
        {
            get { return direction == Direction.Horizontal ? (int) m_Page.x : (int) m_Page.y; }
        }

        public int PageCount
        {
            get { return (int) m_Page.x * (int) m_Page.y; }
        }

        public int InstantiateCount
        {
            get { return (int) InstantiateSize.x * (int) InstantiateSize.y; }
        }


        public float CellScale
        {
            get { return direction == Direction.Horizontal ? CellRect.x : CellRect.y; }
        }

        /// <summary>
        /// content
        /// </summary>
        public float DirectionPos
        {
            get { return direction == Direction.Horizontal ? m_Rect.anchoredPosition.x : m_Rect.anchoredPosition.y; }
        }

        /// <summary>
        /// content rtf
        /// </summary>
        protected RectTransform m_Rect;

        protected RectTransform viewPort;

        private bool isInit = false;

        private void Init()
        {
            if (null != scrollbar)
            {
                scrollbar.value = 0;
                scrollbar.onValueChanged.RemoveAllListeners();
                scrollbar.onValueChanged.AddListener(ScrollbarChange);
            }

            if (null != slider)
            {
                slider.value = 0;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(SliderChange);
            }

            viewPort = transform.Find("ViewPort").GetComponent<RectTransform>();
            m_Rect = viewPort.Find("Content").GetComponent<RectTransform>();
            m_Rect.anchorMax = Vector2.up;
            m_Rect.anchorMin = Vector2.up;
            m_Rect.pivot = Vector2.up;
            m_Rect.anchoredPosition = new Vector2(0f, 0f);
            var _pageGrid = m_Rect.GetComponent<IPageGrid>();
            m_Page = _pageGrid.Page;
            cellGap = _pageGrid.CellGap;
            var curWidth = (this.transform as RectTransform).rect.width;
            var curHeight = (this.transform as RectTransform).rect.height;
            if (this.stdHeight == 0 || this.stdWidth == 0)
            {
                cellScale = Vector2.one;
            }
            else
            {
                var ws = curWidth / this.stdWidth;
                var hs = curHeight / this.stdHeight;
                var scale = ws > hs ? hs : ws;
                if (this.direction == Direction.Horizontal)
                {
                    if (this.m_Page.x > 1)
                    {
                        var gapY = (curHeight - this.m_Page.x * (m_Cell.y * scale)) / (this.m_Page.x - 1); //水平间隔重新算
                        if (gapY > this.cellGap.y)
                        {
                            scale = hs;
                            gapY = (curHeight - this.m_Page.x * (m_Cell.y * scale)) / (this.m_Page.x - 1);
                        }

                        var gapX = gapY != 0 ? (this.cellGap.y / gapY) * this.cellGap.x : this.cellGap.x;
                        gapX = gapX > this.cellGap.x ? cellGap.x : gapX;
                        this.cellGap = new Vector2(gapX, gapY);
                    }
                    else
                    {
                        scale = hs;
                    }
                }
                else
                {
                    if (this.m_Page.y > 1)
                    {
                        //垂直方向滚动 只考虑横向间距够不够 不够的话改变scale 防止scale缩小 间距太大
                        var gapX = (curWidth - this.m_Page.y * (m_Cell.x * scale)) / (this.m_Page.y - 1); //水平间隔重新算
                        if (gapX > this.cellGap.x)
                        {
                            scale = ws;
                            gapX = (curWidth - this.m_Page.y * (m_Cell.x * scale)) / (this.m_Page.y - 1);
                        }

                        var gapY = gapX != 0 ? (this.cellGap.x / gapX) * this.cellGap.y : this.cellGap.y;
                        gapY = gapY > this.cellGap.y ? cellGap.y : gapY;
                        this.cellGap = new Vector2(gapX, gapY);
                    }
                    else
                    {
                        //只有一列垂直滚动的时候 就是充满不解释
                        scale = ws;
                    }
                }

                cellScale = new Vector2(scale, scale);
            }
        }


        //屏幕适配之后item要修改自身宽高
        private Vector2 cellScale;

        private Vector2 m_Cell;

        private int m_CurrentIndex; //当前页显示区域的第一行（列）在整个conten中的位置

        /// <summary>
        /// 重置pagelist
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < m_InstantiateItems.Count; i++)
            {
                m_InstantiateItems[i].gameObject.SetActive(false);
//                m_oldItems.Add(m_InstantiateItems[i]);
                RemoveItem(m_InstantiateItems[i]);
                IPageList.RecycleItem(this.prefabName, m_InstantiateItems[i]);
            }

            m_InstantiateItems.Clear();
            m_PrevPos = 0;
            m_CurrentIndex = 0;
//            (m_Rect.transform as RectTransform).anchoredPosition = Vector3.zero;
            m_Rect.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 由格子数量获取多少行多少列 水平计算行 垂直计算列
        /// </summary>
        /// <param name="num"></param>格子个数
        /// <returns></returns>
        private Vector2 GetRectByNum(int num)
        {
            return direction == Direction.Horizontal
                ? new Vector2(m_Page.x, Mathf.CeilToInt(num / m_Page.x))
                : new Vector2(Mathf.CeilToInt(num / m_Page.y), m_Page.y);
        }


        private void moveItemToIndex(int index, RectTransform item, bool onlyPos = false)
        {
            item.anchoredPosition = GetPosByIndex(index, item);
            if (!onlyPos)
                UpdateItem(index, item.gameObject);
        }


        /// <summary>
        /// 最大移动距离
        /// </summary>
        public float MaxPrevPos
        {
            get
            {
                float result;
                Vector2 max = GetRectByNum(dataCount);
                if (direction == Direction.Horizontal)
                {
                    result = max.y * CellScale - viewPort.rect.width / 2;
                }
                else
                {
                    result = max.x * CellScale - viewPort.rect.height / 2;
                }

                return result;
            }
        }

        private void CreateItem(int index)
        {
            RectTransform item = null;
//            if (m_oldItems.Count > 0)
//            {
//                //从回收池中获得item
//                item = m_oldItems[0];
//                m_oldItems.Remove(item);
//            }
//            else
//            {
            //创建新的item
            item = IPageList.GetItem(this.prefabName);
            item.SetParent(m_Rect, false);
            item.anchorMax = Vector2.up;
            item.anchorMin = Vector2.up;
            item.pivot = Vector2.up;
            item.localScale = new Vector3(scaleX * cellScale.x, scaleY * cellScale.y, 1);
//            }

            item.name = "item" + index;
            item.anchoredPosition = GetPosByIndex(index, item);
            m_InstantiateItems.Add(item);
            item.gameObject.SetActive(true);
            //updateItem(index, item.gameObject);
        }

        private void UpdateItem(int index, GameObject item)
        {
            item.SetActive(index < dataCount);
            if (item.activeSelf)
            {
                if (onItemUpdateAction != null)
                    onItemUpdateAction(index, item);
            }

            IPageListItem listItem = item.GetComponent<IPageListItem>();
            listItem.index = index;
        }

        /// <summary>
        /// 设置content的大小
        /// </summary>
        /// </summary>
        /// <param name="rows"></param>行数
        /// <param name="cols"></param>列数
        private void SetBound(Vector2 bound)
        {
            m_Rect.sizeDelta = new Vector2(bound.y * CellRect.x - cellGap.x, bound.x * CellRect.y - cellGap.y);
        }

        private Vector2 GetPosByIndex(int index, RectTransform item)
        {
            float x, y;
            if (direction == Direction.Horizontal)
            {
                x = index % m_Page.x;
                y = Mathf.FloorToInt(index / m_Page.x);
            }
            else
            {
                x = Mathf.FloorToInt(index / m_Page.y);
                y = index % m_Page.y;
            }

            return new Vector2(y * CellRect.x, -x * CellRect.y);
        }

        PointerEventData mPointerEventData = null;

        void CacheDragPointerEventData(PointerEventData eventData)
        {
            if (mPointerEventData == null)
            {
                mPointerEventData = new PointerEventData(EventSystem.current);
            }

            mPointerEventData.button = eventData.button;
            mPointerEventData.position = eventData.position;
            mPointerEventData.pointerPressRaycast = eventData.pointerPressRaycast;
            mPointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        }

        private string prefabName;

        // <summary>
        /// display
        /// </summary>
        /// <param name="data">Data.</param>
        public void Data(int dataCount, string prefabName)
        {
            // 刷新scrollbar
            if (null != scrollbar)
            {
                scrollbar.value = 0;
            }

            if (null != slider)
            {
                slider.value = 0;
            }

            if (!string.IsNullOrEmpty(prefabName))
            {
                if (this.prefabName != prefabName)
                {
                    this.prefabName = prefabName;
                    m_Cell = IPageList.GetCellSizeDelta(this.prefabName);
                }
            }

            if (isInit == false)
            {
                Init();
                isInit = true;
            }

            if (this.dataCount != dataCount)
            {
                Reset();
            }
            else
            {
                int count = Mathf.Min(m_InstantiateItems.Count, dataCount);
                for (int i = m_CurrentIndex, j = 0; i < m_CurrentIndex + count; i++, j++)
                {
                    RemoveItem(m_InstantiateItems[j]);
                }
            }

            this.dataCount = dataCount;


            if (dataCount > PageCount)
            {
                SetBound(GetRectByNum(dataCount));
            }
            else
            {
                SetBound(m_Page);
            }

            //当数据的大小超过初始化预计大小
            if (dataCount > InstantiateCount)
            {
                while (m_InstantiateItems.Count < InstantiateCount)
                {
                    CreateItem(m_InstantiateItems.Count);
                }
            }
            else
            {
                while (m_InstantiateItems.Count > dataCount)
                {
                    RecycleItem(m_InstantiateItems.Count - 1);
                }

                while (m_InstantiateItems.Count < dataCount)
                {
                    CreateItem(m_InstantiateItems.Count);
                }
            }

            if (dataCount > 0)
            {
                int count = Mathf.Min(m_InstantiateItems.Count, dataCount);
                for (int i = m_CurrentIndex, j = 0; i < m_CurrentIndex + count; i++, j++)
                {
                    UpdateItem(i, m_InstantiateItems[j].gameObject);
                }
            }

//            ShowHideBar();
//            ShowHideSlider();
        }

        private void RecycleItem(int index)
        {
            RectTransform item = m_InstantiateItems[index];
            m_InstantiateItems.Remove(item);
            item.gameObject.SetActive(false);
//            m_oldItems.Add(item);
            IPageList.RecycleItem(this.prefabName, item);
        }

        private void RemoveItem(RectTransform item)
        {
            IPageListItem listItem = item.transform.GetComponent<IPageListItem>();
            if (onItemRemoveAction != null)
                onItemRemoveAction(listItem.index, item.gameObject);
        }


        virtual public void OnBeginDrag(PointerEventData eventData)
        {
            CacheDragPointerEventData(eventData);
            isDrag = true;
        }

        /// <summary>
        /// 控制pagelist是否需要自动旋转到中间位置
        /// true后可以拉动区域增加,拖动后自动对准到选择item中心
        /// pagelist中只有一个item的话拖不动
        /// </summary>
        virtual public void OnEndDrag(PointerEventData eventData)
        {
            mPointerEventData = null;
            OnUpdateScrollBar();
            OnUpdateSlider();
            HandleOffset();
            isDrag = false;
        }

        /// <summary>
        /// 移动到具体item
        /// </summary>
        /// <param name="index"></param>
        public void MoveToItemByIndex(int index)
        {
            int _tmpCount = index / PageScale;
            if (this.direction == Direction.Horizontal)
            {
                var max = -viewPort.rect.width / 2;
                var _offset = -_tmpCount * CellRect.x - viewPort.rect.width / 2;
                _offset = Mathf.Max(_offset, -MaxPrevPos);
                _offset = Mathf.Min(_offset, max);
                m_Rect.localPosition = new Vector2(_offset, m_Rect.localPosition.y);
            }
            else
            {
                var min = viewPort.rect.height / 2;
                var _offset = _tmpCount * CellRect.y + viewPort.rect.height / 2;
                _offset = Mathf.Min(_offset, MaxPrevPos);
                _offset = Math.Max(min, _offset);
                m_Rect.localPosition = new Vector2(m_Rect.localPosition.x, _offset);
            }

            OnUpdateScrollBar();
            OnUpdateSlider();
            HandleOffset();
        }

        //系数
        [Range(0, 1)] [SerializeField] public float coefficient = 0.1f;

        virtual public void OnDrag(PointerEventData eventData)
        {
            if (this.dataCount == 0) return;
            //处理滚动边界
            float _offset = 0f;
            if (this.direction == Direction.Horizontal)
            {
                _offset = ((eventData.position.x - mPointerEventData.position.x) * coefficient +
                           m_Rect.localPosition.x);

                if (_offset < -MaxPrevPos)
                {
                    if (null != onBotAction)
                    {
                        onBotAction();
                    }
                }

                if (_offset > 0)
                {
                    if (null != onTopAction)
                    {
                        onTopAction();
                    }
                }

                _offset = Mathf.Min(Mathf.Max(_offset, -MaxPrevPos), -viewPort.rect.width / 2);
                m_Rect.localPosition = new Vector2(_offset, m_Rect.localPosition.y);
            }
            else
            {
                _offset = (eventData.position.y - mPointerEventData.position.y) * coefficient + m_Rect.localPosition.y;
                if (_offset > MaxPrevPos)
                {
                    if (null != onBotAction)
                    {
                        onBotAction();
                    }
                }

                if (_offset < 0)
                {
                    if (null != onTopAction)
                    {
                        onTopAction();
                    }
                }

                _offset = Mathf.Max(Mathf.Min(_offset, MaxPrevPos), viewPort.rect.height / 2);
                m_Rect.localPosition = new Vector2(m_Rect.localPosition.x, _offset);
            }

            OnUpdateScrollBar();
            OnUpdateSlider();
            HandleOffset();
        }

        private void HandleOffset()
        {
            //处理移出显示区域的item到下面
            while (scale * DirectionPos - m_PrevPos < -CellScale * 2)
            {
                //偏移距离大于两个item时
                if (m_PrevPos <= -MaxPrevPos) return;

                m_PrevPos -= CellScale;

                List<RectTransform> range = m_InstantiateItems.GetRange(0, PageScale);
                m_InstantiateItems.RemoveRange(0, PageScale);
                m_InstantiateItems.AddRange(range);
                foreach (RectTransform removeItem in range)
                {
                    RemoveItem(removeItem);
                }

                for (int i = 0; i < range.Count; i++)
                {
                    moveItemToIndex(m_CurrentIndex * PageScale + m_InstantiateItems.Count + i, range[i]);
                }

                m_CurrentIndex++;
            }

            //处理移出显示区域的item到上面
            while (scale * DirectionPos - m_PrevPos > -CellScale)
            {
                //回移超过一个item
                if (Mathf.RoundToInt(m_PrevPos) >= 0) return;

                m_PrevPos += CellScale;

                m_CurrentIndex--;

                if (m_CurrentIndex < 0) return;

                List<RectTransform> range =
                    m_InstantiateItems.GetRange(m_InstantiateItems.Count - PageScale, PageScale);
                m_InstantiateItems.RemoveRange(m_InstantiateItems.Count - PageScale, PageScale);
                m_InstantiateItems.InsertRange(0, range);

                foreach (RectTransform removeItem in range)
                {
                    RemoveItem(removeItem);
                }

                for (int i = 0; i < range.Count; i++)
                {
                    moveItemToIndex(m_CurrentIndex * PageScale + i, range[i]);
                }
            }
        }

        public void UpdateItemByIndex(int index)
        {
            var tmp = this.direction == Direction.Horizontal ? m_Page.x : m_Page.y;
            var offset = index - m_CurrentIndex * (int) tmp;
            if (offset >= 0 && offset < m_InstantiateItems.Count)
                UpdateItem(index, m_InstantiateItems[offset].gameObject);
        }


        #region ////bar slider////

        private bool isDrag;

        private void ScrollbarChange(float value)
        {
            if (isDrag) return;
            if (null == m_Rect) return;
            var pRect = this.transform as RectTransform;
            if (Direction.Horizontal == direction)
            {
                var scrollLength = m_Rect.rect.width + cellGap.x - pRect.rect.width;
                if (scrollLength <= 0) return;
                float x = scrollLength * scrollbar.value;
                m_Rect.anchoredPosition = new Vector2(-x, m_Rect.anchoredPosition.y);
            }
            else
            {
                var scrollLength = m_Rect.rect.height + cellGap.y - pRect.rect.height;
                if (scrollLength <= 0) return;
                float y = scrollLength * scrollbar.value;
                m_Rect.anchoredPosition = new Vector2(m_Rect.anchoredPosition.x, y);
            }

            HandleOffset();
        }

        private void ShowHideBar()
        {
            if (null == scrollbar) return;
            var pRect = this.transform as RectTransform;
            if (Direction.Horizontal == direction)
            {
                var scrollLength = m_Rect.rect.width - pRect.rect.width;
                if (scrollLength <= 0)
                {
                    return;
                }
            }
            else
            {
                var scrollLength = m_Rect.rect.height - pRect.rect.height;
                if (scrollLength <= 0)
                {
                    return;
                }
            }

            scrollbar.gameObject.SetActive(true);
        }

        private void OnUpdateScrollBar()
        {
            if (null == scrollbar) return;

            float offset = 0f;
            var pRect = this.transform as RectTransform;
            if (Direction.Horizontal == direction)
            {
                var scrollLength = m_Rect.rect.width + cellGap.x - pRect.rect.width;
                if (scrollLength <= 0) return;
                if (Math.Abs(m_Rect.anchoredPosition.x) > 0.01f)
                {
                    offset = Math.Abs(
                        m_Rect.anchoredPosition.x / scrollLength);
                }
            }
            else
            {
                var scrollLength = m_Rect.rect.height + cellGap.y - pRect.rect.height;
                if (scrollLength <= 0) return;
                if (m_Rect.anchoredPosition.y > 0.01f)
                {
                    offset = Math.Abs(m_Rect.anchoredPosition.y / scrollLength);
                }
            }

            if (offset > 1.00f)
            {
                offset = 1.00f;
            }

            scrollbar.value = offset;
        }

        void ShowHideSlider()
        {
            if (null == slider) return;
            var pRect = this.transform as RectTransform;
            if (Direction.Horizontal == direction)
            {
                var scrollLength = m_Rect.rect.width - pRect.rect.width;
                if (scrollLength <= 0)
                {
                    slider.handleRect.gameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                var scrollLength = m_Rect.rect.height - pRect.rect.height;
                if (scrollLength <= 0)
                {
                    slider.handleRect.gameObject.SetActive(false);
                    return;
                }
            }

            slider.handleRect.gameObject.SetActive(true);
        }

        void SliderChange(float value)
        {
            if (isDrag) return;
            if (null == m_Rect) return;
            var pRect = this.transform as RectTransform;
            if (Direction.Horizontal == direction)
            {
                var scrollLength = m_Rect.rect.width + cellGap.x - pRect.rect.width;
                if (scrollLength <= 0) return;
                float x = scrollLength * slider.value;
                m_Rect.anchoredPosition = new Vector2(-x, m_Rect.anchoredPosition.y);
            }
            else
            {
                var scrollLength = m_Rect.rect.height + cellGap.y - pRect.rect.height;
                if (scrollLength <= 0) return;
                float y = scrollLength * slider.value;
                m_Rect.anchoredPosition = new Vector2(m_Rect.anchoredPosition.x, y);
            }

            HandleOffset();
        }

        private void OnUpdateSlider()
        {
            if (null == slider) return;

            float offset = 0f;
            var pRect = this.transform as RectTransform;
            if (Direction.Horizontal == direction)
            {
                var scrollLength = m_Rect.rect.width + cellGap.x - pRect.rect.width;
                if (scrollLength <= 0) return;
                if (Math.Abs(m_Rect.anchoredPosition.x) > 0.01f)
                {
                    offset = Math.Abs(
                        m_Rect.anchoredPosition.x / scrollLength);
                }
            }
            else
            {
                var scrollLength = m_Rect.rect.width + cellGap.y - pRect.rect.height;
                if (scrollLength <= 0) return;
                if (m_Rect.anchoredPosition.y > 0.01f)
                {
                    offset = Math.Abs(m_Rect.anchoredPosition.y / scrollLength);
                }
            }

            if (offset > 1.00f)
            {
                offset = 1.00f;
            }

            slider.value = offset;
        }

        public List<int> Getm_InstantiateItemsIndex()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < m_InstantiateItems.Count; i++)
            {
                var item = m_InstantiateItems[i].GetComponent<IPageListItem>();
                list.Add(item.index);
            }

            return list;
        }

        #endregion
    }
}