using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    //블럭 매치
    public partial class Stage : MonoBehaviour
    {
        //private field
        private float delayTime = 0.3f;

        //default Method

        //public Method
        public void NormMatch(int x, int y, BlockType blockType)
        {
            Queue<BlockDef> qTemp = new Queue<BlockDef>();
            List<Vector2> matchList = new List<Vector2>();

            qTemp.Enqueue(new BlockDef(x, y));
            while (qTemp.Count > 0)
            {
                BlockDef n = qTemp.Dequeue();
                BlockDef w = n, e = new BlockDef(n.x + 1, n.y);
                while ((w.x >= 0) && ColorMatch(w.x, w.y, blockType))//left
                {
                    if (!matchList.Contains(new Vector2(w.x, w.y)))
                    {
                        matchList.Add(new Vector2(w.x, w.y));
                    }
                    if (!matchList.Contains(new Vector2(w.x, w.y - 1)))
                    {
                        if ((w.y > 0) && ColorMatch(w.x, w.y - 1, blockType))//up
                        {
                            qTemp.Enqueue(new BlockDef(w.x, w.y - 1));
                        }
                    }
                    if (!matchList.Contains(new Vector2(w.x, w.y + 1)))
                    {
                        if ((w.y < height - 1) && ColorMatch(w.x, w.y + 1, blockType))//down
                        {
                            qTemp.Enqueue(new BlockDef(w.x, w.y + 1));
                        }
                    }
                    w.x--;
                }
                while ((e.x <= width - 1) && ColorMatch(e.x, e.y, blockType))//right
                {
                    if (!matchList.Contains(new Vector2(e.x, e.y)))
                    {
                        matchList.Add(new Vector2(e.x, e.y));
                    }
                    if (!matchList.Contains(new Vector2(e.x, e.y - 1)))
                    {
                        if ((e.y > 0) && ColorMatch(e.x, e.y - 1, blockType))//up
                        {
                            qTemp.Enqueue(new BlockDef(e.x, e.y - 1));
                        }
                    }
                    if (!matchList.Contains(new Vector2(e.x, e.y + 1)))
                    {
                        if ((e.y < height - 1) && ColorMatch(e.x, e.y + 1, blockType))//down
                        {
                            qTemp.Enqueue(new BlockDef(e.x, e.y + 1));
                        }
                    }
                    e.x++;
                }
            }

            //2개 이상만 터트리기.
            List<int> xList = new List<int>();
            if (matchList.Count >= 2)
            {
                if(matchList.Count >= 5)
                {
                    for(int i = 0; i < matchList.Count; ++i)
                    {
                        int matchX = (int)matchList[i].x;
                        int matchY = (int)matchList[i].y;

                        if (!xList.Contains(matchX)) xList.Add(matchX);

                        if (i == 0) continue;
                        
                        Vector2 v = blockEntities[(int)matchList[0].x, (int)matchList[0].y]._LocalPosition;
                        blockEntities[matchX, matchY].TargetMove(v);
                    }

                    xList.Sort((int a, int b) => a.CompareTo(b));

                    int pickX = (int)matchList[0].x;
                    int pickY = (int)matchList[0].y;
                    int loopStartX = xList[0];
                    int loopEndX = xList[xList.Count - 1];
                    SpecialItem(pickX, pickY, matchList.Count, loopStartX, loopEndX);
                }
                else
                {
                    foreach (Vector2 temp in matchList)
                    {
                        int matchX = (int)temp.x;
                        int matchY = (int)temp.y;

                        blockEntities[matchX, matchY].Hide();
                        if (!xList.Contains(matchX)) xList.Add(matchX);
                    }

                    xList.Sort((int a, int b) => a.CompareTo(b));

                    int loopStartX = xList[0];
                    int pickY = xList[xList.Count - 1];
                    MatchDown(loopStartX, pickY);
                }
            }
            else
            {
                foreach (Vector2 temp in matchList)
                {
                    int matchX = (int)temp.x;
                    int matchY = (int)temp.y;
                    blockEntities[matchX, matchY].Show();
                }
            }
        }
        
        //public void WidthMatch(int y)
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(Co_Match(y));
        //}

        //public void HeightMatch(int x)
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(Co_Match(x));
        //}

        public void SpecialMatch(int x, int y)
        {
            List<BlockDef> blockDefList = blockEntities[x, y].GetComponent<Special>().Match(x, y);
            
            StartCoroutine(Co_Match(blockDefList));
        }

        public void SameColorMatch(int x, int y)
        {
            //List<Block> tempBlocks = new List<Block>();

            //BlockType blockType = blockEntities[x, y]._BlockType;
            //BlockType freColor = blockEntities[x, y]._FreColor;
            //tempBlocks.Add(blockEntities[x, y]);

            //for (int ix = 0; ix < width; ++ix)
            //{
            //    for (int iy = 0; iy < height; ++iy)
            //    {
            //        Block block = blockEntities[ix, iy];
            //        if (freColor == BlockType.none || freColor != block._BlockType) continue;
            //        string strTemp = string.Format("{0}_{1}", blockType, freColor);
            //        block.SetBlockType(blockType, strTemp);
            //        tempBlocks.Add(block);
            //    }
            //}

            //if(tempBlocks.Count != 0) StartCoroutine(Co_SameColorMatch(tempBlocks));
        }
        
        //private Method
        private bool CheckActive(int x, int y)
        {
            if (blockEntities[x, y].isActiveAndEnabled) return true;
            return false;
        }

        private bool CheckActive(Block block)
        {
            if (block.isActiveAndEnabled) return true;
            return false;
        }

        private void SpecialItem(int x, int y, int count, int startX, int endX)
        {
            StopCoroutine(Co_SpecialItem(x, y, count, startX, endX));
            StartCoroutine(Co_SpecialItem(x, y, count, startX, endX));
        }

        private void MatchDown(int startX, int endX)
        {
            for (int x = startX; x <= endX; ++x)
            {
                var stack = new List<BlockEntity>();
                for (int y = height - 1; y >= 0; --y)
                {
                    var block = blockEntities[x, y];
                    if (block._State == State.wait)
                    {
                        if (!stack.Contains(block))
                        {
                            stack.Add(block);
                            block._LocalPosition = new Vector2(x * block._SpriteWidthSize, height * block._SpriteHeightSize);
                            //block.GetComponent<Block>().SetRandomColor(GameMgr.Get().Min, GameMgr.Get().Max);
                        }
                    }

                    else if (stack.Count != 0)
                    {
                        Swap(x, y, x, y + stack.Count);
                        Vector2 vEndPos = new Vector2(x * block._SpriteWidthSize, -(y + stack.Count) * block._SpriteHeightSize);
                        block.DownMove(vEndPos, x, y + stack.Count);
                    }
                }

                for (int i = stack.Count - 1; i >= 0; --i)
                {
                    var block = blockEntities[x, i];

                    block.Show();
                    Vector2 vEndPos = new Vector2(x * block._SpriteWidthSize, -(i * block._SpriteHeightSize));
                    block.DownMove(vEndPos, x, i);
                }
            }

            CheckCanColorMatch();
        }

        private void MatchDown()
        {
            for(int x = 0; x < width; ++x)
            {
                var stack = new List<BlockEntity>();
                for(int y = height - 1; y >= 0; --y)
                {
                    var block = blockEntities[x, y];
                    if (block._State == State.wait)
                    {
                        if(!stack.Contains(block))
                        {
                            stack.Add(block);
                            block._LocalPosition = new Vector2(x * block._SpriteWidthSize, height * block._SpriteHeightSize);
                            //block.GetComponent<Block>().SetRandomColor(GameMgr.Get().Min, GameMgr.Get().Max);
                        }
                    }

                    else if (stack.Count != 0)
                    {
                        Swap(x, y, x, y + stack.Count);
                        Vector2 vEndPos = new Vector2(x * block._SpriteWidthSize, -(y + stack.Count) * block._SpriteHeightSize);
                        block.DownMove(vEndPos, x, y + stack.Count);
                    }
                }

                for (int i = stack.Count - 1; i >= 0; --i)
                {
                    var block = blockEntities[x, i];

                    block.Show();
                    Vector2 vEndPos = new Vector2(x * block._SpriteWidthSize, -(i * block._SpriteHeightSize));
                    block.DownMove(vEndPos, x, i);
                }
            }

            CheckCanColorMatch();
        }

        private void CheckCanColorMatch()
        {
            StopCoroutine("Co_CheckCanColorMatch");
            StartCoroutine("Co_CheckCanColorMatch");
        }

        private void MatchProcess()
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    BlockType blockType = blockEntities[x, y].GetComponent<Block>()._BlockType;
                    if((x - 1) >= 0)
                        if (ColorComparison(x - 1, y, blockType)) return;//left
                    if ((x + 1) < width)
                        if (ColorComparison(x + 1, y, blockType)) return;//right
                    if ((y - 1) >= 0)
                        if (ColorComparison(x, y - 1, blockType)) return;//up
                    if ((y + 1) < height)
                        if (ColorComparison(x, y + 1, blockType)) return;//down
                }
            }

            AllBlockColorSet();
        }

        private void AllBlockColorSet()
        {
            foreach (var block in blockEntities)
            {
                if (block == null) continue;
                block.GetComponent<Block>().SetRandomColor(GameMgr.Get().Min, GameMgr.Get().Max);
            }
        }

        private bool MovingChecked(State state)
        {
            foreach (var block in blockEntities)
            {
                if (block == null) continue;
                if (block._State == state) return true;
            }

            return false;
        }

        private bool ColorMatch(int x, int y, BlockType color)
        {
            if (!blockEntities[x, y].GetComponent<Block>().ColorMatch(color)) return false;
            return true;
        }

        private bool ColorComparison(int x, int y, BlockType color)
        {
            if (!blockEntities[x, y].GetComponent<Block>().ColorComparison(color)) return false;
            return true;
        }

        private void Swap(BlockEntity a, BlockEntity b)
        {
            blockEntities[a._X, a._Y] = b;
            blockEntities[b._X, b._Y] = a;

            a.SetData();
            b.SetData();
        }

        private void Swap(int ax, int ay, int bx, int by)
        {
            var tmp = blockEntities[ax, ay];
            blockEntities[ax, ay] = blockEntities[bx, by];
            blockEntities[bx, by] = tmp;
        }

        private bool SpecialBlock(ref SpecialType boosterType, ref int count,
                                  int x, int y, bool isLeftRight, bool isUpDown)
        {
            bool isTemp = false;
            //switch (boosterType)
            //{
            //    case SpecialType.left_right_arrow:
            //        if (isLeftRight)
            //        {
            //            isTemp = true;
            //            StartCoroutine(Co_WidthMatch(y));
            //            boosterType = SpecialType.none;
            //            ++count;
            //        }
            //        break;
            //    case SpecialType.up_down_arrow:
            //        if (isUpDown)
            //        {
            //            isTemp = true;
            //            StartCoroutine(Co_HeightMatch(x));
            //            boosterType = SpecialType.none;
            //            ++count;
            //        }
            //        break;
            //    case SpecialType.bomb:
            //        isTemp = true;
            //        NineMatch(x, y);
            //        boosterType = SpecialType.none;
            //        ++count;
            //        break;
            //    case SpecialType.ranbow:
            //        isTemp = true;
            //        SameColorMatch(x, y);
            //        boosterType = SpecialType.none;
            //        ++count;
            //        break;
            //}

            return isTemp;
        }

        private bool IsValidBlockEntity(BlockDef blockEntity)
        {
            return blockEntity.x >= 0 && blockEntity.x < width &&
                   blockEntity.y >= 0 && blockEntity.y < height;
        }

        //coroutine Method
        IEnumerator Co_CheckCanColorMatch()
        {
            while (MovingChecked(State.move)) yield return null;
            yield return new WaitForSeconds(2f);//이부분은 나중에 수정
            MatchProcess();
            CheckCanColorMatch();
        }

        //여기 수정해야함. 이제 특수퍼즐은 이미지 체인지 하지않음.
        IEnumerator Co_SpecialItem(int x, int y, int count, int StartX, int endX)
        {
            while (MovingChecked(State.special_move)) yield return null;
            var hitBlock = blockEntities[x, y].GetComponent<Block>();
            BlockType colorType = (hitBlock != null) ? hitBlock._BlockType : BlockType.none;
            blockEntities[x, y].Hide();
            //hitBlock.GetComponent<PooledObject>().pool.ReturnObject(hitBlock.gameObject);

            if (count == 5 || count == 6)
            {
                int iRandom = Random.Range((int)ArrowType.horizontal, (int)ArrowType.vertical + 1);
                CreateSpecial(SpecialType.arrow, x, y);
                blockEntities[x, y].gameObject.AddComponent<ArrowBomb>().UpdateSprite((ArrowType)iRandom);
            }
            else if (count == 7 || count == 8)
            {
                CreateSpecial(SpecialType.bomb, x, y);
            }
            else
            {
                CreateSpecial(SpecialType.ranbow, x, y);
                Ranbow ranbow = blockEntities[x, y].gameObject.AddComponent<Ranbow>();
                Assert.IsNotNull(ranbow);
                ranbow._PreType = colorType;
                string strTemp = string.Format("{0}_{1}", SpecialType.ranbow, colorType);
                ranbow.UpdateSprite(strTemp);
            }
            
            MatchDown(StartX, endX);
        }

        private ObjectPool GetSpecialPool(SpecialType type)
        {
            GamePool gamePools = GameMgr.Get().gamePools;
            switch (type)
            {
                case SpecialType.arrow:
                    return gamePools.arrowBombPool;

                case SpecialType.bomb:
                    return gamePools.bombPool;

                case SpecialType.ranbow:
                    return gamePools.ranbowPool;
            }
            return null;
        }

        private void CreateSpecial(SpecialType type, int x, int y)
        {
            GamePool gamePools = GameMgr.Get().gamePools;
            ObjectPool specialPool = null;
            switch (type)
            {
                case SpecialType.arrow:
                    specialPool = gamePools.arrowBombPool;
                    break;

                case SpecialType.bomb:
                    specialPool = gamePools.bombPool;
                    break;

                case SpecialType.ranbow:
                    specialPool = gamePools.ranbowPool;
                    break;
            }

            Assert.IsNotNull(specialPool);
            var special = CreateBlock(specialPool.GetObject());
            CreateSpecial(special, x, y);
        }

        private void CreateSpecial(GameObject special, int x, int y)
        {
            special.transform.localPosition = blockEntities[x, y]._LocalPosition;
            BlockEntity entity = special.GetComponent<BlockEntity>();
            Assert.IsNotNull(entity);
            blockEntities[x, y] = entity;
            blockEntities[x, y]._State = State.idle;
        }

        private GameObject CreateBlock(GameObject go)
        {
            go.GetComponent<BlockEntity>().Show();
            return go;
        }

        //private GameObject CreateNewBlock()
        //{
        //    var percent = UnityEngine.Random.Range(0, 100);
        //    if (generatedCollectables < neededCollectables &&
        //        percent < level.collectableChance)
        //    {
        //        generatedCollectables += 1;
        //        return CreateBlock(gamePools.GetTileEntity(level, new BlockTile { type = BlockType.Collectable }).gameObject);
        //    }
        //    else
        //    {
        //        return CreateBlock(gamePools.GetTileEntity(level, new BlockTile { type = BlockType.RandomBlock }).gameObject);
        //    }
        //}

        //IEnumerator Co_WidthMatch(int y)
        //{
        //    int x = 0;
        //    while (x < width)
        //    {
        //        var block = blockEntities[x, y].GetComponent<Special>();
        //        if (block != null)
        //        {
        //            block._State = State.wait;
        //            block.Hide();
        //            //test
        //            //block.isTest = false;
        //            //

        //            SpecialType type = block._SpecialType;
        //            if (SpecialBlock(ref type, ref x, x, y, false, true))
        //            {
        //                block._SpecialType = type;
        //                continue;
        //            }

        //            block._SpecialType = SpecialType.none;
        //            ++x;
        //            yield return null;
        //        }
        //    }

        //    yield return new WaitForSeconds(delayTime);

        //    MatchDown();
        //}

        //IEnumerator Co_HeightMatch(int x)
        //{
        //    int y = 0;
        //    while (y < height)
        //    {
        //        var block = blockEntities[x, y].GetComponent<Special>();
        //        if (block != null)
        //        {
        //            block._State = State.wait;
        //            block.Hide();
        //            //test
        //            //block.isTest = false;
        //            //

        //            SpecialType type = block._SpecialType;
        //            if (SpecialBlock(ref type, ref x, x, y, false, true))
        //            {
        //                block._SpecialType = type;
        //                continue;
        //            }

        //            block._SpecialType = SpecialType.none;
        //            ++y;
        //            yield return null;
        //        }
        //    }

        //    yield return new WaitForSeconds(delayTime);

        //    MatchDown();
        //}

        protected IEnumerator Co_Match(List<BlockDef> blockDefList)
        {
            int defCount = blockDefList.Count;
            int count = 0;
            if (defCount != 0)
            {
                while (count < defCount)
                {
                    int x = blockDefList[count].x;
                    int y = blockDefList[count].y;
                    var block = blockEntities[x, y].GetComponent<Special>();
                    if (block != null)
                    {
                        block._State = State.wait;
                        block.Hide();
                        //test
                        //block.isTest = false;
                        //

                        SpecialType type = block._SpecialType;
                        if (SpecialBlock(ref type, ref count, x, y, true, true))
                        {
                            block._SpecialType = type;
                            continue;
                        }

                        block._SpecialType = SpecialType.none;
                        ++count;
                        //yield return null;
                    }
                }

                yield return new WaitForSeconds(delayTime);

                int pickX = blockDefList[0].x;
                int startX = ((pickX - 1) >= 0) ? (pickX - 1) : pickX;
                int endX = ((pickX + 1) < width) ? (pickX + 1) : pickX;
                MatchDown(startX, endX);

                //MatchDown();
            }
        }

        IEnumerator Co_SameColorMatch(List<Block> blockDefList)
        {
            yield return new WaitForSeconds(delayTime);

            int count = 0;
            while (count < blockDefList.Count)
            {
                Block block = blockDefList[count];
                block._State = State.wait;
                block.Hide();
                ++count;
                //test
                //block.isTest = false;
                //
            }

            yield return new WaitForSeconds(delayTime);

            MatchDown();
        }
    }
}