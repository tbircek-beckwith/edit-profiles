
using System;
using System.Collections.Generic;
using System.Linq;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Provides .csv file column locations as Enums values
    /// for more information https://stackoverflow.com/a/15713789
    /// </summary>
    public class Column
    {
        #region Attributes
        /// <summary>
        /// 
        /// </summary>
        protected int index;
        /// <summary>
        /// 
        /// </summary>
        protected string name;

        /// <summary>
        /// Go with a dictionary to enforce unique index
        /// </summary>
        protected static readonly IDictionary<int, Column> values = new Dictionary<int, Column>();

        #endregion

        #region Enums values

        /// <summary>
        /// <see cref="Register.ReplacementValue"/> column location 1.
        /// </summary>
        public static readonly Column ReplacementValue = new Column(1, "Replacement");

        /// <summary>
        /// <see cref="Register.OriginalValue"/> column location 3.
        /// </summary>
        public static readonly Column OriginalValue = new Column(3, "Original");

        /// <summary>
        /// <see cref="Register.RegisterPermissions"/> column location 5.
        /// </summary>
        public static readonly Column RegisterPermissions = new Column(5, "Function");

        /// <summary>
        /// <see cref="Register.Location"/> column location 6.
        /// </summary>
        public static readonly Column Location = new Column(6, "Location");

        /// <summary>
        /// <see cref="Register.DataType"/> column location 7.
        /// </summary>
        public static readonly Column DataType = new Column(7, "DataType");

        /// <summary>
        /// <see cref="Register.MinimumValue"/> column location 8.
        /// </summary>
        public static readonly Column MinimumValue = new Column(8, "Minimum");

        /// <summary>
        /// <see cref="Register.MaximumValue"/> column location 9.
        /// </summary>
        public static readonly Column MaximumValue = new Column(9, "Maximum");

        /// <summary>
        /// <see cref="Register.Increment"/> column location 10.
        /// </summary>
        public static readonly Column Increment = new Column(10, "Increment");

        /// <summary>
        /// <see cref="Register.MBFunction"/> column location 14.
        /// </summary>
        public static readonly Column MBFunction = new Column(14, "MBFunction");

        /// <summary>
        /// <see cref="Register.ProtectionLevel"/> column location 15.
        /// </summary>
        public static readonly Column ProtectionLevel = new Column(15, "ProtectionLevel");

        /// <summary>
        /// <see cref="Register.Permissions"/> column location 16.
        /// </summary>
        public static readonly Column Permissions = new Column(16, "Permissions");

        /// <summary>
        /// <see cref="Register.OptionalName"/> column location 17.
        /// </summary>
        public static readonly Column AltNames = new Column(17, "AltNames");

        /// <summary>
        /// <see cref="Register.OptionalName"/> column location 22.
        /// </summary>
        public static readonly Column DetailNames = new Column(22, "DetailNames");

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        protected Column(int index, string name)
        {
            this.index = index;
            this.name = name;
            values.Add(index, this);
        }

        /// <summary>
        /// Easy int conversion
        /// </summary>
        /// <param name="column"></param>
        public static implicit operator int(Column column) => column.index; //nb: if question is null this will return a null pointer exception

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public static implicit operator Column(int index) => values.TryGetValue(index, out var column) ? column : null;

        /// <summary>
        /// Easy string conversion (also update ToString for the same effect)
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Column: {(char)index}, Field: {index}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        public static implicit operator string(Column column) => column?.ToString();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static implicit operator Column(string name) => name == null ? null : values.Values.FirstOrDefault(item => name.Equals(item.name, StringComparison.CurrentCultureIgnoreCase));
         
        /// <summary>
        /// If you specifically want a Get(int x) function (though not required given the implicit conversion)
        /// </summary>
        /// <param name="foo"></param>
        /// <returns></returns>
        public Column Get(int foo) => foo; //(implicit conversion will take care of the conversion for you)

        #endregion

    }
}
