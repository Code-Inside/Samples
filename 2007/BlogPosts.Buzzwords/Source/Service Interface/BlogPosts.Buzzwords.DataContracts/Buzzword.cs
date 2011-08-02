using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BlogPosts.Buzzwords.DataContracts
{
    /// <summary>
    /// Data Contract Class - Buzzword
    /// </summary>
    [DataContract(Namespace = "http://BlogPosts.Buzzwords.DataContracts/2007/10", Name = "Buzzword")]
    public partial class Buzzword
    {
        private System.Int32 IdField;

        [DataMember(IsRequired = false, Name = "Id", Order = 0)]
        public System.Int32 Id
        {
            get { return IdField; }
            set { IdField = value; }
        }

        private System.String NameField;

        [DataMember(IsRequired = false, Name = "Name", Order = 1)]
        public System.String Name
        {
            get { return NameField; }
            set { NameField = value; }
        }

    }
}
