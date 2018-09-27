using UnityEngine;
using UnityEngine.Assertions;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    //블럭 생성 & 배치
    public partial class Stage : MonoBehaviour
    {
        static private readonly string BLOCK_PATH = "Prefabs/Block/Block";

        static public Stage Create()
        {
            GameObject obj = new GameObject();
            if (obj == null) return null;
            obj.name = "Stage";
            obj.transform.SetParent(null);
            obj.transform.Reset();

            Stage temp = obj.AddComponent<Stage>();
            if (temp == null)
            {
                Destroy(obj);
                obj = null;
            }

            temp.SetUp();

            return temp;
        }

        public BlockEntity[,] blockEntities = null;

        public int width  = 9;
        public int height = 9;

        private BlockEntity blockPrefab = null;

        private void SetUp()
        {
            blockPrefab = Resources.Load<BlockEntity>(BLOCK_PATH);
            BlockSetting();
            CheckCanColorMatch();
        }

        private BlockEntity CreateBlock()
        {
            GamePool gamePools = GameMgr.Get().gamePools;
            BlockEntity entity = gamePools.blockPool.GetObject().GetComponent<BlockEntity>();
            Assert.IsNotNull(entity);
            entity.Show();
            return entity;
        }

        private void BlockSetting()
        {
            if (blockPrefab == null) return;
            //Block Object Create.
            blockEntities = new BlockEntity[width, height];

            for (int iX = 0; iX < width; ++iX)
            {
                for (int iY = 0; iY < height; ++iY)
                {
                    var temp = CreateBlock();
                    Assert.IsNotNull(temp);
                    temp.GetComponent<Block>().SetRandomColor(GameMgr.Get().Min, GameMgr.Get().Max);
                    temp.SetData(iX, iY);
                    BlockArea.Get().Attach(temp.transform);
                    blockEntities[iX, iY] = temp;
                }
            }

           // Position Setting.
            int sizeX = blockEntities[0,0]._SpriteWidthSize;
            int sizeY = blockEntities[0,0]._SpriteHeightSize;
            Vector3 pos = Vector3.zero;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int iIndex = y * width + x;
                    if (iIndex == 0) continue;

                    if ((x % width) != 0)
                    {
                        pos = blockEntities[x - 1, y]._LocalPosition;
                        pos.x += sizeX;
                    }
                    else//(iX % m_iWidth) == 0
                    {
                        pos.x = 0;
                        pos.y -= sizeY;
                    }

                    blockEntities[x, y]._LocalPosition = pos;
                }
            }
        }
    }
}