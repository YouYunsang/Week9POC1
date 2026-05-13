using System;
using UnityEngine;

public sealed class MapToolState : MonoBehaviour
{
    [SerializeField] private MapToolMode _currentToolMode = MapToolMode.None;
    [SerializeField] private MapStampType _currentStampType = MapStampType.Mutant;

    public event Action ToolChanged;

    public MapToolMode CurrentToolMode => _currentToolMode;
    public MapStampType CurrentStampType => _currentStampType;

    public void SelectNone()
    {
        // 현재 맵 도구를 해제한다.
        _currentToolMode = MapToolMode.None;
        ToolChanged?.Invoke();
    }

    public void SelectWallTool()
    {
        // 벽 선 그리기 도구를 선택한다.
        _currentToolMode = MapToolMode.Wall;
        ToolChanged?.Invoke();
    }

    public void SelectMutantStamp()
    {
        // 변이체 스탬프 도구를 선택한다.
        _currentToolMode = MapToolMode.Stamp;
        _currentStampType = MapStampType.Mutant;
        ToolChanged?.Invoke();
    }

    public void SelectKeyCardDoorStamp()
    {
        // ID 카드 문 스탬프 도구를 선택한다.
        _currentToolMode = MapToolMode.Stamp;
        _currentStampType = MapStampType.KeyCardDoor;
        ToolChanged?.Invoke();
    }

    public void SelectBatteryChargerStamp()
    {
        // 배터리 충전기 스탬프 도구를 선택한다.
        _currentToolMode = MapToolMode.Stamp;
        _currentStampType = MapStampType.BatteryCharger;
        ToolChanged?.Invoke();
    }
}