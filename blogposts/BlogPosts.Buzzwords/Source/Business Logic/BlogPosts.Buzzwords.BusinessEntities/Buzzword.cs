using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPosts.Buzzwords.BusinessEntities
{
    public partial class Buzzword
    {
        public Buzzword()
        {
        }

        public Buzzword(System.Int32 id, System.String name)
        {
            this.idField = id;
            this.nameField = name;
        }

        private System.Int32 idField;

        public System.Int32 id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        private System.String nameField;

        public System.String name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

    }
}

