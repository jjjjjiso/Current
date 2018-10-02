using UnityEngine;

namespace WaterBlast.Game.Common
{
    public class BlockArea : MonoSingleton<BlockArea>
    {
        public void Attach(Transform trans)
        {
            trans.parent = transform;
            trans.Reset();
        }

        public void ChildDestory()
        {
            while(transform.childCount > 0)
            {
                Transform temp = transform.GetChild(0);
                Destroy(temp.gameObject);
            }
        }
    }
}