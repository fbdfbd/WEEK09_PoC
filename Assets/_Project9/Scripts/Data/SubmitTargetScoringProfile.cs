using System;
using UnityEngine;

namespace Project9.Data
{
    [Serializable]
    public sealed class SubmitTargetScoringProfile
    {
        [SerializeField, Range(-10, 10)] private int informationValueWeight = 1;
        [SerializeField, Range(-10, 10)] private int integrityWeight = 1;
        [SerializeField, Range(-10, 10)] private int exposureWeight;
        [SerializeField, Range(-10, 10)] private int exposureReductionWeight;
        [SerializeField, Range(0, 20)] private int distortionPenaltyWeight = 1;
        [SerializeField, Range(0, 20)] private int fullMaskPenaltyWeight = 1;

        public int InformationValueWeight => informationValueWeight;
        public int IntegrityWeight => integrityWeight;
        public int ExposureWeight => exposureWeight;
        public int ExposureReductionWeight => exposureReductionWeight;
        public int DistortionPenaltyWeight => distortionPenaltyWeight;
        public int FullMaskPenaltyWeight => fullMaskPenaltyWeight;
    }
}
