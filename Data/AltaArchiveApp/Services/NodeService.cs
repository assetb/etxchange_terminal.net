using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveApp.Services {
    public static class NodeService {
        public static int GetLastLevel(int presentId) {
            return DataManagerService.Instanse().GetLastLevel(presentId);
        }


        public static List<string> ReadRoot(int presentId) {
            return DataManagerService.Instanse().ReadRoot(presentId);
        }


        public static List<string> ReadNodes(int presentId, int reqLevel, string parentValues) {
            return DataManagerService.Instanse().ReadNodes(presentId, reqLevel, parentValues);
        }
    }
}
