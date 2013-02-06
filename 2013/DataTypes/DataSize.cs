namespace DataTypes
{
    using System;

    /// <summary>
    ///     Stores information about the size of data. The base unit is Byte, multiples are expressed in powers of 2.
    /// </summary>
    struct DataSize : IEquatable<DataSize>, IComparable<DataSize>
    {
        #region Constants and Fields

        private readonly ulong bytes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Create a new DataSize Object
        /// </summary>
        /// <param name="bytes">number of bytes</param>
        public DataSize(ulong bytes)
        {
            this.bytes = bytes;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Loads an amount of bytes
        /// </summary>
        /// <param name="unit">unit to load</param>
        /// <param name="value">bytes to load</param>
        public static DataSize GetFrom(Decimal value, DataSizeUnit unit = DataSizeUnit.Byte)
        {
            return new DataSize(Decimal.ToUInt64(value * GetMultiplier(unit)));
        }

        /// <summary>
        ///     Loads an amount of bytes
        /// </summary>
        /// <param name="unit">unit to load</param>
        /// <param name="value">bytes to load</param>
        public static DataSize GetFrom(Decimal value, DataSizeMetricUnit unit)
        {
            return new DataSize(Decimal.ToUInt64(value * GetMultiplier(unit)));
        }

        /// <summary>
        ///     Loads an amount of bytes
        /// </summary>
        /// <param name="unit">unit to load</param>
        /// <param name="value">bytes to load</param>
        public static DataSize GetFrom(ulong value, DataSizeUnit unit = DataSizeUnit.Byte)
        {
            return new DataSize(value * (ulong)GetMultiplier(unit));
        }

        /// <summary>
        ///     Loads an amount of bytes
        /// </summary>
        /// <param name="unit">unit to load</param>
        /// <param name="value">bytes to load</param>
        public static DataSize GetFrom(ulong value, DataSizeMetricUnit unit)
        {
            return new DataSize(value * (ulong)GetMultiplier(unit));
        }

        private static long GetMultiplier(DataSizeUnit unit)
        {
            return (1L << (10 * (int)unit));
        }

        private static long GetMultiplier(DataSizeMetricUnit unit)
        {
            return (long)Math.Pow(10, 3 * (int)unit);
        }

        /// <summary>
        /// Compares the current objekt with a object of the same type
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero (0) This object is smaller than the
        ///     <paramref name="other" />-Parameter.Zero the object is equal <paramref name="other" />. Greater than 0 (zero), this property is greater than <paramref name="other" />     .
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(DataSize other)
        {
            return this.bytes.CompareTo(other.bytes);
        }

        /// <summary>
        /// converts the current value into
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Decimal ConvertTo(DataSizeUnit unit)
        {
            return Decimal.Divide(this.bytes, GetMultiplier(unit));
        }

        /// <summary>
        /// converts the current value into
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Decimal ConvertTo(DataSizeMetricUnit unit)
        {
            return Decimal.Divide(this.bytes, GetMultiplier(unit));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true, if the current object equal to the <paramref name="other" />-parameter is, otherwise false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DataSize other)
        {
            return this.bytes.Equals(other.bytes);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        ///     true, if <paramref name="obj" /> and this instance are the same type and represent the same value, otherwise false.
        /// </returns>
        /// <param name="obj">Another object for comparison. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is DataSize && this.Equals((DataSize)obj);
        }

        /// <summary>
        ///     Gibt den Hashcode für diese Instanz zurück.
        /// </summary>
        /// <returns>
        ///     Eine 32-Bit-Ganzzahl mit Vorzeichen. Diese ist der Hashcode für die Instanz.
        /// </returns>
        public override int GetHashCode()
        {
            return this.bytes.GetHashCode();
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" />-Class that contains the fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            var unit = DataSizeUnit.Byte;
            while (this.bytes >= (ulong)GetMultiplier(unit))
            {
                unit++;
            }

            if (this.bytes > 0)
            {
                unit--;
            }

            return this.GetString(unit);
        }

        /// <summary>
        /// Convert into value with unit
        /// </summary>
        /// <param name="unit">the selected unit</param>
        /// <returns>the format string</returns>
        public string GetString(DataSizeUnit unit)
        {
            return string.Format("{0:0.###} {1}", this.ConvertTo(unit), unit);
        }

        /// <summary>
        /// Convert into value with unit
        /// For metric Units
        /// </summary>
        /// <param name="unit">the selected unit</param>
        /// <returns>the format string</returns>
        public string GetString(DataSizeMetricUnit unit)
        {
            return string.Format("{0:0.###} {1}", this.ConvertTo(unit), unit);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests for equality
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>true, if equal</returns>
        public static bool operator ==(DataSize left, DataSize right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// greater than operator
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>true, if greater</returns>
        public static bool operator >(DataSize left, DataSize right)
        {
            return left.bytes > right.bytes;
        }

        /// <summary>
        /// addition
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>the sum</returns>
        public static DataSize operator +(DataSize left, DataSize right)
        {
            return left.bytes + right.bytes;
        }

        /// <summary>
        /// subtraktion
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>the difference</returns>
        public static DataSize operator -(DataSize left, DataSize right)
        {
            return left.bytes - right.bytes;
        }

        /// <summary>
        /// multiplication
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="multi">multiplier</param>
        /// <returns>das product</returns>
        public static DataSize operator *(DataSize left, uint multi)
        {
            return left.bytes * multi;
        }

        /// <summary>
        ///     greater than or equal operator
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>true, if greater than or equal</returns>
        public static bool operator >=(DataSize left, DataSize right)
        {
            return left.bytes >= right.bytes;
        }

        /// <summary>
        /// inequality operator
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>true, if unequal</returns>
        public static bool operator !=(DataSize left, DataSize right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///  less than operator
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>true, if less</returns>
        public static bool operator <(DataSize left, DataSize right)
        {
            return left.bytes < right.bytes;
        }

        /// <summary>
        /// less than or equal operator
        /// </summary>
        /// <param name="left">DataSize 1</param>
        /// <param name="right">DataSize 2</param>
        /// <returns>true, if less than or equal</returns>
        public static bool operator <=(DataSize left, DataSize right)
        {
            return left.bytes <= right.bytes;
        }

        /// <summary>
        /// converts ulong values
        /// </summary>
        /// <param name="value">value in bytes</param>
        /// <returns>DataSize object</returns>
        public static implicit operator DataSize(ulong value)
        {
            return new DataSize(value);
        }

        /// <summary>
        ///  converts long values
        /// </summary>
        /// <param name="value">value in bytes</param>
        /// <returns>DataSize object</returns>
        public static implicit operator DataSize(long value)
        {
            return new DataSize((ulong)value);
        }

        #endregion
    }
}