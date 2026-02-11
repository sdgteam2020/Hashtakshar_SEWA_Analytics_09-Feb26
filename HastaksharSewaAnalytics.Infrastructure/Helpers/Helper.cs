using System.Xml;

namespace HastaksharSewaAnalytics.Infrastructure.Helpers;

internal static class Helper
{
    public static bool CheckSerialNoExists(string filePath, string serialNo)
    {
        if (!File.Exists(filePath))
            return false;

        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("ns", "http://schemas.datacontract.org/2004/07/DGIS_App_API.Helpers");

        XmlNode node = doc.SelectSingleNode($"/PublicKeysData/ns:XmlDataForPublicKey[ns:SerialNo='{serialNo}']", nsmgr);
        if (node != null)
        {
            return true;

        }
        else
        {
            return false;
        }
    }
}
