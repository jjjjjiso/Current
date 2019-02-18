using System;
using System.Collections.Generic;

using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class Popup : MonoBehaviour
    {
        public delegate void OnClose();
        public OnClose onClose = null;

        static protected Dictionary<string, Popup> popup = new Dictionary<string, Popup>();

        protected string ID = null;
        public string GetID() { return ID; }

        static public T Create<T>(string path, string ID) where T : Popup
        {
            T temp = Resources.Load<T>(path);

            popup[ID] = Instantiate(temp);
            popup[ID].ID = ID;

            PopupMgr.G.Add(popup[ID]);

            return popup[ID] as T;
        }

        static public Popup Find(string ID)
        {
            return (popup.ContainsKey(ID)) ? popup[ID] : null;
        }

        static public void Close(string ID)
        {
            if(popup.ContainsKey(ID))
            {
                popup[ID].Close();
            }
        }

        static public void CloseAll()
        {
            List<Popup> tempPopup = new List<Popup>(popup.Values);

            for(int i = 0; i < tempPopup.Count; ++i)
            {
                tempPopup[i].Close();
            }

            popup.Clear();
        }

        public void Close()
        {
            if(onClose != null)
            {
                onClose();
            }

            PopupMgr.G.Pop(ID);
            Popup temp = popup[ID];
            popup.Remove(ID);
            Destroy(temp.gameObject);
        }

        protected virtual void Delay(Action method, float time)
        {
            MonoExtension.Invoke(this, method, time);
        }
    }
}