//using UnityEngine;
//using System.Collections;
//using System.Runtime.InteropServices;
//using System;
//using System.IO;


//public class NativeShareMgr : SingletonMonoBehaviour<NativeShareMgr>
//{
//    [SerializeField]
//    public string subject = "";
//    [SerializeField]
//    public string textShare = "";
   


//    public void ShareLink()
//    {
//        textShare = String.Format("#TapToMerge# Hi all,I just complete Tap to merge game and my highest Merged Number is {0}, Try to get 10 and pass over come my score and my number.", GameDataMgr.HighScore);
//        #if UNITY_ANDROID
//        Share(textShare +" Google Play : " +GameRemoteSettings.Instance.AndroidLinkShare , subject);
//        #else
//        Share(textShare + " App Store "+ GameRemoteSettings.Instance.IOSLinkShare, subject);
//        #endif
//    }

//    public void ShareScreenShot()
//    {
//        string url = "";

//#if UNITY_ANDROID
//    //    url = urlAndroid;
//#elif UNITY_IOS
// //   url = urlIOS;
//#endif
//        textShare = String.Format("#TapToMerge# Hi all,I just complete Tap to merge game and my highest Merged Number is {0}, Try to get 10 and pass over come my score and my number.", GameDataMgr.HighScore);
//        ShareScreenshotWithText(textShare, subject, url);
//    }

//    void Share(string url, string subject)
//    {
//#if UNITY_ANDROID
//        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
//        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

//        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
//        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), url);
//        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

//        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

//        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
//        currentActivity.Call("startActivity", jChooser);
//#elif UNITY_IOS
//		CallSocialShareAdvanced(url, subject, "", "");
//#else
//        Debug.Log("No sharing set up for this platform.");
//#endif
//    }

//    public string ScreenshotName = "screenshot.png";

//    void ShareScreenshotWithText(string text, string subject, string url)
//    {
//        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
//        if (File.Exists(screenShotPath)) File.Delete(screenShotPath);

//        ScreenCapture.CaptureScreenshot(ScreenshotName);

//        StartCoroutine(delayedShare(screenShotPath, text, url, subject));
//    }

//    //CaptureScreenshot runs asynchronously, so you'll need to either capture the screenshot early and wait a fixed time
//    //for it to save, or set a unique image name and check if the file has been created yet before sharing.
//    IEnumerator delayedShare(string screenShotPath, string text, string url, string subject)
//    {
//        while (!File.Exists(screenShotPath))
//        {
//            yield return new WaitForSeconds(.05f);
//        }

//        Share(text, screenShotPath, url, subject);
//    }

//    public void Share(string shareText, string imagePath, string url, string subject)
//    {
//#if UNITY_ANDROID
//        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
//        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

//        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
//        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
//        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
//        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
//        intentObject.Call<AndroidJavaObject>("setType", "image/png");

//        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

//        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

//        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
//        currentActivity.Call("startActivity", jChooser);
//#elif UNITY_IOS
//		CallSocialShareAdvanced(shareText, subject, url, imagePath);
//#else
//		Debug.Log("No sharing set up for this platform.");
//#endif
//    }

//#if UNITY_IOS
//	public struct ConfigStruct
//	{
//		public string title;
//		public string message;
//	}

//	[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

//	public struct SocialSharingStruct
//	{
//		public string text;
//		public string url;
//		public string image;
//		public string subject;
//	}

//	[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

//	public static void CallSocialShare(string title, string message)
//	{
//		ConfigStruct conf = new ConfigStruct();
//		conf.title  = title;
//		conf.message = message;
//		showAlertMessage(ref conf);
//	}


//	public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
//	{
//		SocialSharingStruct conf = new SocialSharingStruct();
//		conf.text = defaultTxt;
//		conf.url = url;
//		conf.image = img;
//		conf.subject = subject;

//		showSocialSharing(ref conf);
//	}
//#endif
//}