//
//  MaxPostProcessBuildiOS.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 8/5/20.
//  Copyright © 2020 AppLovin. All rights reserved.
//

#if UNITY_IOS || UNITY_IPHONE

using UnityEngine.Networking;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AppLovinMax
{
    [Serializable]
    public class SkAdNetworkData
    {
        [SerializeField] public string[] SkAdNetworkIds;
    }

    public class MaxPostProcessBuildiOS
    {
        private static readonly List<string> AtsRequiringNetworks = new List<string>
        {
            "AdColony",
            "ByteDance",
            "Fyber",
            "Google",
            "GoogleAdManager",
            "InMobi",
            "IronSource",
            "Smaato"
        };

        [PostProcessBuildAttribute(int.MaxValue)]
        public static void MaxPostProcessPlist(BuildTarget buildTarget, string path)
        {
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

//            EnableConsentFlowIfNeeded(plist);
            AddSkAdNetworksInfoIfNeeded(plist);
            UpdateAppTransportSecuritySettingsIfNeeded(plist);

            plist.WriteToFile(plistPath);
        }

//        private static void EnableConsentFlowIfNeeded(PlistDocument plist)
//        {
//            // Check if consent flow is enabled. No need to update info.plist if consent flow is disabled.
//            var consentFlowEnabled = AppLovinSettings.Instance.ConsentFlowEnabled;
//            if (!consentFlowEnabled) return;
//
//            var userTrackingUsageDescription = AppLovinSettings.Instance.UserTrackingUsageDescription;
//            var termsOfServiceUrl = AppLovinSettings.Instance.ConsentFlowTermsOfServiceUrl;
//            var privacyPolicyUrl = AppLovinSettings.Instance.ConsentFlowPrivacyPolicyUrl;
//            if (string.IsNullOrEmpty(userTrackingUsageDescription) || string.IsNullOrEmpty(termsOfServiceUrl) || string.IsNullOrEmpty(privacyPolicyUrl))
//            {
//                AppLovinIntegrationManager.ShowBuildFailureDialog("You cannot use the AppLovin SDK's consent flow without defining a Terms of Service URL, a Privacy Policy URL and the `User Tracking Usage Description` in the AppLovin Integration Manager. \n\n" +
//                                                                  "All 3 values must be included to enable the SDK's consent flow.");
//
//                // No need to update the info.plist here. Default consent flow state will be determined on the SDK side.
//                return;
//            }
//
//            var consentFlowInfoRoot = plist.root.CreateDict("AppLovinConsentFlowInfo");
//            consentFlowInfoRoot.SetBoolean("AppLovinConsentFlowEnabled", consentFlowEnabled);
//            consentFlowInfoRoot.SetString("AppLovinConsentFlowTermsOfService", termsOfServiceUrl);
//            consentFlowInfoRoot.SetString("AppLovinConsentFlowPrivacyPolicy", privacyPolicyUrl);
//
//            plist.root.SetString("NSUserTrackingUsageDescription", userTrackingUsageDescription);
//        }

        private static void AddSkAdNetworksInfoIfNeeded(PlistDocument plist)
        {
            var skAdNetworkData = GetSkAdNetworkData();
            var skAdNetworkIds = skAdNetworkData.SkAdNetworkIds;
            // Check if we have a valid list of SKAdNetworkIds that need to be added.
            if (skAdNetworkIds == null || skAdNetworkIds.Length < 1) return;

            //
            // Add the SKAdNetworkItems to the plist. It should look like following:
            //
            //    <key>SKAdNetworkItems</key>
            //    <array>
            //        <dict>
            //            <key>SKAdNetworkIdentifier</key>
            //            <string>ABC123XYZ.skadnetwork</string>
            //        </dict>
            //        <dict>
            //            <key>SKAdNetworkIdentifier</key>
            //            <string>123QWE456.skadnetwork</string>
            //        </dict>
            //        <dict>
            //            <key>SKAdNetworkIdentifier</key>
            //            <string>987XYZ123.skadnetwork</string>
            //        </dict>
            //    </array>
            //
            var skAdNetworkItems = plist.root["SKAdNetworkItems"];
            var existingSkAdNetworkIds = new HashSet<string>();
            // Check if SKAdNetworkItems array is already in the Plist document and collect all the IDs that are already present.
            if (skAdNetworkItems != null && skAdNetworkItems.GetType() == typeof(PlistElementArray))
            {
                var plistElementDictionaries = skAdNetworkItems.AsArray().values.Where(plistElement => plistElement.GetType() == typeof(PlistElementDict));
                foreach (var plistElement in plistElementDictionaries)
                {
                    var existingId = plistElement.AsDict()["SKAdNetworkIdentifier"];
                    if (existingId == null || existingId.GetType() != typeof(PlistElementString) || string.IsNullOrEmpty(existingId.AsString())) continue;

                    existingSkAdNetworkIds.Add(existingId.AsString());
                }
            }
            // Else, create an array of SKAdNetworkItems into which we will add our IDs.
            else
            {
                skAdNetworkItems = plist.root.CreateArray("SKAdNetworkItems");
            }

            foreach (var skAdNetworkId in skAdNetworkIds)
            {
                // Skip adding IDs that are already in the array.
                if (existingSkAdNetworkIds.Contains(skAdNetworkId)) continue;

                var skAdNetworkItemDict = skAdNetworkItems.AsArray().AddDict();
                skAdNetworkItemDict.SetString("SKAdNetworkIdentifier", skAdNetworkId);
            }
        }

        private static SkAdNetworkData GetSkAdNetworkData()
        {
            var uriBuilder = new UriBuilder("https://dash.applovin.com/docs/v1/unity_integration_manager/sk_ad_networks_info");

            // Get the list of installed ad networks to be passed up
            const string maxMediationDirectory = "Assets/MaxSdk/Mediation/";
            if (Directory.Exists(maxMediationDirectory))
            {
                var mediationNetworkDirectories = Directory.GetDirectories(maxMediationDirectory);
                var installedNetworks = mediationNetworkDirectories.Select(Path.GetFileName).ToList();
                var adNetworks = string.Join(",", installedNetworks);
                if (!string.IsNullOrEmpty(adNetworks))
                {
                    uriBuilder.Query += string.Format("adnetworks={0}", adNetworks);
                }
            }

            var unityWebRequest = UnityWebRequest.Get(uriBuilder.ToString());

#if UNITY_2017_2_OR_NEWER
            var operation = unityWebRequest.SendWebRequest();
#else
            var operation = unityWebRequest.Send();
#endif
            // Wait for the download to complete or the request to timeout.
            while (!operation.isDone) { }

#if UNITY_2017_2_OR_NEWER
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
#else
            if (unityWebRequest.isError)
#endif
            {
                Debug.LogError("Failed to retrieve SKAdNetwork IDs with error: " + unityWebRequest.error);
                return new SkAdNetworkData();
            }

            try
            {
                return JsonUtility.FromJson<SkAdNetworkData>(unityWebRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to parse data '" + unityWebRequest.downloadHandler.text + "' with exception: " + exception);
                return new SkAdNetworkData();
            }
        }

        private static void UpdateAppTransportSecuritySettingsIfNeeded(PlistDocument plist)
        {
            var projectHasAtsRequiringNetworks = AtsRequiringNetworks.Any(atsRequiringNetwork => Directory.Exists(Path.Combine("Assets/MaxSdk/Mediation/", atsRequiringNetwork)));
            if (!projectHasAtsRequiringNetworks) return;

            var root = plist.root.values;
            var atsRoot = root["NSAppTransportSecurity"];
            if (atsRoot == null || atsRoot.GetType() != typeof(PlistElementDict))
            {
                // Add the missing App Transport Security settings for publishers if needed. 
                Debug.Log("[AppLovin MAX] Adding App Transport Security settings...");
                atsRoot = plist.root.CreateDict("NSAppTransportSecurity");
                atsRoot.AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
            }

            var atsRootDict = atsRoot.AsDict().values;
            // Check if both NSAllowsArbitraryLoads and NSAllowsArbitraryLoadsInWebContent are present and remove NSAllowsArbitraryLoadsInWebContent if both are present.
            if (atsRootDict.ContainsKey("NSAllowsArbitraryLoads") && atsRootDict.ContainsKey("NSAllowsArbitraryLoadsInWebContent"))
            {
                Debug.Log("[AppLovin MAX] Removing NSAllowsArbitraryLoadsInWebContent");
                atsRootDict.Remove("NSAllowsArbitraryLoadsInWebContent");
            }
        }
    }
}

#endif
