using System.Collections.Generic;
using System.Linq;
using System.Text;

public sealed class EndingReportBuilder
{
    private readonly RequestStore _requestStore;
    private readonly AgencyStore _agencyStore;
    private readonly RequestHistory _history;
    private readonly PlayerTrustStore _playerTrustStore;
    private readonly RejectDecisionEvaluator _rejectDecisionEvaluator;

    public EndingReportBuilder(
        RequestStore requestStore,
        AgencyStore agencyStore,
        RequestHistory history,
        PlayerTrustStore playerTrustStore,
        RejectDecisionEvaluator rejectDecisionEvaluator)
    {
        _requestStore = requestStore;
        _agencyStore = agencyStore;
        _history = history;
        _playerTrustStore = playerTrustStore;
        _rejectDecisionEvaluator = rejectDecisionEvaluator;
    }

    public EndingReport Build()
    {
        var trust = _playerTrustStore.Trust.Value;
        var worstRelation = _agencyStore.All.Count > 0
            ? _agencyStore.All.Min(agency => agency.Relation)
            : AgencyData.MaxRelation;
        var achievement = CalculateAchievement(trust, worstRelation);

        return new EndingReport(
            ResolveTitle(achievement, worstRelation),
            ResolvePlayerOutcome(achievement, worstRelation),
            $"업무 성취도: {achievement}",
            $"업무 신뢰도: {trust}/{PlayerTrustStore.MaxTrust}",
            BuildAgencyRelationsText(),
            BuildIncidentsText()
        );
    }

    private int CalculateAchievement(int trust, int worstRelation)
    {
        var acceptedCount = _history.Records.Count(record => record.Status == RequestStatus.Accepted);
        var rejectedCount = _history.Records.Count(record => record.Status == RequestStatus.Rejected);
        var deferredCount = _history.Records.Count(record => record.Status == RequestStatus.Deferred);

        var score = trust + acceptedCount * 3 + rejectedCount * 2 - deferredCount * 4;

        if (worstRelation <= 1)
            score -= 20;
        else if (worstRelation <= 3)
            score -= 10;

        return Clamp(score, 0, 100);
    }

    private static string ResolveTitle(int achievement, int worstRelation)
    {
        if (achievement < 35 || worstRelation <= 1)
            return "기록 말소";

        if (achievement >= 75 && worstRelation >= 4)
            return "비공식 제안";

        return "계속 근무";
    }

    private static string ResolvePlayerOutcome(int achievement, int worstRelation)
    {
        if (achievement < 35 || worstRelation <= 1)
            return "당신은 관계기관 합동조사 이후 직무에서 배제되었습니다. 인사 기록에는 전보 처리로 남았지만, 모두가 당신도 전임자들과 같아졌다는 걸 압니다.";

        if (achievement >= 75 && worstRelation >= 4)
            return "당신은 까다로운 청구들을 안정적으로 처리했습니다. 며칠 뒤 군 보안국 소속 장교가 찾아와, 더 중요한 기록을 다룰 자리를 제안합니다.";

        return "큰 문제 없이 임기를 이어갑니다. 다만 몇몇 기관들은 당신을 예의주시합니다.";
    }

    private string BuildAgencyRelationsText()
    {
        var builder = new StringBuilder();

        foreach (var agency in _agencyStore.All.OrderBy(agency => agency.Id))
            builder.AppendLine($"{agency.Name}: {agency.Relation}/{AgencyData.MaxRelation} ({GetRelationGrade(agency.Relation)})");

        return builder.ToString().TrimEnd();
    }

    private string BuildIncidentsText()
    {
        var lines = new List<string>();

        foreach (var record in _history.Records)
        {
            var request = _requestStore.FindOrNull(record.RequestId);
            if (request == null)
                continue;

            if (record.Status == RequestStatus.Accepted && _rejectDecisionEvaluator.ShouldReject(request))
                lines.Add($"{request.Id}: 기각 대상 사건을 접수했습니다.");

            if (record.Status == RequestStatus.Rejected && !_rejectDecisionEvaluator.ShouldReject(request))
                lines.Add($"{request.Id}: 공개 가능성이 있는 사건을 기각했습니다.");
        }

        return lines.Count > 0
            ? string.Join("\n", lines)
            : "특이 사건 없음";
    }

    private static string GetRelationGrade(int relation)
    {
        if (relation <= 1)
            return "파탄";

        if (relation <= 3)
            return "위험";

        if (relation <= 6)
            return "보통";

        if (relation <= 8)
            return "우호";

        return "신뢰";
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min)
            return min;

        if (value > max)
            return max;

        return value;
    }
}
