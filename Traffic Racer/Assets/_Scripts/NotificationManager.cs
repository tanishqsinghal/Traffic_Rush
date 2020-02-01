using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
	[SerializeField] string channelId = null;								//Id of the notification channel
	[SerializeField] string notificationTitle = null;						//Title of the notification
	[SerializeField] string notificationText = null;						//Description text of the notification
	[SerializeField] float fireTime = 6f;									//Fire time of the notification

    // Start is called before the first frame update
    void Start()
    {
		//Cancel all previous notifications
		AndroidNotificationCenter.CancelAllNotifications ();

		//Make notification channel and register it
		AndroidNotificationChannel _notificationChannel = new AndroidNotificationChannel () {
			Id = channelId,
			Name = "Traffic_Racer_Channel",
			Importance = Importance.High,
			Description = "General Notification",
		};
		AndroidNotificationCenter.RegisterNotificationChannel (_notificationChannel);

		//Build a simple notification and schedule it
		AndroidNotification notification = new AndroidNotification ();

		notification.Title = notificationTitle;
		notification.Text = notificationText;
		notification.FireTime = System.DateTime.Now.AddHours (fireTime);
		notification.SmallIcon = "icon_0";
		notification.LargeIcon = "icon_1";
		int notificationIdentifier = AndroidNotificationCenter.SendNotification (notification, channelId);
    }
}
