using AltaArchiveUI.vm;
using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveUI.service
{
    public static class TemporaryDataService
    {
        private static NodeVM NodeVM { get; set; }
        private static PresentTreeVM PresentTreeVM { get; set; }


        public static void SetCurrentNodeVM(NodeVM nodeVM)
        {
            NodeVM = nodeVM;
        }


        public static NodeVM GetCurrentNodeVM()
        {
            return NodeVM != null ? NodeVM : null;
        }


        public static void SetCurrentPresentTreeVM(PresentTreeVM presentTreeVM)
        {
            PresentTreeVM = presentTreeVM;
        }


        public static PresentTreeVM GetCurrentPresentTreeVM()
        {
            return PresentTreeVM != null ? PresentTreeVM : null;
        }
    }
}
