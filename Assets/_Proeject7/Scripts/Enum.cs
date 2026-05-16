public enum GamePhase
{
    RequestReview,
    AgencyAssignment,
    ResponseReview,
    DisclosureDecision,
    Result,
}

public enum RequestStatus
{
    Pending,            // 대기
    Accepted,           // 접수
    SupplementRequired, // 보완요구
    Deferred,           // 보류
    Rejected            // 기각
}

public enum RejectReason
{
    PersonalInformation,
    NationalSecurity,
}

public enum RequestFactTag
{
    ContainsPersonalInfo,
    SecuritySensitive,
    PublicInterestHigh,
}
