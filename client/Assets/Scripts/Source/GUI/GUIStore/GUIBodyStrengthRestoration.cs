﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Game.Resource;

//体力恢复 BodyStrngthRestoration.cs
//Author: Sunyi
//2013-11-26

public class GUIBodyStrengthRestoration : GUIBase
{
    private const string RES_MAIN = "GUI_AlertView"; //主资源地址

    private const string ALERTVIEW = "AlertView"; //对话框资源地址
    private const string ALERTVIEWNONE = "AlertViewNone"; //对话框下一层资源地址
    private const string LABEL_ALERTVIEWNONE = "AlertViewNone/Lab_Desc"; //对话框下一层资源地址Lab_Desc
    private const string ALERT_NONE_OK = "AlertViewNone/Btn_Ok"; //对话框下一层资源确定按钮地址
    private const string CONTENT = "Content"; //对话框内容资源地址
    private const string TOPPANEL = "TopPanel";//导航栏资源地址
    private const string BACK_BUTTON = "TopPanel/Button_Back";//房名返回按钮地址
    private const string HOME_NAME = "TopPanel/Label";//导航栏标签地址
    private const string OK_BUTTON = "AlertView/Btn_Ok";//确定按钮地址
    private const string CANCEL_BUTTON = "AlertView/Btn_Cancel";//恢复按钮地址
    private const string RECOVER_BUTTON = "AlertView/Btn_Revert";//恢复按钮
    private const string EXPAND_BUTTON = "AlertView/Btn_Expand";//扩张按钮

    private GameObject m_cAlertView;//对话框
    private GameObject m_cTopPanel;//导航栏
    private GameObject m_cBtnBack;//返回按钮
    private UILabel m_cLabelContent;//内容标签
    private UILabel m_cLabHomeName;//导航栏标签
    private GameObject m_cBtnRecover;//恢复按钮
    private GameObject m_cBtnCancel;//取消按钮
    private GameObject m_cAlertViewNone;//下一层对话框，用于当前体力已满的提示
    private GameObject m_cBtnAlertNoneOk;//下一层对话框确定按钮地址
    private GameObject m_cBtnAlertOk;//确定按钮
    private GameObject m_cBtnAlertRecover;//恢复按钮
    private GameObject m_cBtnAlertExpand;//扩张按钮
    private UILabel m_cLabAlertContent;//当前竞技点已满消息框内容

    private int m_iAlertLevel = 0;//0为第一次提示，1为体力不足时提示
    private bool m_bIsCurGui;//判断当前页面是不是最上层的GUI


    public GUIBodyStrengthRestoration(GUIManager guiMgr)
        : base(guiMgr, GUI_DEFINE.GUIID_BODYSTRENGTHRESTORATION, GUILAYER.GUI_PANEL)
    { }


    /// <summary>
    /// 展示
    /// </summary>
    public override void Show()
    {
        this.m_eLoadingState = LOADING_STATE.NONE;
        if (this.m_cGUIObject == null)
        {
            this.m_eLoadingState = LOADING_STATE.START;
            GUI_FUNCTION.AYSNCLOADING_SHOW();
            ResourcesManager.GetInstance().LoadResource(GAME_DEFINE.RESOURCE_GUI_PATH, RES_MAIN);
        }
        else
        {
            InitGUI();
        }
    }

    /// <summary>
    /// 初始化GUI
    /// </summary>
    protected override void InitGUI()
    {
        base.Show();

        GUI_FUNCTION.AYSNCLOADING_HIDEN();

        if (this.m_cGUIObject == null)
        {
            this.m_cGUIObject = GameObject.Instantiate((UnityEngine.Object)ResourcesManager.GetInstance().Load(RES_MAIN)) as GameObject;
            this.m_cGUIObject.transform.parent = GameObject.Find(GUI_DEFINE.GUI_ANCHOR_CENTER).transform;
            this.m_cGUIObject.transform.localScale = Vector3.one;

            this.m_cTopPanel = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, TOPPANEL);
            this.m_cTopPanel.SetActive(true);
            this.m_cTopPanel.transform.localPosition = new Vector3(-420, 270, 0);

            this.m_cLabHomeName = GUI_FINDATION.GET_OBJ_COMPONENT<UILabel>(this.m_cGUIObject, HOME_NAME);
            this.m_cLabHomeName.text = "体力回复";

            this.m_cAlertView = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, ALERTVIEW);
            this.m_cAlertView.transform.position = new Vector3(640, 0, 0);

            this.m_cAlertViewNone = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, ALERTVIEWNONE);
            this.m_cAlertViewNone.SetActive(false);

            this.m_cBtnBack = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, BACK_BUTTON);
            GUIComponentEvent backEvent = this.m_cBtnBack.AddComponent<GUIComponentEvent>();
            backEvent.AddIntputDelegate(OnClickBackCutton);

            this.m_cBtnAlertOk = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, OK_BUTTON);
            GUIComponentEvent okEvent = this.m_cBtnAlertOk.AddComponent<GUIComponentEvent>();
            okEvent.AddIntputDelegate(RecoverStrength);
            this.m_cBtnAlertOk.SetActive(false);

            this.m_cBtnCancel = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, CANCEL_BUTTON);
            GUIComponentEvent cancelEvent = this.m_cBtnCancel.AddComponent<GUIComponentEvent>();
            cancelEvent.AddIntputDelegate(OnClickBackCutton);

            this.m_cBtnAlertNoneOk = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, ALERT_NONE_OK);
            GUIComponentEvent alertOkEvent = this.m_cBtnAlertNoneOk.AddComponent<GUIComponentEvent>();
            alertOkEvent.AddIntputDelegate(OnClickAlertOkEvent);

            this.m_cBtnAlertExpand = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, EXPAND_BUTTON);
            this.m_cBtnAlertExpand.SetActive(false);

            this.m_cBtnAlertRecover = GUI_FINDATION.GET_GAME_OBJECT(this.m_cGUIObject, RECOVER_BUTTON);
            this.m_cBtnAlertRecover.SetActive(true);
            GUIComponentEvent alertRevertEvent = this.m_cBtnAlertRecover.AddComponent<GUIComponentEvent>();
            alertRevertEvent.AddIntputDelegate(RecoverStrength);

            this.m_cLabelContent = GUI_FINDATION.GET_OBJ_COMPONENT<UILabel>(this.m_cAlertView, CONTENT);
            StringBuilder sb = new StringBuilder();

            sb.Append("花费" + GAME_DEFINE.DiamondStrenthCost + "个钻石可将#FF0000]体力全回复");
            this.m_cLabelContent.text = sb.ToString();

            this.m_cLabAlertContent = GUI_FINDATION.GET_OBJ_COMPONENT<UILabel>(this.m_cGUIObject, LABEL_ALERTVIEWNONE);
        }

        this.m_bIsCurGui = false;

        this.m_cGUIMgr.SetCurGUIID(this.m_iID);
        SetLocalPos(Vector3.zero);

        CTween.TweenPosition(this.m_cTopPanel, GAME_DEFINE.FADEOUT_GUI_TIME, GAME_DEFINE.FADEIN_GUI_TIME, new Vector3(-420, 270, 0), new Vector3(0, 270, 0));
        CTween.TweenPosition(this.m_cAlertView, GAME_DEFINE.FADEOUT_GUI_TIME, GAME_DEFINE.FADEIN_GUI_TIME, new Vector3(640, 0, 0), Vector3.zero);

        //下方提示
        GUIBackFrameBottom gui = this.m_cGUIMgr.GetGUI(GUI_DEFINE.GUIID_BACKFRAMEBOTTOM) as GUIBackFrameBottom;
        gui.ChangeBottomLabel(GAME_FUNCTION.STRING(STRING_DEFINE.INFO_RENEW_POWER));
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hiden()
    {
        //base.Hiden();

        GUI_FUNCTION.LOCKPANEL_AUTO_HIDEN();

        CTween.TweenPosition(this.m_cAlertView, GAME_DEFINE.FADEIN_GUI_TIME, new Vector3(0, 0, 0), new Vector3(640, 0, 0));
        CTween.TweenPosition(this.m_cTopPanel, GAME_DEFINE.FADEIN_GUI_TIME, new Vector3(0, 270, 0), new Vector3(-420, 270, 0) , Destory);

        this.m_iAlertLevel = 0;
        this.m_bIsCurGui = false;

        ResourcesManager.GetInstance().UnloadUnusedResources();
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destory()
    {
        base.Hiden();

        this.m_cAlertView = null;
        this.m_cTopPanel = null;
        this.m_cBtnBack = null;
        this.m_cLabelContent = null;
        this.m_cLabHomeName = null;
        this.m_cBtnRecover = null;
        this.m_cBtnCancel = null;
        this.m_cAlertViewNone = null;
        this.m_cBtnAlertNoneOk = null;
        this.m_cBtnAlertOk = null;
        this.m_cBtnAlertRecover = null;
        this.m_cBtnAlertExpand = null;
        this.m_cLabAlertContent = null;

        base.Destory();
    }

    /// <summary>
    /// 返回按钮事件
    /// </summary>
    /// <param name="info"></param>
    /// <param name="args"></param>
    private void OnClickBackCutton(GUI_INPUT_INFO info, object[] args)
    {
        if (info.m_eType == GUI_INPUT_INFO.GUI_INPUT_TYPE.CLICK)
        {
            Hiden();

            GUIStore store = (GUIStore)this.m_cGUIMgr.GetGUI(GUI_DEFINE.GUIID_STORE);
            store.Show();
        }
    }

    /// <summary>
    /// 体力恢复按钮事件
    /// </summary>
    /// <param name="info"></param>
    /// <param name="args"></param>
    private void RecoverStrength(GUI_INPUT_INFO info, object[] args)
    {
        if (info.m_eType == GUI_INPUT_INFO.GUI_INPUT_TYPE.CLICK)
        {
            GUIBackFrameTop top = (GUIBackFrameTop)this.m_cGUIMgr.GetGUI(GUI_DEFINE.GUIID_BACKFRAMETOP);
            bool isStrengthFull = top.IsStrengthFull();
            if (!isStrengthFull)
            {
                if (this.m_iAlertLevel == 0)
                {
                    if (Role.role.GetBaseProperty().m_iDiamond > 0)
                    {
                        this.m_bIsCurGui = true;
                        //恢复体力
                        SendAgent.SendStrengthRecoverReq(Role.role.GetBaseProperty().m_iPlayerId);
                        this.m_iAlertLevel = 0;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("水晶不足，是否购买水晶？");
                        this.m_cLabelContent.text = sb.ToString();
                        this.m_iAlertLevel = 1;
                        this.m_cBtnAlertOk.SetActive(true);
                        this.m_cBtnAlertRecover.SetActive(false);
                    }
                }
                else if (this.m_iAlertLevel == 1)
                {
                    Hiden();

                    GUIBackFrameBottom bottom = (GUIBackFrameBottom)this.m_cGUIMgr.GetGUI(GUI_DEFINE.GUIID_BACKFRAMEBOTTOM);
                    bottom.HiddenHalf();

                    GUIGem gem = (GUIGem)this.m_cGUIMgr.GetGUI(GUI_DEFINE.GUIID_GEM);
                    gem.SetLastGuiId(this.m_iID);
                    SendAgent.SendStoreDiamondPrice();

                    GUI_FUNCTION.LOCKPANEL_AUTO_HIDEN();
                }
                
            }
            else {
                this.m_cAlertViewNone.SetActive(true);
                this.m_cLabAlertContent.text = "您的体力已满，无需恢复";
                this.m_cAlertView.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 判断当前页面是不是当前GUI
    /// </summary>
    /// <returns></returns>
    public bool IsCurGui()
    {
        return this.m_bIsCurGui;
    }

    /// <summary>
    /// 当前体力已满，无需恢复，确定按钮事件
    /// </summary>
    /// <param name="info"></param>
    /// <param name="args"></param>
    private void OnClickAlertOkEvent(GUI_INPUT_INFO info, object[] args)
    {
        if (info.m_eType == GUI_INPUT_INFO.GUI_INPUT_TYPE.CLICK)
        {
            this.m_cAlertViewNone.SetActive(false);
            this.m_cAlertView.SetActive(true);
        }
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    /// <returns></returns>
    public override bool Update()
    {
        //资源加载等待
        switch (this.m_eLoadingState)
        {
            case LOADING_STATE.START:
                this.m_eLoadingState++;
                return false;
            case LOADING_STATE.LOADING:
                if (ResourcesManager.GetInstance().GetProgress() >= 1f && ResourcesManager.GetInstance().IsComplete())
                {
                    this.m_eLoadingState++;
                }
                return false;
            case LOADING_STATE.END:
                InitGUI();
                this.m_eLoadingState++;
                break;
        }

        return base.Update();
    }
}

