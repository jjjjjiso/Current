using System;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class PopupMgr : MonoSingleton<PopupMgr>
    {
        //static private readonly string QUIT = "Quit";

        public GameObject black;
        public UITexture blackTexture;
        public bool isFadeCheck = false;

        private Stack<Popup> stackPopup = new Stack<Popup>();

        private void Update()
        {
            // 임시 
            if(stackPopup.Count > 0)
            {
                Popup temp = stackPopup.Peek();
                if (temp != null && temp.GetID() != "In Game Setting")
                {
                    if (!black.activeSelf) black.SetActive(true);
                    if (temp.GetID() != "StartPopup")
                        blackTexture.alpha = 1;
                    else
                        blackTexture.alpha = 0.8f;
                }
            }
            else
            {
                if (black.activeSelf) black.SetActive(false);
            } 

            if (isFadeCheck && Input.GetKeyDown(KeyCode.Escape))
            {
                bool isPopup = (stackPopup.Count == 0) ? false : true;
                if (isPopup) // 열린 팝업들이 있다.
                {
                    Popup popup = stackPopup.Pop();
                    popup.Close();
                    if (popup.GetID() == "In Game Setting") GameMgr.G.uiSettingBtn.OnPressed();
                    else if (popup.GetID() == "SuccessPopup" || popup.GetID() == "FailedPopup") GameMgr.G.GoLooby();
                    else if (popup.GetID() == "Failed Moves Popup") GameMgr.G.Failed();
                }
                else
                {
                    for (int i = 0; i < GameDataMgr.G.isUseInGameItem.Length; ++i)
                    {
                        if (GameDataMgr.G.isUseInGameItem[i])
                        {
                            GameDataMgr.G.isUseInGameItem[i] = false;
                            GameMgr.G.itemUIElements.items[i].ResetInfo();
                            return;
                        }
                    }

                    PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/QuitPopup", "Quit", "Are You Sure?", "Do you want to quit?", "QUIT", false);
                    if (temp != null)
                    {
                        temp.onConfirm += () =>
                        {
#if (UNITY_ANDROID && UNITY_EDITOR)
                            UnityEditor.EditorApplication.isPlaying = false;

#elif (UNITY_ANDROID && !UNITY_EDITOR)
                            Application.Quit();
#endif
                        };
                    }
                }
            }
        }

        public void Add(Popup popup)
        {
            Vector3 originalPos = popup.transform.localPosition;
            popup.transform.parent = transform;
            popup.transform.ResetExceptPosition(originalPos);

            //if (popup.GetID() != LOADING && popup.GetID() != UPDATE)
                stackPopup.Push(popup);
        }

        public void Pop(string ID)
        {
            if (stackPopup.Count == 0) return;

            Stack<Popup> stack = new Stack<Popup>();
            for (int i = 0; i < stackPopup.Count; ++i)
            {
                Popup temp = stackPopup.Pop();
                if (temp.GetID() != ID)
                {
                    stack.Push(temp);
                    --i;
                    continue;
                }
                break;
            }

            for (int i = 0; i < stack.Count; ++i)
            {
                stackPopup.Push(stack.Pop());
            }
        }
        
        public void ShowAdsPopup(string title, string message, string btn, Action actConfirm = null, Action actEixt = null)
        {
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ADPopup", "AD", title, message + "\n" + "(After 10 ads, 200 coins will be paid.)", btn);
            temp.GetComponent<PopupAD>().SetInfo();

            temp.onConfirm += () =>
            {
                if (actConfirm != null)
                {
                    actConfirm();
                    actConfirm = null;
                }
                temp.Close();
                AdsMgr.G.ShowRewardedAd();
            };

            temp.onExit += () =>
            {
                if (actEixt != null)
                {
                    actEixt();
                    actEixt = null;
                }
                temp.Close();
            };
        }

        public void ShowItemPopup(string id, string title, string message, string btn, string itemName, int itemCount, int cost, Action actConfirm = null, bool isBlack = false, bool isTemp = true)
        {
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ItemPopup", id, title, message, btn, false);
            PopupItem popupItem = temp.GetComponent<PopupItem>();
            if (popupItem != null) popupItem.ItemSetting(itemName, itemCount, cost, isBlack, isTemp);

            temp.onConfirm += () =>
            {
                if (actConfirm != null)
                {
                    actConfirm();
                    actConfirm = null;
                }

                temp.SetMessage("Purchase completed!");
                if (popupItem != null) popupItem.UpdateData(false);

                temp.onConfirm += () =>
                {
                    temp.Close();
                };
            };
        }

        public void ShowItemPopup(string id, string title, string message, string btn, BoosterType type, int itemCount, int cost, Action actConfirm = null)
        {
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ItemPopup", id, title, message, btn, false);
            PopupItem popupItem = temp.GetComponent<PopupItem>();
            if (popupItem != null) popupItem.ItemSetting(type, itemCount, cost);

            temp.onConfirm += () =>
            {
                if (actConfirm != null)
                {
                    actConfirm();
                    actConfirm = null;
                }

                temp.SetMessage("Purchase completed!");
                if (popupItem != null) popupItem.UpdateData(false);

                temp.onConfirm += () =>
                {
                    temp.Close();
                };
            };
        }
    }
}