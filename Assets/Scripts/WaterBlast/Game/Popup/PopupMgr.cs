using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                //bool isPopup = (stackPopup.Count == 0) ? false : true;
                //if (isPopup) // 열린 팝업들이 있다.
                //{
                //    Popup popup = stackPopup.Pop();
                //    if (popup.GetID() == QUIT)
                //    {
                //        //원상 복구.
                //        Time.timeScale = 1;
                //    }

                //    popup.Close();
                //}
                //else
                {
                    //시간 멈춤.
                    //Time.timeScale = 0;

//                    PopupYesNo temp = PopupYesNo.Open(QUIT, DataMgr.G.GetIndexUIText(1000));
//                    if (temp != null)
//                    {
//                        temp.onYes += () =>
//                        {
#if (UNITY_ANDROID && UNITY_EDITOR)
                            UnityEditor.EditorApplication.isPlaying = false;

#elif (UNITY_ANDROID && !UNITY_EDITOR)
                            Application.Quit();
#endif
//                        };

//                        temp.onNo += () =>
//                        {
//                            //원상 복구.
//                            Time.timeScale = 1;
//                        };
//                    }
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
    }
}