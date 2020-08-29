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
        public BlockEntity[,] blockerEntites = null;
        private List<BlockDef> blockDef = new List<BlockDef>();

        public int width  = 9;
        public int height = 9;

        private int wSize = 0;
        private int hSize = 0;

        private int oldStickyBlockCount = 0;
        public int currStickyBlockCount = 0;

        private List<GameObject> backgrounds = new List<GameObject>();

        public void Reset()
        {
            foreach(BlockEntity block in blockEntities)
            {
                ReturnObject(block.gameObject);
            }
            
            foreach (BlockEntity blocker in blockerEntites)
            {
                if (blocker == null) continue;
                ReturnObject(blocker.gameObject);
            }

            foreach (GameObject obj in backgrounds)
            {
                Destroy(obj);
            }
            backgrounds.Clear();
        }

        private void SetUp(Transform parent, Sprite texture, int width, int height)
        {
            this.width = width;
            this.height = height;

            BlockSetting();
            CreateBackground(parent, texture);
            BlockIconSetting(true);
            BlockerSetting();

            StartCoroutine(Co_StartItem());
        }

        private void BlockSetting()
        {
            //Block Object Create.
            GameMgr gameMgr = GameMgr.G;
            blockEntities = new BlockEntity[width, height];
            blockerEntites = new BlockEntity[width, height];

            blockDef.Clear();

            BlockType blockType;
            Vector2 pos = Vector2.zero;

            wSize = 72;
            hSize = 73; //원래 h 사이즈 - 85

            for (int x = 0; x < width; ++x)
            {
                pos.x = Point(x, width, wSize);
                for (int y = 0; y < height; ++y)
                {
                    var index = x * width + y;
                    var temp = gameMgr.gamePools.GetBlockEntity(gameMgr.level.blocks[index]);
                    Assert.IsNotNull(temp);
                    if (temp is Booster)
                    {
                        switch((temp as Booster)._BoosterType)
                        {
                            case BoosterType.arrow:
                                int iRandom = Random.Range((int)ArrowType.horizon, (int)ArrowType.vertical + 1);
                                ArrowBomb arrow = temp as ArrowBomb;
                                arrow.UpdateSprite(iRandom);
                                break;
                            case BoosterType.rainbow:
                                Rainbow rainbow = temp as Rainbow;
                                BlockType type = (BlockType)Random.Range((int)BlockType.red, (int)BlockType.purple + 1);
                                rainbow._PreType = type;
                                string strTemp = string.Format("{0}_{1}", BoosterType.rainbow, type);
                                rainbow.UpdateSprite(strTemp);
                                break;
                        }
                    }
                    
                    if (temp is Block && (temp as Block)._BlockType == BlockType.sticky) ++oldStickyBlockCount;

                    temp.SetDepth(y + 11);
                    temp.Show();
                    temp.SetData(x, y);
                    blockEntities[x, y] = temp;
                    
                    pos.y = Point(y, height, hSize);
                    blockEntities[x, y]._LocalPosition = pos;

                    if (IsCheckBlockerType(gameMgr.level.blocks[index].blockerType))
                    {   
                        blockEntities[x, y].SetIconUIColor(0.3f);
                        
                        //Blocker Object Create
                        var cover = gameMgr.gamePools.GetBlockerEntity(gameMgr.level.blocks[index]);
                        Assert.IsNotNull(cover);
                        if (cover is Blocker) (cover as Blocker).sprite.alpha = 1;
                        cover.SetDepth(y + 21);
                        cover.Show();
                        cover.SetData(x, y);
                        cover._LocalPosition = blockEntities[x, y]._LocalPosition;
                        blockerEntites[x, y] = cover;
                    }

                    if (blockerEntites[x, y] == null && temp is Block)
                    {   // Start Item Random Pos List
                        blockType = (temp as Block)._BlockType;
                        if (!IsCheckNotBlockType(blockType))
                        {
                            blockDef.Add(new BlockDef(x, y));
                        }
                    }
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
                    if (blockEntities[x, y] is Block && (blockEntities[x, y] as Block)._BlockType == BlockType.empty) continue;
                    GameObject background = new GameObject("Background");
                    background.layer = parent.gameObject.layer;
                    UITexture tempTexture = background.AddComponent<UITexture>();
                    tempTexture.mainTexture = texture.texture;
                    tempTexture.color = new Color32(10, 10, 10, 255);//Color.black;
                    tempTexture.width = 100;
                    tempTexture.height = 110;
                    background.transform.parent = parent;
                    background.transform.Reset();
                    background.transform.localPosition = blockEntities[x, y]._LocalPosition;

                    backgrounds.Add(background);
                }
            }
        }

        IEnumerator Co_StartItem()
        {
            isWait = true;
            yield return new WaitForSeconds(1.8f);

            List<BlockDef> oldDef = new List<BlockDef>();
            BlockDef newDef;
            int defIdx = 0;
            for (int i = 0; i < GameDataMgr.G.isUseStartItem.Length; ++i)
            {
                if (GameDataMgr.G.isUseStartItem[i])
                {
                    defIdx = Random.Range(0, blockDef.Count);
                    newDef = new BlockDef(blockDef[defIdx].x, blockDef[defIdx].y); 
                    if(oldDef.Contains(newDef))
                    {
                        if(i > 0) --i;
                        blockDef.RemoveAt(defIdx);
                        continue;
                    }
                    else
                    {
                        oldDef.Add(newDef);
                    }

                    StartCoroutine(Co_BoosterChange(newDef.x, newDef.y, ((i * 2) + 5), true));
                    yield return new WaitForSeconds(0.5f);
                }
            }

            isWait = false;
            GameDataMgr.G.Reset();
        }
    }
}