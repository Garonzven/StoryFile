using UnityEngine;
using System.Collections;


namespace DDK.OneSignalExtensions
{
    #if USE_ONE_SIGNAL
    /// <summary>
    /// One signal keys.
    /// <seealso cref="https://documentation.onesignal.com/reference"/> 
    /// <seealso cref="https://documentation.onesignal.com/docs/unity-sdk#section-sending-notifications"/>
    /// </summary>
    public enum OneSignalKeys 
    {
    /// <summary>
    /// string Required
    /// Your OneSignal Application Key
    /// </summary>
    app_id,
    /// <summary>
    /// object Required unless content_available=true, *_background_data=true, or template_id is set.
    /// Message contents to send to players. "en" (English) is required. The key of each hash is either a a 2 
    /// character language code or one of zh-Hans/zh-Hant for Simplified or Traditional Chinese. The value of each 
    /// key is the message that will be sent to users for that language. If do not specify a language English will be used.
    /// Example: {"en": "English Message","es": "Spanish Message"}
    /// </summary>
    contents,
    /// <summary>
    /// object Optional
    /// Notification title to send to Android, Amazon, Chrome apps, and Chrome Websites. By default, this will be 
    /// the application name. If this option is used, a value for "en" (English) is required. The value of each 
    /// key is the title that will be sent to users for that language. If do not specify a language English will be used.
    /// Example: {"en": "English Title","es": "Spanish Title"}
    /// </summary>
    headings,
    /// <summary>
    /// booleanOptional.
    /// Send to iOS devices?
    /// </summary>
    isIos,
    /// <summary>
    /// boolean Optional.
    /// Send to Google Play Android devices?
    /// </summary>
    isAndroid,
    /// <summary>
    /// boolean Optional.
    /// Send to Windows Phone Devices?
    /// </summary>
    isWP,
    /// <summary>
    /// boolean Optional.
    /// Send to Amazon ADM devices?
    /// </summary>
    isAdm,
    /// <summary>
    /// boolean Optional.
    /// Send to Google Chrome App/Extension?
    /// </summary>
    isChrome,
    /// <summary>
    /// object Optional
    /// Send to Chrome Web subscribers?
    /// </summary>
    isChromeWeb,
    /// <summary>
    /// object Optional
    /// Send to Safari subscribers?
    /// </summary>
    isSafari,
    /// <summary>
    /// object Optional
    /// Send to all web subscribers? Includes ChromeWeb, Safari, Firefox, and any future browsers.
    /// </summary>
    isAnyWeb,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Names of segments to send the message to. Any player in any of these segments will be sent this notification. 
    /// Only compatible with excluded_segments, do not use any other targeting parameters with this one.
    /// Example: ["Free Players", "New Players"]
    /// NOTE: 'REST API Key' Required
    /// </summary>
    include_segments,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Use with included_segments. Do not use with any other targeting parameters. Names of segments to exclude 
    /// players from. Any players in any of these segments will not be sent the notification even if they are a 
    /// member of included_segments. Requires API Auth Key.
    /// Example: ["Free Players", "New Players"]
    /// </summary>
    exclude_segments,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Specific players to send your notification to. Do not combine with other targeting parameters. Not 
    /// compatible with any other targeting parameters. Does not require API Auth Key.
    /// Example: ["1dd608f2-c6a1-11e3-851d-000c2940e62c"]
    /// </summary>
    include_player_ids,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_player_ids instead. Specific iOS device tokens to send the notification to 
    /// (all non-alphanumeric characters must be removed from the tokens). Do not combine with other targeting 
    /// parameters. If any of these tokens do not correspond to players in our system, a player will be automatically 
    /// created for each of them. Does not require your "API Auth Key".
    /// Example: ["ce777617da7f548fe7a9ab6febb56cf39fba6d38203..."]
    /// </summary>
    include_ios_tokens,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_playerids instead. Specific Android registration ids to send the notification to. 
    /// Do not combine with other targeting parameters. If these registration ids do not correspond to players in 
    /// our system, players will be automatically created. Does not require your "API Auth Key".
    /// Example: `["APA91bEeiUeSukAAUdnw3O2RB45FWlSpgJ7Ji..."]`
    /// </summary>
    include_android_reg_ids,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_player_ids instead. Specific Windows Phone 8.0 channel URI to send the notification 
    /// to. Do not combine with other targeting parameters. If these channel URIs do not correspond to players in 
    /// our system, players will be automatically created. Does not require your "API Auth Key".
    /// Example: ["http://s.notify.live.net/u/1/bn1/HmQAAACPaLDr-..."]
    /// </summary>
    include_wp_uris,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_player_ids instead. Specific Windows Phone 8.1 channel URI to send the notification 
    /// to. Do not combine with other targeting parameters. If these channel URIs do not correspond to players in 
    /// our system, players will be automatically created. Does not require your "API Auth Key".
    /// Example: ["http://s.notify.live.net/u/1/bn1/HmQAAACPaLDr-..."]
    /// </summary>
    include_wp_wns_uris,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_player_ids instead. Specific Amazon ADM registration ids to send the notification 
    /// to. Do not combine with other targeting parameters. If these registration ids do not correspond to players 
    /// in our system, players will be automatically created. Does not require your "API Auth Key".
    /// Example: ["amzn1.adm-registration.v1.XpvSSUk0Rc3hTVVV..."]
    /// </summary>
    include_amazon_reg_ids,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_playerids instead. Specific Chrome App/Extension Google registration ids to send the notification to. Do not combine with other targeting parameters. If these registration ids do not correspond to players in our system, players will be automatically created. Does not require your "API Auth Key".
    /// Example: `["APA91bEeiUeSukAAUdnw3O2RB45FWlSpgJ7Ji..."]`
    /// </summary>
    include_chrome_reg_ids,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Recommend using include_player_ids instead. Specific Chrome Web Google registration ids to send the 
    /// notification to. Highly recommend using include_player_ids instead as these can change from time to time.
    /// Do not combine with other targeting parameters. If these registration ids do not correspond to players 
    /// in our system, players will be automatically created. Does not require your "API Auth Key".
    /// Example: ["APA91bEeiUeSukAAUdnw3O2RB45FWlSpgJ7Ji_..."]
    /// </summary>
    include_chrome_web_reg_ids,
    /// <summary>
    /// array of strings Optional. Targeting parameter.
    /// Used to target all users across a list of your apps. If a user has 2 or more of the apps installed they will only receive 1 notification.
    /// Example: ["2dd608f2-a6a1-11e3-251d-400c2940e62b", "2dd608f2-a6a1-11e3-251d-500f2950e61c"]
    /// NOTE: Requires your "User Auth Key" in the HTTP header.
    /// </summary>
    app_ids,
    /// <summary>
    /// array of object Optional. Targeting parameter.
    /// Match users by the values set for their tags. Do not combine with other targeting parameters. Possible 
    /// relations are ">", "<", and "=". "<" means that the value of the user's tag must be less than the value 
    /// supplied in the "value" field of this hash. By default users must match all of the hashes specified in 
    /// this array to receive the notification. You may add {"operator": "OR"} between the entries to allow any of 
    /// the tag conditions. A maximum of 200 tags and operators can be used at a time.
    /// Note: Requires "REST API Key".        
    /// Example 1
    /// level='10' AND madePurchase='true'
    /// [{"key": "level", "relation": ">", "value": "10"},
    /// {"key": "madePurchase", "relation": "=","value": "true"}]
    /// Example 2:
    /// level='10' OR level='20'
    /// [{"key": "level", "relation": "=", "value": "10"},
    /// {"operator": "OR"},
    /// {"key": "level", "relation": "=", "value": "20"}]
    /// </summary>
    tags,
    /// <summary>
    /// string Optional
    /// Options are: None, SetTo, or Increase.
    /// None leaves the count unaffected on the device. If you use Increase, it will be based on the current value of the player's badge_count.
    /// </summary>
    ios_badgeType,
    /// <summary>
    /// integer Optional
    /// Sets or increases the badge icon on the device depending on the ios_badgeType value
    /// </summary>
    ios_badgeCount,
    /// <summary>
    /// string Optional
    /// Sound file that is included in your app to play instead of the default device notification sound. Pass 
    /// "nil" to disable vibration and sound for the notification.
    /// Example: "notification.wav"
    /// </summary>
    ios_sound,
    /// <summary>
    /// string Optional
    /// Sound file that is included in your app to play instead of the default device notification sound.
    /// NOTE: Leave off file extension for Android.
    /// Example: "notification"
    /// </summary>
    android_sound,
    /// <summary>
    /// string Optional
    /// Amazon devices
    /// Sound file that is included in your app to play instead of the default device notification sound.
    /// NOTE: Leave off file extension for Android.
    /// Example: "notification"
    /// </summary>
    adm_sound,
    /// <summary>
    /// string Optional
    /// Windows Phone 8.0
    ///Sound file that is included in your app to play instead of the default device notification sound.
    /// Example: "notification.wav"
    /// </summary>
    wp_sound,
    /// <summary>
    /// string Optional
    /// Windows Phone 8.1
    /// Sound file that is included in your app to play instead of the default device notification sound.
    /// Example: "notification.wav"
    /// </summary>
    wp_wns_sound,
    /// <summary>
    /// object Optional
    /// Custom key value pair hash that you can programmatically read in your app's code.
    /// Example: {"abc": "123", "foo": "bar"}
    /// </summary>
    data,
    /// <summary>
    /// mixed Optional
    /// Buttons to add to the notification. Supported by iOS 8.0 and Android 4.1+ devices. Icon only works for Android.
    /// Example: [{"id": "id1", "text": "button1", "icon": "ic_menu_share"}, {"id": "id2", "text": "button2", "icon": "ic_menu_send"}]
    /// </summary>
    buttons,
    /// <summary>
    /// string Optional
    /// Specific Android icon to use. If blank the app icon is used. Must be the drawable resource name.
    /// </summary>
    small_icon,
    /// <summary>
    /// string Optional
    /// Specific Android icon to display to the left of the notification. If blank the small_icon is used. Can be a drawable resource name or a URL.
    /// </summary>
    large_icon,
    /// <summary>
    /// string Optional
    /// Specific Android picture to display in the expanded view. Can be a drawable resource name or a URL.
    /// </summary>
    big_picture,
    /// <summary>
    /// string Optional
    /// Specific Amazon icon to use. If blank the app icon is used. Must be the drawable resource name.
    /// </summary>
    adm_small_icon,
    /// <summary>
    /// string Optional
    /// Specific Amazon icon to display to the left of the notification. If blank the adm_small_icon is used. Can be a drawable resource name or a URL.
    /// </summary>
    adm_large_icon,
    /// <summary>
    /// string Optional
    /// Specific Amazon picture to display in the expanded view. Can be a drawable resource name or a URL.
    /// </summary>
    adm_big_picture,
    /// <summary>
    /// string Optional
    /// For Chrome extensions & apps only, NOT for Chrome web push. The local URL to an icon to use. if blank the 
    /// app icon will be used. For Chrome web push please see "chrome_web_icon".
    /// </summary>
    chrome_icon,
    /// <summary>
    /// string Optional
    /// Specific Chrome large picture to display below the notification text. Must be a local URL.
    /// </summary>
    chrome_big_picture,
    /// <summary>
    /// string Optional
    /// Specific Chrome website image URL to be shown to the left of the notification text. Should be 80x80 @ 24bit 
    /// so it displays on all platforms correctly.
    /// </summary>
    chrome_web_icon,
    /// <summary>
    /// string Optional
    /// Specific Firefox website image URL to be shown to the right of the notification text.
    /// </summary>
    firefox_icon,
    /// <summary>
    /// string Optional
    /// Example: "http://google.com"
    /// </summary>
    url,
    /// <summary>
    /// string Optional
    /// All examples are the exact same date & time.
    /// "Thu Sep 24 2015 14:00:00 GMT-0700 (PDT)"
    // "September 24th 2015, 2:00:00 pm UTC-07:00"
    /// "2015-09-24 14:00:00 GMT-0700"
    /// "Sept 24 2015 14:00:00 GMT-0700"
    /// "Thu Sep 24 2015 14:00:00 GMT-0700 (Pacific Daylight Time)"
    /// </summary>
    send_after,
    /// <summary>
    /// string Optional
    /// Possible values are:
    /// "timezone" (Deliver at a specific time-of-day in each users own timezone)
    /// "last-active" (Deliver at the same time of day as each user last used your app).
    /// If "send_after" is used, this takes effect after the "send_after' time has elapsed.
    /// </summary>
    delayed_option,
    /// <summary>
    /// string Optional
    /// Use with delayed_option=timezone.
    /// Example: "9:00AM"
    /// </summary>
    delivery_time_of_day,
    /// <summary>
    /// string Optional
    /// Sets the devices LED notification light if the device has one. ARGB Hex format.
    /// Example(Blue): "FF0000FF"
    /// </summary>
    android_led_color,
    /// <summary>
    /// string Optional
    /// Sets the background color of the notification circle to the left of the notification text. Only applies to apps 
    /// targeting Android API level 21+ on Android 5.0+ devices.
    /// Example(Red): "FFFF0000"
    /// </summary>
    android_accent_color,
    /// <summary>
    /// integer Optional
    /// Sets the lock screen visibility for apps targeting Android API level 21+ running on Android 5.0+ devices.
    /// -1 = Secret (Notification does not show on the lock screen at all)
    /// 0 = Private (Hides message contents on lock screen if the user set "Hide sensitive notification content" in the system settings)
    /// 1 = Public (OneSignal's default) (Shows the full message on the lock screen unless the user has disabled all notifications 
    /// from showing on the lock screen. Please consider the user and mark private if the contents are.)
    /// </summary>
    android_visibility,
    /// <summary>
    /// boolean Optional
    /// For iOS devices. Sends content-available=1 to wake your app to run custom native code.
    /// </summary>
    content_available,
    /// <summary>
    /// boolean Optional
    /// Fires a Broadcast with a com.onesignal.BackgroundBroadcast.RECEIVE action when a notification is received. 
    /// See our http://documentation.onesignal.com/docs/android-notification-customizations#background-data-silent-notifications.
    /// </summary>
    android_background_data,
    /// <summary>
    /// boolean Optional
    /// Fires a Broadcast with a com.onesignal.BackgroundBroadcast.RECEIVE action when a notification is received. 
    /// See our Background Data Documentation.
    /// </summary>
    amazon_background_data,
    /// <summary>
    /// string Optional
    /// Use a template you setup on our dashboard. You can override the template values by sending other parameters with the request. 
    /// The template_id is the UUID found in the URL when viewing a template on our dashboard.
    /// Example: be4a8044-bbd6-11e4-a581-000c2940e62c
    /// </summary>
    template_id,
    /// <summary>
    /// string Optional
    /// All Android notifications with the same group will be stacked together using 
    /// Android's https://developer.android.com/training/wearables/notifications/stacks.html feature.
    /// </summary>
    android_group,
    /// <summary>
    /// object Optional
    ///Summary message to display when 2+ notifications are stacked together. Default is "# new messages". 
    /// Include $[notif_count] in your message and it will be replaced with the current number. "en" (English) is 
    /// required. The key of each hash is either a a 2 character language code or one of zh-Hans/zh-Hant for 
    /// Simplified or Traditional Chinese. The value of each key is the message that will be sent to users for that language.
    /// Example: "{ "en" : "You have $[notif_count] new messages"}
    /// </summary>
    android_group_message,
    /// <summary>
    /// string Optional
    /// All Amazon notifications with the same group will be stacked together using 
    /// Android's https://developer.android.com/training/wearables/notifications/stacks.html feature.
    /// </summary>
    adm_group,
    /// <summary>
    /// object Optional
    /// Summary message to display when 2+ notifications are stacked together. Default is "# new messages". 
    /// Include $[notif_count] in your message and it will be replaced with the current number. "en" (English) is 
    /// required. The key of each hash is either a a 2 character language code or one of zh-Hans/zh-Hant for 
    /// Simplified or Traditional Chinese. The value of each key is the message that will be sent to users for that language.
    /// Example: "{ "en" : "You have $[notif_count] new messages"}
    /// </summary>
    adm_group_message
    }
    #endif
}
