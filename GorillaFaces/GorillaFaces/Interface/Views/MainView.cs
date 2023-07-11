using ComputerInterface;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GorillaFaces.Interface.Views
{
    internal class MainView : ComputerView
    {
        private UIElementPageHandler<TextItem> _elementPageHandler;
        private UISelectionHandler _selectionHandler;

        public override void OnShow(object[] args)
        {
            base.OnShow(args);

            TextItem[] items = FaceController.CachedFaces.Select(x => new TextItem() { Text = x.Value.Package.Name }).ToArray();
            Main.Log(items.Length + " to display");

            _elementPageHandler = new UIElementPageHandler<TextItem>(EKeyboardKey.Left, EKeyboardKey.Right);
            _elementPageHandler.EntriesPerPage = 5;
            _elementPageHandler.SetElements(items);

            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            _selectionHandler.OnSelected += selectionHandler_OnSelected;
            _selectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");

            DrawPage();
        }

        private void DrawPage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .BeginCenter()
                .MakeBar('=', SCREEN_WIDTH, 0)
                .Append("\nGorillaFaces\nBy Crafterbot\n")
                .MakeBar('=', SCREEN_WIDTH, 0)
                .AppendLines(2)
                ;

            stringBuilder.BeginAlign("left");
            _elementPageHandler.EnumarateElements((item, idx) =>
            {
                stringBuilder.AppendLine(_selectionHandler.GetIndicatedText(idx, item.Text));
            });

            // footer
            stringBuilder.BeginAlign("right");
            _elementPageHandler.AppendFooter(stringBuilder);

            _selectionHandler.MaxIdx = _elementPageHandler.ItemsOnScreen - 1;
            SetText(stringBuilder);
        }

        /* Handlers */

        private void selectionHandler_OnSelected(int obj)
        {
            int TrueIndex = _elementPageHandler.GetAbsoluteIndex(obj);
            if (TrueIndex > FaceController.CachedFaces.Count - 1)
                return;

            string Id = FaceController.CachedFaces.ElementAt(TrueIndex).Key;

            Main.Log("Selected: " + Id);
            Configuration.SelectedFace.Value = Id;
            FaceController.EquipFace(GorillaTagger.Instance.offlineVRRig, Id);
            FaceController.UpdateCustomProperties();
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_selectionHandler.HandleKeypress(key) || _elementPageHandler.HandleKeyPress(key))
            {
                DrawPage();
                return;
            }

            ReturnToMainMenu();
        }

        /* Models & Entry point */

        class TextItem
        {
            internal string Text { get; set; }
        }

        internal class Entry : IComputerModEntry
        {
            string IComputerModEntry.EntryName => "GorillaFaces";
            Type IComputerModEntry.EntryViewType => typeof(MainView);
        }
    }
}
