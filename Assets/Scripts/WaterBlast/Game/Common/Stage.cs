using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    //블럭 생성 & 배치
    public partial class Stage : MonoBehaviour
    {
        static public Stage Create(Transform parent, Sprite texture, int width, int height)
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

            temp.SetUp(parent, texture, width, height);

            return temp;
        }

        public BlockEntity[,] blockEntities = null;

        public int width  = 9;
        public int height = 9;

        private int wSize = 0;
        private int hSize = 0;

        public void Reset()
        {
            foreach(BlockEntity block in blockEntities)
            {
                ReturnObject(block.gameObject);
            }
        }

        private void SetUp(Transform parent, Sprite texture, int width, int height)
        {
            this.width = width;
            this.height = height;

            BlockSetting();
            CreateBackground(parent, texture);
            BlockIconSetting();

            StartCoroutine(Co_StartItem());
        }

        private void BlockSetting()
        {
            //Block Object Create.
            GameMgr gameMgr = GameMgr.G;
            blockEntities = new BlockEntity[width, height];
            
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    var index = x * width + y;
                    var temp = gameMgr.gamePools.GetBlockEntity(gameMgr.level.blocks[index]);
                    Assert.IsNotNull(temp);
                    temp.SetDepth(y + 11);
                    temp.Show();
                    temp.SetData(x, y);
                    blockEntities[x, y] = temp;
                }
            }

            //Position Setting.
            wSize = blockEntities[0,0]._BlockWidthSize;
            hSize = blockEntities[0, 0]._BlockHeightSize - 12; //-12
            Vector2 pos = Vector2.zero;
            for (int x = 0; x < width; ++x)
            {
                pos.x = Point(x, width, wSize);

                for (int y = 0; y < height; ++y)
                {
                    pos.y = Point(y, height, hSize);

                    blockEntities[x, y]._LocalPosition = pos;
                }
            }
        }

        private float Point(int index, int count, int blockSize)
        {
            float point = -1;
            float half = 0.5f;
            int center = (int)(count * half);
            bool isCenter = (index == center) ? true : false;

            if (isCenter)
                point = 0;
            else
                point = (index < center) ? -((center - index) * blockSize) : ((index - center) * blockSize);

            point = (count % 2 == 0) ? point + (blockSize * half) : point;

            return point;
        }

        private void CreateBackground(Transform parent, Sprite texture)
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Block block = blockEntities[x, y] as Block;
                    if (block._BlockType == BlockType.empty) continue;

                    GameObject background = new GameObject("Background");
                    background.layer = parent.gameObject.layer;
                    UITexture tempTexture = background.AddComponent<UITexture>();
                    tempTexture.mainTexture = texture.texture;
                    tempTexture.color = new Color32(10, 10, 10, 255);//Color.black;
                    tempTexture.width = 100;
                    tempTexture.height = 110;
                    background.transform.parent = parent;
                    background.transform.Reset();
                    background.transform.localPosition = block._LocalPosition;
                }
            }
        }

        IEnumerator Co_StartItem()
        {
            isWait = true;
            yield return new WaitForSecondsRealtime(1.8f);

            List<BlockDef> oldDef = new List<BlockDef>();
            for (int i = 0; i < GameDataMgr.G.isUseStartItem.Length; ++i)
            {
                if (GameDataMgr.G.isUseStartItem[i])
                {
                    BlockDef newDef = new BlockDef(Random.Range(0, width), Random.Range(0, height));
                    if(oldDef.Contains(newDef))
                    {
                        if(i > 0) --i;
                        continue;
                    }
                    else
                    {
                        oldDef.Add(newDef);
                    }
                    
                    Block block = blockEntities[newDef.x, newDef.y] as Block;
                    if (block._BlockType == BlockType.empty)
                    {
                        if (i > 0) --i;
                        continue;
                    }

                    StartCoroutine(Co_BoosterChange(newDef.x, newDef.y, ((i * 2) + 5), true));
                    yield return new WaitForSecondsRealtime(0.5f);
                }
            }

            isWait = false;
            GameDataMgr.G.Reset();
        }
    }
}