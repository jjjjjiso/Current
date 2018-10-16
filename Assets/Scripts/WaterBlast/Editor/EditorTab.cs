using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using FullSerializer;

namespace WaterBlast.Editor
{
    public class EditorTab
    {
        protected WaterBlastEditor parentEditor;

        public EditorTab(WaterBlastEditor editor)
        {
            parentEditor = editor;
        }

        /// <summary>
        ///이 탭이 유저에 의해 선택되었을 때에 불려갑니다.
        /// <summary>
        public virtual void OnTabSelected()
        {
        }

        /// <summary>
        ///이 탭이 그려 질 때 호출됩니다.
        /// <summary>
        public virtual void Draw()
        {
        }

        /// <summary>
        ///재정렬 가능리스트를 작성하고 초기화하는 유틸리티 메소드.
        /// <summary>
        /// <param name="headerText"> 헤더 텍스트.</param>
        /// <param name="elements">목록에 포함 된 요소 목록.</param>
        /// <param name="currentElement">리스트의 현재 선택된 요소에 대한 참조.</param>
        /// <param name="drawElement">리스트의 요소가 그려지는 경우에 호출하는 콜백.</param>
        /// <param name="selectElement">목록의 요소가 선택 될 때 호출 할 콜백입니다.</param>
        /// <param name="createElement">리스트의 요소가 작성되었을 때에 불려가는 콜백.</param>
        /// <param name="removeElement">목록의 요소가 제거 될 때 호출 할 콜백입니다.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ReorderableList SetupReorderableList<T>(
            string headerText,
            List<T> elements,
            ref T curElement,
            Action<Rect, T> drawElement,
            Action<T> selectElement,
            Action createElement,
            Action<T> removeElement)
        {
            var list = new ReorderableList(elements, typeof(T), true, true, true, true)
            {
                drawHeaderCallback = (Rect r) => { EditorGUI.LabelField(r, headerText); },
                drawElementCallback = (Rect r, int index, bool isActive, bool isFocused) =>
                {
                    var element = elements[index];
                    drawElement(r, element);
                }
            };

            list.onSelectCallback = l =>
            {
                var selectedElement = elements[list.index];
                selectElement(selectedElement);
            };

            if(createElement != null)
            {
                list.onAddDropdownCallback = (buttonRect, l) =>
                {
                    createElement();
                };
            }

            list.onRemoveCallback = l =>
            {
                if (!EditorUtility.DisplayDialog("경고!", "이 항목을 삭제 하시겠습니까?", "예", "아니요")) return;

                var element = elements[l.index];
                removeElement(element);
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };

            return list;
        }

        protected T LoadJsonFile<T>(string path) where T : class
        {
            if (!File.Exists(path)) return null;

            var file = new StreamReader(path);
            var fileContents = file.ReadToEnd();
            var data = fsJsonParser.Parse(fileContents);
            object deserialized = null;
            var serializer = new fsSerializer();
            serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
            file.Close();
            return deserialized as T;
        }

        protected void SaveJsonFile<T>(string path, T data) where T : class
        {
            fsData serializedData;
            var serializer = new fsSerializer();
            serializer.TrySerialize(data, out serializedData).AssertSuccessWithoutWarnings();
            var file = new StreamWriter(path);
            var json = fsJsonPrinter.PrettyJson(serializedData);
            file.WriteLine(json);
            file.Close();
        }
    }
}