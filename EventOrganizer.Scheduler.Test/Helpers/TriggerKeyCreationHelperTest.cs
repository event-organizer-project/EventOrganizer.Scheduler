using AutoFixture;
using EventOrganizer.Scheduler.Helpers;

namespace EventOrganizer.Scheduler.Test.Helpers
{
    public class TriggerKeyCreationHelperTest
    {
        private Fixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
        }

        [Test]
        public void GetUserId_Should_Return_Expected_Result()
        {
            var eventId = fixture.Create<int>();
            var subscriptionId = fixture.Create<int>();

            var result = TriggerKeyCreationHelper.CreateNotificationTriggerKey(eventId, subscriptionId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo($"notification-{eventId}-{subscriptionId}"));
                Assert.That(result.Group, Is.EqualTo("trigger"));
            });
        }
    }
}
