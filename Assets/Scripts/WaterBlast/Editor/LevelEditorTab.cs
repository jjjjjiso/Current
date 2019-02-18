using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Common;

namespace WaterBlast.Editor
{
    public class LevelEditorTab : EditorTab
    {
        private int width = -1;
        private int height = -1;

        private enum BrushType
        {
            block, 
            blocker,
        }

        private BrushType curBrushType;
        private BlockType curBlockType;
        private ItemType curItemType;
        private BlockerType curBlockerType;

        private enum BrushMode
        {
            tile,
            horizon,
            vertical,
            fill,
        }

        private BrushMode curBrushMode = BrushMode.tile;

        private readonly Dictionary<string, Texture> tileTextures = new Dictionary<string, Texture>();

        private Level curLevel;

        private ReorderableList goalList;
        private Goal curGoal;

        private ReorderableList availableColorBlocksList;
        private ColorType curColorBlock;

        private Vector2 scrollPos;
        
        public LevelEditorTab(WaterBlastEditor editor) : base(editor)
        {
            var editorImagesPath = new DirectoryInfo(Application.dataPath + "/Resources/Game");
            var fileInfo = editorImagesPath.GetFiles("*.png", SearchOption.TopDirectoryOnly);
            foreach(var file in fileInfo)
            {
                var filename = Path.GetFileNameWithoutExtension(file.Name);
                tileTextures[filename] = Resources.Load("Game/" + filename) as Texture;
            }
        }

        /// <summary>
        /// 이 탭이 그려 질 때 호출 됨.
        /// </summary>
        public override void Draw()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;

            GUILayout.Space(15);

            DrawMenu();

            if(curLevel != null)
            {
                var level = curLevel;
                width = level.width;

                GUILayout.Space(15);

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);

                GUILayout.BeginVertical();
                DrawGeneralSettings();
                GUILayout.Space(15);
                DrawStartItemSettings();
                GUILayout.Space(15);
                DrawInGameItemSettings();
                GUILayout.EndVertical();

                GUILayout.Space(300);

                GUILayout.BeginVertical();
                DrawGoalSettings();
                GUILayout.Space(15);
                DrawAvailableColorBlockSettings();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.Space(15);

                DrawLevelEditor();
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 메뉴 그리기.
        /// </summary>
        private void DrawMenu()
        {
            //가로 줄 정렬
            GUILayout.BeginHorizontal();

            if(GUILayout.Button("New", GUILayout.Width(100), GUILayout.Height(50)))
            {
                curLevel = new Level();
                curGoal = null;
                InitializeNewLevel();
                CreateGoalsList();
                CreateAvailableColorBlocksList();
            }

            if (GUILayout.Button("Open", GUILayout.Width(100), GUILayout.Height(50)))
            {
                var path = EditorUtility.OpenFilePanel("Open level", Application.dataPath + "/Resources/Levels",
                    "json");
                if(!string.IsNullOrEmpty(path))
                {
                    curLevel = LoadJsonFile<Level>(path);
                    CreateGoalsList();
                    CreateAvailableColorBlocksList();
                }
            }

            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(50)))
            {
                SaveLevel(Application.dataPath + "/Resources");
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 일반 설정을 그리기.
        /// </summary>
        private void DrawGeneralSettings()
        {
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("General(일반)", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            EditorGUILayout.HelpBox("The general settings of this level.    (이 레벨의 일반 설정)", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Level number", "The number of this level.    (이 레벨의 수)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.id = EditorGUILayout.IntField(curLevel.id, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.LabelField(new GUIContent("Level limit", "The number of turn limits for this level.    (이 레벨의 턴 제한 횟수)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.limit = EditorGUILayout.IntField(curLevel.limit, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Star 1 score", "The score needed to reach the first star.    (첫 번째 별에 도달하는 데 필요한 점수)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.score1 = EditorGUILayout.IntField(curLevel.score1, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Star 2 score", "The score needed to reach the second star.   (두 번째 별에 도달하는 데 필요한 점수)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.score2 = EditorGUILayout.IntField(curLevel.score2, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Star 3 score", "The score needed to reach the third star.    (세 번째 별에 도달하는 데 필요한 점수)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.score3 = EditorGUILayout.IntField(curLevel.score3, GUILayout.Width(70));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 게임 시작 시 아이템 설정 그리기.
        /// </summary>
        private void DrawStartItemSettings()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110;

            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Game Start item", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            EditorGUILayout.HelpBox(
                "Items you want to take when starting this game level.   (이 레벨의 게임 시작 시에 가지고 갈 아이템 설정)",
                MessageType.Info);
            GUILayout.EndHorizontal();

            foreach (var item in Enum.GetValues(typeof(BoosterType)))
            {
                if ((BoosterType)item == BoosterType.none) continue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(StringUtils.DisplayCamelCaseString(item.ToString()));
                var availableBoosters = curLevel.availableStartItem;
                availableBoosters[(BoosterType)item] =
                    EditorGUILayout.Toggle(availableBoosters[(BoosterType)item], GUILayout.Width(30));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        /// <summary>
        /// 게임 내 아이템 설정 그리기.
        /// </summary>
        private void DrawInGameItemSettings()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 110;

            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("In-game item", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            EditorGUILayout.HelpBox(
                "The in-game item settings of this level.   (이 레벨의 게임 내 아이템 설정)",
                MessageType.Info);
            GUILayout.EndHorizontal();

            foreach (var item in Enum.GetValues(typeof(ItemType)))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(StringUtils.DisplayCamelCaseString(item.ToString()));
                var availableBoosters = curLevel.availableInGameItem;
                availableBoosters[(ItemType)item] =
                    EditorGUILayout.Toggle(availableBoosters[(ItemType)item], GUILayout.Width(30));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        /// <summary>
        /// 목표 설정 그리기.
        /// </summary>
        private void DrawGoalSettings()
        {
            EditorGUILayout.LabelField("Goals", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(500));
            EditorGUILayout.HelpBox(
                "This list defines the goals needed to be achieved by the player in order to complete this level." +
                "   (이 목록은 이 레벨을 완료하기 위해 플레이어가 달성해야하는 목표를 정의합니다.)",
                MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(350));
            if (goalList != null)
            {
                goalList.DoLayoutList();
            }
            GUILayout.EndVertical();

            if (curGoal != null)
            {
                DrawGoal(curGoal);
            }

            GUILayout.EndHorizontal();
            
            EditorGUIUtility.labelWidth = 165;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Award boosters(보너스 상품)",
                    "Enable this if you want boosters equal to the number of remaining moves to be awarded to the player at the end of the game."
                    + " (부스터가 게임이 끝날 때 플레이어에게 주어질 남은 동작 수와 같게 하려면 이 기능을 사용합니다.)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.awardBoostersWithRemainingMoves =
                EditorGUILayout.Toggle(curLevel.awardBoostersWithRemainingMoves);
            GUILayout.EndHorizontal();

            if (curLevel.awardBoostersWithRemainingMoves)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Booster", "The type of booster to award."),
                    GUILayout.Width(EditorGUIUtility.labelWidth));
                curLevel.awardedBoosterType =
                    (BoosterType)EditorGUILayout.EnumPopup(curLevel.awardedBoosterType, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
            EditorGUIUtility.labelWidth = 90;
        }

        /// <summary>
        /// 사용 가능한 색상 블록 설정 그리기.
        /// </summary>
        private void DrawAvailableColorBlockSettings()
        {
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Available color blocks(사용 가능한 색상 블록)", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(500));
            EditorGUILayout.HelpBox(
                "This list defines the available color blocks when a new random color block is created."
                + " (이 목록은 새로운 임의의 색상 블록을 만들 때 사용할 수있는 색상 블록을 정의합니다.)",
                MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(350));
            if (availableColorBlocksList != null)
            {
                availableColorBlocksList.DoLayoutList();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 200;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Collectable chance   (수집 할 기회)",
                    "The random chance of a collectable block to be created."
                    + " (수집 가능한 블록이 임의로 생성 될 확률)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.collectableChance = EditorGUILayout.IntField(curLevel.collectableChance, GUILayout.Width(30));
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 90;

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 레벨 편집기 그리기.
        /// </summary>
        private void DrawLevelEditor()
        {
            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            EditorGUILayout.HelpBox(
                "The layout settings of this level. (이 레벨의 레이아웃 설정)",
                MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Width", "The width of this level. (이 레벨의 너비)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.width = EditorGUILayout.IntField(curLevel.width, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            height = curLevel.height;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Height", "The height of this level. (이 레벨의 높이)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curLevel.height = EditorGUILayout.IntField(curLevel.height, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Brush type", "The current type of brush. (현재 브러시 유형)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curBrushType = (BrushType)EditorGUILayout.EnumPopup(curBrushType, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            if (curBrushType == BrushType.block)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Block", "The current type of block.  (블록의 현재 유형)"),
                    GUILayout.Width(EditorGUIUtility.labelWidth));
                curBlockType = (BlockType)EditorGUILayout.EnumPopup(curBlockType, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
            else if (curBrushType == BrushType.blocker)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Blocker", "The current type of blocker.  (현재 차단기 유형)"),
                    GUILayout.Width(EditorGUIUtility.labelWidth));
                curBlockerType =
                    (BlockerType)EditorGUILayout.EnumPopup(curBlockerType, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Brush mode", "The current brush mode.    (현재 브러시 모드)"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            curBrushMode = (BrushMode)EditorGUILayout.EnumPopup(curBrushMode, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            if(width != curLevel.width || height != curLevel.height)
            {
                curLevel.blocks = new List<LevelBlock>(curLevel.width * curLevel.height);
                for (var i = 0; i < curLevel.width; i++)
                {
                    for (var j = 0; j < curLevel.height; j++)
                    {
                        curLevel.blocks.Add(new LevelBlockType() { type = BlockType.random });
                    }
                }
            }

            //for (var i = 0; i < height; i++)
            //{
            //    GUILayout.BeginHorizontal();
            //    for (var j = 0; j < width; j++)
            //    {
            //        var tileIndex = (width * i) + j;
            //        CreateButton(tileIndex);
            //    }
            //    GUILayout.EndHorizontal();
            //}

            for (var i = curLevel.height - 1; i >= 0; --i)
            {
                GUILayout.BeginHorizontal();
                for (var j = 0; j < curLevel.width; j++)
                {
                    var tileIndex = (curLevel.width * j) + i;
                    CreateButton(tileIndex);
                }
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 새로 만든 레벨 초기화
        /// </summary>
        private void InitializeNewLevel()
        {
            foreach (var type in Enum.GetValues(typeof(ColorType)))
            {
                curLevel.availableColors.Add((ColorType)type);
            }

            foreach (var type in Enum.GetValues(typeof(BoosterType)))
            {
                if ((BoosterType)type == BoosterType.none) continue;
                curLevel.availableStartItem.Add((BoosterType)type, true);
            }

            foreach (var type in Enum.GetValues(typeof(ItemType)))
            {
                curLevel.availableInGameItem.Add((ItemType)type, true);
            }
        }

        /// <summary>
        /// 이 레벨의 목표 목록을 작성함.
        /// </summary>
        private void CreateGoalsList()
        {
            goalList = SetupReorderableList("Goals", curLevel.goals, ref curGoal, (r, x) =>
            {
                EditorGUI.LabelField(new Rect(r.x, r.y, 200, EditorGUIUtility.singleLineHeight),
                    x.ToString());
            },
            (x) =>
            {
                curGoal = x;
            },
            () =>
            {
                var menu = new GenericMenu();
                var goalTypes = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Goal));
                foreach(var type in goalTypes)
                {
                    menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.Name)), false,
                        CreateGoalCallback, type);
                }

                menu.ShowAsContext();
            },
            (x) =>
            {
                curGoal = null;
            });
        }

        /// <summary>
        /// 새로운 목표가 작성되면 콜백합니다.
        /// </summary>
        /// <param name="obj">만들 객체의 유형.</param>
        private void CreateGoalCallback(object obj)
        {
            var goal = Activator.CreateInstance((Type)obj) as Goal;
            curLevel.goals.Add(goal);
        }

        /// <summary>
        /// 이 레벨에서 사용 가능한 색상 블록 목록을 작성함.
        /// </summary>
        private void CreateAvailableColorBlocksList()
        {
            availableColorBlocksList = SetupReorderableList("Color blocks", curLevel.availableColors, ref curColorBlock, (rect, x) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                    x.ToString());
            },
                (x) =>
                {
                    curColorBlock = x;
                },
                () =>
                {
                    var menu = new GenericMenu();
                    foreach (var type in Enum.GetValues(typeof(ColorType)))
                    {
                        menu.AddItem(new GUIContent(StringUtils.DisplayCamelCaseString(type.ToString())), false,
                            CreateColorBlockTypeCallback, type);
                    }
                    menu.ShowAsContext();
                },
                (x) =>
                {
                    curColorBlock = ColorType.red;
                });
            availableColorBlocksList.onRemoveCallback = l =>
            {
                if (curLevel.availableColors.Count == 1)
                {
                    EditorUtility.DisplayDialog("Warning", "You need at least one color block type. (최소한 하나의 색상 블록 유형이 필요합니다.)", "Ok");
                }
                else
                {
                    if (!EditorUtility.DisplayDialog("Warning!",
                        "Are you sure you want to delete this item? (이 항목을 삭제 하시겠습니까 ?)", "Yes", "No"))
                    {
                        return;
                    }
                    curColorBlock = ColorType.red;
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            };
        }

        /// <summary>
        /// 새로운 색상 블록 유형이 생성 될 때 호출 할 콜백.
        /// </summary>
        /// <param name="obj">만들 객체의 유형.</param>
        private void CreateColorBlockTypeCallback(object obj)
        {
            var color = (ColorType)obj;
            if (curLevel.availableColors.Contains(color))
            {
                EditorUtility.DisplayDialog("Warning", "This color block type is already present in the list.   (이 색상 블록 유형이 이미 목록에 있습니다.)", "Ok");
            }
            else
            {
                curLevel.availableColors.Add(color);
            }
        }

        /// <summary>
        /// 새로운 타일 버튼을 작성합니다.
        /// </summary>
        /// <param name="tileIndex"></param>
        private void CreateButton(int tileIndex)
        {
            var tileTypeName = string.Empty;
            if (curLevel.blocks[tileIndex] is LevelBlockType)
            {
                var blockTile = (LevelBlockType)curLevel.blocks[tileIndex];
                tileTypeName = blockTile.type.ToString();
            }
            else if (curLevel.blocks[tileIndex] is LevelBoosterType)
            {
                var boosterTile = (LevelBoosterType)curLevel.blocks[tileIndex];
                tileTypeName = boosterTile.type.ToString();
            }
            if (curLevel.blocks[tileIndex].blockerType == BlockerType.ice)
            {
                tileTypeName += "Ice";
            }
            if (tileTextures.ContainsKey(tileTypeName))
            {
                if (GUILayout.Button(tileTextures[tileTypeName], GUILayout.Width(60), GUILayout.Height(60)))
                {
                    DrawTile(tileIndex);
                }
            }
            else
            {
                if (GUILayout.Button("", GUILayout.Width(60), GUILayout.Height(60)))
                {
                    DrawTile(tileIndex);
                }
            }
        }

        /// <summary>
        /// 지정된 인덱스에 타일을 그립니다.
        /// </summary>
        /// <param name="tileIndex"></param>
        private void DrawTile(int tileIndex)
        {
            var x = tileIndex / width;
            var y = tileIndex % height;
            if (curBrushType == BrushType.block)
            {
                switch (curBrushMode)
                {
                    case BrushMode.tile:
                        curLevel.blocks[tileIndex] = new LevelBlockType { type = curBlockType };
                        break;

                    case BrushMode.horizon:
                        for (var i = 0; i < width; i++)
                        {
                            var idx = y + (i * width);
                            curLevel.blocks[idx] = new LevelBlockType { type = curBlockType };
                        }
                        break;

                    case BrushMode.vertical:
                        for (var j = 0; j < height; j++)
                        {
                            var idx = j + (x * height);
                            curLevel.blocks[idx] = new LevelBlockType { type = curBlockType };
                        }
                        break;

                    case BrushMode.fill:
                        for (var i = 0; i < width; i++)
                        {
                            for (var j = 0; j < height; j++)
                            {
                                var idx = j + (i * height);
                                curLevel.blocks[idx] = new LevelBlockType { type = curBlockType };
                            }
                        }
                        break;
                }
            }
            else if (curBrushType == BrushType.blocker)
            {
                switch (curBrushMode)
                {
                    case BrushMode.tile:
                        curLevel.blocks[tileIndex].blockerType = curBlockerType;
                        break;

                    case BrushMode.horizon:
                        for (var i = 0; i < width; i++)
                        {
                            var idx = i + (y * width);
                            curLevel.blocks[idx].blockerType = curBlockerType;
                        }
                        break;

                    case BrushMode.vertical:
                        for (var j = 0; j < height; j++)
                        {
                            var idx = x + (j * width);
                            curLevel.blocks[idx].blockerType = curBlockerType;
                        }
                        break;

                    case BrushMode.fill:
                        //for (var j = 0; j < height; j++)
                        //{
                        //    for (var i = 0; i < width; i++)
                        //    {
                        //        var idx = i + (j * width);
                        //        curLevel.blocks[idx].blockerType = curBlockerType;
                        //    }
                        //}
                        for (var i = 0; i < width; i++)
                        {
                            for (var j = 0; j < height; j++)
                            {
                                var idx = j + (i * height);
                                curLevel.blocks[idx].blockerType = curBlockerType;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 지정된 목표 항목을 그립니다.
        /// </summary>
        /// <param name="goal">그릴 목표 항목.</param>
        private void DrawGoal(Goal goal)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;

            goal.Draw();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        /// <summary>
        /// 현재 레벨을 지정된 경로에 저장합니다.
        /// </summary>
        /// <param name="path"></param>
        public void SaveLevel(string path)
        {
#if UNITY_EDITOR
            if (curLevel == null) return;
            SaveJsonFile(path + "/Levels/" + curLevel.id + ".json", curLevel);
            AssetDatabase.Refresh();
#endif
        }
    }
}