using System;
using Project9.Runtime;

namespace Project9.Systems
{
    public sealed class ReputationSystem
    {
        public void ApplySubmissionResult(ReportSessionState session, SubmissionResult result)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var reputation = session.GetReputation(result.Target.Id);
            if (reputation == null)
            {
                throw new InvalidOperationException($"Missing reputation state for target: {result.Target.Id}");
            }

            reputation.Add(result.ReputationDelta);
        }
    }
}
