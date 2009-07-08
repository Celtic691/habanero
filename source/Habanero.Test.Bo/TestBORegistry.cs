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

using Habanero.BO;
using Habanero.DB;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestBORegistry 
    {
        [SetUp]
        public void SetupTest()
        {
           
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }

        [TearDown]
        public void TearDownTest()
        {
        
        }

 

        [Test]
        public void TestSetDataAccessor()
        {
            //---------------Set up test pack-------------------
            IDataAccessor dataAccessor = new DataAccessorDB();
            //---------------Execute Test ----------------------
            BORegistry.DataAccessor = dataAccessor;
            //---------------Test Result -----------------------
            Assert.AreSame(dataAccessor, BORegistry.DataAccessor);
            //---------------Tear Down -------------------------
        }

        [Test]
        public void TestDataAccessorDBConstructor()
        {
            //---------------Set up test pack-------------------
            
            //---------------Execute Test ----------------------
            IDataAccessor dataAccessor = new DataAccessorDB();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(BusinessObjectLoaderDB), dataAccessor.BusinessObjectLoader);
            //---------------Tear Down -------------------------
        }

        [Test]
        public void TestDataAccessorDB_CreateTransactionCommitter()
        {
            //---------------Set up test pack-------------------
            
            //---------------Execute Test ----------------------
            IDataAccessor dataAccessor = new DataAccessorDB();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(TransactionCommitterDB), dataAccessor.CreateTransactionCommitter());
            //---------------Tear Down -------------------------
        }


        [Test]
        public void TestDataAccessorInMemoryConstructor()
        {
            //---------------Set up test pack-------------------
            
            //---------------Execute Test ----------------------
            IDataAccessor dataAccessor = new DataAccessorInMemory();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(BusinessObjectLoaderInMemory), dataAccessor.BusinessObjectLoader);
            //---------------Tear Down -------------------------
        }


        [Test]
        public void TestDataAccessorInMemory_CreateTransactionCommitter()
        {
            //---------------Set up test pack-------------------

            //---------------Execute Test ----------------------
            IDataAccessor dataAccessor = new DataAccessorInMemory();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(TransactionCommitterInMemory), dataAccessor.CreateTransactionCommitter());
            //---------------Tear Down -------------------------
        }
        
    }

   
}