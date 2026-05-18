public sealed class PhaseTextProvider
{
    public string GetPhaseText(GamePhase phase)
    {
        return phase switch
        {
            GamePhase.Intro => "인트로",
            GamePhase.RequestReview => "청구 검토",
            GamePhase.AgencyAssignment => "관계기관 배정",
            GamePhase.ResponseReview => "기관 회신 검토",
            GamePhase.DisclosureDecision => "공개 범위 결정",
            GamePhase.Result => "결과 확인",
            GamePhase.Ending => "종료",
            _ => "상태 불명"
        };
    }
}
