using AltaArchiveApp.Services;
using AltaBO.archive;
using System.Collections.Generic;

delegate List<Node> DelegateChildParentNodes(Node node);
delegate List<Node> DelegateRootNodes();

namespace AltaArchiveUI.service {
    public static class PresentTreeService {
        // Init delegates
        /*private static DelegateChildParentNodes readArchiveChildNodes = new DelegateChildParentNodes(ArchiveService.ReadChildNodes);
        private static DelegateChildParentNodes readExchangeChildNodes = new DelegateChildParentNodes(ExchangeService.ReadChildNodes);
        private static DelegateChildParentNodes readArchiveParentNodes = new DelegateChildParentNodes(ArchiveService.ReadParentNodes);
        private static DelegateChildParentNodes readExchangeParentNodes = new DelegateChildParentNodes(ExchangeService.ReadParentNodes);

        private static DelegateRootNodes readArchiveRoot = new DelegateRootNodes(ArchiveService.ReadRoot);
        private static DelegateRootNodes readExchangeRoot = new DelegateRootNodes(ExchangeService.ReadRoot);

        public static List<Node> SwitchNode(int type, PresentationEnum present, Node node = null) {
            switch(present) {
                case PresentationEnum.Archive: return type == 1 ? readArchiveChildNodes(node) : type == 2 ? readArchiveParentNodes(node) : readArchiveRoot();
                case PresentationEnum.Exchange: return type == 1 ? readExchangeChildNodes(node) : type == 2 ? readExchangeParentNodes(node) : readExchangeRoot();
                default: return null;
            }
        }


        public static List<Node> ReadChildNodes(PresentationEnum present, Node node) {
            return SwitchNode(1, present, node);
        }


        public static List<Node> ReadParentNodes(PresentationEnum present, Node node) {
            return SwitchNode(2, present, node);
        }


        public static List<Node> ReadRoot(PresentationEnum present) {
            return SwitchNode(3, present);
        }


        public static int LastLevel(PresentationEnum present) {
            switch(present) {
                case PresentationEnum.Archive: return ArchiveService.LastLevel();
                case PresentationEnum.Exchange: return ExchangeService.LastLevel();
                default: return -1;
            }
        }


        public static string GetAddress(PresentationEnum present, Node node) {
            string address = "";
            foreach(Node parent in ReadParentNodes(present, node)) {
                address += parent.Name + "\\";
            }

            return address;
        }


        public static int CreateNode(PresentationEnum present, Node node, Node newNode, bool isChild) {
            switch(present) {
                case PresentationEnum.Archive: return ArchiveService.CreateNode(node, newNode, isChild);
                case PresentationEnum.Exchange: return ExchangeService.CreateNode(node, newNode, isChild);
                default: return -1;
            }
        }


        public static int DeleteNode(PresentationEnum present, Node node) {
            switch(present) {
                case PresentationEnum.Archive: return ArchiveService.DeleteNode(node);
                case PresentationEnum.Exchange: return ExchangeService.DeleteNode(node);
                default: return -1;
            }
        }


        public static Node ReadNode(PresentationEnum present, int nodeId) {
            switch(present) {
                case PresentationEnum.Archive: return ArchiveService.ReadNode(nodeId);
                case PresentationEnum.Exchange: return ExchangeService.ReadNode(nodeId);
                default: return null;
            }
        }


        public static int UpdateNode(PresentationEnum present, Node node) {
            switch(present) {
                case PresentationEnum.Archive: return ArchiveService.UpdateNode(node);
                case PresentationEnum.Exchange: return ExchangeService.UpdateNode(node);
                default: return -1;
            }
        }*/
    }
}