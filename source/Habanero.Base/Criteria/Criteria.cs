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
using Habanero.Base.Exceptions;

namespace Habanero.Base
{
    /// <summary>
    /// A criteria expression that can be used to build up a criteria tree. The IsMatch method can then be
    /// used to check whether a <see cref="IBusinessObject"/> matches the criteria.
    /// The Criteria object collaborates with the <see cref="ISelectQuery"/> to provide the application developer 
    /// as well as the framework developer. For more details See <see cref="ISelectQuery"/>
    /// </summary>
    public class Criteria
    {
        #region LogicalOp enum

        ///<summary>
        /// A logical operator used to conjoin two criteria trees.
        ///</summary>
        public enum LogicalOp
        {
            ///<summary>
            /// The logical And (&&) operator
            ///</summary>
            And,
            /// <summary>
            /// The logical Or (||) operator
            /// </summary>
            Or
        }

        #endregion

        #region ComparisonOp enum

        /// <summary>
        /// An operator used on a leaf criteria - ie a comparison operator to check a property against a value
        /// e.g. &lt;, &gt;=, Like.
        /// </summary>
        public enum ComparisonOp
        {
            ///<summary>
            /// The equals (=) operator
            ///</summary>
            Equals,
            ///<summary>
            /// The Greater than (&gt;) operator.
            ///</summary>
            GreaterThan,
            ///<summary>
            /// The less than (&lt;) operator
            ///</summary>
            LessThan,
            ///<summary>
            /// The Not Equals (&lt;&gt;) Operator.
            ///</summary>
            NotEquals,
            ///<summary>
            /// The less than or equal (&lt;=) Operator.
            ///</summary>
            LessThanEqual,
            ///<summary>
            /// The greater than or equal (&gt;=) Operator.
            ///</summary>
            GreaterThanEqual,
            ///<summary>
            /// The like (Like) Operator.
            ///</summary>
            Like,
            ///<summary>
            /// The Not Like Operator.
            ///</summary>
            NotLike,
            ///<summary>
            /// The IS operator used for IS NULL
            ///</summary>
            Is,
            ///<summary>
            /// The IS Not operator used for IS NOT NULL
            ///</summary>
            IsNot
        }

        #endregion
        /// <summary>
        /// The default date format to be used.
        /// </summary>
        public const string DATE_FORMAT = "yyyy/MM/dd HH:mm:ss";

        private readonly Criteria _leftCriteria;
        private readonly LogicalOp _logicalOp;
        private readonly ComparisonOp _comparisonOp;
        private readonly Criteria _rightCriteria;
        private object _fieldValue;
        protected readonly string[] LogicalOps = {"AND", "OR"};
        protected readonly string[] ComparisonOps = {"=", ">", "<", "<>", "<=", ">=", "LIKE", "NOT LIKE", "IS", "IS NOT"};
        private readonly QueryField _field;

        protected Criteria()
        {
        }

        /// <summary>
        /// Creates a leaf criteria (meaning it has no children in the tree structure).
        /// </summary>
        /// <param name="propName">The property whose value to check</param>
        /// <param name="comparisonOp">The operator to use to compare the property value to the given value</param>
        /// <param name="value">The value to compare to</param>
        public Criteria(string propName, ComparisonOp comparisonOp, object value)
            : this(QueryField.FromString(propName), comparisonOp, value)
        {
 
        }

        /// <summary>
        /// Creates a composite criteria by logically joining two other criteria.
        /// </summary>
        /// <param name="leftCriteria">The left criteria (can be a whole tree structure)</param>
        /// <param name="logicalOp">The logical operator to use to join the left criteria tree with the right</param>
        /// <param name="rightCriteria">The right criteria (can be a whole tree structure)</param>
        public Criteria(Criteria leftCriteria, LogicalOp logicalOp, Criteria rightCriteria)
        {
            _leftCriteria = leftCriteria;
            _logicalOp = logicalOp;
            _rightCriteria = rightCriteria;
        }

        /// <summary>
        /// Creates a leaf criteria (meaning it has no children in the tree structure).
        /// </summary>
        /// <param name="field">The property whose value to check, as a <see cref="QueryField"/></param>
        /// <param name="comparisonOp">The operator to use to compare the property value to the given value</param>
        /// <param name="value">The value to compare to</param>
        public Criteria(QueryField field, ComparisonOp comparisonOp, object value)
        {
            _field = field;
            _comparisonOp = comparisonOp;
            _fieldValue = value;
        }

        ///<summary>
        /// Gets the query Field being used by this criteria object (Where the criteria object is a leaf).
        /// the query field is a query field object representing the objects property as defined in the 
        /// Constructor [Criteria(string propName, ComparisonOp op, object value)].
        ///</summary>
        public virtual QueryField Field
        {
            get { return _field; }
        }

        ///<summary>
        /// Gets the left critieria object (If this is not a leaf criteria)
        ///</summary>
        public virtual Criteria LeftCriteria
        {
            get { return _leftCriteria; }
        }

        ///<summary>
        /// Gets the right criteria object (If this is not a leaf criteria)
        ///</summary>
        public virtual Criteria RightCriteria
        {
            get { return _rightCriteria; }
        }

        ///<summary>
        /// Gets the logical operator being used for this criteria object (If this is not a leaf criteria)
        ///</summary>
        public virtual LogicalOp LogicalOperator
        {
            get { return _logicalOp; }
        }

        ///<summary>
        /// Gets or sets the field value being compared to for this criteria object (If this is a leaf criteria)
        ///</summary>
        public virtual object FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }

        ///<summary>
        /// Gets the comparison operator being used by this Criteria object (If this is a leaf criteria)
        ///</summary>
        public virtual ComparisonOp ComparisonOperator
        {
            get { return _comparisonOp; }
        }


        /// <summary>
        /// Evaluates the businessObject passed in to see if it matches the criteria that have been set up
        /// </summary>
        /// <typeparam name="T">The type of BusinessObject</typeparam>
        /// <param name="businessObject">The businessobject to check for a match against the criteria</param>
        /// <returns>True if the businessobject matches the criteria, false if it does not</returns>
        public bool IsMatch<T>(T businessObject) where T : IBusinessObject
        {
            if (IsComposite())
            {
                switch (_logicalOp)
                {
                    case LogicalOp.And:
                        return _leftCriteria.IsMatch(businessObject) && _rightCriteria.IsMatch(businessObject);
                    case LogicalOp.Or:
                        return _leftCriteria.IsMatch(businessObject) || _rightCriteria.IsMatch(businessObject);
                }
            }

            //todo: criterias with relationships - this will pass the source through to the GetPropertyValue
            object leftValue = businessObject.GetPropertyValue(_field.PropertyName);
            if (leftValue == null)
            {
                return IsNullMatch();
            }

            IComparable boPropertyValue = businessObject.GetPropertyValue(_field.PropertyName) as IComparable;
            if (boPropertyValue == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Property '{0}' on class '{1}' does not implement IComparable and cannot be matched.", _field.PropertyName,
                        businessObject.GetType().FullName));
            }
            IComparable compareToValue = _fieldValue as IComparable;
            compareToValue = ConvertDateTimeStringToValue(compareToValue);
            return IsNonNullMatch(boPropertyValue, compareToValue);
        }

        private static IComparable ConvertDateTimeStringToValue(IComparable y)
        {
            y = ConvertDateTImeToday(y);
            y = ConvertDateTimeNow(y);
            return y;
        }

        private bool IsNonNullMatch(IComparable boPropertyValue, IComparable compareToValue)
        {
            switch (_comparisonOp)
            {
                case ComparisonOp.Equals:
                    return boPropertyValue.Equals(compareToValue);
                case ComparisonOp.GreaterThan:
                    return boPropertyValue.CompareTo(compareToValue) > 0;
                case ComparisonOp.LessThan:
                    return boPropertyValue.CompareTo(compareToValue) < 0;
                case ComparisonOp.NotEquals:
                    return !boPropertyValue.Equals(compareToValue);
                case ComparisonOp.LessThanEqual:
                    return boPropertyValue.CompareTo(compareToValue) <= 0;
                case ComparisonOp.GreaterThanEqual:
                    return boPropertyValue.CompareTo(compareToValue) >= 0;
                case ComparisonOp.Like:
                    return IsLikeMatch(boPropertyValue, compareToValue);
                case ComparisonOp.NotLike:
                    return !IsLikeMatch(boPropertyValue, compareToValue);
                case ComparisonOp.Is:
                    return boPropertyValue == null;//if boPropertyValue not null then always return false.
                case ComparisonOp.IsNot:
                    return boPropertyValue != null;//if boPropertyValue null then always return false.
                default:
                    throw new HabaneroDeveloperException("There is an application exception please contact your system administrator"
                                                         , "The operator " + _comparisonOp + " is not supported by the application");
            }
        }

        private static bool IsLikeMatch(IComparable boPropertyValue, IComparable compareToValue)
        {
            string compareValueStringWPct = compareToValue.ToString();
            string compareValueString_WNoPct = compareValueStringWPct.TrimEnd('%').TrimStart('%');
            if (!compareValueStringWPct.StartsWith("%") && !compareValueStringWPct.EndsWith("%"))
            {
                return boPropertyValue.Equals(compareValueString_WNoPct);
            }
            if (!compareValueStringWPct.StartsWith("%"))
            {
                return boPropertyValue.ToString().StartsWith(compareValueString_WNoPct);
            }
            if (!compareValueStringWPct.EndsWith("%"))
            {
                return boPropertyValue.ToString().EndsWith(compareValueString_WNoPct);
            }
            return boPropertyValue.ToString().Contains(compareValueString_WNoPct);
        }

        private bool IsNullMatch()
        {
            switch (_comparisonOp)
            {
                case ComparisonOp.Equals:
                    return _fieldValue == null;
                case ComparisonOp.GreaterThan:
                    return false;
                case ComparisonOp.LessThan:
                    return false;
                case ComparisonOp.NotEquals:
                    return _fieldValue != null;
                case ComparisonOp.LessThanEqual:
                    return false;
                case ComparisonOp.GreaterThanEqual:
                    return false;
                case ComparisonOp.Like:
                    return _fieldValue == null;
                case ComparisonOp.NotLike:
                    return _fieldValue != null;
                case ComparisonOp.Is:
                    if (_fieldValue == null) return true;
                    return _fieldValue.ToString().ToUpper() == "NULL";
                case ComparisonOp.IsNot:
                    if (_fieldValue == null) return false;
                    return _fieldValue.ToString().ToUpper() != "NULL";
                default:
                    throw new HabaneroDeveloperException("There is an application exception please contact your system administrator"
                                                         , "The operator " + _comparisonOp + " is not supported by the application");
            }
        }

        private static IComparable ConvertDateTImeToday(IComparable y)
        {
            if (y is DateTimeToday)
            {
                y = DateTimeToday.Value;
            }
            return y;
        }

        private static IComparable ConvertDateTimeNow(IComparable y)
        {
            if (y is DateTimeNow)
            {
                y = DateTimeNow.Value;
            }
            return y;
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (IsComposite())
            {
                string leftCriteriaAsString =  LeftCriteria.ToString();
                string rightCriteriaAsString = RightCriteria.ToString();
                return string.Format("({0}) {1} ({2})", leftCriteriaAsString, LogicalOps[(int)LogicalOperator],
                                     rightCriteriaAsString);
            }
            string sourceName = Convert.ToString(_field.Source);
            if (!String.IsNullOrEmpty(sourceName)) sourceName += ".";
            string stringComparisonOp = ComparisonOperatorString();
            return string.Format("{0}{1} {2} {3}", sourceName, Field.PropertyName, stringComparisonOp, GetValueAsString());
            //return ToString(delegate(string propName) { return propName; }, delegate(object value)
            //    {
            //        string valueString;
            //        if (value is DateTime)
            //        {
            //            valueString = ((DateTime) value).ToString(DATE_FORMAT);
            //        }
            //        else if (value is Guid)
            //        {
            //            valueString = ((Guid) value).ToString("B");
            //        }
            //        else
            //        {
            //            valueString = value.ToString();
            //        }
            //        return "'" + valueString + "'";
            //    });
        }

        private string GetValueAsString()
        {
            string valueString;
            if (FieldValue is DateTime)
            {
                valueString = ((DateTime)FieldValue).ToString(DATE_FORMAT);
            }
            else if (FieldValue is Guid)
            {
                valueString = ((Guid)FieldValue).ToString("B");
            }
            else
            {
                valueString = FieldValue.ToString();
            }
            if (CannotBeParametrised())
            {
                return valueString.ToUpper();
            }
            return "'" + valueString + "'";
        }


        ///<summary>
        ///Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            Criteria otherCriteria = obj as Criteria;
            if (otherCriteria == null) return false;
            if (IsComposite())
            {
                if (!otherCriteria.IsComposite()) return false;
                if (!LeftCriteria.Equals(otherCriteria.LeftCriteria)) return false;
                return LogicalOperator == otherCriteria.LogicalOperator && _rightCriteria.Equals(otherCriteria.RightCriteria);
            }
            if (ComparisonOperator != otherCriteria.ComparisonOperator) return false;
            if (String.Compare(Field.PropertyName, otherCriteria.Field.PropertyName) != 0) return false;
            return _fieldValue.Equals(otherCriteria.FieldValue);
        }

        ///<summary>
        ///Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        ///</summary>
        ///
        ///<returns>
        ///A hash code for the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns whether this is a leaf node (not composite, ie false) or a composite node (true).  This is determined
        /// by whether it has child nodes or not.
        /// </summary>
        /// <returns></returns>
        public bool IsComposite()
        {
            return LeftCriteria != null;
        }

        /// <summary>
        /// Creates a Criteria from an IPrimaryKey object. For each property in the primary key an Equals criteria is created, and these
        /// are all chained together with AND clauses to build up a composite Criteria representing the IPrimaryKey
        /// </summary>
        /// <param name="key">The IPrimaryKey to create Criteria for</param>
        /// <returns>The Criteria this IPrimaryKey instance is defined by</returns>
        public static Criteria FromPrimaryKey(IPrimaryKey key)
        {
            if (key.Count == 1)
            {
                return new Criteria(key[0].PropertyName, ComparisonOp.Equals, key[0].Value);
            }
            Criteria lastCriteria = null;
            foreach (IBOProp prop in key)
            {
                Criteria propCriteria = new Criteria(prop.PropertyName, ComparisonOp.Equals, prop.Value);
                lastCriteria = lastCriteria == null
                                   ? propCriteria
                                   : new Criteria(lastCriteria, LogicalOp.And, propCriteria);
            }
            return lastCriteria;
        }

        /// <summary>
        /// Creates a Criteria from an IRelationship object. For each relationship property in the relationship's key a simple 
        /// Equals Criteria object is created, and these are chained together with AND clauses to build up a composite criteria
        /// representing the relationship.
        /// </summary>
        /// <param name="relationship">The IRelationship to create Criteria for</param>
        /// <returns>The Criteria this IRelationship instance is defined by.</returns>
        public static Criteria FromRelationship(IRelationship relationship)
        {
            if (relationship.RelKey.Count == 1)
            {
                IRelProp relProp = relationship.RelKey[0];
                return new Criteria(relProp.RelatedClassPropName, ComparisonOp.Equals, relProp.BOProp.Value);
            }
            Criteria lastCriteria = null;
            foreach (IRelProp relProp in relationship.RelKey)
            {
                Criteria propCriteria = new Criteria(relProp.RelatedClassPropName, ComparisonOp.Equals, relProp.BOProp.Value);
                lastCriteria = lastCriteria == null 
                        ? propCriteria 
                        : new Criteria(lastCriteria, LogicalOp.And, propCriteria);
            }
            return lastCriteria;
        }
        /// <summary>
        /// Indicates whether the sql operator is some variant of "IS" or "IN",
        /// in which case a parameterised value is not used
        /// </summary>
        /// <returns>True if cannot use parameterised Value, false if required</returns>
        public bool CannotBeParametrised()
        {
            return !CanBeParametrised();
//            return (strOp == "IS" || strOp == "IS NOT" || strOp == "NOT IS" || strOp == "IN" || strOp == "NOT IN");
        }

        /// <summary>
        /// Indicates whether the sql operator is some variant of "IS" or "IN",
        /// in which case a parameterised value is not used
        /// </summary>
        /// <returns>True if cannot use parameterised Value, false if required</returns>
        public bool CanBeParametrised()
        {
            return ComparisonOperator != ComparisonOp.Is && ComparisonOperator != ComparisonOp.IsNot;
//            return (strOp == "IS" || strOp == "IS NOT" || strOp == "NOT IS" || strOp == "IN" || strOp == "NOT IN");
        }

        protected string ComparisonOperatorString()
        {
            return ComparisonOps[(int)ComparisonOperator];
        }

        /// <summary>
        /// Merges two given criteria into one, using an AND operator, and returns the merged criteria
        /// </summary>
        /// <returns>Returns the merged criteria</returns>
        public static Criteria MergeCriteria(Criteria criteria1, Criteria criteria2)
        {
            if ((criteria1 == null) && (criteria2 == null)) return null;
            if (criteria2 == null) return criteria1;
            if (criteria1 == null) return criteria2;
            return new Criteria(criteria1, LogicalOp.And, criteria2);
        }
    }
}