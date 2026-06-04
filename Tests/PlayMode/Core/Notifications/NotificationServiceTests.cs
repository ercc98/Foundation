using System;
using NUnit.Framework;
using ErccDev.Foundation.Core.Notifications;

public class NotificationServiceTests
{
    // Minimal fake to verify the facade delegates to whatever is set as the default.
    private class FakeNotificationService : INotificationService
    {
        public int    Count;
        public string LastTitle;

        public event Action<NotificationData> OnNotification;

        public void Notify(in NotificationData data)
        {
            Count++;
            LastTitle = data.Title;
            OnNotification?.Invoke(data);
        }
    }

    [Test]
    public void SetDefault_Swaps_The_Implementation()
    {
        var fake = new FakeNotificationService();
        NotificationService.SetDefault(fake);

        Assert.AreSame(fake, NotificationService.Default);
    }

    [Test]
    public void SetDefault_Ignores_Null()
    {
        var fake = new FakeNotificationService();
        NotificationService.SetDefault(fake);
        NotificationService.SetDefault(null);

        Assert.AreSame(fake, NotificationService.Default);
    }

    [Test]
    public void Notify_Overload_Delegates_To_Default()
    {
        var fake = new FakeNotificationService();
        NotificationService.SetDefault(fake);

        NotificationService.Notify("Coins", "+500", null, NotificationCategory.Reward);

        Assert.AreEqual(1, fake.Count);
        Assert.AreEqual("Coins", fake.LastTitle);
    }

    [Test]
    public void Notify_Struct_Overload_Delegates_To_Default()
    {
        var fake = new FakeNotificationService();
        NotificationService.SetDefault(fake);

        NotificationService.Notify(new NotificationData("Direct"));

        Assert.AreEqual(1, fake.Count);
        Assert.AreEqual("Direct", fake.LastTitle);
    }
}
