public sealed class RequestTextProvider
{
    public string GetStatusText(RequestStatus status)
    {
        return status switch
        {
            RequestStatus.Accepted => "접수",
            RequestStatus.SupplementRequired => "보완요구",
            RequestStatus.Deferred => "보류",
            RequestStatus.Rejected => "기각",
            RequestStatus.Pending => "대기",
            _ => "상태 불명"
        };
    }

    public string ConfirmText => "결재";
    public string NextDayText => "다음 날";
    public string EndText => "종료";

    public string GetAgencyAssignmentText(AgencyData agency)
    {
        return agency.Name;
    }

    public string GetDeadlineText(RequestData request)
    {
        return $"처리기한 D-{request.RemainingDays}";
    }

    public string GetRejectReasonText(RejectReason reason)
    {
        return reason switch
        {
            RejectReason.PersonalInformation => "개인정보",
            RejectReason.NationalSecurity => "국가안보",
            _ => "사유 불명"
        };
    }
}
