using System.Collections.Generic;

namespace MetalMaxSystem
{
    public partial class MMCore
    {
        #region 常量定义
        private static float c_xuLiZengLiang = 1.0f;
        private static float c_shuangJiZengLiang = 0.0625f;
        private static float c_shuangJiShiXian = 0.25f;
        #endregion

        #region 状态变量
        private static bool _xuLiGuanLi = false;
        private static bool _shuangJiGuanLi = false;
        #endregion

        #region 委托定义
        public delegate void KeyDoubleClickFuncref(int player, int key, float timeDiff);
        public delegate void MouseDoubleClickFuncref(int player, int mouseButton, float timeDiff);
        public delegate void KeyChargeFuncref(int player, int key, float chargeValue);
        #endregion

        #region 事件定义
        public static event KeyDoubleClickFuncref KeyDoubleClickEvent;
        public static event MouseDoubleClickFuncref MouseDoubleClickEvent;
        public static event KeyChargeFuncref KeyChargeEvent;
        #endregion

        #region DataTable存储键名
        private const string c_tableKey_XuLiNum = "HD_CDXuLiNum";
        private const string c_tableKey_XuLiTag = "HD_CDXuLiTag";
        private const string c_tableKey_XuLiFixed = "HD_CDFixed_XuLi";
        private const string c_tableKey_SJNum = "HD_CDSJNum";
        private const string c_tableKey_SJTag = "HD_CDSJTag";
        private const string c_tableKey_SJFixed = "HD_CDFixed_SJ";
        private const string c_tableKey_KeyDownState = "KeyDownState";
        private const string c_tableKey_MouseDownState = "MouseDownState";
        private const string c_tableKey_IfKTag = "HD_IfKTag";
        private const string c_tableKey_IfSTag = "HD_IfSTag";
        #endregion

        #region 蓄力值功能
        /// <summary>
        /// 返回蓄力按键的DataTable分组键名,格式为"IntGroup_XuLi" + player,用于区分不同玩家的蓄力按键注册信息
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static string GetXuLiGroupKey(int player)
        {
            return ThreadStringBuilder.Concat("IntGroup_XuLi", player);
        }
        /// <summary>
        /// 返回蓄力按键注册总数的DataTable键名,格式为"IntGroup_XuLi" + player + "Num",用于保存玩家注册的蓄力按键数量
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static string GetXuLiNumKey(int player)
        {
            return ThreadStringBuilder.Concat(c_tableKey_XuLiNum, player);
        }
        /// <summary>
        /// 返回蓄力按键注册信息的DataTable键名,格式为"IntGroup_XuLi" + player + "Tag" + index,用于保存玩家注册的每个蓄力按键对应的按键值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetXuLiTagKey(int player, int index)
        {
            return ThreadStringBuilder.Concat(c_tableKey_XuLiTag, player, '_', index);
        }
        /// <summary>
        /// 返回蓄力按键对应的蓄力值的DataTable键名,格式为"HD_CDFixed_XuLi" + player + "_" + key,用于保存玩家每个蓄力按键当前的蓄力值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetXuLiFixedKey(int player, int key)
        {
            return ThreadStringBuilder.Concat(c_tableKey_XuLiFixed, player, '_', key);
        }
        /// <summary>
        /// 返回按键是否注册为蓄力按键的DataTable键名,格式为"HD_IfKTag" + player + "_" + key,用于快速判断某个按键是否在玩家的蓄力按键列表中注册过（存在即为true,不存在或已移除即为false）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetIfKTagKey(int player, int key)
        {
            return ThreadStringBuilder.Concat(c_tableKey_IfKTag, player, '_', key);
        }
        /// <summary>
        /// 返回按键注册总数（蓄力专用）
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static int GetXuLiNumMax(int player)
        {
            string groupKey = GetXuLiGroupKey(player);
            string numKey = groupKey + "Num";
            if (DataTableIntKeyExists(true, numKey))
            {
                return DataTableIntLoad0(true, numKey);
            }
            return 0;
        }
        /// <summary>
        /// 设置按键注册总数（蓄力专用）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="value"></param>
        private static void SetXuLiNumMax(int player, int value)
        {
            string groupKey = GetXuLiGroupKey(player);
            string numKey = groupKey + "Num";
            DataTableIntSave0(true, numKey, value);
        }
        /// <summary>
        /// 根据注册序号返回对应的按键（蓄力专用）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="regNum"></param>
        /// <returns></returns>
        private static int GetXuLiTagFromRegNum(int player, int regNum)
        {
            string groupKey = GetXuLiGroupKey(player);
            string tagKey = groupKey + "Tag";
            return DataTableIntLoad1(true, tagKey, regNum);
        }
        /// <summary>
        /// 注册蓄力按键,如果按键已注册则不重复注册,否则添加到列表末尾,并在DataTable中保存注册信息和初始蓄力值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="player"></param>
        public static void HD_RegKXL(int key, int player)
        {
            string groupKey = GetXuLiGroupKey(player);
            int num = GetXuLiNumMax(player);

            if (num == 0)
            {
                int i = num + 1;
                SetXuLiNumMax(player, i);
                DataTableIntSave1(true, GetXuLiTagKey(player, i), i, key);
                DataTableBoolSave1(true, GetIfKTagKey(player, key), key, true);
            }
            else
            {
                bool found = false;
                for (int i = 1; i <= num; i++)
                {
                    if (DataTableIntLoad1(true, GetXuLiTagKey(player, i), i) == key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    int i = num + 1;
                    SetXuLiNumMax(player, i);
                    DataTableIntSave1(true, GetXuLiTagKey(player, i), i, key);
                    DataTableBoolSave1(true, GetIfKTagKey(player, key), key, true);
                }
            }
        }
        /// <summary>
        /// 移除蓄力按键,根据注册序号找到对应按键,从DataTable中删除注册信息,并将列表后续按键前移覆盖,最后更新注册总数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="index"></param>
        private static void HD_RemoveKXL(int player, int index)
        {
            string groupKey = GetXuLiGroupKey(player);
            int num = GetXuLiNumMax(player);
            int key = GetXuLiTagFromRegNum(player, index);

            DataTableBoolRemove(true, GetIfKTagKey(player, key));

            for (int i = index; i < num; i++)
            {
                int nextTag = GetXuLiTagFromRegNum(player, i + 1);
                DataTableIntSave1(true, GetXuLiTagKey(player, i), i, nextTag);
            }
            DataTableIntSave1(true, GetXuLiTagKey(player, num), num, 0);
            SetXuLiNumMax(player, num - 1);
        }
        /// <summary>
        /// 注册蓄力按键,如果按键已注册则不重复注册,否则添加到列表末尾,并在DataTable中保存注册信息和初始蓄力值（默认1.0f表示开始蓄力）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        public static void RegistKeyXuLi(int player, int key)
        {
            HD_RegKXL(key, player);
            HD_SetKeyFixedXL(player, key, 1.0f);
        }
        /// <summary>
        /// 移除蓄力按键,根据注册序号找到对应按键,从DataTable中删除注册信息,并将列表后续按键前移覆盖,最后更新注册总数,同时重置蓄力值（默认0.0f）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        public static void RemoveKeyXuLi(int player, int key)
        {
            string groupKey = GetXuLiGroupKey(player);
            int num = GetXuLiNumMax(player);
            for (int i = 1; i <= num; i++)
            {
                if (GetXuLiTagFromRegNum(player, i) == key)
                {
                    HD_RemoveKXL(player, i);
                    HD_SetKeyFixedXL(player, key, 0.0f);
                    break;
                }
            }
        }
        /// <summary>
        /// 设置按键蓄力值,根据玩家和按键找到对应的DataTable键名,保存蓄力值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void HD_SetKeyFixedXL(int player, int key, float value)
        {
            DataTableFloatSave1(true, GetXuLiFixedKey(player, key), key, value);
        }
        /// <summary>
        /// 返回按键蓄力值,根据玩家和按键找到对应的DataTable键名,加载蓄力值,如果不存在则返回0.0f表示未注册或已过期
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float HD_ReturnKeyFixedXL(int player, int key)
        {
            if (DataTableFloatKeyExists(true, GetXuLiFixedKey(player, key)))
            {
                return DataTableFloatLoad1(true, GetXuLiFixedKey(player, key), key);
            }
            return 0.0f;
        }
        /// <summary>
        /// 设置按键双击值,根据玩家和按键找到对应的DataTable键名,保存双击值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void HD_SetKeyFixedSJ(int player, int key, float value)
        {
            DataTableFloatSave1(true, c_tableKey_SJFixed + player + "_" + key, key, value);
        }
        /// <summary>
        /// 返回按键双击值,根据玩家和按键找到对应的DataTable键名,加载双击值,如果不存在则返回-1.0f表示未注册或已过期
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float HD_ReturnKeyFixedSJ(int player, int key)
        {
            if (DataTableFloatKeyExists(true, c_tableKey_SJFixed + player + "_" + key))
            {
                return DataTableFloatLoad1(true, c_tableKey_SJFixed + player + "_" + key, key);
            }
            return -1.0f;
        }
        /// <summary>
        /// 设置蓄力增量,在UpdateXuLi中每帧调用,根据按键状态增加或减少蓄力值
        /// </summary>
        /// <param name="enable"></param>
        public static void StartXuLiGuanLi(bool enable)
        {
            _xuLiGuanLi = enable;
        }
        /// <summary>
        /// 蓄力管理主循环 - 每帧调用
        /// 遍历所有注册的蓄力按键,根据按键状态累加或减少蓄力值
        /// </summary>
        public static void UpdateXuLi(int player)
        {
            if (!_xuLiGuanLi) return;

            string groupKey = GetXuLiGroupKey(player);
            int num = GetXuLiNumMax(player);

            List<int> keysToRemove = new List<int>();

            for (int i = 1; i <= num; i++)
            {
                int key = GetXuLiTagFromRegNum(player, i);
                bool isPressed = false;

                if (key > 98)
                {
                    int mouseButton = key - 98;
                    isPressed = DataTableBoolLoad1(true, c_tableKey_MouseDownState + player, mouseButton);
                }
                else
                {
                    isPressed = DataTableBoolLoad1(true, c_tableKey_KeyDownState + player, key);
                }

                float KXL = HD_ReturnKeyFixedXL(player, key);

                if (isPressed)
                {
                    KXL += c_xuLiZengLiang;
                }
                else
                {
                    KXL -= c_xuLiZengLiang;
                }

                if (KXL < 1.0f)
                {
                    KXL = 0.0f;
                    keysToRemove.Add(key);
                }

                HD_SetKeyFixedXL(player, key, KXL);
            }

            foreach (int key in keysToRemove)
            {
                RemoveKeyXuLi(player, key);
            }
        }
        /// <summary>
        /// 处理按键释放时的蓄力事件,如果蓄力值大于0,则触发KeyChargeEvent事件,并重置蓄力值
        /// </summary>
        public static void ProcessKeyReleaseWithCharge(int player, int key)
        {
            float chargeValue = HD_ReturnKeyFixedXL(player, key);
            if (chargeValue > 0.0f)
            {
                KeyChargeEvent?.Invoke(player, key, chargeValue);
            }
            HD_SetKeyFixedXL(player, key, 0.0f);
            RemoveKeyXuLi(player, key);
        }
        #endregion

        #region 双击功能
        /// <summary>
        /// 返回双击按键的DataTable分组键名,格式为"IntGroup_DoubleClicked" + player,用于区分不同玩家的双击按键注册信息
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static string GetSJGroupKey(int player)
        {
            return "IntGroup_DoubleClicked" + player;
        }
        /// <summary>
        /// 返回双击按键注册总数的DataTable键名,格式为"IntGroup_DoubleClicked" + player + "Num",用于保存玩家注册的双击按键数量
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static int GetSJNumMax(int player)
        {
            string groupKey = GetSJGroupKey(player);
            string numKey = groupKey + "Num";
            if (DataTableIntKeyExists(true, numKey))
            {
                return DataTableIntLoad0(true, numKey);
            }
            return 0;
        }
        /// <summary>
        /// 设置双击按键注册总数,根据玩家找到对应的DataTable键名,保存注册总数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="value"></param>
        private static void SetSJNumMax(int player, int value)
        {
            string groupKey = GetSJGroupKey(player);
            string numKey = groupKey + "Num";
            DataTableIntSave0(true, numKey, value);
        }
        /// <summary>
        /// 根据注册序号返回对应的按键（双击专用）,根据玩家和注册序号找到对应的DataTable键名,加载按键值,如果不存在则返回0表示未注册
        /// </summary>
        /// <param name="player"></param>
        /// <param name="regNum"></param>
        /// <returns></returns>
        private static int GetSJTagFromRegNum(int player, int regNum)
        {
            string groupKey = GetSJGroupKey(player);
            string tagKey = groupKey + "Tag";
            return DataTableIntLoad1(true, tagKey, regNum);
        }
        /// <summary>
        /// 注册双击按键,如果按键已注册则不重复注册,否则添加到列表末尾,并在DataTable中保存注册信息和初始双击值（默认时限值）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="player"></param>
        public static void HD_RegKSJ(int key, int player)
        {
            string groupKey = GetSJGroupKey(player);
            int num = GetSJNumMax(player);

            if (num == 0)
            {
                int i = num + 1;
                SetSJNumMax(player, i);
                DataTableIntSave1(true, groupKey + "Tag", i, key);
                DataTableBoolSave1(true, c_tableKey_IfSTag + player + "_" + key, key, true);
            }
            else
            {
                bool found = false;
                for (int i = 1; i <= num; i++)
                {
                    if (DataTableIntLoad1(true, groupKey + "Tag", i) == key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    int i = num + 1;
                    SetSJNumMax(player, i);
                    DataTableIntSave1(true, groupKey + "Tag", i, key);
                    DataTableBoolSave1(true, c_tableKey_IfSTag + player + "_" + key, key, true);
                }
            }
        }
        /// <summary>
        /// 移除双击按键,根据注册序号找到对应按键,从DataTable中删除注册信息,并将列表后续按键前移覆盖,最后更新注册总数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="index"></param>
        private static void HD_RemoveKSJ(int player, int index)
        {
            string groupKey = GetSJGroupKey(player);
            int num = GetSJNumMax(player);
            int key = GetSJTagFromRegNum(player, index);

            DataTableBoolRemove(true, c_tableKey_IfSTag + player + "_" + key);

            for (int i = index; i < num; i++)
            {
                int nextTag = GetSJTagFromRegNum(player, i + 1);
                DataTableIntSave1(true, groupKey + "Tag", i, nextTag);
            }
            DataTableIntSave1(true, groupKey + "Tag", num, 0);
            SetSJNumMax(player, num - 1);
        }
        /// <summary>
        /// 注册双击按键,如果按键已注册则不重复注册,否则添加到列表末尾,并在DataTable中保存注册信息和初始双击值（默认时限值）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        public static void RegistKeyShuangJi(int player, int key)
        {
            HD_RegKSJ(key, player);
            HD_SetKeyFixedSJ(player, key, c_shuangJiShiXian);
        }
        /// <summary>
        /// 移除双击按键,根据注册序号找到对应按键,从DataTable中删除注册信息,并将列表后续按键前移覆盖,最后更新注册总数,同时重置双击值（默认-1.0f表示未注册或已过期）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        public static void RemoveKeyShuangJi(int player, int key)
        {
            string groupKey = GetSJGroupKey(player);
            int num = GetSJNumMax(player);
            for (int i = 1; i <= num; i++)
            {
                if (GetSJTagFromRegNum(player, i) == key)
                {
                    HD_RemoveKSJ(player, i);
                    HD_SetKeyFixedSJ(player, key, -1.0f);
                    break;
                }
            }
        }
        /// <summary>
        /// 设置双击增量,在UpdateShuangJi中每帧调用,根据按键状态衰减双击值,如果按键未在时限内按下第二次则过期失效
        /// </summary>
        /// <param name="value"></param>
        public static void SetShuangJiZengLiang(float value)
        {
        }
        /// <summary>
        /// 设置双击时限,在ProcessKeyDoubleClick和ProcessMouseDoubleClick中调用,当第一次按下时初始化双击值,如果在时限内按下第二次则触发双击事件
        /// </summary>
        /// <param name="enable"></param>
        public static void StartShuangJiGuanLi(bool enable)
        {
            _shuangJiGuanLi = enable;
        }
        /// <summary>
        /// 双击管理主循环 - 每帧调用
        /// 遍历所有注册的双击按键,衰减双击值（与Galaxy版本一致）
        /// </summary>
        public static void UpdateShuangJi(int player)
        {
            if (!_shuangJiGuanLi) return;

            string groupKey = GetSJGroupKey(player);
            int num = GetSJNumMax(player);

            for (int i = 1; i <= num; i++)
            {
                int key = GetSJTagFromRegNum(player, i);
                float KSJ = HD_ReturnKeyFixedSJ(player, key);

                if (KSJ != -1.0f && KSJ >= 0.0f)
                {
                    KSJ -= c_shuangJiZengLiang;
                    if (KSJ < 0.0f)
                    {
                        KSJ = -1.0f;
                        HD_RemoveKSJ(player, i);
                    }
                    HD_SetKeyFixedSJ(player, key, KSJ);
                }
            }
        }
        /// <summary>
        /// 处理按键双击事件,当第一次按下时初始化双击值,如果在时限内按下第二次则触发双击事件并重置双击值,否则等待下一次按下重新初始化双击值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ProcessKeyDoubleClick(int player, int key)
        {
            if (!_shuangJiGuanLi) return false;

            float KSJ = HD_ReturnKeyFixedSJ(player, key);

            if (KSJ > 0.0f)
            {
                // 在时限内按下第二次,触发双击事件
                KeyDoubleClickEvent?.Invoke(player, key, KSJ);
                HD_SetKeyFixedSJ(player, key, -1.0f);
                RemoveKeyShuangJi(player, key);
                return true;
            }

            // 第一次按下,初始化双击值
            HD_SetKeyFixedSJ(player, key, c_shuangJiShiXian);
            return false;
        }
        /// <summary>
        /// 处理鼠标双击事件,当第一次按下时初始化双击值,如果在时限内按下第二次则触发双击事件并重置双击值,否则等待下一次按下重新初始化双击值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool ProcessMouseDoubleClick(int player, int mouseButton)
        {
            if (!_shuangJiGuanLi) return false;

            int key = mouseButton + 98;
            float KSJ = HD_ReturnKeyFixedSJ(player, key);

            if (KSJ > 0.0f)
            {
                // 在时限内按下第二次,触发双击事件
                MouseDoubleClickEvent?.Invoke(player, mouseButton, KSJ);
                HD_SetKeyFixedSJ(player, key, -1.0f);
                RemoveKeyShuangJi(player, key);
                return true;
            }

            // 第一次按下,初始化双击值
            HD_SetKeyFixedSJ(player, key, c_shuangJiShiXian);
            return false;
        }
        #endregion

        #region 按键状态管理
        /// <summary>
        /// 设置按键按下状态,在OnKeyDown和OnKeyUp中调用,根据玩家和按键找到对应的DataTable键名,保存按键状态（true表示按下, false表示释放）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public static void SetKeyDownState(int player, int key, bool state)
        {
            DataTableBoolSave1(true, c_tableKey_KeyDownState + player, key, state);
        }
        /// <summary>
        /// 返回按键按下状态,根据玩家和按键找到对应的DataTable键名,加载按键状态,如果不存在则返回false表示未按下
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKeyDownState(int player, int key)
        {
            return DataTableBoolLoad1(true, c_tableKey_KeyDownState + player, key);
        }
        /// <summary>
        /// 设置鼠标按下状态,在OnMouseDown和OnMouseUp中调用,根据玩家和鼠标按钮找到对应的DataTable键名,保存鼠标按钮状态（true表示按下, false表示释放）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mouseButton"></param>
        /// <param name="state"></param>
        public static void SetMouseDownState(int player, int mouseButton, bool state)
        {
            DataTableBoolSave1(true, c_tableKey_MouseDownState + player, mouseButton, state);
        }
        /// <summary>
        /// 返回鼠标按下状态,根据玩家和鼠标按钮找到对应的DataTable键名,加载鼠标按钮状态,如果不存在则返回false表示未按下
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool GetMouseDownState(int player, int mouseButton)
        {
            return DataTableBoolLoad1(true, c_tableKey_MouseDownState + player, mouseButton);
        }
        /// <summary>
        /// 按键按下事件处理,在OnKeyDown中调用,设置按键按下状态,如果启用蓄力管理则注册蓄力按键并初始化蓄力值,如果启用双击管理则处理双击事件并设置双击值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        public static void OnKeyDown(int player, int key)
        {
            SetKeyDownState(player, key, true);
            if (_xuLiGuanLi)
            {
                RegistKeyXuLi(player, key);
                HD_SetKeyFixedXL(player, key, 1.0f);
            }
            if (_shuangJiGuanLi)
            {
                if (ProcessKeyDoubleClick(player, key))
                {
                }
                else
                {
                    HD_RegKSJ(key, player);
                    HD_SetKeyFixedSJ(player, key, c_shuangJiShiXian);
                }
            }
        }
        /// <summary>
        /// 按键释放事件处理,在OnKeyUp中调用,设置按键按下状态,如果启用蓄力管理则处理蓄力事件并重置蓄力值,如果启用双击管理则不需要处理双击事件因为双击只在按下时判断和触发
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        public static void OnKeyUp(int player, int key)
        {
            SetKeyDownState(player, key, false);
            if (_xuLiGuanLi)
            {
                ProcessKeyReleaseWithCharge(player, key);
            }
        }
        /// <summary>
        /// 鼠标按钮按下事件处理,在OnMouseDown中调用,设置鼠标按钮按下状态,如果启用蓄力管理则注册蓄力按键并初始化蓄力值,如果启用双击管理则处理双击事件并设置双击值
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mouseButton"></param>
        public static void OnMouseDown(int player, int mouseButton)
        {
            int key = mouseButton + 98;
            SetMouseDownState(player, mouseButton, true);
            if (_xuLiGuanLi)
            {
                RegistKeyXuLi(player, key);
                HD_SetKeyFixedXL(player, key, 1.0f);
            }
            if (_shuangJiGuanLi)
            {
                if (ProcessMouseDoubleClick(player, mouseButton))
                {
                }
                else
                {
                    HD_RegKSJ(key, player);
                    HD_SetKeyFixedSJ(player, key, c_shuangJiShiXian);
                }
            }
        }
        /// <summary>
        /// 鼠标按钮释放事件处理,在OnMouseUp中调用,设置鼠标按钮按下状态,如果启用蓄力管理则处理蓄力事件并重置蓄力值,如果启用双击管理则不需要处理双击事件因为双击只在按下时判断和触发
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mouseButton"></param>
        public static void OnMouseUp(int player, int mouseButton)
        {
            int key = mouseButton + 98;
            SetMouseDownState(player, mouseButton, false);
            if (_xuLiGuanLi)
            {
                ProcessKeyReleaseWithCharge(player, key);
            }
        }
        #endregion

    }
}

//// 1. 启用蓄力和双击管理
//MMCore.StartXuLiGuanLi(true);
//MMCore.StartShuangJiGuanLi(true);

//// 2. 订阅事件
//MMCore.KeyChargeEvent += OnKeyCharge;
//MMCore.KeyDoubleClickEvent += OnKeyDoubleClick;

//// 3. 在主循环中更新
//void Update()
//{
//    MMCore.UpdateXuLi(playerId);
//    MMCore.UpdateShuangJi(playerId);
//}

//// 4. 事件处理
//void OnKeyCharge(int player, int key, float chargeValue)
//{
//    Debug.Log($"玩家{player}按键{key}蓄力值: {chargeValue}");
//}

//void OnKeyDoubleClick(int player, int key, float timeDiff)
//{
//    Debug.Log($"玩家{player}按键{key}双击，间隔{timeDiff}秒");
//}