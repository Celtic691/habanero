//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.BO.ConcurrencyControl;
using Habanero.BO.Loaders;
using Habanero.BO.ObjectManager;
using Habanero.DB;

namespace Habanero.BO
{
    /// <summary>
    /// This is a simple number generator class. This class implements no locking strategy.
    /// I.e. If two users try to retrieve a number of the same type concurrently then they
    /// will retrieve the same number. The object that the number is being used for e.g. member number
    /// will raise an error when that object is being saved if the member number is an alternate key.
    /// There is also no guarantee that the number will not have missing numbers in the sequence.
    /// E.g. member 1 asks for a number and gets 0001, member2 gets 0002 and member3 gets 0003.
    /// member3 is saved and member2 and member1 are not then there will be missing numbers in the sequence.
    /// </summary>
    public class NumberGenerator : INumberGenerator
    {
        private readonly string _tableName;
        private readonly BOSequenceNumber _boSequenceNumber;

        ///<summary>
        /// Creates a number generator of the specified type. If no record is currently in the database for that type.
        /// Then this will create an entry in the table with the seed number of zero.
        ///</summary>
        ///<param name="numberType">Type of number</param>
        public NumberGenerator(string numberType)
        {
            this._tableName = "";
            _boSequenceNumber = LoadSequenceNumber(numberType);
        }

        ///<summary>
        /// Creates a number generator of the specified type. If no record is currently in the database for that type.
        /// Then this will create an entry in the table with the seed number of zero.
        ///</summary>
        ///<param name="numberType">Type of number</param>
        ///<param name="tableName">the table that the sequence number is being stored in.</param>
        public NumberGenerator(string numberType, string tableName)
        {
            this._tableName = tableName;
            _boSequenceNumber = LoadSequenceNumber(numberType);
        }

        public int NextNumber()
        {
            _boSequenceNumber.SequenceNumber++;
            return _boSequenceNumber.SequenceNumber.Value;
        }

        private BOSequenceNumber LoadSequenceNumber(string numberType)
        {
            if (!ClassDef.ClassDefs.Contains(typeof(BOSequenceNumber)))
            {
                BOSequenceNumber.LoadNumberGenClassDef(_tableName);
            }
            Criteria criteria = new Criteria("NumberType", Criteria.Op.Equals, numberType);
            BOSequenceNumber sequenceBOSequenceNumber =
                BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject<BOSequenceNumber>(criteria);

            if (sequenceBOSequenceNumber != null) return sequenceBOSequenceNumber;
            sequenceBOSequenceNumber = CreateSequenceForType(numberType);
            return sequenceBOSequenceNumber;
        }

        private static BOSequenceNumber CreateSequenceForType(string numberType)
        {
            BOSequenceNumber sequenceBOSequenceNumber = new BOSequenceNumber();
            sequenceBOSequenceNumber.NumberType = numberType;
            sequenceBOSequenceNumber.SequenceNumber = 0;
            sequenceBOSequenceNumber.Save();
            return sequenceBOSequenceNumber;
        }


        ///<summary>
        /// Updates the number generator table.
        ///</summary>
        public void Save()
        {
            _boSequenceNumber.Save();
        }

        public void SetSequenceNumber(int newSequenceNumber)
        {
            _boSequenceNumber.SequenceNumber = newSequenceNumber;
            _boSequenceNumber.Save();
        }

        private BusinessObject GetTransactionalBO()
        {
            return _boSequenceNumber;
        }

        public void AddToTransaction(ITransactionCommitter transactionCommitter)
        {
            BusinessObject busObject = this.GetTransactionalBO();
            busObject.UpdateObjectBeforePersisting(transactionCommitter);
            transactionCommitter.AddBusinessObject(busObject);
        }
    }
    ///<summary>
    /// A simple sequential number generator business object
    ///</summary>
    internal class BOSequenceNumber : BusinessObject
    {
//        internal static void LoadNumberGenClassDef()
//        {
//            XmlClassLoader itsLoader = new XmlClassLoader();
//            ClassDef itsClassDef =
//                itsLoader.LoadClass(
//                    @"
//               <class name=""BOSequenceNumber"" assembly=""Habanero.BO"" table=""NumberGenerator"">
//					<property  name=""SequenceNumber"" type=""Int32"" />
//                    <property  name=""NumberType""/>
//                    <primaryKey isObjectID=""false"">
//                        <prop name=""NumberType"" />
//                    </primaryKey>
//			    </class>
//			");
//            ClassDef.ClassDefs.Add(itsClassDef);
//            return;
//        }
        internal static void LoadNumberGenClassDef()
        {
            LoadNumberGenClassDef(null);
        }
        internal static void LoadNumberGenClassDef(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) {
                tableName = "NumberGenerator"; }
            XmlClassLoader itsLoader = new XmlClassLoader();
            string classDef = "<class name=\"BOSequenceNumber\" assembly=\"Habanero.BO\" table=\"" + tableName + "\">" +
                              "<property  name=\"SequenceNumber\" type=\"Int32\" />" +
                              "<property  name=\"NumberType\"/>" +
                              "<primaryKey isObjectID=\"false\">" +
                              "<prop name=\"NumberType\" />" +
                              "</primaryKey>" +
                              "</class>";
            ClassDef itsClassDef = itsLoader.LoadClass(classDef);
            ClassDef.ClassDefs.Add(itsClassDef);
            return;
        }

        public virtual String NumberType
        {
            get { return ((String)(base.GetPropertyValue("NumberType"))); }
            set { base.SetPropertyValue("NumberType", value); }
        }

        public virtual Int32? SequenceNumber
        {
            get { return ((Int32?)(base.GetPropertyValue("SequenceNumber"))); }
            set { base.SetPropertyValue("SequenceNumber", value); }
        }

        public static void DeleteAllNumbers()
        {
            DatabaseConnection.CurrentConnection.ExecuteRawSql("Delete From numbergenerator");
        }
    }
}