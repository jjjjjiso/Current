using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

using WaterBlast.System;

namespace WaterBlast.Game.Common
{
    public abstract class Goal
    {
        public abstract bool IsComplete(GameState state);

#if UNITY_EDITOR

        public abstract void Draw();

#endif
    }

    /*/// <summary>
    /// 도달 범위 골
    /// </summary>
    public class ReachScoreGoal : Goal
    {
        public int score;

        public override bool IsComplete(GameState state)
        {
            return state.score >= score;
        }

#if UNITY_EDITOR

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Score");
            score = EditorGUILayout.IntField(score, GUILayout.Width(70));
            GUILayout.EndHorizontal();
        }

#endif

        public override string ToString()
        {
            return "Reach " + score + " points";
        }
    }*/

    public class CollectBlockGoal : Goal
    {
        public BlockType blockType;
        public int amount;
        public int amount_plus = 0;

        public override bool IsComplete(GameState state)
        {
            return state.collectedBlocks[blockType] >= (amount + amount_plus);
        }

#if UNITY_EDITOR

        public override void Draw()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Type");
            blockType = (BlockType)EditorGUILayout.EnumPopup(blockType, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Amount");
            amount = EditorGUILayout.IntField(amount, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

#endif

        public override string ToString()
        {
            return "Collect " + amount + " " + blockType;
        }
    }

    public class CollectBlockerGoal : Goal
    {
        public BlockerType blockerType;
        public int amount;
        public int amount_plus = 0;

        public override bool IsComplete(GameState state)
        {
            return state.collectedBlockers[blockerType] >= (amount + amount_plus);
        }

#if UNITY_EDITOR

        public override void Draw()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Type");
            blockerType = (BlockerType)EditorGUILayout.EnumPopup(blockerType, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Amount");
            amount = EditorGUILayout.IntField(amount, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

#endif

        public override string ToString()
        {
            return "Collect " + amount + " " + blockerType;
        }
    }

    public class CollectBoosterGoal : Goal
    {
        public BoosterType boosterType;
        public ColorType colorType;
        public int amount;

        public override bool IsComplete(GameState state)
        {
            if (boosterType != BoosterType.rainbow)
                return state.collectedBoosters[boosterType] >= amount;

            return state.collectedRainbows[colorType] >= amount;
        }

#if UNITY_EDITOR

        public override void Draw()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Type");
            boosterType = (BoosterType)EditorGUILayout.EnumPopup(boosterType, GUILayout.Width(100));
            GUILayout.EndHorizontal();
            
            if (boosterType == BoosterType.rainbow)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("ColorType");
                colorType = (ColorType)EditorGUILayout.EnumPopup(colorType, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Amount");
            amount = EditorGUILayout.IntField(amount, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

#endif

        public override string ToString()
        {
            return "Collect " + amount + " " + boosterType;
        }
    }
}