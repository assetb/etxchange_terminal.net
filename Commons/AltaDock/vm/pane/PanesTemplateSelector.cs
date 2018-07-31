using System.Windows.Controls;
using System.Windows;
using altaik.baseapp.vm;

namespace AltaDock.vm.pane
{
    internal class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PanelViewTemplate
        {
            get;
            set;
        }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PanelViewModelBase) return PanelViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
