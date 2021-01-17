using System;
using UnityEngine;
using UnityEngine.Advertisements;

using WaterBlast.Game.Popup;

namespace WaterBlast.Game.Manager
{
    public class AdsMgr : MonoDontDestroySingleton<AdsMgr>
    {
        private readonly string android_game_id = "2855937";
        private readonly string ios_game_id = "2855936";

        private readonly string video = "video";
        private readonly string rewardedVideo = "rewardedVideo";

        public Action fncSuccess;

        protected override void OnAwake()
        {
            base.OnAwake();
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_ANDROID
            Advertisement.Initialize(android_game_id);
#elif UNITY_IOS
            Advertisement.Initialize(ios_game_id);
#endif
        }

        /// <summary> 몇초(보통 5초) 후에 스킵이 가능한 광고 </summary>
        public void ShowAd()
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show(video);
            }
        }

        /// <summary> 스킵이 불가능한 Rewarded 광고 </summary>
        public void ShowRewardedAd()
        {
            if (Advertisement.IsReady(rewardedVideo))
            {
                // 광고가 끝난 뒤 콜백함수 "HandleShowResult" 호출 
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show(rewardedVideo, options);
            }
        }

        /// <summary> 광고가 종료된 후 자동으로 호출되는 콜백 함수 </summary>
        private void HandleShowResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished: // 광고 시청이 완료되었을 때 처리
                    Debug.Log("The ad was successfully shown.");
                    DoSomeRewardAction();
                    break;
                case ShowResult.Skipped: // 광고가 스킵되었을 때 처리
                    Debug.Log("The ad was skipped before reaching the end.");
                    DoSomeSkippedAction();
                    break;
                case ShowResult.Failed: // 광고 시청에 실패했을 때 처리
                    Debug.LogError("The ad failed to be shown.");
                    DoSomeSkippedFailed();
                    break;
            }
        }

        private void DoSomeRewardAction()
        {
            UserDataMgr.G.AddCoins(UserDataMgr.G.coinRewardedCount < UserDataMgr.G.coinRewardedMaxCount ? GameDataMgr.G.adsRewardCost : GameDataMgr.G.itemCost);

            ++UserDataMgr.G.coinRewardedCount;
            if (UserDataMgr.G.coinRewardedCount > UserDataMgr.G.coinRewardedMaxCount)
                UserDataMgr.G.coinRewardedCount = UserDataMgr.G.coinRewardedMaxCount;

            if (LobbyMgr.G != null && LobbyMgr.G.levelNumber != null) LobbyMgr.G.levelNumber.UpdateCoin();

            if (fncSuccess != null)
            {
                fncSuccess();
                fncSuccess = null;
            }
        }

        private void DoSomeSkippedAction()
        {
            PopupConfirm.Open("Prefabs/Popup/QuitPopup", "QuitPopup", "Ad skip!", "It is skipped and cannot be rewarded.", "OK");
        }

        private void DoSomeSkippedFailed()
        {
            PopupConfirm.Open("Prefabs/Popup/QuitPopup", "QuitPopup", "Sorry", "The ad failed to be shown. \n Please try again.", "OK");
        }
    }
}