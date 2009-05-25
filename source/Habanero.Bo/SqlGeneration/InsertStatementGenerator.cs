//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
// 
// This file is part of Habanero Standard.
// 
//     Habanero Standard is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Habanero Standard is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with Habanero Standard.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System.Collections;
using System.Data;
using System.Text;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.BO;
using Habanero.DB;

namespace Habanero.BO.SqlGeneration
{
    /// <summary>
    /// Generates "insert" sql statements to insert a specified business
    /// object's properties into the database
    /// </summary>
    public class InsertStatementGenerator
    {
        private BusinessObject _bo;
        private StringBuilder _dbFieldList;
        private StringBuilder _dbValueList;
        private ParameterNameGenerator _gen;
        private SqlStatement _insertSql;
        private SqlStatementCollection _statementCollection;
        private IDbConnection _conn;
        private bool _firstField;
        private ClassDef _currentClassDef;

        /// <summary>
        /// Constructor to initialise the generator
        /// </summary>
        /// <param name="bo">The business object whose properties are to
        /// be inserted</param>
        /// <param name="conn">A database connection</param>
        public InsertStatementGenerator(BusinessObject bo, IDbConnection conn)
        {
            _bo = bo;
            _conn = conn;
        }

        /// <summary>
        /// Generates a collection of sql statements to insert the business
        /// object's properties into the database
        /// </summary>
        /// <returns>Returns a sql statement collection</returns>
        public SqlStatementCollection Generate()
        {
            _statementCollection = new SqlStatementCollection();
            _currentClassDef = _bo.ClassDef;
            BOPropCol propsToInclude;
            string tableName;

            propsToInclude = GetPropsToInclude(_currentClassDef);
            tableName = _bo.TableName;
            GenerateSingleInsertStatement(propsToInclude, tableName);

            if (_bo.ClassDef.IsUsingClassTableInheritance())
            {
                _currentClassDef = _bo.ClassDef.SuperClassClassDef;
                while (_currentClassDef.IsUsingClassTableInheritance())
                {
                    propsToInclude = GetPropsToInclude(_currentClassDef);
                    tableName = _currentClassDef.TableName;
                    GenerateSingleInsertStatement(propsToInclude, tableName);
                    _currentClassDef = _currentClassDef.SuperClassClassDef;
                }
                propsToInclude = GetPropsToInclude(_currentClassDef);
                tableName = _currentClassDef.InheritedTableName;
                GenerateSingleInsertStatement(propsToInclude, tableName);
            }

            return _statementCollection;
        }

        /// <summary>
        /// Generates an "insert" sql statement for the properties in the
        /// business object
        /// </summary>
        /// <param name="propsToInclude">A collection of properties to insert,
        /// if the previous include-all boolean was not set to true</param>
        /// <param name="tableName">The table name</param>
        private void GenerateSingleInsertStatement(BOPropCol propsToInclude, string tableName)
        {
            ISupportsAutoIncrementingField supportsAutoIncrementingField = null;
            if (_bo.HasAutoIncrementingField)
            {
                supportsAutoIncrementingField = new SupportsAutoIncrementingFieldBO(_bo);
            }
            this.InitialiseStatement(tableName, supportsAutoIncrementingField);
           

            foreach (BOProp prop in _bo.Props.SortedValues)
            {
                // BOProp prop = (BOProp) item.Value;
                if (propsToInclude.Contains(prop.PropertyName))
                {
                    if (propsToInclude.AutoIncrementingPropertyName != prop.PropertyName)
                        AddPropToInsertStatement(prop);
                }
            }

            ClassDef classDef = _currentClassDef; //rather than _bo.ClassDef
            ClassDef classDefWithSTI = null;
            foreach (ClassDef def in classDef.ImmediateChildren)
            {
                if (def.IsUsingSingleTableInheritance())
                {
                    classDefWithSTI = def;
                    break;
                }
            }

            if (classDef.IsUsingSingleTableInheritance() || classDefWithSTI != null)
            {
                string discriminator;
                if (classDef.SuperClassDef != null)
                {
                    discriminator = classDef.SuperClassDef.Discriminator;
                }
                else
                {
                    discriminator = classDefWithSTI.SuperClassDef.Discriminator;
                }
                if (discriminator == null)
                {
                    throw new InvalidXmlDefinitionException("A super class has been defined " +
                        "using Single Table Inheritance, but no discriminator column has been set.");
                }
                PropDef propDef = new PropDef(discriminator, typeof(string), PropReadWriteRule.ReadWrite, null);
                BOProp className = new BOProp(propDef, _bo.ClassDef.ClassName);
                AddPropToInsertStatement(className);
            }

            _insertSql.Statement.Append(@"INSERT INTO " + tableName + " (" + _dbFieldList + ") VALUES (" +
                                       _dbValueList + ")");
            _statementCollection.Insert(0, _insertSql);
        }

        /// <summary>
        /// Initialises the sql statement
        /// </summary>
        private void InitialiseStatement(string tableName, ISupportsAutoIncrementingField supportsAutoIncrementingField)
        {
            _dbFieldList = new StringBuilder(_bo.Props.Count * 20);
            _dbValueList = new StringBuilder(_bo.Props.Count * 20);
            InsertSqlStatement statement = new InsertSqlStatement(_conn);
            statement.TableName = tableName;
            statement.SupportsAutoIncrementingField = supportsAutoIncrementingField;

            _insertSql = statement;
            
            _gen = new ParameterNameGenerator(_conn);
            _firstField = true;
        }

        /// <summary>
        /// Adds the specified property value as a parameter
        /// </summary>
        /// <param name="prop">The business object property</param>
        private void AddPropToInsertStatement(BOProp prop)
        {
            string paramName;
            if (!_firstField)
            {
                _dbFieldList.Append(", ");
                _dbValueList.Append(", ");
            }
            _dbFieldList.Append(prop.DatabaseFieldName);
            paramName = _gen.GetNextParameterName();
            _dbValueList.Append(paramName);
            _insertSql.AddParameter(paramName, prop.Value);
            //_insertSql.AddParameter(paramName, DatabaseUtil.PrepareValue(prop.PropertyValue));
            _firstField = false;
        }

        /// <summary>
        /// Determines which parent ID field to add to the insertion list, depending on which
        /// ID attribute was specified in the class definition.  There are four possibilities:
        ///    1) The child contains a foreign key to the parent, with the parent ID's name
        ///    2) No attribute was given, assumes the above.
        ///    3) The child's ID has a copy of the parent's ID value
        ///    4) The child has no ID and just inherits the parent's ID (still has the parent's
        ///        ID as a field in its own table)
        /// </summary>
        private void AddParentID(BOPropCol propsToInclude)
        {
            ClassDef currentClassDef = _currentClassDef;
            while (currentClassDef.SuperClassClassDef != null &&
                currentClassDef.SuperClassClassDef.PrimaryKeyDef == null)
            {
                currentClassDef = currentClassDef.SuperClassClassDef;
            }
            if (currentClassDef.SuperClassClassDef.PrimaryKeyDef == null) return;

            string parentIDCopyFieldName = currentClassDef.SuperClassDef.ID;
            PrimaryKeyDef parentID = currentClassDef.SuperClassClassDef.PrimaryKeyDef;
            if (parentIDCopyFieldName == null ||
                parentIDCopyFieldName == "" ||
                parentID.KeyName == parentIDCopyFieldName)
            {
                propsToInclude.Add(
                    currentClassDef.SuperClassClassDef.PrimaryKeyDef.CreateBOKey(_bo.Props).GetBOPropCol());
            }
            else if (parentIDCopyFieldName != currentClassDef.PrimaryKeyDef.KeyName)
            {
                if (parentID.Count > 1)
                {
                    throw new InvalidXmlDefinitionException("For a super class definition " +
                        "using class table inheritance, the ID attribute can only refer to a " +
                        "parent with a single primary key.  Leaving out the attribute will " +
                        "allow composite primary keys where the child's copies have the same " +
                        "field name as the parent.");
                }
                BOProp parentProp = parentID.CreateBOKey(_bo.Props).GetBOPropCol()[parentID.KeyName];
                PropDef profDef = new PropDef(parentIDCopyFieldName, parentProp.PropertyType, PropReadWriteRule.ReadWrite, null);
                BOProp newProp = new BOProp(profDef);
                newProp.Value = parentProp.Value;
                propsToInclude.Add(newProp);
            }
        }

        /// <summary>
        /// Builds a collection of properties to include in the insertion,
        /// depending on the inheritance type
        /// </summary>
        private BOPropCol GetPropsToInclude(ClassDef currentClassDef)
        {
            BOPropCol propsToInclude = currentClassDef.PropDefcol.CreateBOPropertyCol(true);
            
            if (currentClassDef.IsUsingClassTableInheritance())
            {
                AddParentID(propsToInclude);
            }

            while (currentClassDef.IsUsingSingleTableInheritance() ||
                currentClassDef.IsUsingConcreteTableInheritance())
            {
                propsToInclude.Add(currentClassDef.SuperClassClassDef.PropDefcol.CreateBOPropertyCol(true));
                currentClassDef = currentClassDef.SuperClassClassDef;
            }

            return propsToInclude;
        }
    }
}