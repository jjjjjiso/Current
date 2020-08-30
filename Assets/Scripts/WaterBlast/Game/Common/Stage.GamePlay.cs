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

        private Block block = null;
        private Blocker blocker = null;
        private Booster booster = null;
        private GameObject particles = null;
        
        public void NormMatch(BlockEntity pickBlock)
        {
            if (pickBlock is Blocker) return;
            if (IsCheckNotInfectionBlock(pickBlock as Block)) return;

            int x = pickBlock._X;
            int y = pickBlock._Y;

            List<BlockEntity> matchedBlocks = new List<BlockEntity>();
            List<BlockEntity> matchedBlockers = new List<BlockEntity>();
            GetMatches(pickBlock, matchedBlocks, matchedBlockers);

            SetInfectionCount();

            bool isMatchBlock = false;
            if (matchedBlocks.Count >= 2) //2개 이상 터트리기.
            {
                AddCollectedBlock(matchedBlocks);               //미션 체크.
                GameMgr.G.ReduceTheNumberOfLimitCount();        //제한 횟수 감소
                UpdateScore(matchedBlocks.Count * basicScore);  // 점수 (버블 점수 어찌 할지 상의 후 코드 수정)
                
                if (matchedBlocks.Count < 5)
                {
                    particles = null;
                    for (int i = 0; i < matchedBlocks.Count; ++i)
                    {
                        block = matchedBlocks[i] as Block;
                        if (block == null) continue;
                        particles = GameMgr.G.gamePools.GetParticles(block);
                        if (particles != null) CreateParticle(particles, block._LocalPosition);
                        block.Hide();
                        
                        CheckMissionBlock(block);
                    }
                    isMatchBlock = MatchDown();
                }
                else
                {
                    CheckMissionBlock((matchedBlocks[0] as Block));

                    for (int i = 1; i < matchedBlocks.Count; ++i)
                    {
                        block = matchedBlocks[i] as Block;
                        if (blockerEntites[block._X, block._Y] != null) continue;
                        Vector2 vTarget = pickBlock._LocalPosition;
                        block.TargetMove(vTarget);
                        
                        CheckMissionBlock(block);
                    }
                    StartCoroutine(Co_BoosterChange(x, y, matchedBlocks.Count));
                }

                for (int i = 0; i < matchedBlockers.Count; ++i)
                {
                    ReturnBlocker(matchedBlockers[i] as Blocker);
                }
            }
            else
            {
                isOne = true;
                pickBlock.PlayAnim("NoMatches");
                pickBlock._State = State.idle;
            }
            
            StartCoroutine(Co_CheckStickyBlock());
            StartCoroutine(Co_CheckRadiationBlocker());
            if (!GameMgr.G.isGameEnd) BlockIconSetting();
        }

        //레인보우 터트릴때도 옆에 전염블록있는지 검사하고 터트려야함.
        public void BoosterMatches(BlockEntity pickBlock, bool isComboCheck = true)
        {
            Booster pickBooster = pickBlock as Booster;
            if (pickBooster == null) return;
            if (blockerEntites[pickBooster._X, pickBooster._Y] != null) return;

            comboBoosters.Add(pickBooster);
            List<BlockEntity> matchedBlocks = null;

            if (isComboCheck) SetInfectionCount();

            matchedBlocks = pickBooster.Match(pickBooster._X, pickBooster._Y, ref currStickyBlockCount);

            bool isCombo = false;
            int score = (matchedBlocks.Count * basicScore) + pickBooster.BonusScore();
            if (isComboCheck)
            {
                //제한 횟수 감소
                GameMgr.G.ReduceTheNumberOfLimitCount();
                List<BlockEntity> tempComboBoosters = new List<BlockEntity>();
                List<BlockEntity> matchedBlockers = new List<BlockEntity>();
                GetMatches(pickBooster, tempComboBoosters, matchedBlockers, false);

                if (tempComboBoosters.Count > 1)
                {
                    currStickyBlockCount = oldStickyBlockCount;
                    currRadiationCount = oldRadiationount;
                    foreach (Booster comboBooster in tempComboBoosters)
                    {
                        comboBooster.TargetMove(pickBooster._LocalPosition);
                    }

                    matchedBlocks.Clear();
                    score = 0;
                    if (pickBooster._BoosterType == BoosterType.rainbow) rainbowColor = (pickBooster as Rainbow)._PreType;
                    matchedBlocks = BoosterCombo(pickBooster._X, pickBooster._Y, ref isCombo, ref score);

                    for (int i = 0; i < matchedBlockers.Count; ++i)
                    {
                        ReturnBlocker(matchedBlockers[i] as Blocker);
                    }
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

        private void SetInfectionCount()
        {
            if (oldStickyBlockCount > currStickyBlockCount) oldStickyBlockCount = currStickyBlockCount;
            if (oldRadiationount > currRadiationCount) oldRadiationount = currRadiationCount;

            currStickyBlockCount = oldStickyBlockCount;
            currRadiationCount = oldRadiationount;
        }

        // 전염 블록 개수에 따라 추가 전염 시키거나 가만히 있거나
        IEnumerator Co_CheckStickyBlock()
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;
            if (oldStickyBlockCount <= 0) yield break;
            if (oldStickyBlockCount > currStickyBlockCount) // 없앴다.
            {
                oldStickyBlockCount = currStickyBlockCount;
            }
            else // 그대로다.
            {
                if (isOne)
                {
                    isOne = false;
                    yield break;
                }

                foreach(BlockEntity entity in blockEntities)
                {
                    block = entity as Block;
                    if (block == null) continue;
                    if (block._BlockType != BlockType.sticky) continue;

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
                            if (IsCheckNotInfectionBlock(blockTemp)) continue;
                            ++oldStickyBlockCount;
                            currStickyBlockCount = oldStickyBlockCount;
                            //Debug.Log("+ " + oldStickyBlockCount);
                            ReturnObject(blockTemp.gameObject);
                            CreateStickyBlock(blockTemp._X, blockTemp._Y);
                            yield break;
                        }
                    }
                }
            }
        }

        // 전염 블록 개수에 따라 추가 전염 시키거나 가만히 있거나
        IEnumerator Co_CheckRadiationBlocker()
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;
            if (oldRadiationount <= 0) yield break;
            if (oldRadiationount > currRadiationCount) // 없앴다.
            {
                oldRadiationount = currRadiationCount;
            }
            else // 그대로다.
            {
                if (isOne)
                {
                    isOne = false;
                    yield break;
                }

                foreach (BlockEntity entity in blockEntities)
                {
                    blocker = blockerEntites[entity._X, entity._Y] as Blocker;
                    if (blocker == null) continue;
                    if (blocker._BlockerType != BlockerType.radiation) continue;

                    block = entity as Block;
                    if (block == null) continue;

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
                            if (IsCheckNotInfectionBlock(blockTemp)) continue;
                            ++oldRadiationount;
                            currRadiationCount = oldRadiationount;
                            //Debug.Log("+ " + oldRadiationount);
                            CreateRadiationBlocker(blockTemp._X, blockTemp._Y);
                            yield break;
                        }
                    }
                }
            }
        }

        private void CheckMissionBlock(Block block)
        {
            CheckMissionBlock(block, BlockType.box);
            CheckMissionBlock(block, BlockType.sticky);
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
                    if (blockerEntites[hitBlock._X, hitBlock._Y] != null)
                    {
                        // 블록커 제거
                        ReturnBlocker(blockerEntites[hitBlock._X, hitBlock._Y] as Blocker);
                        ++count;
                        continue;
                    }

                    hitBlock.Hide();
                    booster = hitBlock as Booster;
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
                    StartCoroutine(Co_CheckStickyBlock());
                    StartCoroutine(Co_CheckRadiationBlocker());
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
                blocks = comboBoosters[0].ComboMatch(x, y, ref currStickyBlockCount);
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
                    block = blockEntities[x, y] as Block;
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
        
        public bool UseItem(ItemType type, BlockEntity block)
        {
            SetInfectionCount();

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
                    //StartCoroutine(Co_BlockIconSetting());
                    BlockIconSetting();
                    break;
            }
            
            return true;
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

            if (blockerEntites[block._X, block._Y] != null)
            {
                ReturnBlocker(blockerEntites[block._X, block._Y] as Blocker);
            }
            else
            {
                if (block is Block && (block as Block)._BlockType == BlockType.sticky) --currStickyBlockCount;
                block.Hide();

                var particles = GameMgr.G.gamePools.GetParticles(block);
                if (particles != null) CreateParticle(particles, block._LocalPosition);
            }

            MatchDown();

            yield return new WaitForSeconds(.1f);

            hammer.transform.localScale = Vector2.one;
            hammer.SetActive(false);
        }

        IEnumerator Co_UseMittHorizonItem(int y)
        {
            while (IsMoving(State.move) || IsMoving(State.booster_move)) yield return null;
            isWait = true;

            BlockEntity blockEntity;
            int x = 0;
            while (x < width)
            {
                blockEntity = blockEntities[x, y];

                //미션 체크.
                AddCollectedBlock(blockEntity);

                if (blockerEntites[blockEntity._X, blockEntity._Y] != null)
                {
                    ReturnBlocker(blockerEntites[blockEntity._X, blockEntity._Y] as Blocker);
                }
                else
                {
                    block = blockEntity as Block;
                    if (block != null)
                    {
                        if (block._BlockType == BlockType.sticky) --currStickyBlockCount;
                        block.Hide();

                        var particles = GameMgr.G.gamePools.GetParticles(block);
                        if (particles != null) CreateParticle(particles, block._LocalPosition);
                    }
                    else
                    {
                        BoosterMatches(blockEntity, false);
                    }
                }

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
            BlockEntity blockEntity;
            while (y < height)
            {
                blockEntity = blockEntities[x, y];

                //미션 체크.
                AddCollectedBlock(blockEntity);
                if (blockerEntites[blockEntity._X, blockEntity._Y] != null)
                {
                    ReturnBlocker(blockerEntites[blockEntity._X, blockEntity._Y] as Blocker);
                }
                else
                {
                    block = blockEntity as Block;
                    if (block != null)
                    {
                        if (block._BlockType == BlockType.sticky) --currStickyBlockCount;
                        block.Hide();

                        var particles = GameMgr.G.gamePools.GetParticles(block);
                        if (particles != null) CreateParticle(particles, block._LocalPosition);
                    }
                    else
                    {
                        BoosterMatches(blockEntity, false);
                    }
                }

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

        private void GetMatches(BlockEntity blockEntity, List<BlockEntity> matchedBlocks, List<BlockEntity> matchedBlocker, bool isColorMatch = true)
        {
            if (matchedBlocker == null && blockerEntites[blockEntity._X, blockEntity._Y] != null) return;
            
            int x = blockEntity._X;
            int y = blockEntity._Y;

            var pickBlock = blockEntity as Block;

            // 검사하려는 기준 블럭이 블록커가 씌워져있는지 체크. 블록커가 씌워져있을 경우 블록커 씌워져있고 색이 같은것만 체크한다.
            bool isCurBlocker = blockerEntites[x, y] != null;
            bool isAddBlocker = false;

            var topBlock = new BlockDef(x, y + 1);
            var bottomBlock = new BlockDef(x, y - 1);
            var leftBlock = new BlockDef(x - 1, y);
            var rightBlock = new BlockDef(x + 1, y);
            var surroundingBlocks = new List<BlockDef> { topBlock, bottomBlock, leftBlock, rightBlock };

            var hasMatch = false;
            var hasBlockerMatch = false;

            foreach (var surroundingBlock in surroundingBlocks)
            {
                if (IsValidBlockEntity(surroundingBlock))
                {
                    var tempBlockEntity = blockEntities[surroundingBlock.x, surroundingBlock.y];
                    if (tempBlockEntity != null && tempBlockEntity.gameObject.activeSelf)
                    {
                        if (isColorMatch)
                        {
                            block = tempBlockEntity as Block;
                            if (block != null)
                            {
                                blocker = blockerEntites[block._X, block._Y] as Blocker;
                                if (!isCurBlocker && blocker == null && block._BlockType == pickBlock._BlockType)
                                {
                                    hasMatch = true;
                                }
                                
                                if (matchedBlocker != null && blocker != null)
                                {
                                    isAddBlocker = true;
                                    if (blocker._BlockerType == BlockerType.bubble) isAddBlocker = block._BlockType == pickBlock._BlockType;

                                    if (isAddBlocker && !matchedBlocker.Contains(blocker))
                                    {
                                        matchedBlocker.Add(blocker);
                                        hasBlockerMatch = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            booster = tempBlockEntity as Booster;
                            if (booster != null && IsCombo(booster))
                            {
                                hasMatch = true;
                            }
                        }
                    }
                }
            }

            if (!hasMatch && !hasBlockerMatch)
            {
                return;
            }

            if (hasMatch && !matchedBlocks.Contains(blockEntity))
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
                                block = tempBlockEntity as Block;
                                if (block != null)
                                {
                                    blocker = blockerEntites[block._X, block._Y] as Blocker;
                                    if (!isCurBlocker && blocker == null && block._BlockType == pickBlock._BlockType)
                                    {
                                        GetMatches(tempBlockEntity, matchedBlocks, matchedBlocker, isColorMatch);
                                    }
                                    if (matchedBlocker != null && blocker != null)
                                    {
                                        isAddBlocker = true;
                                        if (blocker._BlockerType == BlockerType.bubble) isAddBlocker = block._BlockType == pickBlock._BlockType;

                                        if (isAddBlocker)
                                        {
                                            switch (blocker._BlockerType)
                                            {
                                                case BlockerType.radiation:
                                                    if (!matchedBlocker.Contains(blocker))
                                                        GetMatches(tempBlockEntity, matchedBlocks, matchedBlocker, isColorMatch);
                                                    break;
                                                default:
                                                    GetMatches(tempBlockEntity, matchedBlocks, matchedBlocker, isColorMatch);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                booster = tempBlockEntity as Booster;
                                if (booster != null && IsCombo(booster))
                                {
                                    GetMatches(tempBlockEntity, matchedBlocks, matchedBlocker, isColorMatch);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private void CheckMissionBlock(Block block, BlockType type)
        {
            var topBlock = new BlockDef(block._X, block._Y + 1);
            var bottomBlock = new BlockDef(block._X, block._Y - 1);
            var leftBlock = new BlockDef(block._X - 1, block._Y);
            var rightBlock = new BlockDef(block._X + 1, block._Y);

            var surroundingBlocks = new List<BlockDef> { topBlock, bottomBlock, leftBlock, rightBlock };
            
            foreach (var temp in surroundingBlocks)
            {
                if (IsValidBlockEntity(temp))
                {
                    Block entity = blockEntities[temp.x, temp.y] as Block;
                    if (entity == null) continue;
                    if (!entity.gameObject.activeSelf) continue;
                    if (entity._BlockType != type) continue;
                    if (type == BlockType.box || type == BlockType.sticky) 
                    {
                        if (type == BlockType.sticky) --currStickyBlockCount;

                        var particles = GameMgr.G.gamePools.GetParticles(entity);
                        if (particles != null) CreateParticle(particles, entity._LocalPosition);
                    }
                    if (blockerEntites[entity._X, entity._Y] != null) 
                    {   // 블록커 미션
                        Blocker blocker = blockerEntites[entity._X, entity._Y] as Blocker;
                        ReturnBlocker(blocker);
                        continue;
                    }
                    
                    entity.Hide();
                }
            }
        }

        private bool MatchDown()
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
                        if (!blockEntity.gameObject.activeSelf) continue;
                        if (IsCheckBlock(blockEntity, BlockType.box) ||
                            IsCheckBlock(blockEntity, BlockType.sticky))
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

                        if (yy == 0 && (IsCheckBlock(blockEntity, BlockType.can) || IsCheckBlock(blockEntity, BlockType.paper)))
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

            if (!GameMgr.G.isGameEnd) BlockIconSetting();
            BlockerSetting();
            return true;
        }

        IEnumerator Co_MoveMission(BlockEntity block)
        {
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            // 이펙트 뿌려주면서 사라진다.
            block.Hide();
            MatchDown();
        }

        Coroutine co_blockIconSet;
        private void BlockIconSetting(bool init = false)
        {
            if (co_blockIconSet != null)
            {
                StopCoroutine(co_blockIconSet);
                co_blockIconSet = null;
            }

            co_blockIconSet = StartCoroutine(Co_BlockIconSetting(init));
        }

        List<BlockEntity> oldMatchedBlocks;
        bool isMatched;
        IEnumerator Co_BlockIconSetting(bool init)
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            oldMatchedBlocks = new List<BlockEntity>();
            isMatched = false;

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
                    GetMatches(block, matchedBlocks, null, true);

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

            if (!isMatched && !init)
            {
                AllMixBlocks();
            }

            co_blockIconSet = null;
        }

        private void BlockerSetting()
        {
            StopCoroutine("Co_BlockerSetting");
            StartCoroutine("Co_BlockerSetting");
        }

        IEnumerator Co_BlockerSetting()
        {
            while (isWait) yield return null;
            while (IsMoving(State.move)) yield return null;
            while (IsMoving(State.booster_move)) yield return null;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if (blockerEntites[x, y] == null) continue;
                    blocker = blockerEntites[x, y] as Blocker;
                    // y - 8 이면 맨 위 기본 76, 90
                    blocker.sprite.height = 76;
                    if (blocker._Y == 8)
                    {
                        blocker.sprite.height = 90;
                        continue;
                    }

                    BlockEntity block = blockEntities[x, y];
                    var topBlock = new BlockDef(block._X, block._Y + 1);
                    if (IsValidBlockEntity(topBlock))
                    {
                        booster = blockEntities[topBlock.x, topBlock.y] as Booster;
                        if (booster == null) continue;
                        if (!booster.gameObject.activeSelf) continue;

                        booster.SetDepth(blocker._BlockDepth + 1);
                        blocker.sprite.height = 90;
                    }
                }
            }
        }

        private void AllMixBlocks()
        {
            List<BlockEntity> tempEntities = MixBlocks();
            for (int i = 0; i < tempEntities.Count; ++i)
            {
                block = blockEntities[tempEntities[i]._X, tempEntities[i]._Y] as Block;
                if (block == null) continue;
                block.MixMove(tempEntities[i]._LocalPosition, tempEntities[i]._X, tempEntities[i]._Y);
            }
        }

        private List<BlockEntity> MixBlocks()
        {
            List<BlockEntity> list = new List<BlockEntity>();
            foreach (BlockEntity temp in blockEntities)
            {
                block = temp as Block;
                if (block == null) continue;
                if (IsCheckNotBlockType(block._BlockType)) continue;
                if (!block.gameObject.activeSelf) continue;
                if (blockerEntites[block._X, block._Y] != null) continue;
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

        private void CreateStickyBlock(int x, int y)
        {
            BlockEntity entity = GameMgr.G.gamePools.GetBlockEntity(new LevelBlockType() { type = BlockType.sticky });
            Assert.IsNotNull(entity);
            
            entity.Show();
            entity._LocalPosition = blockEntities[x, y]._LocalPosition;
            entity.SetDepth(blockEntities[x, y]._BlockDepth);
            blockEntities[x, y] = entity;
            blockEntities[x, y].SetData(x, y);

            Block block = entity as Block;
            block.img.alpha = 0f;
            LeanTween.cancel(block.img.gameObject);
            LeanTween.value(block.img.gameObject, (a) => { block.img.alpha = a; }, block.img.alpha, 1f, 0.2f).setEaseOutCirc();
        }

        private void CreateRadiationBlocker(int x, int y)
        {
            var cover = GameMgr.G.gamePools.GetBlockerEntity(new LevelBlock() { blockerType = BlockerType.radiation });
            Assert.IsNotNull(cover);

            cover.SetDepth(y + 21);
            cover.Show();
            cover.SetData(x, y);
            cover._LocalPosition = blockEntities[x, y]._LocalPosition;
            blockerEntites[x, y] = cover;

            Blocker blocker = cover as Blocker;
            blocker.sprite.alpha = 0f;
            LeanTween.cancel(blocker.sprite.gameObject);
            LeanTween.value(blocker.sprite.gameObject, (a) => { blocker.sprite.alpha = a; }, blocker.sprite.alpha, 1f, 0.2f).setEaseOutCirc();
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

        private void ReturnBlocker(Blocker blocker)
        {
            if (blocker._BlockerType == BlockerType.bubble)
            {
                ReturnBlockerObject(blocker);
            }
            else if (blocker._BlockerType == BlockerType.radiation)
            {
                --currRadiationCount;

                LeanTween.cancel(blocker.sprite.gameObject);
                LeanTween.value(blocker.sprite.gameObject, (a) => { blocker.sprite.alpha = a; }, blocker.sprite.alpha, 0f, 0.2f).setOnComplete(() =>
                {
                    ReturnBlockerObject(blocker);
                });
            }
        }
        
        private void ReturnBlockerObject(Blocker blocker)
        {
            if (blocker == null)
            {
                DebugX.LogError("blocker none delete index x = " + blocker._X + " y = " + blocker._Y);
                return;
            }
            particles = GameMgr.G.gamePools.GetBlockerParticles(blocker);
            if (particles != null) CreateParticle(particles, blocker._LocalPosition);
            blockerEntites[blocker._X, blocker._Y] = null;
            ReturnObject(blocker.gameObject);
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
            var blocker = blockerEntites[x, y] as Blocker;
            if (blocks.Contains(entity)) return;
            if (entity != null)
            {
                if (!entity.gameObject.activeSelf) return;
                var block = entity as Block;
                if (block != null && block._BlockType == BlockType.empty) return;
                if (block != null && block._BlockType == BlockType.can) return;
                if (block != null && block._BlockType == BlockType.paper) return;
                
                if (block != null && block._BlockType == BlockType.sticky) --currStickyBlockCount;

                blocks.Add(entity);
            }
        }

        private void ComboBoostersClear()
        {
            for (int i = 0; i < comboBoosterIndex.Count; ++i)
            {
                int x = (int)comboBoosterIndex[i].x;
                int y = (int)comboBoosterIndex[i].y;
                booster = blockEntities[x, y] as Booster;
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
            block = blockEntity as Block;
            if (block != null && block._BlockType == type) return true;
            return false;
        }

        /// <summary> 감염 안되는 블록 타입 </summary>
        private bool IsCheckNotInfectionBlock(Block block)
        {
            if (block == null) return false;
            if (block._BlockType == BlockType.empty) return true;
            if (block._BlockType == BlockType.sticky) return true;
            if (block._BlockType == BlockType.box) return true;
            if (block._BlockType == BlockType.can) return true;
            if (block._BlockType == BlockType.paper) return true;
            if (blockerEntites[block._X, block._Y] != null) return true;

            return false;
        }

        public bool IsCheckNotBlockType(BlockType type)
        {
            return type == BlockType.empty || type == BlockType.can || type == BlockType.box || type == BlockType.paper || type == BlockType.sticky;
        }

        public bool IsCheckBlockerType(BlockerType type)
        {
            return type == BlockerType.bubble || type == BlockerType.radiation;
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
            block = blockEntity as Block;
            if (block != null)
            {
                if (blockerEntites[block._X, block._Y] != null)
                {   //blocker
                    Blocker blocker = blockEntity as Blocker;
                    if (blocker != null && GameMgr.G._GameState.collectedBlockers.ContainsKey(blocker._BlockerType))
                    {
                        GameMgr.G._GameState.collectedBlockers[blocker._BlockerType] += 1;
                    }
                }
                else if (GameMgr.G._GameState.collectedBlocks.ContainsKey(block._BlockType))
                {
                    GameMgr.G._GameState.collectedBlocks[block._BlockType] += 1;
                }
            }

            GameMgr.G.UpdateGoalUI();
        }
        
        private void AddCollectedBlock(List<BlockEntity> blockEntitys)
        {
            for (int i = 0; i < blockEntitys.Count; ++i)
            {
                int x = blockEntitys[i]._X;
                int y = blockEntitys[i]._Y;
                AddCollectedBlock(blockEntities[x, y]);
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
                StartCoroutine(Co_RainbowAndAnotherBomb(blocks, BoosterType.arrow, true));
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
                booster = block as Booster;
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

                if (!isFinale && blockerEntites[hitBlock._X, hitBlock._Y] != null)
                {   // 블록커 제거
                    ReturnBlocker(blockerEntites[hitBlock._X, hitBlock._Y] as Blocker);
                    ++count;
                    continue;
                }

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
            StartCoroutine(Co_RainbowAndAnotherBomb(boosters, isFinale));
        }

        // 폭탄 터트리기.
        IEnumerator Co_RainbowAndAnotherBomb(List<Booster> boosters, bool isFinale = false)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();
            int count = 0;
            while(count < boosters.Count)
            {
                while (IsMoving(State.move)) yield return null;
                booster = blockEntities[boosters[count]._X, boosters[count]._Y] as Booster;
                if (booster == null)
                {
                    ++count;
                    continue;
                }
                blocks = booster.Match(booster._X, booster._Y, ref currStickyBlockCount);
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