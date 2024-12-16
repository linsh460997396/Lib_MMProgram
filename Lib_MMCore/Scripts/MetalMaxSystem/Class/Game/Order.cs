﻿#if UNITY_EDITOR|| UNITY_STANDALONE
//Unity编辑器、独立应用程序（不包括Web播放器）
using Vector2F = UnityEngine.Vector2;
#elif MonoGame
//使用VS2022的MonoGame插件框架
using Vector2F = Microsoft.Xna.Framework.Vector2;
#else
using Vector2F = System.Numerics.Vector2;
#endif

namespace MetalMaxSystem
{
    /// <summary>
    /// 指令
    /// </summary>
    public class Order
    {
        public int Player { get; set; }
        public int Index { get; set; }
        public string Link { get; set; }
        public Unit TargetUnit { get; set; }
        private Vector2F _targetVector;
        public Vector2F TargetVector
        {
            get
            {
                return _targetVector;
            }

            set
            {
                _targetVector = value;
            }
        }

        /// <summary>
        /// 目标类型：0=无，1=单位，2=二维点，3=物品
        /// </summary>
        public int TargetType { get; set; }

        public Order(Abilcmd cmd)
        {
            Link = cmd.Link;
            Index = cmd.Index;
        }

    }
}
