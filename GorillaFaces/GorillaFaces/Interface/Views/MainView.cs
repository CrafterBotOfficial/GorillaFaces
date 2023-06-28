using ComputerInterface;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GorillaFaces.Interface.Views
{
    internal class MainView : ComputerView
    {
        internal class ViewEntry : IComputerModEntry
        {
            public string EntryName => Main.Name;
            public Type EntryViewType => typeof(MainView);
        }

        private const string GrayColorHex = "#ffffff50";

        private UIElementPageHandler<TextItem> pageHandler;
        private UISelectionHandler selectionHandler;

        public override void OnShow(object[] args)
        {
            base.OnShow(args);

            List<TextItem> elements = Main.Instance.Faces.Select(x => new TextItem(x.Value.Package.Name)).ToList();

            pageHandler = new UIElementPageHandler<TextItem>(EKeyboardKey.Left, EKeyboardKey.Right);
            pageHandler.EntriesPerPage = 5;
            pageHandler.SetElements(elements.ToArray());

            selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            selectionHandler.MaxIdx = pageHandler.EntriesPerPage - 1;
            selectionHandler.OnSelected += SelectionHandler_OnSelected;
            selectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");

            DrawPage();
        }

        private void DrawPage()
        {
            StringBuilder stringBuilder = new StringBuilder();

            //// Header
            stringBuilder
                .BeginCenter()
                .MakeBar('=', SCREEN_WIDTH, 0)
                .AppendLines(1)
                .AppendLine(Main.Name)
                .AppendLine($"<color={GrayColorHex}>By Crafterbot</color>")
                .MakeBar('=', SCREEN_WIDTH, 0)
                .EndAlign()
                .AppendLines(2)
                ;

            //// Body
            pageHandler.EnumarateElements((item, idx) => stringBuilder.AppendLine(selectionHandler.GetIndicatedText(idx, item.Text)));

            //// Footer
            stringBuilder.BeginAlign("right");
            pageHandler.AppendFooter(stringBuilder);

            SetText(stringBuilder);
        }

        /* Handle methods */

        private void SelectionHandler_OnSelected(int obj)
        {
            int AbsoluteIndex = pageHandler.GetAbsoluteIndex(obj);
            string Id = Main.Instance.Faces.ElementAt(AbsoluteIndex).Key;
            Main.Instance.EquipFace(Id);
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            base.OnKeyPressed(key);

            if (pageHandler.HandleKeyPress(key) || selectionHandler.HandleKeypress(key))
            {
                DrawPage();
                return;
            }

            if (key == EKeyboardKey.Back)
                ReturnToMainMenu();
        }

        internal class TextItem
        {
            internal string Text;
            internal TextItem(string text)
            {
                Text = text;
            }
        }
    }
}
