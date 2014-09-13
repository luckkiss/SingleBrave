﻿//  FriendLockLikePktAck.cs
//  Author: Cheng Xia
//  2013-1-13

using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Base;
using CodeTitans.JSon;
using Game.Network;


/// <summary>
/// 好友标记喜欢应答数据
/// </summary>
public class FriendLockLikePktAck : HTTPPacketBase
{
    public FriendLockLikePktAck()
    {
        this.m_strAction = PACKET_DEFINE.FRIEND_LOCKLIKE_REQ;
    }
}

/// <summary>
/// 好友标记喜欢答工厂类
/// </summary>
public class FriendLockLikePktAckFactory : HTTPPacketFactory
{
    /// <summary>
    /// 获取action
    /// </summary>
    /// <returns></returns>
    public override string GetPacketAction()
    {
        return PACKET_DEFINE.FRIEND_LOCKLIKE_REQ;
    }

    /// <summary>
    /// 创建数据包
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public override HTTPPacketBase Create(IJSonObject json)
    {
        FriendLockLikePktAck ack = PACKET_HEAD.PACKET_ACK_HEAD<FriendLockLikePktAck>(json);

        return ack;
    }
}
