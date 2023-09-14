using ComputerInterface;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GorillaFaces.Interface.Views
{
    public class MainView : ComputerView
    {
        private UIElementPageHandler<TextItem> ElementPageHandler;
        private UISelectionHandler SelectionHandler;

        public override void OnShow(object[] args)
        {
            base.OnShow(args);

            TextItem[] items = FaceController.CachedFaces.Select(x => new TextItem() { Text = x.Value.Package.Name }).ToArray();
            Main.Log(items.Length + " to display");

            ElementPageHandler = new UIElementPageHandler<TextItem>(EKeyboardKey.Left, EKeyboardKey.Right);
            ElementPageHandler.EntriesPerPage = 5;
            ElementPageHandler.SetElements(items);

            SelectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            SelectionHandler.OnSelected += selectionHandler_OnSelected;
            SelectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");

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
            ElementPageHandler.EnumarateElements((item, idx) =>
            {
                stringBuilder.AppendLine(SelectionHandler.GetIndicatedText(idx, item.Text));
            });

            // footer
            stringBuilder.BeginAlign("right");
            ElementPageHandler.AppendFooter(stringBuilder);

            SelectionHandler.MaxIdx = ElementPageHandler.ItemsOnScreen - 1;
            SetText(stringBuilder);
        }

        /* Handlers */

        private void selectionHandler_OnSelected(int obj)
        {
            int TrueIndex = ElementPageHandler.GetAbsoluteIndex(obj);
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
            if (SelectionHandler.HandleKeypress(key) || ElementPageHandler.HandleKeyPress(key))
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

        public class Entry : IComputerModEntry
        {
            string IComputerModEntry.EntryName => "GorillaFaces";
            Type IComputerModEntry.EntryViewType => typeof(MainView);
        }
    }
}
