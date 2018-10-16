using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using WaterBlast.Game.Manager;
using WaterBlast.Game.UI;

namespace WaterBlast.Game.Common
{
    //블럭 매치
    public partial class Stage : MonoBehaviour
    {
        private List<Vector2> comboBoosterIndex = new List<Vector2>();
        private List<Booster> comboBoosters = new List<Booster>();
        private BlockType rainbowColor = BlockType.empty;

        private float delayTime = 0.35f;

        public bool isBoosterWait = false;
        
        public void NormMatch(int x, int y, LevelBlock levelBlock)
        {
            List<Vector2> matchList = GetMatches(x, y, levelBlock);

            //2개 이상 터트리기.
            if (matchList.Count >= 2)
                ExplodeBlockEntities(matchList);
            else
                blockEntities[x, y]._State = State.idle;
        }

        private void ExplodeBlockEntities(List<Vector2> matchList)
        {
            if(GameMgr.Get().level.limit > 0)
                --GameMgr.Get().level.limit;

            bool isBooster = (matchList.Count >= 5) ? true : false;

            for (int i = 0; i < matchList.Count; ++i)
            {
                int x = (int)matchList[i].x;
                int y = (int)matchList[i].y;

                Block block = blockEntities[x, y] as Block;

                if (!isBooster)
                {
                    var particles = GameMgr.Get().gamePools.GetParticles(block);
                    if (particles != null) CreateParticle(particles, block._LocalPosition);

                    block.Hide();
                }
                else
                {
                    if(i == 0) continue;
                    Vector2 v = blockEntities[(int)matchList[0].x, (int)matchList[0].y]._LocalPosition;
                    block.TargetMove(v);
                }
                 
                if(GameMgr.Get()._GameStage.collectedBlocks.ContainsKey(block._BlockType))
                    GameMgr.Get()._GameStage.collectedBlocks[block._BlockType] += 1;
            }

            if(!isBooster)
                MatchDown();
            else
            {
                int pickX = (int)matchList[0].x;
                int pickY = (int)matchList[0].y;
                StartCoroutine(Co_BoosterChange(pickX, pickY, matchList.Count));
            }
        }

        public void BoosterMatches(int x, int y, bool isComboCheck = true)
        {
            List<BlockDef> blockDefList = null;
            Booster booster = blockEntities[x, y] as Booster;
            bool isCombo = false;
            
            comboBoosters.Add(booster);

            blockDefList = booster.Match(x, y);

            if (isComboCheck)
            {
                if (GameMgr.Get().level.limit > 0)
                    --GameMgr.Get().level.limit;
                
                List <Vector2> comboBoosters = GetMatches(x, y);

                if (comboBoosters.Count > 1)
                {
                    foreach (Vector2 combo in comboBoosters)
                    {
                        int xx = (int)combo.x;
                        int yy = (int)combo.y;
                        Booster comboBooster = blockEntities[xx, yy] as Booster;
                        comboBooster.TargetMove(blockEntities[x, y]._LocalPosition);
                    }

                    blockDefList.Clear();
                    
                    if (booster._BoosterType == BoosterType.rainbow) rainbowColor = booster.GetComponent<Rainbow>()._PreType;
                    blockDefList = BoosterCombo(x, y, ref isCombo);
                }
            }

            booster._IsCombo = false;
            ReturnObject(booster.gameObject);
            CreateNewBlock(x, y);

            comboBoosters.Clear();

            if (blockDefList != null)
            {
                if(!isCombo) StartCoroutine(Co_CreateBoosterParticles(booster, false));
                StartCoroutine(Co_BoosterMatch(blockDefList));
            }
        }

        private List<BlockDef> BoosterCombo(int x, int y, ref bool isCombo)
        {
            List<BlockDef> blockList = null;

            comboBoosters.Sort(delegate (Booster a, Booster b) { return (b._BoosterType.CompareTo(a._BoosterType)); });

            if (comboBoosters[0]._BoosterType == comboBoosters[1]._BoosterType)
            {
                isCombo = true;
                blockList = comboBoosters[0].ComboMatch(x, y);
                StartCoroutine(Co_CreateBoosterParticles(comboBoosters[0], isCombo));
            }
            else
            {
                switch (BoosterComboType(comboBoosters[0]._BoosterType, comboBoosters[1]._BoosterType))
                {
                    case BoosterSynthesis.arrowBombAndBomb:
                        ArrowBombAndBomb(x, y);
                        break;
                    case BoosterSynthesis.arrowBombAndRainbow:
                        RainbowAndAnotherBomb(BoosterType.arrow);
                        break;
                    case BoosterSynthesis.BombAndRainbow:
                        RainbowAndAnotherBomb(BoosterType.bomb);
                        break;
                }
            }

            return blockList;
        }

        private BoosterSynthesis BoosterComboType(BoosterType a, BoosterType b)
        {
            BoosterSynthesis type = BoosterSynthesis.none;
            switch (a)
            {
                case BoosterType.arrow:
                    if (b == BoosterType.bomb) type = BoosterSynthesis.arrowBombAndBomb;
                    else if (b == BoosterType.rainbow) type = BoosterSynthesis.arrowBombAndRainbow;
                    break;
                case BoosterType.bomb:
                    if (b == BoosterType.arrow) type = BoosterSynthesis.arrowBombAndBomb;
                    else if (b == BoosterType.rainbow) type = BoosterSynthesis.BombAndRainbow;
                    break;
                case BoosterType.rainbow:
                    if (b == BoosterType.arrow) type = BoosterSynthesis.arrowBombAndRainbow;
                    else if (b == BoosterType.bomb) type = BoosterSynthesis.BombAndRainbow;
                    break;
            }

            return type;
        }

        private void ArrowBombAndBomb(int x, int y)
        {
            List<BlockDef> blocks = new List<BlockDef>();
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                for (int iy = 0; iy < height; ++iy)
                {
                    AddBlockEntity(blocks, ix, iy);
                }
            }

            for (int iy = y - 1; iy <= y + 1; ++iy)
            {
                for (int ix = 0; ix < width; ++ix)
                {
                    AddBlockEntity(blocks, ix, iy);
                }
            }

            StartCoroutine(Co_ArrowBombAndBombParticles(x, y));
            StartCoroutine(Co_BoosterMatch(blocks));
        }

        private void RainbowAndAnotherBomb(BoosterType type)
        {
            List<BlockDef> blocks = new List<BlockDef>();
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Block block = blockEntities[x, y] as Block;
                    if (block == null) continue;
                    if (rainbowColor != block._BlockType) continue;
                    blocks.Add(new BlockDef(x, y));
                }
            }

            StartCoroutine(Co_RainbowAndAnotherBomb(blocks, type));
        }

        private List<Vector2> GetMatches(int x, int y, LevelBlock levelBlock = null)
        {
            Queue<BlockDef> qTemp = new Queue<BlockDef>();
            List<Vector2> matchList = new List<Vector2>();

            qTemp.Enqueue(new BlockDef(x, y));
            while (qTemp.Count > 0)
            {
                BlockDef l = qTemp.Dequeue();
                BlockDef r = new BlockDef(l.x + 1, l.y);

                while (IsValidIndex(l.x, width) && IsMatches(l, levelBlock))
                {
                    if (!matchList.Contains(new Vector2(l.x, l.y)))
                        matchList.Add(new Vector2(l.x, l.y));

                    BlockDef up = new BlockDef(l.x, l.y - 1);
                    BlockDef down = new BlockDef(l.x, l.y + 1);

                    if (!matchList.Contains(new Vector2(up.x, up.y)))
                    {
                        if ((l.y > 0) && IsMatches(up, levelBlock))
                            qTemp.Enqueue(up);
                    }

                    if (!matchList.Contains(new Vector2(down.x, down.y)))
                    {
                        if ((l.y < height - 1) && IsMatches(down, levelBlock))
                            qTemp.Enqueue(down);
                    }
                    l.x--;
                }
                
                while (IsValidIndex(r.x, width) && IsMatches(r, levelBlock))
                {
                    if (!matchList.Contains(new Vector2(r.x, r.y)))
                        matchList.Add(new Vector2(r.x, r.y));

                    BlockDef up = new BlockDef(r.x, r.y - 1);
                    BlockDef down = new BlockDef(r.x, r.y + 1);

                    if (!matchList.Contains(new Vector2(up.x, up.y)))
                    {
                        if ((r.y > 0) && IsMatches(up, levelBlock))
                            qTemp.Enqueue(up);
                    }

                    if (!matchList.Contains(new Vector2(down.x, down.y)))
                    {
                        if ((r.y < height - 1) && IsMatches(down, levelBlock))
                            qTemp.Enqueue(down);
                    }
                    r.x++;
                }
            }

            return matchList;
        }

        private void MatchDown()
        {
            for (int x = 0; x < width; ++x)
            {
                var stack = new List<BlockEntity>();
                int emptyIdx = -1;
                int emptyCnt = 0;

                for (int y = 0; y < height; ++y)
                {
                    var blockEntity = blockEntities[x, y];
                    if (IsEmptyBlock(blockEntity))
                    {
                        if (stack.Count != 0) ++emptyCnt;
                        emptyIdx = y;
                        continue;
                    }

                    if (blockEntity._State == State.wait)
                    {
                        if (!stack.Contains(blockEntity))
                        {
                            stack.Add(blockEntity);
                            blockEntity._LocalPosition = new Vector2(Point(x, width), (15 * blockSize));
                        }
                    }

                    else if (stack.Count != 0)
                    {
                        int yy = (y - stack.Count);
                        if (emptyIdx != -1 && emptyIdx < yy) emptyCnt = 0;
                        yy -= emptyCnt;
                        Swap(x, y, x, yy);
                        Vector2 vEndPos = new Vector2(Point(x, width), Point(yy, height));
                        blockEntity.DownMove(vEndPos, x, yy);
                    }
                }

                for (int i = stack.Count - 1; i >= 0; --i)
                {
                    int y = ((height - 1) - i);
                    if (emptyIdx != -1 && emptyIdx < y) emptyCnt = 0;
                    y -= emptyCnt;
                    var blockEntity = blockEntities[x, y];
                    blockEntity.Show();
                    Vector2 vEndPos = new Vector2(Point(x, width), Point(y, height));
                    blockEntity.DownMove(vEndPos, x, y);
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
                    Block block = blockEntities[x, y] as Block;
                    if (block == null) continue;

                    var left  = new BlockDef(x - 1, y);
                    var right = new BlockDef(x + 1, y);
                    var up    = new BlockDef(x, y - 1);
                    var down  = new BlockDef(x, y + 1);

                    if (IsValidIndex(left.x, width) ) if (IsColorComparison(left, block._BlockType) ) return;
                    if (IsValidIndex(right.x, width)) if (IsColorComparison(right, block._BlockType)) return;
                    if (IsValidIndex(up.y, height)  ) if (IsColorComparison(up, block._BlockType)   ) return;
                    if (IsValidIndex(down.y, height)) if (IsColorComparison(down, block._BlockType) ) return;
                }
            }

            AllBlockColorSet();
        }

        private void AllBlockColorSet()
        {
            foreach (var block in blockEntities)
            {
                if (block == null) continue;
                block.Show();
            }
        }

        private ObjectPool GetBoosterPool(BoosterType type)
        {
            GamePool gamePools = GameMgr.Get().gamePools;
            switch (type)
            {
                case BoosterType.arrow:
                    return gamePools.arrowBombPool;

                case BoosterType.bomb:
                    return gamePools.bombPool;

                case BoosterType.rainbow:
                    return gamePools.rainbowPool;
            }
            return null;
        }

        private void CreateBooster(BoosterType type, int x, int y)
        {
            GamePool gamePools = GameMgr.Get().gamePools;
            ObjectPool boosterPool = null;
            switch (type)
            {
                case BoosterType.arrow:
                    boosterPool = gamePools.arrowBombPool;
                    break;

                case BoosterType.bomb:
                    boosterPool = gamePools.bombPool;
                    break;

                case BoosterType.rainbow:
                    boosterPool = gamePools.rainbowPool;
                    break;
            }

            Assert.IsNotNull(boosterPool);
            var booster = CreateBlock(boosterPool.GetObject());
            CreateBooster(booster, x, y);
        }

        private void CreateBooster(GameObject booster, int x, int y)
        {
            booster.transform.localPosition = blockEntities[x, y]._LocalPosition;
            BlockEntity entity = booster.GetComponent<BlockEntity>();
            Assert.IsNotNull(entity);
            blockEntities[x, y] = entity;
            blockEntities[x, y]._State = State.idle;
            blockEntities[x, y].SetData(x, y);
        }

        private void CreateNewBlock(int x, int y)
        {
            BlockEntity entity = GameMgr.Get().gamePools.blockPool.GetObject().GetComponent<BlockEntity>();
            Assert.IsNotNull(entity);
            entity.transform.localPosition = blockEntities[x, y]._LocalPosition;
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

        private void CreateParticle(GameObject particles, Vector2 localPosition)
        {
            particles.transform.localPosition = localPosition;
            particles.GetComponent<BlockParticles>().Playing();
        }

        private void ReturnObject(GameObject obj)
        {
            obj.GetComponent<PooledObject>().pool.ReturnObject(obj);
        }

        private void AddBlockEntity(List<BlockDef> blocks, int x, int y)
        {
            if (x < 0 || x >= width ||
                y < 0 || y >= height) return;

            BlockDef def = new BlockDef(x, y);
            if (!blocks.Contains(def)) blocks.Add(def);
        }

        private void ComboBoostersClear()
        {
            for (int i = 0; i < comboBoosterIndex.Count; ++i)
            {
                int x = (int)comboBoosterIndex[i].x;
                int y = (int)comboBoosterIndex[i].y;

                Booster booster = blockEntities[x, y] as Booster;

                if (booster == null) continue;

                booster._IsCombo = false;
                ReturnObject(booster.gameObject);
                CreateNewBlock(booster._X, booster._Y);
            }

            comboBoosterIndex.Clear();
        }

        private bool GetCombo(int x, int y)
        {
            var up = new BlockDef(x, y - 1);
            var down = new BlockDef(x, y + 1);
            var left = new BlockDef(x - 1, y);
            var right = new BlockDef(x + 1, y);

            bool isCombo = false;

            if (IsCombo(up.x, up.y)) isCombo = true;
            if (IsCombo(down.x, down.y)) isCombo = true;
            if (IsCombo(left.x, left.y)) isCombo = true;
            if (IsCombo(right.x, right.y)) isCombo = true;

            return isCombo;
        }

        private bool IsCombo(int x, int y)
        {
            if (!IsValidBlockEntity(x, y)) return false;

            Booster booster = blockEntities[x, y] as Booster;
            if (booster == null) return false;
            if (!booster.gameObject.activeSelf) return false;

            return true;
        }

        private bool IsMatches(BlockDef blockDef, LevelBlock levelBlock)
        {
            bool isMatch = false;
            if (levelBlock != null)
            {
                if (levelBlock is LevelBlockType)
                {
                    isMatch = IsColorMatch(blockDef, (levelBlock as LevelBlockType).type);
                }
            }
            else
            {
                isMatch = IsCombo(blockDef);
            }

            return isMatch;
        }

        private bool IsCombo(BlockDef blockDef)
        {
            Booster booster = blockEntities[blockDef.x, blockDef.y] as Booster;
            if (booster == null) return false;
            if (!booster.gameObject.activeSelf) return false;

            if (((comboBoosters[0]._X != booster._X) || (comboBoosters[0]._Y != booster._Y))
                && (!comboBoosterIndex.Contains(new Vector2(booster._X, booster._Y))))
            {
                comboBoosters.Add(booster);
            }


            Rainbow rainbow = booster as Rainbow;
            if (rainbow != null) rainbowColor = rainbow._PreType;

            comboBoosterIndex.Add(new Vector2(blockDef.x, blockDef.y));
            booster._IsCombo = true;

            return true;
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
            ColorBlock block = blockEntities[x, y] as ColorBlock;
            if (block == null) return false;
            if (!block.ColorMatch(color)) return false;
            return true;
        }

        private bool IsColorMatch(BlockDef blockDef, BlockType color)
        {
            if (!IsColorMatch(blockDef.x, blockDef.y, color)) return false;
            return true;
        }

        private bool IsColorComparison(BlockDef blockDef, BlockType color)
        {
            ColorBlock block = blockEntities[blockDef.x, blockDef.y] as ColorBlock;
            if (block == null) return false;
            if (!block.ColorComparison(color)) return false;
            return true;
        }

        private bool IsValidBlockEntity(BlockDef blockEntity)
        {
            return IsValidBlockEntity(blockEntity.x, blockEntity.y);
        }

        private bool IsValidBlockEntity(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private bool IsValidIndex(int index, int line)
        {
            return index >= 0 && index < line;
        }

        private bool IsEmptyBlock(BlockEntity blockEntity)
        {
            Block block = blockEntity as Block;
            if (block != null && block._BlockType == BlockType.empty) return true;
            return false;
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
        
        IEnumerator Co_CheckCanColorMatch()
        {
            while (IsMoving(State.move)) yield return null;
            //이부분은 나중에 수정..작아졌다 커지면서 바뀌기.
            yield return new WaitForSeconds(2f);
            MatchProcess();
            CheckCanColorMatch();
        }
        
        IEnumerator Co_BoosterChange(int x, int y, int count)
        {
            while (IsMoving(State.booster_move)) yield return null;
            var hitBlock = blockEntities[x, y] as Block;
            BlockType colorType = (hitBlock != null) ? hitBlock._BlockType : BlockType.empty;
            ReturnObject(hitBlock.gameObject);

            if (count == 5 || count == 6)
            {
                int iRandom = Random.Range((int)ArrowType.horizon, (int)ArrowType.vertical + 1);
                CreateBooster(BoosterType.arrow, x, y);
                blockEntities[x, y].gameObject.GetComponent<ArrowBomb>().UpdateSprite(iRandom);
            }
            else if (count == 7 || count == 8)
            {
                CreateBooster(BoosterType.bomb, x, y);
            }
            else
            {
                CreateBooster(BoosterType.rainbow, x, y);
                Rainbow rainbow = blockEntities[x, y] as Rainbow;
                Assert.IsNotNull(rainbow);
                rainbow._PreType = colorType;
                string strTemp = string.Format("{0}_{1}", BoosterType.rainbow, colorType);
                rainbow.UpdateSprite(strTemp);
            }

            MatchDown();
        }

        IEnumerator Co_BoosterMatch(List<BlockDef> blockDefList)
        {
            while (IsMoving(State.booster_move)) yield return null;
            ComboBoostersClear();
            int defCount = blockDefList.Count;
            int count = 0;
            if (defCount != 0)
            {
                while (count < defCount)
                {
                    int x = blockDefList[count].x;
                    int y = blockDefList[count].y;
                    var hitBlock = blockEntities[x, y];
                    hitBlock._State = State.wait;
                    hitBlock.Hide();

                    Booster booster = hitBlock as Booster;
                    if (booster != null)
                    {
                        if (!booster._IsCombo)
                        {
                            BoosterMatches(x, y, false);
                            continue;
                        }
                        else
                        {
                            booster._IsCombo = false;
                            ReturnObject(booster.gameObject);
                            CreateNewBlock(x, y);
                        }
                    }
                    else
                    {
                        var particles = GameMgr.Get().gamePools.GetParticles(hitBlock);
                        if (particles != null) CreateParticle(particles, hitBlock._LocalPosition);
                    }

                    ++count;
                }

                yield return new WaitForSeconds(delayTime);

                
                MatchDown();
            }
        }

        IEnumerator Co_RainbowAndAnotherBomb(List<BlockDef> blocks, BoosterType type)
        {
            while (IsMoving(State.booster_move)) yield return null;
            isBoosterWait = true;

            List<Booster> boosters = new List<Booster>();
            int count = 0;
            while (count < blocks.Count)
            {
                int x = blocks[count].x;
                int y = blocks[count].y;

                var hitBlock = blockEntities[x, y] as Block;
                ReturnObject(hitBlock.gameObject);
                CreateBooster(type, x, y);

                var block = blockEntities[x, y];

                if (type == BoosterType.arrow)
                {
                    int iRandom = Random.Range((int)ArrowType.horizon, (int)ArrowType.vertical + 1);
                    block.GetComponent<ArrowBomb>().UpdateSprite(iRandom);
                }
                boosters.Add(block as Booster);
                ++count;
                yield return new WaitForSeconds(0.2f);
            }

            StartCoroutine(Co_RainbowAndAnotherBomb(boosters));
        }

        IEnumerator Co_RainbowAndAnotherBomb(List<Booster> boosters)
        {
            List<BlockDef> blocks = new List<BlockDef>();
            int count = 0;
            while(count < boosters.Count)
            {
                while (IsMoving(State.move)) yield return null;
                Booster booster = boosters[count];
                booster = blockEntities[booster._X, booster._Y] as Booster;
                if (booster == null)
                {
                    ++count;
                    continue;
                }
                blocks = booster.Match(booster._X, booster._Y);
                
                yield return StartCoroutine(Co_BoosterMatch(blocks));
                ++count;
            }

            isBoosterWait = false;
        }

        IEnumerator Co_CreateBoosterParticles(Booster booster, bool isCombo)
        {
            while (IsMoving(State.booster_move)) yield return null;

            booster.CreateParticle(isCombo);
        }

        IEnumerator Co_ArrowBombAndBombParticles(int x, int y)
        {
            while (IsMoving(State.booster_move)) yield return null;

            GameObject particles = null;
            Vector2 localPosition = Vector2.zero;

            for (int iy = y - 1; iy <= y + 1; ++iy)
            {
                if (!IsValidIndex(iy, height)) continue;
                particles = GameMgr.Get().gamePools.lineHorizontalParticlesPool.GetObject();
                localPosition = particles.transform.localPosition;
                localPosition.y = Point(iy, height);
                CreateParticle(particles, localPosition);
            }

            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                if (!IsValidIndex(ix, width)) continue;
                particles = GameMgr.Get().gamePools.lineVerticalParticlesPool.GetObject();
                localPosition = particles.transform.localPosition;
                localPosition.x = Point(ix, width);
                CreateParticle(particles, localPosition);
            }
        }
    }
}