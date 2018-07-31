using AltaBO.archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveApp.Services {
    public static class DocumentCashService {
        private static Dictionary<Node,List<Document>> store;



        public static void Save(Node savedNode, List<Document> documents) {
            if(store == null) store = new Dictionary<Node, List<Document>>();

            if(store.Keys.Contains(savedNode)) store.Remove(savedNode);
            store.Add(savedNode, documents);
        }


        public static List<Document> Restore(Node restoredNode) {
            if(store == null) return null;
            if(store.Keys.Contains(restoredNode)) {
                List<Document> triedToGet;
                bool tryGet = store.TryGetValue(restoredNode, out triedToGet);
                if(tryGet) return triedToGet;
                else return new List<Document>();
            }
            return null;
        }



    }
}
