using System;
using System.Collections.Generic;
using System.Text;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestBoProp
    {
        private PropDef mPropDef;
        private BOProp mProp;

        [SetUp]
        public void init()
        {
            mPropDef = new PropDef("PropName", typeof(string), PropReadWriteRule.ReadOnly, null);
            mProp = mPropDef.CreateBOProp(false);
        }

        [Test]
        public void TestSetBOPropValue()
        {
            mProp.Value = "Prop Value";
            Assert.AreEqual("Prop Value", mProp.Value);
        }

        [Test]
        public void TestRestorePropValue()
        {
            mProp.InitialiseProp("OrigionalValue");
            mProp.Value = "Prop New Value";
            Assert.AreEqual("Prop New Value", mProp.Value);
            Assert.IsTrue(mProp.IsDirty);
            Assert.IsTrue(mProp.isValid);
            mProp.RestorePropValue();
            Assert.AreEqual("OrigionalValue", mProp.Value);
            Assert.IsFalse(mProp.IsDirty);
            Assert.IsTrue(mProp.isValid);
        }

        [Test]
        public void TestPropCompulsoryRestore()
        {
            //Test compulsory with no default set
            PropDef lPropDefWithRules = new PropDef("PropNameWithRules", "System", "String",
                                                    PropReadWriteRule.ReadWrite, null, null, true);
            lPropDefWithRules.PropRule = new PropRuleString(lPropDefWithRules.PropertyName, "", -1, -1, null);
            BOProp lBOProp = lPropDefWithRules.CreateBOProp(true);
            Assert.IsFalse(lBOProp.isValid);
            Assert.IsTrue(lBOProp.InvalidReason.Length > 0);
            lBOProp.Value = "New Value";
            Assert.IsTrue(lBOProp.isValid);
            Assert.IsFalse(lBOProp.InvalidReason.Length > 0);
            lBOProp.RestorePropValue();
            Assert.IsFalse(lBOProp.isValid);
            Assert.IsTrue(lBOProp.InvalidReason.Length > 0);
        }

        [Test]
        public void TestPropBrokenRuleRestore()
        {
            //Test compulsory with no default set
            PropDef lPropDefWithRules = new PropDef("PropNameWithRules", typeof(string),
                                                    PropReadWriteRule.ReadWrite, null);
            lPropDefWithRules.PropRule = new PropRuleString(lPropDefWithRules.PropertyName, "", 50, 51, null);
            BOProp lBOProp = lPropDefWithRules.CreateBOProp(true);
            Assert.IsTrue(lBOProp.isValid);
            try
            {
                lBOProp.Value = "New Value";
            }
            catch (InvalidPropertyValueException)
            {
                //do nothing
            }
            Assert.IsFalse(lBOProp.isValid);
            lBOProp.RestorePropValue();
            Assert.IsTrue(lBOProp.isValid);
        }

        [Test]
        public void TestBackupProp()
        {
            mProp.InitialiseProp("OrigionalValue");
            mProp.Value = "Prop New Value";
            Assert.AreEqual("Prop New Value", mProp.Value);
            mProp.BackupPropValue();
            Assert.AreEqual("Prop New Value", mProp.Value);
            Assert.IsFalse(mProp.IsDirty);
            Assert.IsTrue(mProp.isValid);
        }

        [Test]
        //Test that the proprety is not being set to dirty when the 
        // value is set but has not changed.
        public void TestDirtyProp()
        {
            mProp.InitialiseProp("OrigionalValue");
            mProp.Value = "OrigionalValue";
            Assert.IsFalse(mProp.IsDirty);
            Assert.IsTrue(mProp.isValid);
        }

        [Test]
        //Test persisted property value is returned correctly.
        public void TestPersistedPropValue()
        {
            mProp.InitialiseProp("OrigionalValue");
            mProp.Value = "New Value";
            Assert.IsTrue(mProp.IsDirty);
            Assert.AreEqual("OrigionalValue", mProp.PersistedPropertyValue);
            Assert.AreEqual("PropName = 'New Value'", mProp.DatabaseNameFieldNameValuePair(null));
            Assert.AreEqual("PropName = 'OrigionalValue'", mProp.PersistedDatabaseNameFieldNameValuePair(null));
        }

        [Test]
        //Test DirtyXML.
        public void TestDirtyXml()
        {
            mProp.InitialiseProp("OrigionalValue");
            mProp.Value = "New Value";
            Assert.IsTrue(mProp.IsDirty);
            string dirtyXml = "<" + mProp.PropertyName + "><PreviousValue>OrigionalValue" +
                              "</PreviousValue><NewValue>New Value</NewValue></" +
                              mProp.PropertyName + ">";
            Assert.AreEqual(dirtyXml, mProp.DirtyXml);
        }
    }

}
