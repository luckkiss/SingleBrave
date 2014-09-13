﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Base;
using Game.Network;

//  SessionManager.cs
//  Author: Lu Zexi
//  2013-11-12


/// <summary>
/// 会话定义
/// </summary>
public enum SESSION_DEFINE
{
    LOGIN_SESSION = 0,
    MAX,
}


/// <summary>
/// 会话管理类
/// </summary>
public class SessionManager : Singleton<SessionManager>
{
    public delegate void CALLBACK();

    private HTTPSession[] m_vecSession; //会话列

    private CALLBACK m_cCallBack;    //数据包完成回调

    public SessionManager()
    {
        this.m_vecSession = new HTTPSession[(int)SESSION_DEFINE.MAX];
        this.m_vecSession[(int)SESSION_DEFINE.LOGIN_SESSION] = new HTTPSession(GAME_SETTING.SESSION_LOGIN_PATH, new HTTPDispatchFactory<LoginDispatch>());
    }

    /// <summary>
    /// 设置回调
    /// </summary>
    /// <param name="cal"></param>
    public void SetCallBack(CALLBACK cal)
    {
        this.m_cCallBack += cal;
    }

    /// <summary>
    /// 调用回调
    /// </summary>
    public void CallBack()
    {
        if( this.m_cCallBack != null )
            this.m_cCallBack();
        this.m_cCallBack = null;
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="packet"></param>
    public void Send(SESSION_DEFINE index, HTTPPacketBase packet)
    {
        if (this.m_vecSession.Length <= (int)index)
        {
            GAME_LOG.ERROR("Session index is out of len.");
            return;
        }
        if (packet.GetAction() != PACKET_DEFINE.GUIDE_STEP_REQ)
        {
            GUI_FUNCTION.LOADING_SHOW();
        }
        this.m_vecSession[(int)index].Send(packet);
    }

    /// <summary>
    /// 准备发送数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="packet"></param>
    public void SendReady(SESSION_DEFINE index, HTTPPacketBase packet)
    {
        if (this.m_vecSession.Length <= (int)index)
        {
            GAME_LOG.ERROR("Session index is out of len.");
            return;
        }
        this.m_vecSession[(int)index].SendReady(packet);
    }

    /// <summary>
    /// 刷新数据包
    /// </summary>
    public bool Refresh()
    {
        bool ok = false;
        for (int i = 0; i < (int)SESSION_DEFINE.MAX; i++)
        {
            if (this.m_vecSession[(int)i].Send())
            {
                GUI_FUNCTION.LOADING_SHOW();
                ok = true;
            }
        }
        return ok;
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    public void Update()
    {
        for (int i = 0; i < (int)SESSION_DEFINE.MAX; i++)
        {

            if (this.m_vecSession[i] != null)
            {
                this.m_vecSession[i].Update();
            }
        }
    }

}
