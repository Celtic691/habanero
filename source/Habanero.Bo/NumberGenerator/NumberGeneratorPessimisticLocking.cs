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
using Habanero.DB;

namespace Habanero.BO
{
    /// <summary>
    /// This is a slightly more complex number generator class. This class implements a pessimistic locking strategy.
    /// I.e. If two users try to retrieve a number of the same type concurrently then the first user will 
    /// be allowed to retrieve the number and the second user will be given a locking error untill
    /// the first user either persists their changes, cancells their changes or times out.
    /// It is critical when using this strategy that the developer calls the NextNumber as close to the persisting
    /// of the objects as possible so as to ensure that the lock is held for as short a time as possible.
    /// This is the number generator to use when you cannot have missing numbers in the sequence i.e. you 
    /// cannot have invoice1 and then invoice3 with no invoice2.
    /// If you need a simpler number generator that implements no locking strategy then use
    /// NumberGenerator <see cref="NumberGenerator"/>

    /// </summary>
    public class NumberGeneratorPessimisticLocking : INumberGenerator
    {
        private readonly BOSequenceNumberLocking _boSequenceNumber;

        ///<summary>
        /// Creates a number generator with Pesssimistic locking.
        ///</summary>
        ///<param name="numberType"></param>
        public NumberGeneratorPessimisticLocking(string numberType)
        {
            _boSequenceNumber = LoadSequenceNumber(numberType);
        }

        public int NextNumber()
        {
            _boSequenceNumber.SequenceNumber++;
            return _boSequenceNumber.SequenceNumber.Value;
        }

        private static BOSequenceNumberLocking LoadSequenceNumber(string numberType)
        {
            BOSequenceNumberLocking sequenceBOSequenceNumber =
                BOLoader.Instance.GetBusinessObject<BOSequenceNumberLocking>(string.Format("NumberType = '{0}'", numberType));
            if (sequenceBOSequenceNumber == null)
            {
                sequenceBOSequenceNumber = CreateSequenceForType(numberType);
            }
            return sequenceBOSequenceNumber;
        }

        private static BOSequenceNumberLocking CreateSequenceForType(string numberType)
        {
            BOSequenceNumberLocking sequenceBOSequenceNumber = new BOSequenceNumberLocking();
            sequenceBOSequenceNumber.NumberType = numberType;
            sequenceBOSequenceNumber.SequenceNumber = 0;
            sequenceBOSequenceNumber.Save();
            return sequenceBOSequenceNumber;
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
    internal class BOSequenceNumberLocking : BusinessObject
    {
        public BOSequenceNumberLocking()
        {
            SetConcurrencyControl(new PessimisticLockingDB(
                    this, 15,
                    this.Props["DateTimeLocked"],
                    this.Props["UserLocked"],
                    this.Props["MachineLocked"],
                    this.Props["OperatingSystemUserLocked"],
                    this.Props["Locked"]));
        }
        internal static void LoadNumberGenClassDef()
        {
            XmlClassLoader itsLoader = new XmlClassLoader();
            ClassDef itsClassDef =
                itsLoader.LoadClass(
                    @"
               <class name=""BOSequenceNumberLocking"" assembly=""Habanero.BO"" table=""NumberGenerator"">
					<property  name=""SequenceNumber"" type=""Int32"" />
                    <property  name=""NumberType""/>
					<property  name=""DateTimeLocked"" type=""DateTime"" />
					<property  name=""UserLocked"" />
					<property  name=""Locked"" type=""Boolean""/>
					<property  name=""MachineLocked"" />
					<property  name=""OperatingSystemUserLocked"" />
                    <primaryKey isObjectID=""false"">
                        <prop name=""NumberType"" />
                    </primaryKey>
			    </class>
			");
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