using System.Collections.Generic;
using Project9.Data;

namespace Project9.Runtime
{
    public sealed class ReportSessionFactory
    {
        public ReportSessionState Create(
            ReportDefinition reportDefinition,
            IReadOnlyList<SubmitTargetDefinition> submitTargets)
        {
            return new ReportSessionState(reportDefinition, submitTargets);
        }
    }
}
