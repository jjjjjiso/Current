using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class Popup : MonoBehaviour
    {
        public delegate void OnClose();
        public OnClose onClose = null;

        static protected Dictionary<string, Popup> popup = new Dictionary<string, Popup>();

        protected string id = null;
        
        static public T Create<T>(string path, string id) where T : Popup
        {
            T temp = Resources.Load<T>(path);

            popup[id] = Instantiate(temp);
            popup[id].id = id;

            PopupRoot.Add(popup[id]);

            return popup[id] as T;
        }

        static public Popup Find(string id)
        {
            if(popup.ContainsKey(id))
            {
                return popup[id];
            }

            return null;
        }

        static public void Close(string id)
        {
            if(popup.ContainsKey(id))
            {
                popup[id].Close();
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

            Popup temp = popup[id];
            popup.Remove(id);
            Destroy(temp.gameObject);
        }
    }
}