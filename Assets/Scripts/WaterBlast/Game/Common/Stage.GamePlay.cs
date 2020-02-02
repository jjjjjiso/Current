using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using WaterBlast.System;
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

        private const int basicScore   = 10;

        private List<Vector2> comboBoosterIndex = new List<Vector2>();
        private List<Booster> comboBoosters = new List<Booster>();
        private BlockType rainbowColor = BlockType.empty;

        private float delayTime = 0.35f;

        private bool isOne = false;
        public bool isWait   = false;
        public bool isFinale = false;

        int tmpCount;
        public void NormMatch(BlockEntity pickBlock)
        {
            int x = pickBlock._X;
            int y = pickBlock._Y;
            List<BlockEntity> matchedBlocks = new List<BlockEntity>();
            GetMatches(pickBlock, matchedBlocks);
            
            currRadiationBlockCount = oldRadiationBlockCount;

            if (matchedBlocks.Count >= 2) //2개 이상 터트리기.
            {
                AddCollectedBlock(matchedBlocks);               //미션 체크.
                GameMgr.G.ReduceTheNumberOfLimitCount();        //제한 횟수 감소
                UpdateScore(matchedBlocks.Count * basicScore);  // 점수

                if (matchedBlocks.Count < 5)
                {
                    for (int i = 0; i < matchedBlocks.Count; ++i)
                    {
                        Block block = matchedBlocks[i] as Block;
                        var particles = GameMgr.G.gamePools.GetParticles(block);
                        if (particles != null) CreateParticle(particles, block._LocalPosition);
                        block.Hide();
                        
                        CheckMissionBlock(block);
                    }
                    MatchDown();
                }
                else
                {
                    CheckMissionBlock((matchedBlocks[0] as Block));

                    for (int i = 1; i < matchedBlocks.Count; ++i)
                    {
                        Block block = matchedBlocks[i] as Block;
                        Vector2 vTarget = pickBlock._LocalPosition;
                        block.TargetMove(vTarget);
                        
                        CheckMissionBlock(block);
                    }
                    StartCoroutine(Co_BoosterChange(x, y, matchedBlocks.Count));
                }
            }
            else
            {
                isOne = true;
                pickBlock.PlayAnim("NoMatches");
                pickBlock._State = State.idle;
            }
            
            StartCoroutine(Co_CheckRadiationBlock());
        }

        // 전염 블록 개수에 따라 추가 전염 시키거나 가만히 있거나
        IEnumerator Co_CheckRadiationBlock()
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;
            if (oldRadiationBlockCount <= 0) yield break;
            if (oldRadiationBlockCount > currRadiationBlockCount) // 없앴다.
            {
                oldRadiationBlockCount = currRadiationBlockCount;
            }
            else // 그대로다.
            {
                if(isOne)
                {
                    isOne = false;
                    yield break;
                }

                foreach(BlockEntity entity in blockEntities)
                {
                    Block block = entity as Block;
                    if (block == null) continue;
                    if (block._BlockType != BlockType.radiation) continue;

                    var topBlock = new BlockDef(block._X, block._Y + 1);
                    var bottomBlock = new BlockDef(block._X, block._Y - 1);
                    var leftBlock = new BlockDef(block._X - 1, block._Y);
                    var rightBlock = new BlockDef(block._X + 1, block._Y);

                    var surroundingBlocks = new List<BlockDef> { topBlock, bottomBlock, leftBlock, rightBlock };
                    
                    foreach (var temp in surroundingBlocks)
                    {
                        if (IsValidBlockEntity(temp))
                        {
                            Block blockTemp = blockEntities[temp.x, temp.y] as Block;
                            if (blockTemp == null) continue;
                            if (!blockTemp.gameObject.activeSelf) continue;
                            if (blockTemp._BlockType == BlockType.radiation) continue;
                            ++oldRadiationBlockCount;
                            currRadiationBlockCount = oldRadiationBlockCount;
                            Debug.Log("+ " + oldRadiationBlockCount);
                            int x = blockTemp._X;
                            int y = blockTemp._Y;
                            ReturnObject(blockTemp.gameObject);
                            CreateRadiationBlock(x, y);
                            yield break;
                        }
                    }
                }
            }
        }

        private void CheckMissionBlock(Block block)
        {
            CheckMissionBlock(block, BlockType.bubble);
            CheckMissionBlock(block, BlockType.box1);
            CheckMissionBlock(block, BlockType.box2);
            CheckMissionBlock(block, BlockType.radiation);
        }

        IEnumerator Co_BoosterChange(int x, int y, int count, bool isStartItem = false)
        {
            while (IsMoving(State.booster_move)) yield return null;
            var hitBlock = blockEntities[x, y] as Block;
            BlockType colorType = (hitBlock != null) ? hitBlock._BlockType : BlockType.empty;
            ReturnObject(hitBlock.gameObject);

            int score = 0;
            switch(count)
            {
                case 5:
                case 6:
                    {
                        int iRandom = Random.Range((int)ArrowType.horizon, (int)ArrowType.vertical + 1);
                        CreateBooster(BoosterType.arrow, x, y);
                        ArrowBomb arrow = blockEntities[x, y] as ArrowBomb;
                        arrow.UpdateSprite(iRandom);
                        score = arrow.BonusScore();
                    }
                    break;
                case 7:
                case 8:
                    {
                        CreateBooster(BoosterType.bomb, x, y);
                        score = blockEntities[x, y].GetComponent<Bomb>().BonusScore();
                    }
                    break;
                default:
                    {
                        CreateBooster(BoosterType.rainbow, x, y);
                        Rainbow rainbow = blockEntities[x, y] as Rainbow;
                        Assert.IsNotNull(rainbow);
                        rainbow._PreType = colorType;
                        string strTemp = string.Format("{0}_{1}", BoosterType.rainbow, colorType);
                        rainbow.UpdateSprite(strTemp);
                        score = rainbow.BonusScore();
                    }
                    break;
            }

            if(!isStartItem)
            {
                // 점수
                UpdateScore(score);
                MatchDown();
            }
        }
        
        //레인보우 터트릴때도 옆에 전염블록있는지 검사하고 터트려야함.
        public void BoosterMatches(BlockEntity pickBlock, bool isComboCheck = true)
        {
            if(isComboCheck) currRadiationBlockCount = oldRadiationBlockCount;

            Booster pickBooster = pickBlock as Booster;
            comboBoosters.Add(pickBooster);
            List<BlockEntity> matchedBlocks = null;
            matchedBlocks = pickBooster.Match(pickBooster._X, pickBooster._Y, ref currRadiationBlockCount);

            bool isCombo = false;
            int score = (matchedBlocks.Count * basicScore) + pickBooster.BonusScore();
            if (isComboCheck)
            {
                //제한 횟수 감소
                GameMgr.G.ReduceTheNumberOfLimitCount();
                List<BlockEntity> tempComboBoosters = new List<BlockEntity>();
                GetMatches(pickBooster, tempComboBoosters, false);
                if (tempComboBoosters.Count > 1)
                {
                    currRadiationBlockCount = oldRadiationBlockCount;
                    foreach (Booster comboBooster in tempComboBoosters)
                    {
                        comboBooster.TargetMove(pickBooster._LocalPosition);
                    }

                    matchedBlocks.Clear();
                    score = 0;
                    if (pickBooster._BoosterType == BoosterType.rainbow) rainbowColor = pickBooster.GetComponent<Rainbow>()._PreType;
                    matchedBlocks = BoosterCombo(pickBooster._X, pickBooster._Y, ref isCombo, ref score);
                }
            }

            pickBooster._IsCombo = false;
            comboBoosters.Clear();
            if (matchedBlocks != null)
            {
                AddCollectedBlock(matchedBlocks);
                //점수
                UpdateScore(score);

                if (!isCombo)
                    StartCoroutine(Co_CreateBoosterParticles(pickBooster, false));
                
                StartCoroutine(Co_BoosterMatch(matchedBlocks));
            }
        }

        bool isBooFirst = false;
        IEnumerator Co_BoosterMatch(List<BlockEntity> boosters)
        {
            while (IsMoving(State.booster_move)) yield return null;
            ComboBoostersClear();
            int defCount = boosters.Count;
            int count = 0;
            int booCount = 0;
            if (defCount != 0)
            {
                while (count < defCount)
                {
                    int x = boosters[count]._X;
                    int y = boosters[count]._Y;
                    var hitBlock = blockEntities[x, y];
                    if(!hitBlock.gameObject.activeSelf)
                    {
                        ++count;
                        continue;
                    }
                    hitBlock.Hide();
                    Booster booster = hitBlock as Booster;
                    if (booster != null)
                    {
                        if(booCount != 0)
                        {
                            if (!booster._IsCombo)
                            {
                                BoosterMatches(booster, false);
                                isBooFirst = true;
                                continue;
                            }
                            else
                            {
                                booster._IsCombo = false;
                                ReturnObject(booster.gameObject);
                                CreateNewBlock(x, y);
                            }
                        }

                        ++booCount;
                    }
                    else
                    {
                        //미션 체크.
                        AddCollectedBlock(hitBlock);
                        var particles = GameMgr.G.gamePools.GetParticles(hitBlock);
                        if (particles != null) CreateParticle(particles, hitBlock._LocalPosition);
                    }
                    ++count;
                }
                yield return new WaitForSeconds(delayTime);
                
                if (!isBooFirst)
                {
                    StartCoroutine(Co_CheckRadiationBlock());
                    MatchDown();
                    GameMgr.G.GameEnd();
                }
            }

            isBooFirst = false;
        }

        private List<BlockEntity> BoosterCombo(int x, int y, ref bool isCombo, ref int score)
        {
            List<BlockEntity> blocks = null;
            comboBoosters.Sort(delegate (Booster a, Booster b) { return (b._BoosterType.CompareTo(a._BoosterType)); });

            int bonusScore = (comboBoosters[0].BonusScore() + comboBoosters[1].BonusScore()) * 2;
            if (comboBoosters[0]._BoosterType == comboBoosters[1]._BoosterType)
            {
                isCombo = true;
                blocks = comboBoosters[0].ComboMatch(x, y, ref currRadiationBlockCount);
                StartCoroutine(Co_CreateBoosterParticles(comboBoosters[0], isCombo));

                score = (blocks.Count * basicScore) + bonusScore;
            }
            else
            {
                switch (BoosterComboType(comboBoosters[0]._BoosterType, comboBoosters[1]._BoosterType))
                {
                    case BoosterSynthesis.arrowBombAndBomb:
                        ArrowBombAndBomb(x, y, bonusScore);
                        break;
                    case BoosterSynthesis.arrowBombAndRainbow:
                        RainbowAndAnotherBomb(BoosterType.arrow, bonusScore);
                        break;
                    case BoosterSynthesis.BombAndRainbow:
                        RainbowAndAnotherBomb(BoosterType.bomb, bonusScore);
                        break;
                }
            }

            return blocks;
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

        private void ArrowBombAndBomb(int x, int y, int bonusScore)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();
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
            
            AddCollectedBlock(blocks);

            //점수
            int score = (blocks.Count * basicScore) + bonusScore;
            UpdateScore(score);

            StartCoroutine(Co_ArrowBombAndBombParticles(x, y));
            
            StartCoroutine(Co_BoosterMatch(blocks));
        }

        private void RainbowAndAnotherBomb(BoosterType type, int bonusScore)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Block block = blockEntities[x, y] as Block;
                    if (block == null) continue;
                    if (rainbowColor != block._BlockType) continue;
                    blocks.Add(block);
                }
            }

            AddCollectedBlock(blocks);

            //점수
            int score = (blocks.Count * basicScore) + bonusScore;
            UpdateScore(score);

            StartCoroutine(Co_RainbowAndAnotherBomb(blocks, type));
        }
        
        public void UseItem(ItemType type, BlockEntity block)
        {
            switch (type)
            {
                case ItemType.hammer:
                    //점수
                    UpdateScore(basicScore);
                    //미션 체크.
                    AddCollectedBlock(block);
                    StartCoroutine(Co_UseHammerItem(block));
                    break;
                case ItemType.horizon:
                    {
                        StartCoroutine(Co_UseMittHorizonItem(block._Y));
                        Vector2 start = new Vector2(-410f, block._LocalPosition.y);
                        Vector2 end = new Vector2(420f, block._LocalPosition.y);
                        StartCoroutine(Co_MittMove(start, end, (int)ItemType.horizon));
                    }
                    break;
                case ItemType.vertical:
                    {
                        StartCoroutine(Co_UseMittVerticalItem(block._X));
                        Vector2 start = new Vector2(block._LocalPosition.x, -515f);
                        Vector2 end = new Vector2(block._LocalPosition.x, 545f);
                        StartCoroutine(Co_MittMove(start, end, (int)ItemType.vertical));
                    }
                    break;
                case ItemType.mix:
                    AllMixBlocks();
                    StartCoroutine(Co_BlockIconSetting());
                    break;
            }
        }

        IEnumerator Co_UseHammerItem(BlockEntity block)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;
            isWait = true;

            GameObject hammer = GameMgr.G.gameItemAnim[(int)ItemType.hammer];
            hammer.transform.localPosition = new Vector2(80f, -650f);
            hammer.SetActive(true);

            Vector2 start = hammer.transform.localPosition;
            Vector2 end = block._LocalPosition;
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
            block.Hide();

            var particles = GameMgr.G.gamePools.GetParticles(block);
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
                block.Hide();

                var particles = GameMgr.G.gamePools.GetParticles(block);
                if (particles != null) CreateParticle(particles, block._LocalPosition);

                ++x;
                yield return new WaitForSeconds(.05f);
            }

            isWait = false;
            //점수
            UpdateScore(width * basicScore);
            GameMgr.G.GameEnd();
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
                block.Hide();

                var particles = GameMgr.G.gamePools.GetParticles(block);
                if (particles != null) CreateParticle(particles, block._LocalPosition);

                ++y;
                yield return new WaitForSeconds(.03f);
            }

            isWait = false;
            //점수
            UpdateScore(height * basicScore);
            GameMgr.G.GameEnd();
        }

        IEnumerator Co_MittMove(Vector2 startPos, Vector2 endPos, int index)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;

            isWait = true;
            GameObject mitt = GameMgr.G.gameItemAnim[index];
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
            MatchDown();
        }
        
        private void GetMatches(BlockEntity blockEntity, List<BlockEntity> matchedBlocks, bool isColorMatch = true)
        {
            int x = blockEntity._X;
            int y = blockEntity._Y;

            var topBlock = new BlockDef(x, y + 1);
            var bottomBlock = new BlockDef(x, y - 1);
            var leftBlock = new BlockDef(x - 1, y);
            var rightBlock = new BlockDef(x + 1, y);
            var surroundingBlocks = new List<BlockDef> { topBlock, bottomBlock, leftBlock, rightBlock };

            var hasMatch = false;
            foreach (var surroundingBlock in surroundingBlocks)
            {
                if (IsValidBlockEntity(surroundingBlock))
                {
                    var tempBlockEntity = blockEntities[surroundingBlock.x, surroundingBlock.y];
                    if (tempBlockEntity != null && tempBlockEntity.gameObject.activeSelf)
                    {
                        var block = tempBlockEntity.GetComponent<Block>();
                        if (isColorMatch)
                        {
                            if(block != null)
                            {
                                if (block._BlockType == blockEntity.GetComponent<Block>()._BlockType)
                                {
                                    hasMatch = true;
                                }
                            }
                        }
                        else
                        {
                            var booster = tempBlockEntity.GetComponent<Booster>();
                            if (booster != null && IsCombo(booster))
                            {
                                hasMatch = true;
                            }
                        }
                    }
                }
            }

            if (!hasMatch)
            {
                return;
            }

            if (!matchedBlocks.Contains(blockEntity))
            {
                matchedBlocks.Add(blockEntity);
            }

            foreach (var surroundingBlock in surroundingBlocks)
            {
                if (IsValidBlockEntity(surroundingBlock))
                {
                    var tempBlockEntity = blockEntities[surroundingBlock.x, surroundingBlock.y];
                    if (tempBlockEntity != null && tempBlockEntity.gameObject.activeSelf)
                    {
                        if(!matchedBlocks.Contains(tempBlockEntity))
                        {
                            if (isColorMatch)
                            {
                                var block = tempBlockEntity.GetComponent<Block>();
                                if (block != null && block._BlockType == blockEntity.GetComponent<Block>()._BlockType)
                                {
                                    GetMatches(tempBlockEntity, matchedBlocks);
                                }
                            }
                            else
                            {
                                var booster = tempBlockEntity.GetComponent<Booster>();
                                if (booster != null && IsCombo(booster))
                                {
                                    GetMatches(tempBlockEntity, matchedBlocks, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        List<BlockDef> tmp = new List<BlockDef>();
        private void CheckMissionBlock(Block block, BlockType type)
        {
            tmp.Clear();

            var topBlock = new BlockDef(block._X, block._Y + 1);
            var bottomBlock = new BlockDef(block._X, block._Y - 1);
            var leftBlock = new BlockDef(block._X - 1, block._Y);
            var rightBlock = new BlockDef(block._X + 1, block._Y);

            var surroundingBlocks = new List<BlockDef> { topBlock, bottomBlock, leftBlock, rightBlock };

            int count = 0;
            foreach (var temp in surroundingBlocks)
            {
                if (IsValidBlockEntity(temp))
                {
                    //if (tmp.Count > 0 && tmp[count].x == temp.x && tmp[count].y == temp.y) continue;
                    Block entity = blockEntities[temp.x, temp.y] as Block;
                    if (entity == null) continue;
                    if (!entity.gameObject.activeSelf) continue;
                    if (entity._BlockType != type) continue;
                    if (type == BlockType.radiation) --currRadiationBlockCount;

                    tmp.Add(temp);
                    entity.Hide();
                    ++count;
                }
            }
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
                    if (IsCheckBlock(blockEntity, BlockType.empty))
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
                        if (IsCheckBlock(blockEntity, BlockType.box1) ||
                            IsCheckBlock(blockEntity, BlockType.box2) ||
                            IsCheckBlock(blockEntity, BlockType.radiation))
                        {
                            stack.Clear();
                            continue;
                        }
                        
                        int yy = (y - stack.Count);
                        if (emptyIdx != -1 && emptyIdx < yy) emptyCnt = 0;
                        yy -= emptyCnt;
                        Swap(x, y, x, yy);
                        Vector2 vEndPos = new Vector2(Point(x, width, wSize), Point(yy, height, hSize));
                        blockEntity.DownMove(vEndPos, x, yy);

                        if(yy == 0 && (IsCheckBlock(blockEntity, BlockType.can) || IsCheckBlock(blockEntity, BlockType.paper)))
                        {
                            StartCoroutine(Co_MoveMission(blockEntity));
                        }
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
            
            if(!GameMgr.G.isGameEnd) BlockIconSetting();
        }

        IEnumerator Co_MoveMission(BlockEntity block)
        {
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            // 이펙트 뿌려주면서 사라진다.
            block.Hide();
            MatchDown();
        }

        private void BlockIconSetting()
        {
            StopCoroutine("Co_BlockIconSetting");
            StartCoroutine("Co_BlockIconSetting");
        }

        IEnumerator Co_BlockIconSetting()
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            List<BlockEntity> oldMatchedBlocks = new List<BlockEntity>();
            bool isMatched = false;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    ColorBlock block = blockEntities[x, y] as ColorBlock;
                    if (block == null) continue;
                    if (oldMatchedBlocks.Contains(block)) continue;
                    if (block._BlockType == BlockType.empty) continue;
                    if (!block.gameObject.activeSelf) continue;

                    List<BlockEntity> matchedBlocks = new List<BlockEntity>();
                    matchedBlocks.Add(block);
                    GetMatches(block, matchedBlocks);

                    foreach (BlockEntity tempBlock in matchedBlocks)
                    {
                        if (oldMatchedBlocks.Contains(tempBlock)) continue;
                        oldMatchedBlocks.Add(tempBlock);
                    }

                    int count = matchedBlocks.Count;
                    if (count == 0) continue;
                    if (count >= 5)
                    {
                        foreach (BlockEntity tempBlock in matchedBlocks)
                        {
                            block = blockEntities[tempBlock._X, tempBlock._Y] as ColorBlock;
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

                            isMatched = true;
                        }
                        continue;
                    }
                    else
                    {
                        if (count > 1) isMatched = true;

                        foreach (BlockEntity tempBlock in matchedBlocks)
                        {
                            block = blockEntities[tempBlock._X, tempBlock._Y] as ColorBlock;
                            if (block == null) continue;
                            block.SetIcon(string.Format(FACE_ICON_FORMAT, block._BlockType));
                        }
                    }
                }
            }

            if (!isMatched)
            {
                AllMixBlocks();
            }
        }

        private void AllMixBlocks()
        {
            List<BlockEntity> tempEntities = MixBlocks();
            int empty = 0;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Block block = blockEntities[x, y] as Block;
                    if (block != null && block._BlockType == BlockType.empty)
                    {
                        ++empty;
                        continue;
                    }

                    int idx = (width * x + y) - empty;
                    blockEntities[x, y].MixMove(tempEntities[idx]._LocalPosition, x, y);
                }
            }
        }

        private List<BlockEntity> MixBlocks()
        {
            List<BlockEntity> list = new List<BlockEntity>();
            foreach (BlockEntity temp in blockEntities)
            {
                Block block = temp as Block;
                if (block != null && block._BlockType == BlockType.empty) continue;
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
            GamePool gamePools = GameMgr.G.gamePools;
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
            var booster = CreateBlock(boosterPool.GetObj());
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

        private void CreateRadiationBlock(int x, int y)
        {
            BlockEntity entity = GameMgr.G.gamePools.GetBlockEntity(new LevelBlockType() { type = BlockType.radiation });
            Assert.IsNotNull(entity);
            entity.Show();
            entity.transform.localPosition = blockEntities[x, y]._LocalPosition;
            entity.SetDepth(blockEntities[x, y]._BlockDepth);
            blockEntities[x, y] = entity;
            blockEntities[x, y].SetData(x, y);
        }

        private void CreateNewBlock(int x, int y)
        {
            BlockEntity entity = GameMgr.G.gamePools.GetBlockEntity(new LevelBlockType() { type = BlockType.random });
            Assert.IsNotNull(entity);
            entity.transform.localPosition = blockEntities[x, y]._LocalPosition;
            blockEntities[x, y] = entity;
            Block block = blockEntities[x, y] as Block;
            if (block == null) return;
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

        private void AddBlockEntity(List<BlockEntity> blocks, int x, int y)
        {
            if (x < 0 || x >= width ||
                y < 0 || y >= height) return;
            
            var entity = blockEntities[x, y];
            if (entity != null)
            {
                if (!entity.gameObject.activeSelf) return;
                var block = entity as Block;
                if (block != null && block._BlockType == BlockType.empty) return;
                if (block != null && block._BlockType == BlockType.can) return;
                if (block != null && block._BlockType == BlockType.paper) return;
                if (blocks.Contains(entity)) return;

                if (block != null && block._BlockType == BlockType.radiation) --currRadiationBlockCount;

                blocks.Add(entity);
            }
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

        private bool IsCombo(Booster booster)
        {
            if (booster == null) return false;
            if (!booster.gameObject.activeSelf) return false;

            if (((comboBoosters[0]._X != booster._X) || (comboBoosters[0]._Y != booster._Y))
                && (!comboBoosterIndex.Contains(new Vector2(booster._X, booster._Y))))
            {
                comboBoosters.Add(booster);
            }

            Rainbow rainbow = booster as Rainbow;
            if (rainbow != null) rainbowColor = rainbow._PreType;

            comboBoosterIndex.Add(new Vector2(booster._X, booster._Y));
            booster._IsCombo = true;

            return true;
        }

        public bool IsMoving(State state)
        {
            foreach (var block in blockEntities)
            {
                if (block == null) continue;
                if (block._State == state) return true;
            }

            return false;
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

        private bool IsCheckBlock(BlockEntity blockEntity, BlockType type)
        {
            Block block = blockEntity as Block;
            if (block != null && block._BlockType == type) return true;
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

        private void UpdateScore(int score)
        {
            GameMgr mgr = GameMgr.G;
            mgr._GameState.score += score;
            mgr.gameUI.progressBar.UpdateProgressBar(mgr._GameState.score);
        }

        private void AddCollectedBlock(BlockEntity blockEntity)
        {
            Block block = blockEntity as Block;
            if (block != null)
            {
                if (GameMgr.G._GameState.collectedBlocks.ContainsKey(block._BlockType))
                {
                    GameMgr.G._GameState.collectedBlocks[block._BlockType] += 1;
                }  
            }
            //blocker

            GameMgr.G.UpdateGoalUI();
        }
        
        private void AddCollectedBlock(List<BlockEntity> blockEntitys)
        {
            for (int i = 0; i < blockEntitys.Count; ++i)
            {
                int x = blockEntitys[i]._X;
                int y = blockEntitys[i]._Y;
                AddCollectedBlock(blockEntities[x, y]);
                //blocker
            }
        }

        public void FinalFinale(int count)
        {
            isFinale = true;
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
                List<BlockEntity> blocks = new List<BlockEntity>();
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
                    if (!blocks.Contains(block))
                        blocks.Add(block);
                    else
                        if (i > 0)
                        {
                            --i;
                            continue;
                        }
                }

                //남은 횟수만큼 화살폭탄 랜덤으로 뿌리기.
                StartCoroutine(Co_RainbowAndAnotherBomb(blocks, BoosterType.arrow));
            }
            
            StartCoroutine(Co_FinalConfirmation());
        }

        //마지막 피날래 끝나고 남은 부스터 터트리기.
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
                StartCoroutine(Co_RainbowAndAnotherBomb(boosters, true));
            else
            {
                isWait = false;
                isFinale = false;
            }
        }

        IEnumerator Co_RainbowAndAnotherBomb(List<BlockEntity> blocks, BoosterType type, bool isFinale = false)
        {
            while (IsMoving(State.booster_move)) yield return null;
            isWait = true;

            List<Booster> boosters = new List<Booster>();
            int count = 0;
            while (count < blocks.Count)
            {
                int x = blocks[count]._X;
                int y = blocks[count]._Y;

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

                if(GameMgr.G.isGameEnd)
                {
                    //제한 횟수 감소
                    GameMgr.G.ReduceTheNumberOfLimitCount();
                }
                yield return new WaitForSeconds(0.15f);
            }
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(Co_RainbowAndAnotherBomb(boosters));
        }

        // 폭탄 터트리기.
        IEnumerator Co_RainbowAndAnotherBomb(List<Booster> boosters, bool isFinale = false)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();
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
                blocks = booster.Match(booster._X, booster._Y, ref currRadiationBlockCount);
                if(GameMgr.G.isGameEnd)
                {
                    Rainbow rainbow = booster as Rainbow;
                    if(rainbow != null)
                    {
                        rainbow.Hide();
                    }
                }
                
                yield return StartCoroutine(Co_BoosterMatch(blocks));
                ++count;
            }

            isWait = false;
            if(isFinale) this.isFinale = false;
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
                particles = GameMgr.G.gamePools.lineHorizontalParticlesPool.GetObj();
                localPosition = particles.transform.localPosition;
                localPosition.y = Point(iy, height, hSize);
                CreateParticle(particles, localPosition);
            }

            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                if (!IsValidIndex(ix, width)) continue;
                particles = GameMgr.G.gamePools.lineVerticalParticlesPool.GetObj();
                localPosition = particles.transform.localPosition;
                localPosition.x = Point(ix, width, wSize);
                CreateParticle(particles, localPosition);
            }
        }
    }
}