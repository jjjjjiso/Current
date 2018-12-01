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
        private readonly string ARROW_ICON_FORMAT   = "arrow_icon_{0}";
        private readonly string TNT_ICON_FORMAT     = "tnt_icon_{0}";
        private readonly string RAINBOW_ICON_FORMAT = "rainbow_icon_{0}";
        private readonly string FACE_ICON_FORMAT    = "face_icon_{0}";

        private List<Vector2> comboBoosterIndex = new List<Vector2>();
        private List<Booster> comboBoosters = new List<Booster>();
        private BlockType rainbowColor = BlockType.empty;

        private float delayTime = 0.35f;

        public bool isWait = false;

        public void NormMatch(int x, int y, LevelBlock levelBlock)
        {
            List<Vector2> matchList = GetMatches(x, y, levelBlock);

            //2개 이상 터트리기.
            if (matchList.Count >= 2)
            {
                //미션 체크.
                AddCollectedBlock(matchList);
                ExplodeBlockEntities(matchList);
            } 
            else
            {
                blockEntities[x, y].PlayAnim("NoMatches");
                blockEntities[x, y]._State = State.idle;
            }
        }

        private void ExplodeBlockEntities(List<Vector2> matchList)
        {
            //제한 횟수 감소
            GameMgr.Get().ReduceTheNumberOfLimitCount();

            //임의 점수
            AddScore(matchList.Count * 10);

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

        public void BoosterMatches(int x, int y, bool isComboCheck = true)
        {
            List<BlockDef> blockDefList = null;
            Booster booster = blockEntities[x, y] as Booster;
            bool isCombo = false;
            
            comboBoosters.Add(booster);

            blockDefList = booster.Match(x, y);

            if (isComboCheck)
            {
                //제한 횟수 감소
                GameMgr.Get().ReduceTheNumberOfLimitCount();

                List <Vector2> comboBoosters = GetMatches(x, y, null);

                //임의 점수
                AddScore(comboBoosters.Count);

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
            comboBoosters.Clear();

            if (blockDefList != null)
            {
                AddScore(blockDefList.Count * 15);
                if (!isCombo) StartCoroutine(Co_CreateBoosterParticles(booster, false));
                StartCoroutine(Co_BoosterMatch(blockDefList));
            }
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
                    if(!hitBlock.gameObject.activeSelf)
                    {
                        ++count;
                        continue;
                    }

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
                        //미션 체크.
                        AddCollectedBlock(hitBlock);
                        var particles = GameMgr.Get().gamePools.GetParticles(hitBlock);
                        if (particles != null) CreateParticle(particles, hitBlock._LocalPosition);
                    }

                    ++count;
                }

                yield return new WaitForSeconds(delayTime);


                MatchDown();

                GameMgr.Get().GameEnd();
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
                    if (comboBoosterIndex != null && comboBoosterIndex.Contains(new Vector2(ix, iy))) continue;
                    AddBlockEntity(blocks, ix, iy);
                }
            }

            for (int iy = y - 1; iy <= y + 1; ++iy)
            {
                for (int ix = 0; ix < width; ++ix)
                {
                    if (comboBoosterIndex != null && comboBoosterIndex.Contains(new Vector2(ix, iy))) continue;
                    AddBlockEntity(blocks, ix, iy);
                }
            }

            //임의 점수.
            AddScore((width + height) * 6);

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

            //임의 점수.
            AddScore(blocks.Count);

            StartCoroutine(Co_RainbowAndAnotherBomb(blocks, type));
        }
        
        public void UseItem(ItemType type, int x = -1, int y = -1)
        {
            switch (type)
            {
                case ItemType.hammer:
                    //임의 점수
                    AddScore(10);
                    //미션 체크.
                    AddCollectedBlock(blockEntities[x, y]);
                    StartCoroutine(Co_UseHammerItem(x, y));
                    break;
                case ItemType.horizon:
                    {
                        StartCoroutine(Co_UseMittHorizonItem(y));
                        Vector2 start = new Vector2(-410f, blockEntities[x, y]._LocalPosition.y);
                        Vector2 end = new Vector2(420f, blockEntities[x, y]._LocalPosition.y);
                        StartCoroutine(Co_MittMove(start, end, x, y, (int)ItemType.horizon));
                    }
                    break;
                case ItemType.vertical:
                    {
                        StartCoroutine(Co_UseMittVerticalItem(x));
                        Vector2 start = new Vector2(blockEntities[x, y]._LocalPosition.x, -515f);
                        Vector2 end = new Vector2(blockEntities[x, y]._LocalPosition.x, 545f);
                        StartCoroutine(Co_MittMove(start, end, x, y, (int)ItemType.vertical));
                    }
                    break;
                case ItemType.mix:
                    AllMixBlocks();
                    //AllBlockColorSet();
                    StartCoroutine(Co_BlockIconSetting());
                    break;
            }
        }

        IEnumerator Co_UseHammerItem(int x, int y)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;

            isWait = true;

            GameObject hammer = GameMgr.Get().gameItemAnim[(int)ItemType.hammer];
            hammer.transform.localPosition = new Vector2(80f, -650f);
            hammer.SetActive(true);

            Vector2 start = hammer.transform.localPosition;
            Vector2 end = blockEntities[x, y]._LocalPosition;
            end.x += 180;
            end.y -= 50;

            float speed = 1.5f;
            float time = 0;
            float totalTime = 1f / speed;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                hammer.transform.localPosition = Vector3.Lerp(start, end, time * speed);
                yield return null;
            }

            hammer.transform.localPosition = end;
            hammer.GetComponent<Animator>().SetTrigger("Plump");

            yield return new WaitForSeconds(.6f);

            isWait = false;

            BlockEntity block = blockEntities[x, y];
            block._State = State.wait;
            block.Hide();

            var particles = GameMgr.Get().gamePools.GetParticles(block);
            if (particles != null) CreateParticle(particles, block._LocalPosition);

            MatchDown();

            yield return new WaitForSeconds(.1f);

            hammer.transform.localScale = Vector2.one;
            hammer.SetActive(false);
        }

        IEnumerator Co_UseMittHorizonItem(int y)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;

            isWait = true;

            int x = 0;
            while (x < width)
            {
                BlockEntity block = blockEntities[x, y];

                //미션 체크.
                AddCollectedBlock(block);

                block._State = State.wait;
                block.Hide();

                var particles = GameMgr.Get().gamePools.GetParticles(block);
                if (particles != null) CreateParticle(particles, block._LocalPosition);

                ++x;
                yield return new WaitForSeconds(.05f);
            }

            isWait = false;
            //임의 점수
            AddScore(width);
            MatchDown();

            GameMgr.Get().GameEnd();
        }

        IEnumerator Co_UseMittVerticalItem(int x)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;

            isWait = true;

            int y = 0;
            while (y < height)
            {
                BlockEntity block = blockEntities[x, y];

                //미션 체크.
                AddCollectedBlock(block);

                block._State = State.wait;
                block.Hide();

                var particles = GameMgr.Get().gamePools.GetParticles(block);
                if (particles != null) CreateParticle(particles, block._LocalPosition);

                ++y;
                yield return new WaitForSeconds(.03f);
            }

            isWait = false;

            //임의 점수
            AddScore(height);
            MatchDown();

            GameMgr.Get().GameEnd();
        }

        IEnumerator Co_MittMove(Vector2 startPos, Vector2 endPos,  int x, int y, int index)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;

            isWait = true;
            GameObject mitt = GameMgr.Get().gameItemAnim[index];
            mitt.transform.localPosition = startPos;
            mitt.SetActive(true);

            float speed = 1.8f;
            float time = 0;
            float totalTime = 1f / speed;

            while (time < totalTime)
            {
                time += Time.deltaTime;
                mitt.transform.localPosition = Vector3.Lerp(startPos, endPos, time * speed);
                yield return null;
            }

            mitt.transform.localPosition = endPos;

            mitt.SetActive(false);
            isWait = false;
        }

        //수정해야함....ㅜㅜ
        private List<Vector2> GetMatches(int x, int y, LevelBlock levelBlock, bool isColorMatch = true)
        {
            Queue<BlockDef> qTemp = new Queue<BlockDef>();
            List<Vector2> matchList = new List<Vector2>();

            qTemp.Enqueue(new BlockDef(x, y));
            while (qTemp.Count > 0)
            {
                BlockDef l = qTemp.Dequeue();
                BlockDef r = new BlockDef(l.x + 1, l.y);

                while (IsValidIndex(l.x, width) && IsMatches(l, levelBlock, isColorMatch))
                {
                    if (!matchList.Contains(new Vector2(l.x, l.y)))
                        matchList.Add(new Vector2(l.x, l.y));

                    BlockDef up = new BlockDef(l.x, l.y - 1);
                    BlockDef down = new BlockDef(l.x, l.y + 1);

                    if (!matchList.Contains(new Vector2(up.x, up.y)))
                    {
                        if ((l.y > 0) && IsMatches(up, levelBlock, isColorMatch))
                            qTemp.Enqueue(up);
                    }

                    if (!matchList.Contains(new Vector2(down.x, down.y)))
                    {
                        if ((l.y < height - 1) && IsMatches(down, levelBlock, isColorMatch))
                            qTemp.Enqueue(down);
                    }
                    l.x--;
                }
                
                while (IsValidIndex(r.x, width) && IsMatches(r, levelBlock, isColorMatch))
                {
                    if (!matchList.Contains(new Vector2(r.x, r.y)))
                        matchList.Add(new Vector2(r.x, r.y));

                    BlockDef up = new BlockDef(r.x, r.y - 1);
                    BlockDef down = new BlockDef(r.x, r.y + 1);

                    if (!matchList.Contains(new Vector2(up.x, up.y)))
                    {
                        if ((r.y > 0) && IsMatches(up, levelBlock, isColorMatch))
                            qTemp.Enqueue(up);
                    }

                    if (!matchList.Contains(new Vector2(down.x, down.y)))
                    {
                        if ((r.y < height - 1) && IsMatches(down, levelBlock, isColorMatch))
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
                            ReturnObject(blockEntity.gameObject);
                            CreateNewBlock(x, y);
                            blockEntity = blockEntities[x, y];

                            stack.Add(blockEntity);
                            blockEntity._LocalPosition = new Vector2(Point(x, width, wSize), (15 * hSize));
                        }
                    }

                    else if (stack.Count != 0)
                    {
                        int yy = (y - stack.Count);
                        if (emptyIdx != -1 && emptyIdx < yy) emptyCnt = 0;
                        yy -= emptyCnt;
                        Swap(x, y, x, yy);
                        Vector2 vEndPos = new Vector2(Point(x, width, wSize), Point(yy, height, hSize));
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
                    Vector2 vEndPos = new Vector2(Point(x, width, wSize), Point(y, height, hSize));
                    blockEntity.DownMove(vEndPos, x, y);
                }
            }
            
            if(!GameMgr.Get().isGameEnd) BlockIconSetting();
        }

        private void BlockIconSetting()
        {
            StopCoroutine("Co_BlockIconSetting");
            StartCoroutine("Co_BlockIconSetting");
        }

        //여기도 수정해야함. 
        IEnumerator Co_BlockIconSetting()
        {
            while (isWait)yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            List<Vector2> matchListCheck = new List<Vector2>();
            ColorBlock block = null;
            bool isMatches = false;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if (matchListCheck.Contains(new Vector2(x, y))) continue;

                    for (int j = 0; j < (int)ColorType.purple + 1; ++j)
                    {
                        block = blockEntities[x, y] as ColorBlock;
                        if (block == null) continue;
                        if (block._BlockType == BlockType.empty) continue;

                        BlockType blockType = BlockType.red;
                        switch ((ColorType)j)
                        {
                            case ColorType.red:
                                blockType = BlockType.red;
                                break;
                            case ColorType.orange:
                                blockType = BlockType.orange;
                                break;
                            case ColorType.yellow:
                                blockType = BlockType.yellow;
                                break;
                            case ColorType.green:
                                blockType = BlockType.green;
                                break;
                            case ColorType.blue:
                                blockType = BlockType.blue;
                                break;
                            case ColorType.purple:
                                blockType = BlockType.purple;
                                break;
                        }
                        
                        List<Vector2> matchList = GetMatches(x, y, new LevelBlockType() { type = blockType }, false);

                        foreach(Vector2 match in matchList)
                        {
                            if(!matchListCheck.Contains(match))
                                matchListCheck.Add(match);
                        }

                        int count = matchList.Count;
                        if (count == 0) continue;
                        if (count >= 5)
                        {
                            foreach (Vector2 vec in matchList)
                            {
                                int xx = (int)vec.x;
                                int yy = (int)vec.y;
                                block = blockEntities[xx, yy] as ColorBlock;
                                if (block == null) continue;
                                if (count == 5 || count == 6)
                                {
                                    block.SetIcon(string.Format(ARROW_ICON_FORMAT, block._BlockType));
                                }
                                else if (count == 7 || count == 8)
                                {
                                    block.SetIcon(string.Format(TNT_ICON_FORMAT, block._BlockType));
                                }
                                else
                                {
                                    block.SetIcon(string.Format(RAINBOW_ICON_FORMAT, block._BlockType));
                                }

                                isMatches = true;
                            }
                            break;
                        }
                        else
                        {
                            if(count > 1) isMatches = true;

                            foreach (Vector2 vec in matchList)
                            {
                                int xx = (int)vec.x;
                                int yy = (int)vec.y;
                                block = blockEntities[xx, yy] as ColorBlock;
                                if (block == null) continue;
                                block.SetIcon(string.Format(FACE_ICON_FORMAT, block._BlockType));
                            }
                        }
                    }
                }
            }

            if(!isMatches)
            {
                AllMixBlocks();
            }
        }

        private void AllMixBlocks()
        {
            List<BlockEntity> tempEntities = MixBlocks();
            for (int x = 0; x < width; ++x)
            {
                for(int y = 0; y < height; ++y)
                {
                    int idx = width * x + y;
                    blockEntities[x, y].DownMove(tempEntities[idx]._LocalPosition, x, y);
                }
            }
        }

        private List<BlockEntity> MixBlocks()
        {
            List<BlockEntity> list = new List<BlockEntity>();
            foreach (BlockEntity temp in blockEntities)
            {
                list.Add(temp);
            }

            int count = list.Count;
            while (count > 1)
            {
                int index = (Random.Range(0, count) % count);
                count--;
                Swap(list[index]._X, list[index]._Y, list[count]._X, list[count]._Y);
            }

            return list;
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
            entity.SetDepth(blockEntities[x, y]._BlockDepth);
            blockEntities[x, y] = entity;
            blockEntities[x, y]._State = State.idle;
            blockEntities[x, y].SetData(x, y);
        }

        private void CreateNewBlock(int x, int y)
        {
            BlockEntity entity = GameMgr.Get().gamePools.GetBlockEntity(new LevelBlockType() { type = BlockType.random });
            Assert.IsNotNull(entity);
            entity.transform.localPosition = blockEntities[x, y]._LocalPosition;
            blockEntities[x, y] = entity;
            Block block = blockEntities[x, y] as Block;
            if (block == null) return;
            block._State = State.wait;
            block.SetData(x, y);
            block.SetIcon(string.Format(FACE_ICON_FORMAT, block._BlockType));
            block.Hide();
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

        private bool IsMatches(BlockDef blockDef, LevelBlock levelBlock, bool isColorMatch = true)
        {
            bool isMatch = false;
            if (levelBlock != null)
            {
                if (levelBlock is LevelBlockType)
                {
                    if(isColorMatch)
                        isMatch = IsColorMatch(blockDef, (levelBlock as LevelBlockType).type);
                    else
                        isMatch = IsColorComparison(blockDef, (levelBlock as LevelBlockType).type);
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

        private void AddScore(int score)
        {
            GameMgr mgr = GameMgr.Get();
            mgr._GameState.score += score;
            mgr.progressBar.UpdateProgressBar(mgr._GameState.score);
        }

        private void AddCollectedBlock(BlockEntity blockEntity)
        {
            Block block = blockEntity as Block;
            if (block != null)
            {
                if (GameMgr.Get()._GameState.collectedBlocks.ContainsKey(block._BlockType))
                {
                    GameMgr.Get()._GameState.collectedBlocks[block._BlockType] += 1;
                }  
            }
            //blocker
        }

        private void AddCollectedBlock(List<Vector2> blockEntitys)
        {
            for(int i = 0; i < blockEntitys.Count; ++i)
            {
                int x = (int)blockEntitys[i].x;
                int y = (int)blockEntitys[i].y;
                AddCollectedBlock(blockEntities[x, y]);
                //blocker
            }
        }

        private void AddCollectedBlock(List<BlockDef> blockEntitys)
        {
            for (int i = 0; i < blockEntitys.Count; ++i)
            {
                AddCollectedBlock(blockEntities[blockEntitys[i].x, blockEntitys[i].y]);
                //blocker
            }
        }

        public void FinalFinale(int count)
        {
            StartCoroutine(Co_FinalFinale(count));
        }

        IEnumerator Co_FinalFinale(int count)
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            foreach (BlockEntity entity in blockEntities)
            {
                ColorBlock block = entity as ColorBlock;
                if (block == null) continue;
                block.SetIcon(string.Format(FACE_ICON_FORMAT, block._BlockType));
            }

            yield return new WaitForSeconds(.8f);

            if(count > 0)
            {
                isWait = true;

                List<BlockDef> blockDefs = new List<BlockDef>();
                for (int i = 0; i < count; ++i)
                {
                    int random = Random.Range(0, blockEntities.Length);
                    int x = random / width;
                    int y = random % height;
                    ColorBlock block = blockEntities[x, y] as ColorBlock;
                    if (block == null)
                    {
                        if (i > 0) --i;
                        continue;
                    }
                    if (!blockDefs.Contains(block._BlockData))
                        blockDefs.Add(block._BlockData);
                    else 
                        if (i > 0) --i;
                }
                
                StartCoroutine(Co_RainbowAndAnotherBomb(blockDefs, BoosterType.arrow));
            }
            
            StartCoroutine(Co_FinalConfirmation());
        }

        IEnumerator Co_FinalConfirmation()
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            isWait = true;

            List<Booster> boosters = new List<Booster>();
            foreach (BlockEntity block in blockEntities)
            {
                Booster booster = block as Booster;
                if (booster == null) continue;
                boosters.Add(booster);
            }

            if (boosters.Count > 0)
                StartCoroutine(Co_RainbowAndAnotherBomb(boosters));
            else
                isWait = false;
        }

        IEnumerator Co_RainbowAndAnotherBomb(List<BlockDef> blocks, BoosterType type)
        {
            while (IsMoving(State.booster_move)) yield return null;
            isWait = true;

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

                if(GameMgr.Get().isGameEnd)
                {
                    //제한 횟수 감소
                    GameMgr.Get().ReduceTheNumberOfLimitCount();
                }

                yield return new WaitForSeconds(0.15f);
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
                Booster booster = blockEntities[boosters[count]._X, boosters[count]._Y] as Booster;
                if (booster == null)
                {
                    ++count;
                    continue;
                }
                blocks = booster.Match(booster._X, booster._Y);

                if(GameMgr.Get().isGameEnd)
                {
                    Rainbow rainbow = booster as Rainbow;
                    if(rainbow != null)
                    {
                        rainbow._State = State.wait;
                        rainbow.Hide();
                    }
                }

                yield return StartCoroutine(Co_BoosterMatch(blocks));
                ++count;
            }

            isWait = false;
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
                localPosition.y = Point(iy, height, hSize);
                CreateParticle(particles, localPosition);
            }

            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                if (!IsValidIndex(ix, width)) continue;
                particles = GameMgr.Get().gamePools.lineVerticalParticlesPool.GetObject();
                localPosition = particles.transform.localPosition;
                localPosition.x = Point(ix, width, wSize);
                CreateParticle(particles, localPosition);
            }
        }
    }
}