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
        // XmlNode node = doc.SelectSingleNode($"/PublicKeysData/XmlDataForPublicKey[SerialNo='{serialNo}']");

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
    //public static void SaveToXml(PublicKeyData data, string filePath)
    //{
    //    DataContractSerializer serializer = new DataContractSerializer(typeof(PublicKeyData));

    //    if (!File.Exists(filePath))
    //    {
    //        // Create new XML document with root and first entry
    //        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    //        using (XmlWriter writer = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
    //        {
    //            writer.WriteStartDocument();
    //            writer.WriteStartElement("PublicKeysData"); // Root Node
    //            serializer.WriteObject(writer, data);
    //            writer.WriteEndElement();
    //            writer.WriteEndDocument();
    //        }
    //    }
    //    else
    //    {
    //        // Load existing XML, add new entry, and save back
    //        XmlDocument doc = new XmlDocument();
    //        doc.Load(filePath);

    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            using (XmlWriter writer = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true }))
    //            {
    //                serializer.WriteObject(writer, data);
    //            }
    //            ms.Position = 0;

    //            XmlDocument tempDoc = new XmlDocument();
    //            tempDoc.Load(ms);
    //            XmlNode newNode = doc.ImportNode(tempDoc.DocumentElement, true);

    //            doc.DocumentElement.AppendChild(newNode);
    //        }

    //        doc.Save(filePath);
    //    }
    //}
}
