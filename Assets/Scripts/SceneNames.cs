using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneNames
{
    public enum SceneTypeNames {
        StartMenu,
        _3DScene,
        _2DScene,
        ExitMenu,
        OptionsMenu,
    }

    public enum AllSceneNames {
        TimeToSwitchToWorld,
        TimeToSwitchToPuzzle,
        TimeToSwitchToNextDay,
        FirstScene,
        SecondScene,
        ThirdScene,
        FourthScene,
        FifthScene
    }

    public static readonly AllSceneNames[] firstDayScenes = new AllSceneNames[] {
        AllSceneNames.FirstScene,
        AllSceneNames.SecondScene,
        AllSceneNames.ThirdScene,
        AllSceneNames.TimeToSwitchToPuzzle,
        AllSceneNames.FourthScene,
        AllSceneNames.TimeToSwitchToWorld,
        AllSceneNames.FifthScene,
        AllSceneNames.TimeToSwitchToNextDay
    };

    public static readonly AllSceneNames[] secondDayScenes = new AllSceneNames[] {
        AllSceneNames.FirstScene,
        AllSceneNames.SecondScene,
        AllSceneNames.ThirdScene,
        AllSceneNames.TimeToSwitchToPuzzle,
        AllSceneNames.FourthScene,
        AllSceneNames.TimeToSwitchToWorld,
        AllSceneNames.FifthScene,
        AllSceneNames.TimeToSwitchToNextDay
    };

    public static readonly AllSceneNames[] thirdDayScenes = new AllSceneNames[] {
        AllSceneNames.FirstScene,
        AllSceneNames.SecondScene,
        AllSceneNames.ThirdScene,
        AllSceneNames.TimeToSwitchToPuzzle,
        AllSceneNames.FourthScene,
        AllSceneNames.TimeToSwitchToWorld,
        AllSceneNames.FifthScene,
        AllSceneNames.TimeToSwitchToNextDay
    };

    public static readonly AllSceneNames[][] scenesOfAllDays = new AllSceneNames[][] {
        firstDayScenes,
        secondDayScenes,
        thirdDayScenes
    };
}