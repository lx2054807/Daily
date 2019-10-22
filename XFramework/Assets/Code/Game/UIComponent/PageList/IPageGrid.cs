﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BDFramework.UI
{
    public class IPageGrid : MonoBehaviour
    {
        /// <summary>
        /// 必须设置；显示区域几行几列
        /// </summary>
        [SerializeField]
        private Vector2 m_Page = new Vector2(1, 1);

        /// <summary>
        /// item之间间隔
        /// </summary>
        [SerializeField]
        private Vector2 cellGap = new Vector2(0f, 0f);

        public Vector2 CellGap
        {
            get { return cellGap; }
            set { m_Page = value; }
        }

        public Vector2 Page
        {
            get { return m_Page; }
            set { m_Page = value; }
        }
    }
}