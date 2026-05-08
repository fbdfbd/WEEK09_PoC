namespace App.Gameplay.Runtime
{
    /// <summary>
    /// Stability               : 안정도. 변화 속에서 버티는 정서적 안정
    /// Autonomy                : 자율성. 자기 취향/호기심/표현/결정 능력
    /// Trust                   : 플레이어를 향한 신뢰. 라포
    /// ControlAwareness        : 통제 인식 정도(육성 대상 내부 수치. 이건 UI에 노출되지 않음). 초반엔 위화감, 후반엔 통제 인식
    /// InstitutionEvaluation   : 기관의 평가(외부-내부수치. UI에 노출되지 않음... 진행 자체에 영향이 아직은 없을 예정(PoC에서는).
    ///                           강하면 강제성의 느낌이고, 없다면 무의미한 느낌)
    /// </summary>
    public enum CharacterStatType
    {
        Stability,
        Autonomy,
        Trust,
        ControlAwareness,
        InstitutionEvaluation
    }
}
