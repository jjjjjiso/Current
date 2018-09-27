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
        private float delayTime = 0.35f;

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
                while ((w.x >= 0) && IsColorMatch(w.x, w.y, blockType))//left
                {
                    if (!matchList.Contains(new Vector2(w.x, w.y)))
                    {
                        matchList.Add(new Vector2(w.x, w.y));
                    }
                    if (!matchList.Contains(new Vector2(w.x, w.y - 1)))
                    {
                        if ((w.y > 0) && IsColorMatch(w.x, w.y - 1, blockType))//up
                        {
                            qTemp.Enqueue(new BlockDef(w.x, w.y - 1));
                        }
                    }
                    if (!matchList.Contains(new Vector2(w.x, w.y + 1)))
                    {
                        if ((w.y < height - 1) && IsColorMatch(w.x, w.y + 1, blockType))//down
                        {
                            qTemp.Enqueue(new BlockDef(w.x, w.y + 1));
                        }
                    }
                    w.x--;
                }
                while ((e.x <= width - 1) && IsColorMatch(e.x, e.y, blockType))//right
                {
                    if (!matchList.Contains(new Vector2(e.x, e.y)))
                    {
                        matchList.Add(new Vector2(e.x, e.y));
                    }
                    if (!matchList.Contains(new Vector2(e.x, e.y - 1)))
                    {
                        if ((e.y > 0) && IsColorMatch(e.x, e.y - 1, blockType))//up
                        {
                            qTemp.Enqueue(new BlockDef(e.x, e.y - 1));
                        }
                    }
                    if (!matchList.Contains(new Vector2(e.x, e.y + 1)))
                    {
                        if ((e.y < height - 1) && IsColorMatch(e.x, e.y + 1, blockType))//down
                        {
                            qTemp.Enqueue(new BlockDef(e.x, e.y + 1));
                        }
                    }
                    e.x++;
                }
            }

            //2개 이상만 터트리기.
            if (matchList.Count >= 2)
            {
                if(matchList.Count >= 5)
                {
                    for(int i = 0; i < matchList.Count; ++i)
                    {
                        int matchX = (int)matchList[i].x;
                        int matchY = (int)matchList[i].y;

                        if (i == 0) continue;
                        
                        Vector2 v = blockEntities[(int)matchList[0].x, (int)matchList[0].y]._LocalPosition;
                        blockEntities[matchX, matchY].TargetMove(v);
                    }
                    
                    int pickX = (int)matchList[0].x;
                    int pickY = (int)matchList[0].y;
                    StartCoroutine(Co_SpecialBlockChange(pickX, pickY, matchList.Count));
                }
                else
                {
                    foreach (Vector2 temp in matchList)
                    {
                        int matchX = (int)temp.x;
                        int matchY = (int)temp.y;

                        blockEntities[matchX, matchY].Hide();
                    }

                    MatchDown();
                }
            }
            else
            {
                foreach (Vector2 temp in matchList)
                {
                    int matchX = (int)temp.x;
                    int matchY = (int)temp.y;
                    blockEntities[matchX, matchY]._State = State.idle;
                }
            }
        }

        public void SpecialMatch(int x, int y)
        {
            List<BlockDef> blockDefList = blockEntities[x, y].GetComponent<Special>().Match(x, y);
            ReturnObject(blockEntities[x, y].gameObject);
            CreateNewBlock(x, y);
            StartCoroutine(Co_SpecialMatch(blockDefList));
        }

        //protected void SpecialCombo(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    switch (skillType)
        //    {
        //        case SpecialType.arrow:
        //            ArrowBombAndArrowBomb(blocks, w, h, x, y);
        //            break;
        //        case SpecialType.bomb:
        //            ArrowBombAndBomb(blocks, w, h, x, y);
        //            break;
        //        case SpecialType.ranbow:
        //            break;
        //    }
        //}

        //protected void ArrowBombAndArrowBomb(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    //left
        //    for (int ix = x; ix >= 0; --ix)
        //    {
        //        AddBlock(blocks, ix, y);
        //    }
        //    //up
        //    for (int iy = y; iy >= 0; --iy)
        //    {
        //        AddBlock(blocks, x, iy);
        //    }
        //    //right
        //    for (int ix = x + 1; ix < w; ++ix)
        //    {
        //        AddBlock(blocks, ix, y);
        //    }
        //    //down
        //    for (int iy = y + 1; iy < h; ++iy)
        //    {
        //        AddBlock(blocks, x, iy);
        //    }
        //}

        //protected void ArrowBombAndBomb(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    for (int ix = x - 1; ix <= x + 1; ++ix)
        //    {
        //        //up
        //        for (int iy = y; iy >= 0; --iy)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //        //down
        //        for (int iy = y + 1; iy < h; ++iy)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }

        //    for (int iy = y - 1; iy <= y + 1; ++iy)
        //    {
        //        //left
        //        for (int ix = x; ix >= 0; --ix)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //        //right
        //        for (int ix = x + 1; ix < w; ++ix)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }
        //}

        //protected virtual void ArrowBombAndRanbow(List<BlockDef> blocks, int x, int y)
        //{
        //    Stage s = GameMgr.Get()._Stage;
        //    for (int ix = 0; ix < s.width; ++ix)
        //    {
        //        for (int iy = 0; iy < s.height; ++iy)
        //        {
        //            BlockEntity entity = s.blockEntities[ix, iy];
        //            Block block = entity.GetComponent<Block>();
        //            if (block == null) continue;
        //            if (preType == BlockType.none || preType != block._BlockType) continue;
        //            block.SetSprite(type.ToString());
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }
        //}

        //protected void AddBlock(List<BlockDef> blocks, int x, int y)
        //{
        //    if (x < 0 || x >= width ||
        //        y < 0 || y >= height) return;

        //    BlockDef def = new BlockDef(x, y);
        //    if (!blocks.Contains(def)) blocks.Add(def);
        //}

        //protected bool GetCombo(int x, int y, SpecialType type)
        //{
        //    var up = new BlockDef(x, y - 1);
        //    var down = new BlockDef(x, y + 1);
        //    var left = new BlockDef(x - 1, y);
        //    var right = new BlockDef(x + 1, y);

        //    bool isCombo = false;

        //    if (IsCombo(x, y, up.x, up.y, type)) isCombo = true;
        //    if (IsCombo(x, y, down.x, down.y, type)) isCombo = true;
        //    if (IsCombo(x, y, left.x, left.y, type)) isCombo = true;
        //    if (IsCombo(x, y, right.x, right.y, type)) isCombo = true;

        //    return isCombo;
        //}

        //protected bool IsCombo(int pickX, int pickY, int x, int y, SpecialType type)
        //{
        //    if (!IsValidBlockEntity(x, y)) return false;
        //    Stage s = GameMgr.Get()._Stage;
        //    Special special = s.blockEntities[x, y].GetComponent<Special>();
        //    if (special == null) return false;
        //    //if (special._SpecialType != type) return false;
        //    if (skillType < special._SpecialType)
        //        skillType = special._SpecialType;
        //    special.TargetMove(s.blockEntities[pickX, pickY]._LocalPosition);
        //    special._isCombo = true;
        //    return true;
        //}

        //private Method

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
            entity.transform.localScale = Vector3.one;
            blockEntities[x, y] = entity;
            blockEntities[x, y]._State = State.idle;
            blockEntities[x, y].SetData(x, y);
        }

        private void CreateNewBlock(int x, int y)
        {
            BlockEntity entity = GameMgr.Get().gamePools.blockPool.GetObject().GetComponent<BlockEntity>();
            Assert.IsNotNull(entity);
            entity.transform.localPosition = blockEntities[x, y]._LocalPosition;
            entity.transform.localScale = Vector3.one;
            blockEntities[x, y] = entity;
            blockEntities[x, y]._State = State.wait;
            blockEntities[x, y].SetData(x, y);
            blockEntities[x, y].Hide();
        }

        private GameObject CreateBlock(GameObject go)
        {
            go.GetComponent<BlockEntity>().Show();
            return go;
        }

        private void ReturnObject(GameObject obj)
        {
            obj.GetComponent<PooledObject>().pool.ReturnObject(obj);
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
                    Block block = blockEntities[x, y].GetComponent<Block>();
                    if (block == null) continue;

                    var left  = new BlockDef(x - 1, y);
                    var right = new BlockDef(x + 1, y);
                    var up    = new BlockDef(x, y - 1);
                    var down  = new BlockDef(x, y + 1);

                    if (IsValidX(left.x) ) if (IsColorComparison(left, block._BlockType) ) return;
                    if (IsValidX(right.x)) if (IsColorComparison(right, block._BlockType)) return;
                    if (IsValidY(up.y)   ) if (IsColorComparison(up, block._BlockType)   ) return;
                    if (IsValidY(down.y) ) if (IsColorComparison(down, block._BlockType) ) return;
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

        private bool IsMoving(State state)
        {
            foreach (var block in blockEntities)
            {
                if (block == null) continue;
                if (block._State == state) return true;
            }

            return false;
        }

        private bool IsColorMatch(int x, int y, BlockType color)
        {
            Block block = blockEntities[x, y].GetComponent<Block>();
            if (block == null) return false;
            if (!block.ColorMatch(color)) return false;
            return true;
        }

        private bool IsColorComparison(BlockDef blockDef, BlockType color)
        {
            Block block = blockEntities[blockDef.x, blockDef.y].GetComponent<Block>();
            if (block == null) return false;
            if (!block.ColorComparison(color)) return false;
            return true;
        }

        private bool IsValidBlockEntity(BlockDef blockEntity)
        {
            return blockEntity.x >= 0 && blockEntity.x < width &&
                   blockEntity.y >= 0 && blockEntity.y < height;
        }

        private bool IsValidBlockEntity(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private bool IsValidX(int x)
        {
            return x >= 0 && x < width;
        }

        private bool IsValidY(int y)
        {
            return y >= 0 && y < height;
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

       

        //coroutine Method
        IEnumerator Co_CheckCanColorMatch()
        {
            while (IsMoving(State.move)) yield return null;
            yield return new WaitForSeconds(2f);//이부분은 나중에 수정
            MatchProcess();
            CheckCanColorMatch();
        }
        
        IEnumerator Co_SpecialBlockChange(int x, int y, int count)
        {
            while (IsMoving(State.special_move)) yield return null;
            var hitBlock = blockEntities[x, y].GetComponent<Block>();
            BlockType colorType = (hitBlock != null) ? hitBlock._BlockType : BlockType.none;
            ReturnObject(hitBlock.gameObject);

            if (count == 5 || count == 6)
            {
                int iRandom = Random.Range((int)ArrowType.horizontal, (int)ArrowType.vertical + 1);
                CreateSpecial(SpecialType.arrow, x, y);
                blockEntities[x, y].gameObject.GetComponent<ArrowBomb>().UpdateSprite(iRandom);
            }
            else if (count == 7 || count == 8)
            {
                CreateSpecial(SpecialType.bomb, x, y);
            }
            else
            {
                CreateSpecial(SpecialType.ranbow, x, y);
                Ranbow ranbow = blockEntities[x, y].gameObject.GetComponent<Ranbow>();
                Assert.IsNotNull(ranbow);
                ranbow._PreType = colorType;
                string strTemp = string.Format("{0}_{1}", SpecialType.ranbow, colorType);
                ranbow.UpdateSprite(strTemp);
            }
            
            MatchDown();
        }

        protected IEnumerator Co_SpecialMatch(List<BlockDef> blockDefList)
        {
            while (IsMoving(State.special_move)) yield return null;
            int defCount = blockDefList.Count;
            int count = 0;
            if (defCount != 0)
            {
                while (count < defCount)
                {
                    int x = blockDefList[count].x;
                    int y = blockDefList[count].y;
                    var hitBlock = blockEntities[x, y];
                    Assert.IsNotNull(hitBlock);
                    hitBlock._State = State.wait;
                    hitBlock.Hide();

                    Special special = hitBlock.GetComponent<Special>();
                    if(special != null)
                    {
                        if (!special._isCombo)
                        {
                            SpecialMatch(x, y);
                            continue;
                        }
                        else
                        {
                            ReturnObject(blockEntities[x, y].gameObject);
                            CreateNewBlock(x, y);
                            special._isCombo = false;
                        }
                    }

                    ++count;
                    //yield return null;
                }

                yield return new WaitForSeconds(delayTime);

                MatchDown();
            }
        }
    }
}