﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using Game.Network;

//  ActivityBattleStartHandle.cs
//  Author: Lu Zexi
//  2014-01-08




/// <summary>
/// 活动战斗开始句柄
/// </summary>
public class ActivityBattleStartHandle : HTTPHandleBase
{
    /// <summary>
    /// 获取action
    /// </summary>
    /// <returns></returns>
    public override string GetAction()
    {
        return PACKET_DEFINE.ACTIVITY_BATTLE_START_REQ;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public override bool Excute(HTTPPacketBase packet)
    {
        ActivityBattleStartPktAck ack = (ActivityBattleStartPktAck)packet;

        GUI_FUNCTION.LOADING_HIDEN();

        if (ack.m_iErrorCode != 0)
        {
            GAME_LOG.ERROR("ack error desc : " + ack.m_strErrorDes);
            return false;
        }

        Role.role.GetBaseProperty().m_iBattleID = ack.m_iBattleID;

        //SendAgent.SendBattleGateEndReq(Role.role.GetBaseProperty().m_iPlayerId, WorldManager.s_CurrentWorldId, WorldManager.s_CurrentAreaIndex,
        //        WorldManager.s_CurrentDungeonIndex, WorldManager.s_CurrentGateIndex, 200, 200, null,null,null,
        //        new int[] { 0, 0, 0, 0, 0 });
        //return true;

        //GameManager.GetInstance().GetGUIManager().Destory();

        GameManager.GetInstance().GetGUIManager().GetGUI(GUI_DEFINE.GUIID_BACKFRAMETOP).Hiden();
        GameManager.GetInstance().GetGUIManager().GetGUI(GUI_DEFINE.GUIID_BACKFRAMEBOTTOM).Hiden();

        GUIActivityBattle gatebattleGui = (GUIActivityBattle)GameManager.GetInstance().GetGUIManager().GetGUI(GUI_DEFINE.GUIID_ACTIVITY_BATTLE);

        HeroTeam team = Role.role.GetTeamProperty().GetTeam(Role.role.GetBaseProperty().m_iCurrentTeam);
        Hero[] heros = new Hero[6];
        for (int i = 0; i < team.m_vecTeam.Length; i++)
        {
            Hero item = Role.role.GetHeroProperty().GetHero(team.m_vecTeam[i]);
            heros[i] = item;
        }
        heros[5] = Role.role.GetBattleFriendProperty().GetSelectFriend().m_cLeaderHero;

        LeaderSkillTable selfLeaderSkill = LeaderSkillTableManager.GetInstance().GetLeaderSkillTable(Role.role.GetHeroProperty().GetHero(team.m_iLeadID).m_iLeaderSkillID);
        LeaderSkillTable friendLeaderSkill = null;
        if (Role.role.GetFriendProperty().IsMyFriend(Role.role.GetBattleFriendProperty().GetSelectFriend().m_iID))
        {
            friendLeaderSkill = LeaderSkillTableManager.GetInstance().GetLeaderSkillTable(Role.role.GetBattleFriendProperty().GetSelectFriend().m_cLeaderHero.m_iLeaderSkillID);
        }
        

        
        ActivityDungeonTable dungeonTable = WorldManager.GetActivityDungeonTable(WorldManager.s_iCurEspDungeonId);
        ActivityGateTable gateTable = WorldManager.GetActivityGateTable(dungeonTable.ID, WorldManager.s_iCurEspDungeonGateIndex);

        gatebattleGui.m_iGateID = gateTable.ID;
        gatebattleGui.m_iDungeonID = dungeonTable.ID;
        gatebattleGui.SetSelfLeaderSkill(selfLeaderSkill);
        gatebattleGui.SetFriendLeaderSkill(friendLeaderSkill);
        gatebattleGui.SetLeaderIndex(team.GetLeaderIndex());
        gatebattleGui.SetBattleSelfHero(heros);
        gatebattleGui.SetItem(Role.role.GetItemProperty().GetAllBattleItem());
        gatebattleGui.Show();

        return true;
    }

}