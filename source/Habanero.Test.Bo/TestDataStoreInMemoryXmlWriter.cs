using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestDataStoreInMemoryXmlWriter
    {
        
        [SetUp]
        public void Setup()
        {
            ClassDef.ClassDefs.Clear();
            new Address();
        }

        [TearDown]
        public void TearDownTest()
        {
        }

        [Test]
        public void Test_Construct()
        {
            //---------------Set up test pack-------------------
            DataStoreInMemory dataStore = new DataStoreInMemory();
            //---------------Assert PreConditions---------------            
            //---------------Execute Test ----------------------
            DataStoreInMemoryXmlWriter writer = new DataStoreInMemoryXmlWriter(new MemoryStream());
            //---------------Test Result -----------------------
            
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_Write()
        {
            //---------------Set up test pack-------------------
            DataStoreInMemory dataStore = new DataStoreInMemory();
            dataStore.Add(new Car());
            MemoryStream stream = new MemoryStream();
            DataStoreInMemoryXmlWriter writer = new DataStoreInMemoryXmlWriter(stream);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, dataStore.Count);
            Assert.AreEqual(0, stream.Length);
            //---------------Execute Test ----------------------
            writer.Write(dataStore);
            //---------------Test Result -----------------------
            Assert.AreNotEqual(0, stream.Length);
        }

        [Test]
        public void Test_Write_WithXmlWriterSettings()
        {
            //---------------Set up test pack-------------------
            DataStoreInMemory dataStore = new DataStoreInMemory();
            dataStore.Add(new Car());
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Auto;
            xmlWriterSettings.NewLineOnAttributes = true;
            DataStoreInMemoryXmlWriter writer = new DataStoreInMemoryXmlWriter(stream, xmlWriterSettings);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, dataStore.Count);
            Assert.AreEqual(0, stream.Length);
            //---------------Execute Test ----------------------
            writer.Write(dataStore);
            //---------------Test Result -----------------------
            Assert.AreNotEqual(0, stream.Length);
        }

        [Test]
        public void Test_Read()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Clear();
            MyBO.LoadClassDefsNoUIDef();
            DataStoreInMemory savedDataStore = new DataStoreInMemory();
            savedDataStore.Add(new MyBO());
            MemoryStream writeStream = new MemoryStream();
            DataStoreInMemoryXmlWriter writer = new DataStoreInMemoryXmlWriter(writeStream);
            writer.Write(savedDataStore);
            BusinessObjectManager.Instance = new BusinessObjectManager();
            DataStoreInMemory loadedDataStore = new DataStoreInMemory();
            writeStream.Seek(0, SeekOrigin.Begin);
            DataStoreInMemoryXmlReader reader = new DataStoreInMemoryXmlReader(writeStream);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, savedDataStore.Count);
            //---------------Execute Test ----------------------
            loadedDataStore.AllObjects = reader.Read();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, loadedDataStore.Count);
        }

        [Test]
        public void Test_Read_ShouldLoadPropertiesCorrectly()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Clear();
            MyBO.LoadClassDefsNoUIDef();
            DataStoreInMemory savedDataStore = new DataStoreInMemory();
            MyBO savedBO = new MyBO();
            savedBO.TestProp = TestUtil.GetRandomString();
            savedBO.TestProp2 = TestUtil.GetRandomString();
            TransactionCommitterInMemory transactionCommitter = new TransactionCommitterInMemory(savedDataStore);
            transactionCommitter.AddBusinessObject(savedBO);
            transactionCommitter.CommitTransaction();
            MemoryStream writeStream = new MemoryStream();
            DataStoreInMemoryXmlWriter writer = new DataStoreInMemoryXmlWriter(writeStream);
            writer.Write(savedDataStore);
            BusinessObjectManager.Instance = new BusinessObjectManager();
            DataStoreInMemory loadedDataStore = new DataStoreInMemory();
            writeStream.Seek(0, SeekOrigin.Begin);
            DataStoreInMemoryXmlReader reader = new DataStoreInMemoryXmlReader(writeStream);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, savedDataStore.Count);
            //---------------Execute Test ----------------------
            loadedDataStore.AllObjects = reader.Read();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, loadedDataStore.Count);
            IBusinessObject loadedBO;
            bool success = loadedDataStore.AllObjects.TryGetValue(savedBO.MyBoID.Value, out loadedBO);
            Assert.IsTrue(success);
            Assert.IsNotNull(loadedBO);
            Assert.IsInstanceOfType(typeof(MyBO), loadedBO);
            MyBO loadedMyBO = (MyBO) loadedBO;
            Assert.AreNotSame(savedBO, loadedMyBO);
            Assert.AreEqual(savedBO.MyBoID, loadedMyBO.MyBoID);
            Assert.AreEqual(savedBO.Props["MyBoID"].PersistedPropertyValue, loadedMyBO.Props["MyBoID"].PersistedPropertyValue);
            Assert.AreEqual(savedBO.TestProp, loadedMyBO.TestProp);
            Assert.AreEqual(savedBO.Props["TestProp"].PersistedPropertyValue, loadedMyBO.Props["TestProp"].PersistedPropertyValue);
            Assert.AreEqual(savedBO.TestProp2, loadedMyBO.TestProp2);
            Assert.AreEqual(savedBO.Props["TestProp2"].PersistedPropertyValue, loadedMyBO.Props["TestProp2"].PersistedPropertyValue);
        }

        [Test]
        public void Test_ReadWrite_MultipleObjects()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Clear();
            MyBO.LoadClassDefsNoUIDef();
            DataStoreInMemory savedDataStore = new DataStoreInMemory();
            MyBO bo1 = new MyBO();
            Car bo2 = new Car();
            savedDataStore.Add(bo1);
            savedDataStore.Add(bo2);
            MemoryStream writeStream = new MemoryStream();
            DataStoreInMemoryXmlWriter writer = new DataStoreInMemoryXmlWriter(writeStream);
            writer.Write(savedDataStore);
            BusinessObjectManager.Instance = new BusinessObjectManager();
            DataStoreInMemory loadedDataStore = new DataStoreInMemory();
            writeStream.Seek(0, SeekOrigin.Begin);
            DataStoreInMemoryXmlReader reader = new DataStoreInMemoryXmlReader(writeStream);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, savedDataStore.Count);
            //---------------Execute Test ----------------------
            loadedDataStore.AllObjects = reader.Read();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, loadedDataStore.Count);
            Assert.IsNotNull(loadedDataStore.Find<MyBO>(bo1.ID));
            Assert.IsNotNull(loadedDataStore.Find<Car>(bo2.ID));
        }
        
    }

  
}