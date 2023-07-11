using ComputerInterface;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using System;
using System.Linq;
using System.Text;

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

            _elementPageHandler = new UIElementPageHandler<TextItem>(EKeyboardKey.Left, EKeyboardKey.Right);
            _elementPageHandler.SetElements(items);
            _elementPageHandler.EntriesPerPage = 5;

            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            _selectionHandler.OnSelected += selectionHandler_OnSelected;
            _selectionHandler.MaxIdx = _elementPageHandler.EntriesPerPage;
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

            stringBuilder.BeginAlign("left");
            _elementPageHandler.AppendFooter(stringBuilder);

            SetText(stringBuilder);
        }

        /* Handlers */

        private void selectionHandler_OnSelected(int obj)
        {
            int TrueIndex = _elementPageHandler.GetAbsoluteIndex(obj);
            if (TrueIndex > FaceController.CachedFaces.Count - 1)
                return;

            string Id = FaceController.CachedFaces.ElementAt(TrueIndex).Key;

            Main.Log("Selected: " + FaceController.CachedFaces.ElementAt(TrueIndex).Key);
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
