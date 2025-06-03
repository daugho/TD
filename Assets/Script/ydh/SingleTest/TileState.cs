public enum TileState
{
    None,           // 아무것도 없음
    Installable,     // 설치 가능
    Uninstallable,   // 설치 불가
    Installed,        // 이미 설치됨
    StartPoint,    // 출발 지점
    EndPoint,        //도착지점
    MasterInstallable,     // 마스터전용 타일
    ClientInstallable       //클라이언트 전용 타일
}
public enum TileAccessType
{
    Everyone,
    MasterOnly,
    ClientOnly
}
public enum EditMode
{
    TileStateEdit,     // 설치 상태 편집 모드
    AccessTypeEdit     // 접근 권한 설정 모드
}

