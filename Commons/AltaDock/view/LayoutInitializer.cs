using altaik.baseapp.vm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace AltaDock.view
{
    class LayoutInitializer : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            LayoutAnchorablePane destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null && destinationContainer.FindParent<LayoutFloatingWindow>() != null) return false;

            var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
            if (toolsPane != null) {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;

        }


        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }


        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            LayoutDocumentPane destPane = destinationContainer as LayoutDocumentPane;
            if (destinationContainer != null && destinationContainer.FindParent<LayoutFloatingWindow>() != null) return false;

            if(((PanelViewModelBase)anchorableToShow.Content).DockLocation == altaik.baseapp.helper.PanelDockLocationEnum.DetailsDocument) {
                ((LayoutPanel)layout.Children.First()).Children.Add(new LayoutDocumentPane(anchorableToShow));
                return true;
            }

            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {

        }
    
    }
}
