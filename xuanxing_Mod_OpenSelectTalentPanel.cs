using HarmonyLib;
using Mods;
using System;
using System.Reflection;
using Tool.Database;
using UnityEngine;
using Utils;

namespace xuanxing_Mod_OpenSelectTalentPanel
{
    [HarmonyPatch]
    public class RefreshTalentPatch : UserMod
    {
        public override void OnLoad()
        {
            base.OnLoad();
            Define define = new Define();
            Traverse.Create(define).Field("selectTalentRollCount").SetValue(RefreshTalentPatch.config.RollCount);
            UnityEngine.Debug.Log("【天赋自选mod】执行：OnLoad");
        }
        //游戏加载时执行
        //用户设置
        public static RollCountConfig config = new RollCountConfig();
        public class RollCountConfig
        {
            [IntField(0, 5, "天赋自选花费点数 | RollCount")]
            public int RollCount = 3;
        }
        //选定飞船后执行
        //获取OpenSelectTalentPanel
        //前置补丁
        private static readonly MethodInfo _openSelectMethod = AccessTools.Method(typeof(PilotPanel), "OpenSelectTalentPanel", new[] { typeof(DataTalent) });
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PilotPanel), "RefreshTalent")]
        static bool Prefix_RefreshTalent(PilotPanel __instance, DataTalent dataTalent, bool unlock, Transform baseTran, ref TalentSlotAndDes __result)
        //获取失败为true
        //执行失败为true
        {
            UnityEngine.Debug.Log("【天赋自选mod】执行：Prefix");
            try
            {
                if (_openSelectMethod == null) { UnityEngine.Debug.LogError("【天赋自选mod】失败：_openSelectMethod == null"); return true; }
                TalentSlotAndDes talentSlotAndDes = UtilsClass.CreateResourcesByLowUse<TalentSlotAndDes>(__instance.talentSlots, baseTran, FilePath.talentSlotAndDes, null);
                var action = (Action<DataTalent>)Delegate.CreateDelegate(typeof(Action<DataTalent>), __instance, _openSelectMethod);
                talentSlotAndDes.Refresh(dataTalent, unlock, action);
                __result = talentSlotAndDes;
                UnityEngine.Debug.Log("【天赋自选mod】成功");
                return false;
            }
            catch
            {
                UnityEngine.Debug.LogError("【天赋自选mod】失败：try-catch存在问题");
                return true;
            }
        }
    }
}