using System.Windows;
using System.Windows.Controls;
using altaik.baseapp.vm;

namespace AltaDock.vm.pane
{
    internal class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle
        {
            get;
            set;
        }

        public Style PanelStyle
        {
            get;
            set;
        }


        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is PanelViewModelBase) return PanelStyle;

            return base.SelectStyle(item, container);
        }
    }
}
