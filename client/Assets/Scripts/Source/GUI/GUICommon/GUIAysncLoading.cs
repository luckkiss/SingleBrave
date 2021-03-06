using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Resource;


//  GUILoading.cs
//  Author: Cheng Xia
//  2014-1-29



/// <summary>
/// GUI加载类
/// </summary>
public class GUIAysncLoading : GUIBase
{
    private string RES_MAIN = "_GUI_AYSNC_LOADING";   //主资源


    public GUIAysncLoading(GUIManager guiMgr)
        : base(guiMgr, GUI_DEFINE.GUIID_AYSNC_LOADING, UILAYER.GUI_LOADING)
    {
        //
    }

    /// <summary>
    /// 展示
    /// </summary>
    public override void Show()
    {
        base.Show();
        if (this.m_cGUIObject == null)
        {
            this.m_cGUIObject = GameObject.Instantiate(Resources.Load(RES_MAIN)) as GameObject;
            this.m_cGUIObject.transform.parent = GameObject.Find(GUI_DEFINE.GUI_ANCHOR_CENTER).transform;
            this.m_cGUIObject.transform.localScale = Vector3.one;
        }

        SetLocalPos(Vector3.zero);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hiden()
    {
        base.Hiden();
        SetLocalPos(Vector3.one * 0xFFFF);
        Destory();
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    /// <returns></returns>
    public override bool Update()
    {
        base.Update();

        if (!IsShow()) return false;

        return true;
    }
}

