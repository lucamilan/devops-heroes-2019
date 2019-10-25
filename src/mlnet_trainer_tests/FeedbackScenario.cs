using System.Collections.Generic;
using System.Linq;
using mlnet_model;

namespace mlnet_trainer_tests.Scenario_01
{
    public class FeedbackScenario
    {
        public static IEnumerable<object[]> Inputs =>
            new List<object[]>
            {
                new object[] {"I loving DevOps Heroes 2019!", true},
                new object[] {"I highly recommend this session", true},
                new object[]{"Not enjoyed, I don't recommend this speech",false},
                new object[] {"This session and that speakers are fantastic", false},
            };
    }
}