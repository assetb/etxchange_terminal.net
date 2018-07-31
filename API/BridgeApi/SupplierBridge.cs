using System;
using System.Net;

namespace BridgeApp
{
    public static class SupplierBridge
    {
        public static void OpenSupplierCabinet(int supplierId) {
            string url = "http://88.204.230.204:81/AuthorizationCenter";
            OpenSupplierCabinet(url, supplierId);
        }

        public static void OpenSupplierCabinet(string urlSupplierCabinet, int supplierId) {
            System.Diagnostics.Process.Start(String.Format("{0}/api/login/broker/supplier/{1}?redirectUrl={2}", urlSupplierCabinet, supplierId, "/SupplierCabinet"));
        }
    }
}
