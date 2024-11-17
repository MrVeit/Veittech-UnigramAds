using System;
using UnigramAds.Common;
using UnigramAds.Core.Bridge;
using UnigramAds.Utils;

namespace UnigramAds.Core
{
    public sealed class UnigramAdsSDK
    {
        private static UnigramAdsSDK _instance;

        public static UnigramAdsSDK Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(UnigramAdsSDK))
                    {
                        if (_instance == null)
                        {
                            _instance = new UnigramAdsSDK(null);
                        }
                    }
                }

                return _instance;
            }
        }

        public string AppId { get; private set; }

        public string InterstitialAdUnit { get; private set; }
        public string RewardedAdUnit { get; private set; }
        public string BannerAdUnit { get; private set; }

        public bool IsInitialized { get; private set; }
        public bool IsTestMode { get; private set; }

        public AdTypes DebugAdType { get; private set; }

        private UnigramAdsSDK(Builder builder)
        {
            this.IsInitialized = builder.IsInitialized;
            this.IsTestMode = builder.IsTestMode;

            this.AppId = builder.AppId;
            this.InterstitialAdUnit = builder.InterstitialAdUnit;
            this.RewardedAdUnit = builder.RewardedAdUnit;
            this.BannerAdUnit = builder.BannerAdUnit;
        }

        public sealed class Builder
        {
            internal string AppId { get; private set; }

            internal string InterstitialAdUnit { get; private set; }
            internal string RewardedAdUnit { get; private set; }
            internal string BannerAdUnit { get; private set; }

            internal bool IsInitialized { get; private set; }
            internal bool IsTestMode { get; private set; }

            internal AdTypes DebugAdType { get; private set; }

            public Builder(string appId)
            {
                this.AppId = appId;
            }

            public Builder(string interAdUnit,
                string rewardAdUnit, string bannerAdUnit)
            {
                this.InterstitialAdUnit = interAdUnit;
                this.RewardedAdUnit = rewardAdUnit;
                this.BannerAdUnit = bannerAdUnit;
            }

            public Builder WithTestMode(AdTypes adType)
            {
                if (adType == AdTypes.None)
                {
                    UnityEngine.Debug.Log($"{UnigramAdsLogger.PREFIX} " +
                        $"Unsuppored ad type, test mode disabled");

                    return this;
                }

                this.IsTestMode = true;
                this.DebugAdType = adType;

                UnigramAdsLogger.Enabled();

                UnigramAdsLogger.Log($"Enabled test mode for ad type: {adType}");

                return this;
            }

            public Builder WithAdNetwork()
            {
                return this;
            }

            public UnigramAdsSDK Build(Action<bool> initializationFinished)
            {
                if (!UnigramUtils.IsSupporedPlatform())
                {
                    return null;
                }

                /*
                AdsGramBridge.Init(AppId, IsTestMode,
                    DebugAdType, (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        UnigramAdsLogger.Log("Ads Gram is initialized");
                    }
                });
                */

                AdSonarBridge.Init((isSuccess) =>
                {
                    initializationFinished?.Invoke(isSuccess);

                    IsInitialized = isSuccess;

                    if (IsInitialized)
                    {
                        UnigramAdsLogger.Log("Ad Sonar is initialized");
                    }
                });

                _instance = new UnigramAdsSDK(this);

                return _instance;
            }
        }
    }
}