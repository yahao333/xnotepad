using System.Windows;
using System.Windows.Controls;

namespace NotesApp.Views
{
    public class CardTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoteTemplate { get; set; }
        public DataTemplate ColorTemplate { get; set; }
        public DataTemplate TaskTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cardItem = item as CardItem;
            switch (cardItem.Type)
            {
                case CardType.Note:
                    return NoteTemplate;
                case CardType.Color:
                    return ColorTemplate;
                case CardType.Task:
                    return TaskTemplate;
                default:
                    return base.SelectTemplate(item, container);
            }
        }
    }
}
