using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Collections;

/// <summary>
/// Source Code is from this Blog:
/// http://blogs.geekdojo.net/ryan/archive/2004/04/28/1797.aspx
/// </summary>
namespace DynVCard
{
    public class VCard
    {

        private string _formattedName;
        private string _lastName;
        private string _firstName;
        private string _middleName;
        private string _namePrefix;
        private string _nameSuffix;
        private DateTime _birthDate = DateTime.MinValue;
        private ArrayList _addresses = new ArrayList();
        private ArrayList _phoneNumber = new ArrayList();
        private ArrayList _emailAddresses = new ArrayList();
        private string _title;
        private string _role;
        private string _organization;

        public VCard()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Public Properties
        public string FormattedName
        {
            get { return _formattedName; }
            set { _formattedName = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }
        public string NamePrefix
        {
            get { return _namePrefix; }
            set { _namePrefix = value; }
        }
        public string NameSuffix
        {
            get { return _nameSuffix; }
            set { _nameSuffix = value; }
        }
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; }
        }
        public ArrayList Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }
        public ArrayList PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }
        public ArrayList EmailAddresses
        {
            get { return _emailAddresses; }
            set { _emailAddresses = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        #endregion

        #region Public Methods

        public void Generate(string filePath, FileMode mode)
        {
            FileStream fs = new FileStream(filePath, mode);

            Generate(fs);
        }

        public void Generate(Stream outputStream)
        {
            StreamWriter sw = new StreamWriter(outputStream);

            using (sw)
            {
                sw.Write(this.ToString());
            }
        }
        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Start VCard
            sb.Append("BEGIN:VCARD\r\n");

            //Add Formatted name
            if (_formattedName != null)
            {
                sb.Append("FN:" + _formattedName + "\r\n");
            }

            //Add the name
            sb.Append("N:" + _lastName + ";");
            sb.Append(_firstName + ";");
            sb.Append(_middleName + ";");
            sb.Append(_namePrefix + ";");
            sb.Append(_nameSuffix + "\r\n");

            //Add a birthday
            if (_birthDate != DateTime.MinValue)
            {
                sb.Append("BDAY:" + _birthDate.ToString("yyyyMMdd") + "\r\n");
            }

            //Add Delivery Addresses
            foreach (DeliveryAddress da in _addresses)
            {
                sb.Append(da.ToString());
            }

            //Add phone numbers
            foreach (TelephoneNumber phone in _phoneNumber)
            {
                sb.Append(phone.ToString());
            }

            //Add email address
            foreach (string email in _emailAddresses)
            {
                sb.Append("EMAIL; INTERNET:" + email + "\r\n");
            }

            //Add Title
            sb.Append("TITLE:" + _title + "\r\n");

            //Business Category
            sb.Append("ROLE:" + _role + "\r\n");

            //Organization
            sb.Append("ORG:" + _organization + "\r\n");

            sb.Append("END:VCARD\r\n");

            return sb.ToString();
        }
    }

    public class DeliveryAddress
    {
        private string _postOfficeAddress;
        private string _extendedAddress;
        private string _street;
        private string _locality;
        private string _region;
        private string _postalCode;
        private string _country;

        public AddressType DeliveryAddressType;

        public DeliveryAddress()
        {

        }

        public string PostOfficeAddress
        {
            get { return _postOfficeAddress; }
            set { _postOfficeAddress = value; }
        }
        public string ExtendedAddress
        {
            get { return _extendedAddress; }
            set { _extendedAddress = value; }
        }
        public string Street
        {
            get { return _street; }
            set { _street = value; }
        }
        public string Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ADR;" + Enum.GetName(typeof(AddressType), DeliveryAddressType) + ":");
            sb.Append(_postOfficeAddress + ";");
            sb.Append(_extendedAddress + ";");
            sb.Append(_street + ";");
            sb.Append(_locality + ";");
            sb.Append(_region + ";");
            sb.Append(_postalCode + ";");
            sb.Append(_country + "\r\n");

            return sb.ToString();
        }

    }

    public enum AddressType
    {
        DOM,
        INTL,
        POSTAL,
        PARCEL,
        HOME,
        WORK
    }

    public class TelephoneNumber
    {
        private string _phoneNumber;

        public TelephoneNumberType PhoneNumberType;

        public TelephoneNumber()
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TEL;" + Enum.GetName(typeof(TelephoneNumberType), PhoneNumberType) + ":");
            sb.Append(_phoneNumber + "\r\n");
            return sb.ToString();
        }

        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
            }
        }
    }

    public enum TelephoneNumberType
    {
        PREF,
        WORK,
        HOME,
        VOICE,
        FAX,
        MSG,
        CELL,
        PAGER,
        BBS,
        MODEM,
        CAR
    }
}
