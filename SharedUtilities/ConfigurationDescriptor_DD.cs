using System;
using System.Collections.Generic;

//
//  2022-05-23  Mark Stega
//              Created
//

namespace ICEBG.SharedUtilities;

public class ConfigurationDescriptor_DD
{
    #region enums

    public enum eVersion
    {
        unknown = 0,
        v001 = 1,
        v002 = 2,
        v003 = 3,
        v004 = 4,
        v005 = 5,
        v006 = 6,
    };

    public enum eDispositionStatus
    {
        created = 1,
        inProgress = 2,
        successfulSend = 3,
        failed = 4,
        failedAcknowledged = 5
    };

    #endregion

    #region members

    // Warning - These are used in the desrialization/deserialization process - Understand that before
    // making any changes as you are likely to invalidate stored data

    static private string kVersion = "Version";

    private static string kUI_ApplicationReleaseDate = "kUI_ApplicationReleaseDate";
    private static string kUI_ApplicationVersion = "kUI_ApplicationVersion";
    private static string kUI_AutoLogoffInterval = "kUI_AutoLogoffInterval";
    private static string kGRPC_Endpoint = "kGRPC_Endpoint";

    #endregion

    #region properties

    public eVersion pVersion { get; set; }
    public static string pUI_ApplicationReleaseDate { get; private set; }
    public static string pUI_ApplicationVersion { get; private set; }
    public static int pUI_AutoLogoffInterval { get; set; }
    public static string pGRPC_Endpoint { get; private set; }


    #endregion

    #region ctor

    public ConfigurationDescriptor_DD()
    {
        pVersion = eVersion.v001;

        // UI
        pUI_ApplicationReleaseDate = "05/06/2022";
        pUI_ApplicationVersion = "1.0.0";
        pUI_AutoLogoffInterval = 0;

        // GRPC
        pGRPC_Endpoint = "https://T570:44350/";

    }

    #endregion

    #region Serialization

    public string Serialize()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        dictionary.Add(kVersion, ((int)pVersion).ToString());

        //dictionary.Add(kCommunicationDescriptor, CommunicationDescriptor);
        //dictionary.Add(kCommunicationID, CommunicationID);
        //dictionary.Add(kCommunicationType, CommunicationType);
        //dictionary.Add(kCreationTime, CreationTime.ToString("MM/dd/yyyy h:mm:ss tt"));
        //dictionary.Add(kCreationUserID, CreationUserID);
        //dictionary.Add(kCreationUserName, CreationUserName);
        //dictionary.Add(kDestinationName, DestinationName);
        //dictionary.Add(kDestinationAddress, DestinationAddress);
        //dictionary.Add(kDestinationIsEMail, DestinationIsEMail.ToString());
        //dictionary.Add(kDispositionDescriptor, DispositionDescriptor);
        //dictionary.Add(kDispositionStatus, DispositionStatus.ToString());
        //dictionary.Add(kDispositionTime, DispositionTime.ToString("MM/dd/yyyy h:mm:ss tt"));
        //dictionary.Add(kFileIsBody, FileIsBody.ToString());
        //dictionary.Add(kFileIdentity, FileIdentity);
        //dictionary.Add(kFileName, FileName);
        //dictionary.Add(kGUIDdescriptor, GUIDdescriptor.ToString());
        //dictionary.Add(kIsPASS, IsPASS.ToString());
        //dictionary.Add(kOutgoingAdressHint, OutgoingAddressHint);
        //dictionary.Add(kURL, URL);

        return SimpleSerialization.SerializeDictionary(dictionary);
    }

    public ConfigurationDescriptor_DD Deserialize(string p_SerialValue)
    {
        Dictionary<string, string> dictionary = SimpleSerialization.DeserializeDictionary(p_SerialValue);

        pVersion = (eVersion)Convert.ToInt32(dictionary[kVersion]);

        switch (pVersion)
        {
            case eVersion.v001:
                GetFieldsV001(dictionary);

                break;

            default:
                throw new Exception("Error deserializing Configuration_DD");
        }

        return this;
    }


    // ******** V001 ********
    private void GetFieldsV001(Dictionary<string, string> dictionary)
    {
        //CommunicationID = dictionary[kCommunicationID];
        //CreationTime = Convert.ToDateTime(dictionary[kCreationTime]);
        //CreationUserID = dictionary[kCreationUserID];
        //CreationUserName = dictionary[kCreationUserName];
        //DestinationName = dictionary[kDestinationName];
        //DestinationAddress = dictionary[kDestinationAddress];
        //DestinationIsEMail = Convert.ToBoolean(dictionary[kDestinationIsEMail]);
        //DispositionDescriptor = dictionary[kDispositionDescriptor];
        //DispositionStatus = Convert.ToInt32(dictionary[kDispositionStatus]);
        //DispositionTime = Convert.ToDateTime(dictionary[kDispositionTime]);
        //FileName = dictionary[kFileName];
        //GUIDdescriptor = new Guid(dictionary[kGUIDdescriptor]);
        //URL = dictionary[kURL];
    }

    #endregion

}
