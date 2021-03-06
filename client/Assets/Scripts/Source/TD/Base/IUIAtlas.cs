

using System.Collections;
using System.Collections.Generic;


//  IUIAtlas.cs
//  Author: Lu Zexi
//  2012-11-21



/// <summary>
/// 图集接口
/// </summary>
public interface IUIAtlas
{
    /// <summary>
    /// 精灵图列表
    /// </summary>
    List<ISprite> ISpriteList
    {
        get;
        set;
    }
}

